namespace Sorter.DataStructures;

public class Statistics
{
    /// <summary>
    /// Execution time
    /// </summary>
    public TimeSpan Time { get; set; }
    /// <summary>
    /// Reads of a single line from the file
    /// </summary>
    public long LineReads { get; set; }
    /// <summary>
    /// Writes of a single line to the file
    /// </summary>
    public long LineWrites { get; set; }
    /// <summary>
    /// Reads of an address from an index
    /// </summary>
    public long IndexReads { get; set; }
    /// <summary>
    /// Writes of an address from an index
    /// </summary>
    public long IndexWrites { get; set; }
    /// <summary>
    /// Reads of a set of lines from the file at once
    /// </summary>
    public long BulkReads { get; set; }
    /// <summary>
    /// Writes of a set of lines from the file at once
    /// </summary>
    public long BulkWrites { get; set; }

    public void Reset()
    {
        Time = TimeSpan.Zero;
        LineReads = 0;
        LineWrites = 0;
        IndexReads = 0;
        IndexWrites = 0;
        BulkReads = 0;
        BulkWrites = 0;
    }

    public override string ToString()
    {
        List<string> strings = [];

        strings.Add($"Sort completed!");
        strings.Add($"Time: {Time:hh\\:mm\\:ss}");
        if (LineReads > 0)
        {
            strings.Add($"Line reads: {LineReads}");
        }
        if (LineWrites > 0)
        {
            strings.Add($"Line writes: {LineWrites}");
        }
        if (IndexReads > 0)
        {
            strings.Add($"Index reads: {IndexReads}");
        }
        if (IndexWrites > 0)
        {
            strings.Add($"Index writes: {IndexWrites}");
        }
        if (BulkReads > 0)
        {
            strings.Add($"Bulk reads: {BulkReads}");
        }
        if (BulkWrites > 0)
        {
            strings.Add($"Bulk writes: {BulkWrites}");
        }

        return string.Join(Environment.NewLine, strings);
    }
}