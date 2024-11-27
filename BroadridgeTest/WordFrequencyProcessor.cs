using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BroadridgeTest
{
   

    public class WordFrequencyProcessor : IWordFrequencyProcessor
    {
        private readonly Regex _wordPattern = new Regex(@"\b\w+\b", RegexOptions.Compiled);

        /// <summary>
        /// Main method for processing input and output.
        /// </summary>
        /// <param name="inputFilePath">Path to the input text file.</param>
        /// <param name="outputFilePath">Path to the output text file.</param>
        public async Task RunAsync(string inputFilePath, string outputFilePath)
        {
            if (string.IsNullOrWhiteSpace(inputFilePath) || string.IsNullOrWhiteSpace(outputFilePath))
            {
                throw new ArgumentException("Input and output file paths must not be null or whitespace.");
            }

            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException("Input file does not exist.");
            }

            var wordFrequencies = await ProcessFileAsync(inputFilePath);
            await WriteOutputAsync(outputFilePath, wordFrequencies);
        }

        public async Task<ConcurrentDictionary<string, int>> ProcessFileAsync(string filePath)
        {
            var wordFrequencies = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            var lineQueue = new BlockingCollection<string>(boundedCapacity: 1000); // Queue with bounded capacity

            try
            {
                // Producer: Read file lines asynchronously
                var producerTask = Task.Run(async () =>
                {
                    try
                    {
                        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
                        using var reader = new StreamReader(stream);

                        string? line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            lineQueue.Add(line);
                        }
                    }
                    finally
                    {
                        lineQueue.CompleteAdding(); // Mark the queue as complete
                    }
                });

                // Consumer: Process lines in parallel
                var consumerTask = Task.Run(() =>
                {
                    Parallel.ForEach(lineQueue.GetConsumingEnumerable(), line =>
                    {
                        foreach (Match match in _wordPattern.Matches(line.ToLower()))
                        {
                            string word = match.Value;
                            wordFrequencies.AddOrUpdate(word, 1, (key, count) => count + 1);
                        }
                    });
                });

                await Task.WhenAll(producerTask, consumerTask);
            }
            catch (IOException ex)
            {
                throw new IOException("An error occurred while reading the file.", ex);
            }



            return wordFrequencies;
        }
        public async Task WriteOutputAsync(string outputFilePath, ConcurrentDictionary<string, int> wordFrequencies)
        {
            var sortedWordFrequencies = wordFrequencies
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key);

            try
            {
                using (StreamWriter writer = new StreamWriter(outputFilePath))
                {
                    foreach (var kv in sortedWordFrequencies)
                    {
                        await writer.WriteLineAsync($"{kv.Key},{kv.Value}");
                    }
                }
            }
            catch (IOException ex)
            {
                throw new IOException("An error occurred while writing to the output file.", ex);
            }
        }
    }
}
