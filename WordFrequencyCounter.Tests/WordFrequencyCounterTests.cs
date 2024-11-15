using BroadridgeTest;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Xunit;

public class WordFrequencyProcessorTests
{
    // Test for ProcessFileAsync
    [Fact]
    public async Task ProcessFileAsync_ShouldCountWordFrequenciesCorrectly()
    {
        // Arrange
        var processor = new WordFrequencyProcessor();
        string testContent = "Hello world! Hello universe.";
        string testFilePath = "test_input.txt";

        await File.WriteAllTextAsync(testFilePath, testContent);

        // Act
        var wordFrequencies = await processor.ProcessFileAsync(testFilePath);

        // Assert
        Assert.Equal(2, wordFrequencies["hello"]);
        Assert.Equal(1, wordFrequencies["world"]);
        Assert.Equal(1, wordFrequencies["universe"]);

        // Cleanup
        File.Delete(testFilePath);
    }

    // Test for WriteOutputAsync
    [Fact]
    public async Task WriteOutputAsync_ShouldWriteSortedOutputToFile()
    {
        // Arrange
        var processor = new WordFrequencyProcessor();
        var wordFrequencies = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["hello"] = 3,
            ["world"] = 2,
            ["test"] = 2,
            ["a"] = 1
        };
        string outputFilePath = "test_output.txt";

        // Act
        await processor.WriteOutputAsync(outputFilePath, wordFrequencies);

        // Assert
        var lines = await File.ReadAllLinesAsync(outputFilePath);
        Assert.Equal("hello,3", lines[0]);
        Assert.Equal("test,2", lines[1]);
        Assert.Equal("world,2", lines[2]);
        Assert.Equal("a,1", lines[3]);

        // Cleanup
        File.Delete(outputFilePath);
    }

    // Integration Test for RunAsync
    [Fact]
    public async Task RunAsync_ShouldProcessFileAndGenerateCorrectOutput()
    {
        // Arrange
        var processor = new WordFrequencyProcessor();
        string inputFilePath = "integration_input.txt";
        string outputFilePath = "integration_output.txt";
        string testContent = "This is a test. This test is simple.";

        await File.WriteAllTextAsync(inputFilePath, testContent);

        // Act
        await processor.RunAsync(inputFilePath, outputFilePath);

        // Assert
        var lines = await File.ReadAllLinesAsync(outputFilePath);

        // Updated assertions to reflect actual correct order
        Assert.Equal("is,2", lines[0]);
        Assert.Equal("test,2", lines[1]);
        Assert.Equal("this,2", lines[2]);
        Assert.Equal("a,1", lines[3]);
        Assert.Equal("simple,1", lines[4]);

        // Cleanup
        File.Delete(inputFilePath);
        File.Delete(outputFilePath);
    }


}
