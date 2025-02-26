using System;
using System.IO;
using System.Linq;

namespace DirectoryDiff
{
    public class DirectoryComparer
    {
        private readonly FileComparer _fileComparer = new FileComparer();

        public ComparisonResult CompareDirectories(string directory1, string directory2)
        {
            var result = new ComparisonResult();

            // Get all files from both directories (non-recursive for now)
            var files1 = Directory.GetFiles(directory1).Select(f => Path.GetFileName(f)).ToList();
            var files2 = Directory.GetFiles(directory2).Select(f => Path.GetFileName(f)).ToList();

            // Find files only in directory1
            foreach (var file in files1.Except(files2))
            {
                result.OnlyInFirst.Add(file);
            }

            // Find files only in directory2
            foreach (var file in files2.Except(files1))
            {
                result.OnlyInSecond.Add(file);
            }

            // Compare common files
            foreach (var file in files1.Intersect(files2))
            {
                string fullPath1 = Path.Combine(directory1, file);
                string fullPath2 = Path.Combine(directory2, file);

                if (_fileComparer.AreFilesIdentical(fullPath1, fullPath2))
                {
                    result.Identical.Add(file);
                }
                else
                {
                    var difference = _fileComparer.GetFileDifference(fullPath1, fullPath2, file);
                    result.Different.Add(difference);
                }
            }

            return result;
        }
    }
}