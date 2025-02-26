using System;
using System.IO;

namespace DirectoryDiff
{
    public class FileComparer
    {
        public bool AreFilesIdentical(string filePath1, string filePath2)
        {
            // Check if files exists
            if (!File.Exists(filePath1) || !File.Exists(filePath2))
            {
                return false;
            }

            // Compare file sizes first (quick check)
            var fileInfo1 = new FileInfo(filePath1);
            var fileInfo2 = new FileInfo(filePath2);

            if (fileInfo1.Length != fileInfo2.Length)
            {
                return false;
            }

            // Compare last modified dates
            // Note: We could make this comparison more flexible with a tolerance parameter
            if (fileInfo1.LastWriteTimeUtc != fileInfo2.LastWriteTimeUtc)
            {
                return false;
            }

            // For now, we'll consider files identical if size and date match
            // In a more advanced version, we could add actual content comparison
            return true;
        }

        public FileDifference GetFileDifference(string filePath1, string filePath2, string relativePath)
        {
            var fileInfo1 = new FileInfo(filePath1);
            var fileInfo2 = new FileInfo(filePath2);

            return new FileDifference
            {
                FileName = relativePath,
                Size1 = fileInfo1.Length,
                Size2 = fileInfo2.Length,
                Modified1 = fileInfo1.LastWriteTimeUtc,
                Modified2 = fileInfo2.LastWriteTimeUtc
            };
        }
    }
}