using System.Diagnostics;
using System.Text;

namespace TestFileGenerator;

/// <summary>
/// Generates a file that consists of lines of a following format:
/// <number>.<string>
/// with some lines having the same string part.
/// The file uses ASCII encoding to fit more lines in the file
/// </summary>
public static class Generator
{
    public enum GenerationResult { Success, Canceled, Error }
    public delegate void ReportProgressDelegate(int percentage);

    /// <summary>
    /// Generates a file with specified parameters
    /// </summary>
    public static GenerationResult Generate(string path, ulong size, double repeatProbability, int maxStringLength, int maxNumber, ref int progress, CancellationToken cancellationToken)
    {
        try
        {
            ulong bytesGenerated = 0;
            ulong saveLineEveryNBytes = size / 1000;

            List<string> existingStrings = [];

            using (StreamWriter outputFile = new(path, false, Encoding.ASCII, 100*1024*1024))
            {
                while (!cancellationToken.IsCancellationRequested && bytesGenerated < size)
                {
                    Stopwatch sw1 = Stopwatch.StartNew();
                    int length = Random.Shared.Next(1, maxStringLength + 1);
                    bool repeat = Random.Shared.NextDouble() < repeatProbability;
                    int number = Random.Shared.Next(0, maxNumber + 1);

                    if (repeat && existingStrings.Count == 0)
                    {
                        repeat = false;
                    }

                    sw1.Stop();
                    Stopwatch sw2 = Stopwatch.StartNew();

                    string str;
                    if (repeat)
                    {
                        str = existingStrings[Random.Shared.Next(existingStrings.Count)];
                    }
                    else
                    {
                        str = GenerateRandomString(length);
                    }

                    if (bytesGenerated > (ulong)existingStrings.Count * saveLineEveryNBytes)
                    {
                        existingStrings.Add(str);
                    }

                    sw2.Stop();
                    Stopwatch sw3 = Stopwatch.StartNew();

                    string line;
                    if (bytesGenerated == 0)
                    {
                        line = $"{number}.{str}";
                    }
                    else
                    {
                        line = $"{Environment.NewLine}{number}.{str}";
                    }
                    outputFile.Write(line);

                    bytesGenerated += (ulong)line.Length;

                    progress = (int)(bytesGenerated * 100 / size);

                    sw3.Stop();
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                return GenerationResult.Canceled;
            }
        }
        catch
        {
            return GenerationResult.Error;
        }

        return GenerationResult.Success;
    }

    public static string GenerateRandomString(int length)
    {
        const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        StringBuilder sb = new(length);

        for (int i = 0; i < length; i++)
        {
            sb.Append(allowedChars[Random.Shared.Next(allowedChars.Length)]);
        }

        return sb.ToString();
    }
}
