using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace datasets
{
    public class HaikuInfo
    {
        [Index(0)]
        public string Line0 { get; set; }
        [Index(1)]
        public string Line1 { get; set; }
        [Index(2)]
        public string Line2 { get; set; }
        [Index(3)]
        public string Source { get; set; }
        [Index(4)]
        public string SyllableCountLine0 { get; set; }
        [Index(5)]
        public string SyllableCountLine1 { get; set; }
        [Index(6)]
        public string SyllableCountLine2 { get; set; }
    }

    class Program
    {
        private const string endTextToken = "<|endoftext|>";
        private static string[] allSources = new[]
        {
            "gutenberg",
            "haikuzao",
            "img2poems",
            "sballas",
            "tempslibres",
            "twaiku",
        };

        static void Main(string[] args)
        {
            // Create separate files
            // Write as plaintext with endTextToken
            var currentPath = Directory.GetCurrentDirectory();
            var inputFile = "docmarionum1-haikurnn-haikus.csv";

            var fileWriters = new Dictionary<string, StreamWriter>();
            foreach (var source in allSources)
            {
                var fileName = $"{currentPath}\\{source}.txt";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                fileWriters.Add(source, File.CreateText(fileName));
            }

            var count = 0;
            using (var reader = new StreamReader($"{currentPath}\\{inputFile}"))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<HaikuInfo>();
                foreach (var record in records)
                {
                    var writer = fileWriters[record.Source];

                    if (ShouldSkip(record))
                    {
                        continue;
                    }

                    writer.WriteLine(record.Line0);
                    writer.WriteLine(record.Line1);
                    writer.WriteLine(record.Line2);
                    writer.WriteLine(endTextToken);
                    writer.WriteLine(string.Empty);
                    count++;
                    if (count % 1000 == 0)
                    {
                        Console.WriteLine($"Processed {count} haiku.");
                    }
                }
            }

            foreach (var writer in fileWriters)
            {
                writer.Value.Dispose();
            }

            Console.WriteLine($"Processed {count} haiku. Press any key to exit");
            Console.ReadKey();
        }

        private static bool ShouldSkip(HaikuInfo haikuInfo)
        {
            return haikuInfo.SyllableCountLine0.Equals("0") ||
                haikuInfo.SyllableCountLine1.Equals("0") ||
                haikuInfo.SyllableCountLine2.Equals("0");
        }
    }
}
