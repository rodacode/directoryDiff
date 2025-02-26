using System;
using System.IO;

namespace DirectoryDiff
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Directory Difference Tool");
            Console.WriteLine("------------------------");

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: DirectoryDiff <directory1> <directory2>");
                Console.WriteLine("Example: DirectoryDiff /Users/me/folder1 /Users/me/folder2");
                
                // For testing, you can uncomment these lines to use hardcoded paths
                // string directory1 = "/path/to/directory1";
                // string directory2 = "/path/to/directory2";
                // RunComparison(directory1, directory2);
                
                return;
            }

            string directory1 = args[0];
            string directory2 = args[1];
            
            RunComparison(directory1, directory2);
        }

        static void RunComparison(string directory1, string directory2)
        {
            // Validate directories
            if (!Directory.Exists(directory1))
            {
                Console.WriteLine($"Error: Directory not found: {directory1}");
                return;
            }

            if (!Directory.Exists(directory2))
            {
                Console.WriteLine($"Error: Directory not found: {directory2}");
                return;
            }

            // Create a directory comparer and run the comparison
            var comparer = new DirectoryComparer();
            var result = comparer.CompareDirectories(directory1, directory2);

            // Display the results
            DisplayResults(result, directory1, directory2);
        }

static void DisplayResults(ComparisonResult result, string directory1, string directory2)
{
    Console.WriteLine($"\nComparison between:");
    Console.WriteLine($"  1: {directory1}");
    Console.WriteLine($"  2: {directory2}");
    Console.WriteLine($"\nResults summary:");
    Console.WriteLine($"  Files only in directory 1: {result.OnlyInFirst.Count}");
    Console.WriteLine($"  Files only in directory 2: {result.OnlyInSecond.Count}");
    Console.WriteLine($"  Files with differences: {result.Different.Count}");
    Console.WriteLine($"  Identical files: {result.Identical.Count}");

    // Display files only in first directory
    if (result.OnlyInFirst.Count > 0)
    {
        Console.WriteLine($"\nFiles only in directory 1:");
        foreach (var file in result.OnlyInFirst.OrderBy(f => f))
        {
            Console.WriteLine($"  {file}");
        }
    }

    // Display files only in second directory
    if (result.OnlyInSecond.Count > 0)
    {
        Console.WriteLine($"\nFiles only in directory 2:");
        foreach (var file in result.OnlyInSecond.OrderBy(f => f))
        {
            Console.WriteLine($"  {file}");
        }
    }

    // Display different files
    if (result.Different.Count > 0)
    {
        Console.WriteLine($"\nFiles that differ:");
        foreach (var diff in result.Different.OrderBy(d => d.FileName))
        {
            Console.WriteLine($"  {diff.FileName}");
            Console.WriteLine($"    Size: {diff.Size1} vs {diff.Size2} bytes");
            Console.WriteLine($"    Modified: {diff.Modified1.ToLocalTime()} vs {diff.Modified2.ToLocalTime()}");
        }
    }
}
    }
}