# Word Frequency Counter

A C# console application that reads an input text file and generates an output file listing each word's frequency in the input file. The output file lists each word, sorted by frequency (descending), and alphabetically within the same frequency.

## Purpose

This application processes a text file, counts the occurrences of each word, and outputs the results to another file. It’s designed for performance, handling large files efficiently through multi-threaded processing.

## Features

- Counts words in a case-insensitive manner.
- Supports large files by processing line-by-line.
- Multi-threaded for performance.
- Sorts results by frequency (descending) and alphabetically for words with the same frequency.
- Handles empty files and files with only punctuation gracefully.

## Getting Started

### Prerequisites

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)

### Building the Project

1. Clone the repository.
2. Open the terminal in the project directory.
3. Run `dotnet build` to compile the application.

### Running the Application

To execute the application, use the following command:

```bash
dotnet run --project BroadridgeTest input.txt output.txt
