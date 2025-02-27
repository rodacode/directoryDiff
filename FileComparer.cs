using System;
using System.IO;
using System.Security.Cryptography;

namespace DirectoryDiff
{
    public class FileComparer
    {
        private bool _compareContent = false;

        public FileComparer(bool compareContent = false)
        {
            _compareContent = compareContent;
        }

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

            // If content comparison is enabled and sizes match, compare contents
            if (_compareContent)
            {
                return CompareFileContents(filePath1, filePath2);
            }
            else
            {
                // Compare last modified dates if not comparing content
                if (fileInfo1.LastWriteTimeUtc != fileInfo2.LastWriteTimeUtc)
                {
                    return false;
                }

                // Consider files identical if size and date match
                return true;
            }
        }

        private bool CompareFileContents(string filePath1, string filePath2)
        {
            // Use a buffer size that balances memory usage and performance
            const int bufferSize = 4096;
            
            using (var file1 = File.OpenRead(filePath1))
            using (var file2 = File.OpenRead(filePath2))
            {
                byte[] buffer1 = new byte[bufferSize];
                byte[] buffer2 = new byte[bufferSize];
                
                int bytesRead1, bytesRead2;
                
                // Read and compare chunks of the files
                do
                {
                    bytesRead1 = file1.Read(buffer1, 0, bufferSize);
                    bytesRead2 = file2.Read(buffer2, 0, bufferSize);
                    
                    if (bytesRead1 != bytesRead2)
                    {
                        return false;
                    }
                    
                    for (int i = 0; i < bytesRead1; i++)
                    {
                        if (buffer1[i] != buffer2[i])
                        {
                            return false;
                        }
                    }
                }
                while (bytesRead1 > 0);
                
                return true;
            }
        }

        public FileDifference GetFileDifference(string filePath1, string filePath2, string relativePath)
        {
            var fileInfo1 = new FileInfo(filePath1);
            var fileInfo2 = new FileInfo(filePath2);
            
            bool contentSame = false;
            if (_compareContent && fileInfo1.Length == fileInfo2.Length)
            {
                contentSame = CompareFileContents(filePath1, filePath2);
            }

            return new FileDifference
            {
                FileName = relativePath,
                Size1 = fileInfo1.Length,
                Size2 = fileInfo2.Length,
                Modified1 = fileInfo1.LastWriteTimeUtc,
                Modified2 = fileInfo2.LastWriteTimeUtc,
                ContentSame = contentSame
            };
        }
    }
}