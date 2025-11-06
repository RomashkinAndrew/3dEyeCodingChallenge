namespace Sorter.Sorters;

public interface IProgressReporting
{
    public event Action<string> Log;
    public int Progress { get; }
}
