using System;
using System.Collections.Generic;

namespace DirectoryDiff
{
    public class ComparisonResult
    {
        public List<string> OnlyInFirst { get; set; } = new List<string>();
        public List<string> OnlyInSecond { get; set; } = new List<string>();
        public List<FileDifference> Different { get; set; } = new List<FileDifference>();
        public List<string> Identical { get; set; } = new List<string>();
    }

    public class FileDifference
    {
        public string FileName { get; set; }
        public long Size1 { get; set; }
        public long Size2 { get; set; }
        public DateTime Modified1 { get; set; }
        public DateTime Modified2 { get; set; }
        public bool ContentSame { get; set; }
    }
}