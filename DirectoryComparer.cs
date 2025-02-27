using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace DirectoryDiff
{
    public class DirectoryComparer
    {
        private readonly FileComparer _fileComparer;

        public DirectoryComparer(bool compareContent = false)
        {
            _fileComparer = new FileComparer(compareContent);
        }
        
        public ComparisonResult CompareDirectories(string directory1, string directory2)
        {
            var result = new ComparisonResult();

            // Perform recursive comparison
            CompareDirectoriesRecursive(directory1, directory2, "", result);

            return result;
        }

        private void CompareDirectoriesRecursive(string baseDir1, string baseDir2, string relativePath, ComparisonResult result)
        {
            // Get the full paths of the current directories to compare
            string currentDir1 = Path.Combine(baseDir1, relativePath);
            string currentDir2 = Path.Combine(baseDir2, relativePath);

            // Get all files in the current directories
            string[] files1 = Directory.Exists(currentDir1) ? Directory.GetFiles(currentDir1) : new string[0];
            string[] files2 = Directory.Exists(currentDir2) ? Directory.GetFiles(currentDir2) : new string[0];

            // Get relative file paths for comparison
            var relativeFiles1 = files1.Select(f => Path.GetFileName(f)).ToList();
            var relativeFiles2 = files2.Select(f => Path.GetFileName(f)).ToList();

            // Find files only in directory1
            foreach (var file in relativeFiles1.Except(relativeFiles2))
            {
                string relativeFilePath = Path.Combine(relativePath, file);
                result.OnlyInFirst.Add(relativeFilePath);
            }

            // Find files only in directory2
            foreach (var file in relativeFiles2.Except(relativeFiles1))
            {
                string relativeFilePath = Path.Combine(relativePath, file);
                result.OnlyInSecond.Add(relativeFilePath);
            }

            // Compare common files
            foreach (var file in relativeFiles1.Intersect(relativeFiles2))
            {
                string fullPath1 = Path.Combine(currentDir1, file);
                string fullPath2 = Path.Combine(currentDir2, file);
                string relativeFilePath = Path.Combine(relativePath, file);

                if (_fileComparer.AreFilesIdentical(fullPath1, fullPath2))
                {
                    result.Identical.Add(relativeFilePath);
                }
                else
                {
                    var difference = _fileComparer.GetFileDifference(fullPath1, fullPath2, relativeFilePath);
                    result.Different.Add(difference);
                }
            }

            // Get all subdirectories
            string[] dirs1 = Directory.Exists(currentDir1) ? Directory.GetDirectories(currentDir1) : new string[0];
            string[] dirs2 = Directory.Exists(currentDir2) ? Directory.GetDirectories(currentDir2) : new string[0];

            // Get just the directory names without the full path
            var dirNames1 = dirs1.Select(d => Path.GetFileName(d)).ToList();
            var dirNames2 = dirs2.Select(d => Path.GetFileName(d)).ToList();

            // Find directories that exist in both places and recursively compare them
            foreach (var dir in dirNames1.Intersect(dirNames2))
            {
                string newRelativePath = Path.Combine(relativePath, dir);
                CompareDirectoriesRecursive(baseDir1, baseDir2, newRelativePath, result);
            }

            // Add directories that only exist in dir1
            foreach (var dir in dirNames1.Except(dirNames2))
            {
                string dirPath = Path.Combine(currentDir1, dir);
                AddDirectoryFilesRecursive(dirPath, baseDir1, result.OnlyInFirst);
            }

            // Add directories that only exist in dir2
            foreach (var dir in dirNames2.Except(dirNames1))
            {
                string dirPath = Path.Combine(currentDir2, dir);
                AddDirectoryFilesRecursive(dirPath, baseDir2, result.OnlyInSecond);
            }
        }

        private void AddDirectoryFilesRecursive(string directory, string basePath, List<string> fileList)
        {
            // Add all files in this directory
            foreach (var file in Directory.GetFiles(directory))
            {
                // Get the path relative to the base directory
                string relativePath = file.Substring(basePath.Length).TrimStart(Path.DirectorySeparatorChar);
                fileList.Add(relativePath);
            }

            // Recursively process subdirectories
            foreach (var dir in Directory.GetDirectories(directory))
            {
                AddDirectoryFilesRecursive(dir, basePath, fileList);
            }
        }
    }
}