using Sorter.DataStructures;
using System.IO.MemoryMappedFiles;

namespace Sorter.Sorters;

/// <summary>
/// Creates an index containing addresses and lengths of every line in 
/// the source file (either in RAM or on a disk), sorts the index and
/// constructs a sorted file from it
/// 
/// Limitations:
/// Line length < BufferSize
/// Line length < ushort.MaxValue
/// </summary>
public class HeapSorter(string fileName) : IProgressReporting
{
    /// <summary>
    /// I/O buffer size
    /// </summary>
    public int BufferSize { get; set; } = 1024 * 1024;
    /// <summary>
    /// Create index in RAM instead of in a file on a disk
    /// </summary>
    public bool UseRamIndex { get; set; }
    public string IndexFileName { get; set; } = "index.bin";
    public event Action<string> Log = null!;
    public int Progress { get; private set; } = 0;

    long sourceSize;
    long lineCount;
    long indexIncrement;

    MemoryMappedFile source = null!;
    MemoryMappedViewAccessor sourceAccessor = null!;
    MemoryMappedFile index = null!;
    MemoryMappedViewAccessor indexAccessor = null!;

    //An idex is a jagged array to avoid reallocations
    readonly List<ulong[]> ramIndex = [];

    readonly Statistics statistics = new();

