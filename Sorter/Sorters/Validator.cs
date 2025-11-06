using Sorter.DataStructures;

namespace Sorter.Sorters;

public class Validator(string fileName) : IProgressReporting
{
    public int Progress { get; private set; }
    public event Action<string> Log = null!;

    public bool Validate(CancellationToken cancellationToken)
    {
        Log?.Invoke("Validation started");
        DateTime start = DateTime.Now;

        Progress = 0;
        using DataFile file = new(fileName, DataFile.Mode.Read);
        
        Line? previousLine = null;
        long bytesProcessed = 0; 
        long linesProcessed = 0;

        while(!file.EndReached)
        {
            Line line = file.ReadLine();

            if (!line.IsValid)
            {
                Progress = 100;
                Log?.Invoke($"Validation failed: invalid line in {DateTime.Now - start:hh\\:mm\\:ss}");
                return false;
            }

            if (previousLine.HasValue && previousLine.Value > line)
            {
                Progress = 100;
                Log?.Invoke($"Validation failed: lines are not in order in {DateTime.Now - start:hh\\:mm\\:ss}");
                return false;
            }

            previousLine = line;
            bytesProcessed+=line.Length+2;
            linesProcessed++;
            Progress = (int)(bytesProcessed * 100 / file.LengthInBytes);

            if (cancellationToken.IsCancellationRequested)
            {
                Progress = 0;
                Log?.Invoke("Cancelled");
                return false;
            }
        }

        Log?.Invoke($"Validation succeeded in {DateTime.Now - start:hh\\:mm\\:ss}");
        return true;
    }
}
