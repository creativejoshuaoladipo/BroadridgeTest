using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BroadridgeTest
{
    

    public interface IWordFrequencyProcessor
    {
        /// <summary>
        /// Runs the word frequency processing from input file to output file.
        /// </summary>
        /// <param name="inputFilePath">Path to the input text file.</param>
        /// <param name="outputFilePath">Path to the output text file.</param>
        Task RunAsync(string inputFilePath, string outputFilePath);

        /// <summary>
        /// Processes the input file and counts the frequency of each word.
        /// </summary>
        /// <param name="filePath">Path to the input file.</param>
        /// <returns>A concurrent dictionary with each word as the key and its frequency as the value.</returns>
        Task<ConcurrentDictionary<string, int>> ProcessFileAsync(string filePath);

        /// <summary>
        /// Writes word frequencies to the specified output file, sorted by frequency and alphabetically.
        /// </summary>
        /// <param name="outputFilePath">Path to the output file.</param>
        /// <param name="wordFrequencies">Dictionary of words and their frequencies.</param>
        Task WriteOutputAsync(string outputFilePath, ConcurrentDictionary<string, int> wordFrequencies);
    }

}
