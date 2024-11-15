using System;
using System.Threading.Tasks;
using BroadridgeTest;
using Microsoft.Extensions.DependencyInjection;

await MainAsync(args);

static async Task MainAsync(string[] args)
{
    Console.WriteLine("Welcome to the Word Frequency Counter!");
    Console.WriteLine("Please provide the input file path as the first argument and the output file path as the second argument.");
    Console.WriteLine("Example: WordFrequencyCounter input.txt output.txt");

    if (args.Length < 2)
    {
        Console.WriteLine("Usage: WordFrequencyCounter <inputFile> <outputFile>");
        return;
    }

    string inputFilePath = args[0];
    string outputFilePath = args[1];

    var serviceProvider = new ServiceCollection()
        .AddSingleton<IWordFrequencyProcessor, WordFrequencyProcessor>()
        .BuildServiceProvider();

    var processor = serviceProvider.GetService<IWordFrequencyProcessor>();

    try
    {
        await processor.RunAsync(inputFilePath, outputFilePath);
        Console.WriteLine("Word frequencies have been successfully written to the output file.");
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine(ex.Message);
    }
    catch (IOException ex)
    {
        Console.WriteLine("An error occurred during file processing: " + ex.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    }
}
