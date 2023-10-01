using System;
using System.IO;
using System.IO.Compression;

class Program
{
    static void Main()
    {
        Console.Write(@"Enter the folder path (e.g.: D:\Downloads\WRZ): ");
        string folderPath = Console.ReadLine();

        if (Directory.Exists(folderPath))
        {
            ProcessFiles(folderPath);
        }
        else
        {
            Console.WriteLine("Invalid folder path.");
        }
    }
    static void ProcessFiles(string folderPath)
    {
        string[] files = Directory.GetFiles(folderPath, "*.wrz");

        foreach (string file in files)
        {
            if (!file.Contains("_processed"))
            {
                string extension = Path.GetExtension(file);

                string extractedFilePath = Decompress(new FileInfo(file));

                // Read the text file and perform replacements
                string[] lines = File.ReadAllLines(extractedFilePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    string old = lines[i];
                    old = old.Replace("ä", "ae");
                    old = old.Replace("ö", "oe");
                    old = old.Replace("ü", "ue");
                    old = old.Replace("Ä", "Ae");
                    old = old.Replace("Ö", "Oe");
                    old = old.Replace("Ü", "Ue");
                    old = old.Replace("ß", "ss");
                    lines[i] = old;
                }

                // Write the modified text back to the file
                File.WriteAllLines(extractedFilePath, lines);

                CompressFile(extractedFilePath, extractedFilePath + "_processed.wrz");

                // Clean up temporary extracted file
                File.Delete(extractedFilePath);

            }
        }

        Console.WriteLine("Process completed successfully.");
    }
    public static string Decompress(FileInfo fileToDecompress)
    {
        using (FileStream originalFileStream = fileToDecompress.OpenRead())
        {
            string currentFileName = fileToDecompress.FullName;
            string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

            using (FileStream decompressedFileStream = File.Create(newFileName))
            {
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(decompressedFileStream);
                    Console.WriteLine("working on: {0}", fileToDecompress.Name);
                }
            }
            return newFileName;
        }

    }

    public static void CompressFile(string fileToCompress, string outputZipFilePath)
    {
        using FileStream originalFileStream = File.Open(fileToCompress, FileMode.Open);
        using FileStream compressedFileStream = File.Create(outputZipFilePath);
        using var compressor = new GZipStream(compressedFileStream, CompressionMode.Compress);
        originalFileStream.CopyTo(compressor);
    }
}