    public void Sort(CancellationToken cancellationToken)
    {
        Log?.Invoke("Heap sort started");

        statistics.Reset();
        DateTime start = DateTime.Now;
        FileInfo fileInfo = new(fileName);
        sourceSize = fileInfo.Length;

        try
        {
            CreateIndex(cancellationToken);

            source = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open);
            sourceAccessor = source.CreateViewAccessor();

            HeapSort(cancellationToken);
            WriteToOutputFile(cancellationToken);
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
    /// Creates an index of a source file
    /// </summary>
    void CreateIndex(CancellationToken cancellationToken)
    {
        Log?.Invoke("Creating index...");
        using DataFile source = new(fileName, DataFile.Mode.Read, BufferSize);
        indexIncrement = UseRamIndex ? 1_000_000 : Math.Max(100, sourceSize / 100);
        lineCount = 0;
        Progress = 0;

        if (UseRamIndex)
        {
            ramIndex.Add(new ulong[indexIncrement]);
        }
        else
        {
            if (File.Exists(IndexFileName))
            {
                File.Delete(IndexFileName);
            }

            index = MemoryMappedFile.CreateFromFile(IndexFileName, FileMode.Create, null, indexIncrement);
            indexAccessor = index.CreateViewAccessor();
        }

        long bytesProcessed = 0;
        while (!source.EndReached)
        {
            Address address = source.ReadLineAddress();
            AddToIndex(lineCount, address.Position, address.Length);
            lineCount++;
            bytesProcessed += address.Length + 2;
            Progress = (int)(bytesProcessed * 100 / sourceSize);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
        }

        statistics.IndexWrites += lineCount;
        statistics.BulkReads += source.BulkReads;
    }

    void AddToIndex(long index, long position, ushort length)
    {
        ulong raw = Address.ToRaw(position, length);

        if (UseRamIndex)
        {
            //If the index is too small - we add a new array of a fixed size
            if ((index + 1) * sizeof(ulong) > ramIndex.Count * indexIncrement)
            {
                ramIndex.Add(new ulong[indexIncrement]);
            }

            ramIndex[(int)(index / indexIncrement)][(int)(index % indexIncrement)] = raw;

            return;
        }

        //If the index is too small - we increase file size and recteate MemoryMappedFile and MemoryMappedViewAccessor
        if ((index + 1) * sizeof(ulong) > indexAccessor.Capacity)
        {
            long newCapacity = indexAccessor.Capacity + indexIncrement;
            indexAccessor.Dispose();
            this.index.Dispose();

            this.index = MemoryMappedFile.CreateFromFile(IndexFileName, FileMode.Open, null, newCapacity);
            indexAccessor = this.index.CreateViewAccessor();
        }

        indexAccessor.Write(index * sizeof(ulong), raw);
    }

    /// <summary>
    /// Sorts the index using heap sort algorithm
    /// </summary>
    void HeapSort(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        Log?.Invoke("Preparing sort...");
        Progress = 0;
        long initialIndex = lineCount / 2 - 1;

        for (long i = initialIndex; i >= 0; i--)
        {
            Heapify(lineCount, i);

            if (initialIndex > 0)
            {
                Progress = (int)((initialIndex - i) * 100 / initialIndex);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
        }

        Log?.Invoke("Sorting index...");
        Progress = 0;

        for (long i = lineCount - 1; i >= 0; i--)
        {
            SwapValues(i, 0);
            Heapify(i, 0);
            Progress = (int)((lineCount - i) * 100 / lineCount);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
        }
    }

    //We try to reuse as much index and vaue information as possible
    //without unnessesarily reading the index of the source
    void Heapify(long size, long index, Address? indexAddress = null, Line? indexValue = null)
    {
        long largestIndex = index;
        long leftChild = 2 * index + 1;
        long rightChild = 2 * index + 2;

        indexAddress ??= GetAddress(index);
        indexValue ??= GetValue(indexAddress.Value);
        Address largestIndexAddress = indexAddress.Value;
        Line largestIndexValue = indexValue.Value;

        if (leftChild < size)
        {
            Address leftChildAddress = GetAddress(leftChild);
            Line leftChildValue = GetValue(leftChildAddress);
            if (leftChildValue.CompareTo(largestIndexValue) > 0)
            {
                largestIndex = leftChild;
                largestIndexAddress = leftChildAddress;
                largestIndexValue = leftChildValue;
            }
        }
        if (rightChild < size)
        {
            Address rightChildAddress = GetAddress(rightChild);
            Line rightChildValue = GetValue(rightChildAddress);
            if (rightChildValue.CompareTo(largestIndexValue) > 0)
            {
                largestIndex = rightChild;
                largestIndexAddress = rightChildAddress;
            }
        }

        if (largestIndex != index)
        {
            SwapValues(index, largestIndex, indexAddress, largestIndexAddress);
            Heapify(size, largestIndex, indexAddress, indexValue);
        }
    }

    Address GetAddress(long index)
    {
        if (UseRamIndex)
        {
            statistics.IndexReads++;
            ulong raw = ramIndex[(int)(index / indexIncrement)][(int)(index % indexIncrement)];
            return new(raw);
        }
        else
        {
            indexAccessor.Read(index * sizeof(ulong), out ulong raw);

            Address address = new(raw);
            return address;
        }
    }

    Line GetValue(Address address)
    {
        byte[] bytes = new byte[address.Length];
        sourceAccessor.ReadArray(address.Position, bytes, 0, address.Length);
        statistics.LineReads++;
        return Line.Parse(bytes, 0, address.Length);
    }

    Line GetValue(long index)
    {
        return GetValue(GetAddress(index));
    }

    void SwapValues(long index1, long index2, Address? address1 = null, Address? address2 = null)
    {
        address1 ??= GetAddress(index1);
        address2 ??= GetAddress(index2);

        if (UseRamIndex)
        {
            ramIndex[(int)(index1 / indexIncrement)][(int)(index1 % indexIncrement)] = address2.Value.Raw;
            ramIndex[(int)(index2 / indexIncrement)][(int)(index2 % indexIncrement)] = address1.Value.Raw;
        }
        else
        {
            indexAccessor.Write(index1 * sizeof(ulong), address2.Value.Raw);
            indexAccessor.Write(index2 * sizeof(ulong), address1.Value.Raw);
        }
        statistics.IndexWrites += 2;
    }

    /// <summary>
    /// Creates an output file from the source file and the sorted index
    /// </summary>
    void WriteToOutputFile(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        Log?.Invoke("Writing to output...");
        Progress = 0;

        string outputFileName = Path.Combine(Path.GetDirectoryName(fileName)!, $"{Path.GetFileNameWithoutExtension(fileName)}_tmp.{Path.GetExtension(fileName)}");
        try
        {
            using (DataFile output = new(outputFileName, DataFile.Mode.Write, BufferSize, sourceSize))
            {
                for (long i = 0; i < lineCount; i++)
                {
                    Line line = GetValue(i);
                    output.WriteLine(line);
                    Progress = (int)(i * 100 / lineCount);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
                statistics.BulkWrites += output.BulkWrites;
            }

            sourceAccessor.Dispose();
            source.Dispose();

            File.Delete(fileName);
            File.Move(outputFileName, fileName);
            Progress = 100;
        }
        finally
        {
            if (File.Exists(outputFileName))
            {
                File.Delete(outputFileName);
            }
        }
    }

    void Cleanup()
    {
        ramIndex.Clear();
        sourceAccessor?.Dispose();
        indexAccessor?.Dispose();
        source?.Dispose();
        index?.Dispose();
    }
}
