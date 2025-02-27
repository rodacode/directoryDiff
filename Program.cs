using System;
using System.IO;
using System.Linq;

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
                Console.WriteLine("Two paths of directories need to be pass in the args");
                Console.WriteLine("Usage: DirectoryDiff <directory1> <directory2> [options]");
                Console.WriteLine("Example: DirectoryDiff /Users/me/folder1 /Users/me/folder2 --content");
                Console.WriteLine("\nOptions:");
                Console.WriteLine("  --content    Compare file contents (slower but more accurate)");
                return;
            }

            string directory1 = args[0];
            string directory2 = args[1];

            // Check for content comparison flag
            bool compareContent = args.Length > 2 && args.Contains("--content");

            if (compareContent)
            {
                Console.WriteLine("Content comparison enabled (this may take longer)");
            }

            RunComparison(directory1, directory2, compareContent);
        }


        static void RunComparison(string directory1, string directory2, bool compareContent)
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

            // Create a directory comparer with the content comparison option
            var comparer = new DirectoryComparer(compareContent);
            var result = comparer.CompareDirectories(directory1, directory2);

            // Display the results
            DisplayResults(result, directory1, directory2, compareContent);
        }

        static void DisplayResults(ComparisonResult result, string directory1, string directory2, bool contentCompared)
        {
            Console.WriteLine($"\nComparison between:");
            Console.WriteLine($"  1: {directory1}");
            Console.WriteLine($"  2: {directory2}");

            Console.WriteLine($"\nResults summary:");
            Console.WriteLine($"  Files only in directory 1: {result.OnlyInFirst.Count}");
            Console.WriteLine($"  Files only in directory 2: {result.OnlyInSecond.Count}");
            Console.WriteLine($"  Files with differences: {result.Different.Count}");
            Console.WriteLine($"  Identical files: {result.Identical.Count}");
            Console.WriteLine($"  Total files compared: {result.OnlyInFirst.Count + result.OnlyInSecond.Count + result.Different.Count + result.Identical.Count}");

            // Display files only in first directory
            if (result.OnlyInFirst.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nFILES ONLY IN DIRECTORY 1:");
                Console.ResetColor();
                foreach (var file in result.OnlyInFirst.OrderBy(f => f))
                {
                    Console.WriteLine($"  {file}");
                }
            }

            // Display files only in second directory
            if (result.OnlyInSecond.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nFILES ONLY IN DIRECTORY 2:");
                Console.ResetColor();
                foreach (var file in result.OnlyInSecond.OrderBy(f => f))
                {
                    Console.WriteLine($"  {file}");
                }
            }

            // Display different files
            if (result.Different.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\nFILES THAT DIFFER:");
                Console.ResetColor();
                foreach (var diff in result.Different.OrderBy(d => d.FileName))
                {
                    Console.WriteLine($"  {diff.FileName}");

                    if (diff.Size1 != diff.Size2)
                    {
                        Console.WriteLine($"    Size: {diff.Size1} vs {diff.Size2} bytes");
                    }

                    if (diff.Modified1 != diff.Modified2)
                    {
                        Console.WriteLine($"    Modified: {diff.Modified1.ToLocalTime()} vs {diff.Modified2.ToLocalTime()}");
                    }

                    if (contentCompared && diff.Size1 == diff.Size2)
                    {
                        Console.WriteLine($"    Content: {(diff.ContentSame ? "Same" : "Different")}");
                    }
                }
            }

            // Display identical files
            if (result.Identical.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nIDENTICAL FILES:");
                Console.ResetColor();

                // Group identical files by directory for better readability
                var groupedByDir = result.Identical
                    .OrderBy(f => f)
                    .GroupBy(f => Path.GetDirectoryName(f) ?? "")
                    .OrderBy(g => g.Key);

                foreach (var group in groupedByDir)
                {
                    string dirPath = string.IsNullOrEmpty(group.Key) ? "[Root]" : group.Key;
                    Console.WriteLine($"  In {dirPath}:");

                    foreach (var file in group)
                    {
                        Console.WriteLine($"    {Path.GetFileName(file)}");
                    }
                }
            }
        }
    }
}