using System;
using System.IO;
using System.Linq;

namespace WebApplication3.Utilities
{
    public static class DataPreprocessor
    {
        public static void EncodeData(string inputFile, string outputFile)
        {
            var lines = File.ReadAllLines(inputFile);
            var header = lines[0];
            var data = lines.Skip(1).Select(line =>
            {
                var columns = line.Split(',');

                // Encode Gender
                columns[5] = columns[5] switch
                {
                    "Male" => "1.0",
                    "Female" => "0.0",
                    "Other" => "2.0",
                    _ => "0.0"
                };

                // Encode EmployementType
                columns[6] = columns[6] switch
                {
                    "Full-time" => "1.0",
                    "Part-time" => "2.0",
                    "Contract" => "3.0",
                    "Freelance" => "4.0",
                    _ => "0.0"
                };

                // Encode Education
                columns[3] = columns[3] switch
                {
                    "High School" => "1.0",
                    "Bachelor" => "2.0",
                    "Master" => "3.0",
                    "PhD" => "4.0",
                    _ => "0.0"
                };

                // Encode Skills (Example: Machine Learning = 1, Data Science = 2)
                columns[4] = columns[4] switch
                {
                    "Frontend" => "1.0",
                    "Backend" => "2.0",
                    "Fullstack" => "3.0",
                    "Mobile" => "4.0",
                    "Machine Learning" => "5.0",
                    "DevOps" => "6.0",
                    "Cloud" => "7.0",
                    _ => "0.0"
                };

                return string.Join(",", columns);
            });

            File.WriteAllLines(outputFile, new[] { header }.Concat(data));
            Console.WriteLine("Encoding complete. Encoded file saved at: " + outputFile);
        }
    }
}
