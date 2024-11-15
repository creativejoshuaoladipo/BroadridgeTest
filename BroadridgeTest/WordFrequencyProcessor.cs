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

            try
            {
                await Parallel.ForEachAsync(File.ReadLinesAsync(filePath), async (line, _) =>
                {
                    foreach (Match match in _wordPattern.Matches(line.ToLower()))
                    {
                        string word = match.Value;
                        wordFrequencies.AddOrUpdate(word, 1, (key, count) => count + 1);
                    }
                    await Task.CompletedTask;
                });
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
