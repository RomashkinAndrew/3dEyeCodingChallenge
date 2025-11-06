using System.IO.MemoryMappedFiles;
using System.Text;

namespace Sorter.DataStructures;

/// <summary>
/// Wrapper around text files containing data lines.
/// Implements line read/write operations
/// </summary>
public class DataFile : IDisposable
{
    readonly MemoryMappedFile file;
    readonly MemoryMappedViewAccessor accessor;

    public long LengthInBytes { get; private set; }
    public bool EndReached { get; private set; }
    public long BulkReads { get; private set; }
    public long BulkWrites { get; private set; }

    readonly byte[] buffer;
    long filePointer = 0;
    long bufferStartPosition = 0;
    int bufferPointer = 0;
    int bytesInBuffer = 0;
    private bool disposedValue;

    public enum Mode { Read, Write }
    //Syntax salt to prevent mixing up read and write operations
    readonly Mode mode;

    public DataFile(string name, Mode mode, int bufferSize = 1024 * 1024, long? lengthInBytes = null)
    {
        if (lengthInBytes == null)
        {
            FileInfo fileInfo = new(name);
            lengthInBytes = fileInfo.Length;
        }

        LengthInBytes = lengthInBytes.Value;
        this.mode = mode;
        buffer = new byte[bufferSize];

        file = MemoryMappedFile.CreateFromFile(name, FileMode.OpenOrCreate, null, LengthInBytes);
        accessor = file.CreateViewAccessor();
    }

    /// <summary>
    /// Reads a line from the file
    /// </summary>
    public Line ReadLine()
    {
        if (mode == Mode.Write)
        {
            throw new NotSupportedException("File is in a write mode");
        }

        if (filePointer == 0 && bufferPointer == 0)
        {
            bufferPointer = buffer.Length;
            ReadToBuffer();
        }

        if (TryExtractLine(out Line line))
        {
            return line;
        }

        if (filePointer == LengthInBytes)
        {
            return ExtractRemainder();
        }

        ReadToBuffer();

        if (TryExtractLine(out line))
        {
            return line;
        }
        return ExtractRemainder();
    }

    /// <summary>
    /// Reads only address of the line, without parsing it
    /// </summary>
    public Address ReadLineAddress()
    {
        if (mode == Mode.Write)
        {
            throw new NotSupportedException("File is in a write mode");
        }

        if (filePointer == 0 && bufferPointer == 0)
        {
            bufferPointer = buffer.Length;
            ReadToBuffer();
        }

        if (TryExtractLineAddress(out Address address))
        {
            return address;
        }

        if (filePointer == LengthInBytes)
        {
            return ExtractRemainderAddress();
        }

        ReadToBuffer();

        if (TryExtractLineAddress(out address))
        {
            return address;
        }
        return ExtractRemainderAddress();
    }

    void ReadToBuffer()
    {
        int remainderLength = buffer.Length - bufferPointer;

        for (int i = 0; i < remainderLength; i++)
        {
            buffer[i] = buffer[buffer.Length - remainderLength + i];
        }

        int bytesToRead = (int)Math.Min(buffer.Length - remainderLength, LengthInBytes - filePointer);

        accessor.ReadArray(filePointer, buffer, remainderLength, bytesToRead);

        bufferStartPosition = filePointer - remainderLength;
        filePointer += bytesToRead;
        bufferPointer = 0;
        bytesInBuffer = remainderLength + bytesToRead;
        BulkReads++;
    }

    bool TryExtractLine(out Line line)
    {
        for (int i = bufferPointer; i < bytesInBuffer; i++)
        {
            if (buffer[i] == '\n')
            {
                line = Line.Parse(buffer, bufferPointer, i - bufferPointer - 1);
                bufferPointer = i + 1;
                return true;
            }
        }
        line = new();
        return false;
    }

    bool TryExtractLineAddress(out Address address)
    {
        for (int i = bufferPointer; i < bytesInBuffer; i++)
        {
            if (buffer[i] == '\n')
            {
                address = new Address(bufferStartPosition + bufferPointer, (ushort)(i - bufferPointer - 1));
                bufferPointer = i + 1;
                return true;
            }
        }
        address = new();
        return false;
    }

    Line ExtractRemainder()
    {
        EndReached = true;
        return Line.Parse(buffer, bufferPointer, bytesInBuffer - bufferPointer);
    }

    Address ExtractRemainderAddress()
    {
        EndReached = true;
        return new Address(bufferStartPosition + bufferPointer, (ushort)(bytesInBuffer - bufferPointer));
    }

    /// <summary>
    /// Writes a line to the file 
    /// </summary>
    public void WriteLine(Line line)
    {
        WriteLine(line.ToString());
    }

    /// <summary>
    /// Writes a line to the file 
    /// </summary>
    public void WriteLine(string line)
    {
        if (mode == Mode.Read)
        {
            throw new NotSupportedException("File is in a read mode");
        }

        byte[] bytes = Encoding.ASCII.GetBytes(line);

        if (bufferPointer + bytes.Length + 2 > buffer.Length)
        {
            Flush();
        }

        if (filePointer != 0 || bufferPointer != 0)
        {
            buffer[bufferPointer++] = (byte)'\r';
            buffer[bufferPointer++] = (byte)'\n';
        }

        Array.Copy(bytes, 0, buffer, bufferPointer, bytes.Length);
        bufferPointer += bytes.Length;
    }

    /// <summary>
    /// Flushes all unwritten ;ines to the file
    /// </summary>
    public void Flush()
    {
        if (mode == Mode.Read)
        {
            throw new NotSupportedException("File is in a read mode");
        }

        accessor.WriteArray(filePointer, buffer, 0, bufferPointer);
        filePointer += bufferPointer;
        bufferPointer = 0;
        BulkWrites++;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (mode == Mode.Write)
                {
                    Flush();
                }

                accessor.Dispose();
                file.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
