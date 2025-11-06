using Sorter.DataStructures;

namespace Sorter.Sorters;

/// <summary>
/// The file is divided into chunks (called runs), which are loaded into RAM sequentially or in parallel.
/// Each run is sorted and written to disk. Then runs are being merged by reading lines 
/// from all runs simultaneously, and the smallest line is written to the output file.
/// 
/// Limitations:
/// Line length < BufferSize
/// RunSize * MaxThreads should fit into RAM
/// </summary>

public class MergeSorter(string fileName) : IProgressReporting
{
    int runCount;
    long totalLines;

    /// <summary>
    /// I/O buffer size
    /// </summary>
    public int BufferSize { get; set; } = 1024 * 1024;
    /// <summary>
    /// Max degree of parallelism during the run creation phase
    /// </summary>
    public int MaxThreads { get; set; } = 5;
    /// <summary>
    /// Size of a single run in bytes
    /// </summary>
    public long RunSize { get; set; } = 1 * 1024 * 1024 * 1024;
    public string RunFolder { get; set; } = "Runs";
    public int Progress { get; private set; }
    public event Action<string>? Log = null;

    readonly Statistics statistics = new();

    public void Sort(CancellationToken cancellationToken)
    {
        Log?.Invoke("Merge sort started");
        statistics.Reset();
        DateTime start = DateTime.Now;

        if (Directory.Exists(RunFolder))
        {
            Directory.Delete(RunFolder, true);
        }

        Directory.CreateDirectory(RunFolder);

        try
        {
            CreateRuns(cancellationToken);
            MergeRuns(cancellationToken);
        }
        finally
        {
            Cleanup();
        }

        if (cancellationToken.IsCancellationRequested)
        {
            Progress = 0;
            Log?.Invoke("Cancelled");
            return;
        }

        statistics.Time = DateTime.Now - start;
        Log?.Invoke(statistics.ToString());
    }

    /// <summary>
    /// Creates runs, sorts them and writes to disk
    /// </summary>
    void CreateRuns(CancellationToken cancellationToken)
    {
        Log?.Invoke("Creating runs...");
        Progress = 0;
        totalLines = 0;
        long processedBytes = 0;

        using DataFile source = new(fileName, DataFile.Mode.Read, BufferSize);

        runCount = (int)Math.Ceiling((double)source.LengthInBytes / RunSize);

        totalLines = 0;

        Parallel.For(0, runCount, new ParallelOptions() { MaxDegreeOfParallelism = MaxThreads }, (i) =>
        {
            List<Line> lines = [];

            long runLengthBytes = 0;
            lock (source)
            {
                while (runLengthBytes < RunSize && !source.EndReached)
                {
                    Line line = source.ReadLine();
                    lines.Add(line);

                    runLengthBytes += line.Length + 2;
                    processedBytes += line.Length + 2;
                    Interlocked.Increment(ref totalLines);

                    Progress = (int)(processedBytes * 100 / source.LengthInBytes / 3);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
            runLengthBytes -= 2;

            if (lines.Count == 0)
            {
                return;
            }

            lines.Sort();

            processedBytes += runLengthBytes;
            Progress = (int)(processedBytes * 100 / source.LengthInBytes/3);

            using DataFile run = new(GetRunFileName(i), DataFile.Mode.Write, BufferSize, runLengthBytes);
            foreach (Line line in lines)
            {
                run.WriteLine(line);
                processedBytes += line.Length + 2;
                Progress = (int)(processedBytes * 100 / source.LengthInBytes / 3);
            }
            lines.Clear();
            statistics.BulkWrites += run.BulkWrites;
        });

        statistics.BulkReads += source.BulkReads;
    }

    /// <summary>
    /// Reads runs from the disk and merges them
    /// </summary>
    void MergeRuns(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        Log?.Invoke("Merging runs...");
        using DataFile destination = new(fileName, DataFile.Mode.Write, BufferSize);

        List<DataFile> runs = [];
        List<Line> currentLines = [];

        long linesProcessed = 0;

        try
        {
            for (int i = 0; i < runCount; i++)
            {
                string runFileName = GetRunFileName(i);
                if (!File.Exists(runFileName))
                {
                    continue;
                }
                DataFile run = new(runFileName, DataFile.Mode.Read, BufferSize);
                runs.Add(run);
                currentLines.Add(runs[i].ReadLine());
            }

            while (runs.Count > 0)
            {
                int minIndex = 0;

                for (int i = 1; i < currentLines.Count; i++)
                {
                    if (currentLines[i] < currentLines[minIndex])
                    {
                        minIndex = i;
                    }
                }

                destination.WriteLine(currentLines[minIndex]);

                if (runs[minIndex].EndReached)
                {
                    runs[minIndex].Dispose();
                    runs.RemoveAt(minIndex);
                    currentLines.RemoveAt(minIndex);
                }
                else
                {
                    currentLines[minIndex] = runs[minIndex].ReadLine();
                    linesProcessed++;
                    Progress = (int)(linesProcessed * 100 / totalLines);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }

            statistics.BulkWrites += destination.BulkWrites;
        }
        finally
        {
            foreach (DataFile run in runs)
            {
                run.Dispose();
            }
        }
        Progress = 100;
    }

    string GetRunFileName(int index)
    {
        return Path.Combine(RunFolder, $"{index}.bin");
    }

    void Cleanup()
    {
        if (Directory.Exists(RunFolder))
        {
            Directory.Delete(RunFolder, true);
        }
    }
}
