using Sorter.Sorters;

namespace Sorter;

public partial class MainForm : Form
{
    IProgressReporting? worker;
    CancellationTokenSource cancellation = new();
    bool fileSelected = false;

    public MainForm()
    {
        InitializeComponent();
        comboBox_bufferSizeUnit.SelectedIndex = 1;
        comboBox_runSizeUnit.SelectedIndex = 0;
    }

    private void button_openFile_Click(object sender, EventArgs e)
    {
        if (openFileDialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        label_fileName.Text = Path.GetFileName(openFileDialog.FileName);
        fileSelected = true;
    }

    private void button_mergeSort_Click(object sender, EventArgs e)
    {
        if (worker != null)
        {
            cancellation.Cancel();
            return;
        }

        if (!int.TryParse(textBox_bufferSize.Text, out int bufferSize))
        {
            MessageBox.Show("Invalid buffer size");
            return;
        }

        if (!long.TryParse(textBox_runSize.Text, out long runSize))
        {
            MessageBox.Show("Invalid run size");
            return;
        }

        if (!int.TryParse(textBox_threads.Text, out int maxThreads))
        {
            MessageBox.Show("Invalid max threads");
            return;
        }

        bufferSize *= (int)GetUnitMultiplier(comboBox_bufferSizeUnit.SelectedIndex);
        runSize *= GetUnitMultiplier(comboBox_runSizeUnit.SelectedIndex);

        Task.Run(() =>
        {
            cancellation = new();

            MergeSorter mergeSorter = new(openFileDialog.FileName)
            {
                BufferSize = bufferSize,
                RunSize = runSize,
                MaxThreads = maxThreads
            };
            mergeSorter.Log += Log;
            worker = mergeSorter;
            mergeSorter.Sort(cancellation.Token);
            worker = null;
        });
    }

    private void button_heapSort_Click(object sender, EventArgs e)
    {
        if (worker != null)
        {
            cancellation.Cancel();
            return;
        }

        if (!int.TryParse(textBox_bufferSize.Text, out int bufferSize))
        {
            MessageBox.Show("Invalid buffer size");
            return;
        }

        bufferSize *= (int)GetUnitMultiplier(comboBox_bufferSizeUnit.SelectedIndex);

        Task.Run(() =>
        {
            cancellation = new();

            HeapSorter heapSorter = new(openFileDialog.FileName)
            {
                BufferSize = bufferSize,
                UseRamIndex = checkBox_ramIndex.Checked
            };
            heapSorter.Log += Log;
            worker = heapSorter;
            heapSorter.Sort(cancellation.Token);
            worker = null;
        });
    }

    private void button_validate_Click(object sender, EventArgs e)
    {
        if (worker != null)
        {
            cancellation.Cancel();
            return;
        }

        Task.Run(() =>
        {
            cancellation = new();

            Validator validator = new(openFileDialog.FileName);
            validator.Log += Log;
            worker = validator;
            validator.Validate(cancellation.Token);
            worker = null;
        });
    }

    static long GetUnitMultiplier(int index)
    {
        return index switch
        {
            0 => 1_073_741_824L,  // GB
            1 => 1_048_576L,      // MB
            2 => 1_024L,          // KB
            _ => 1L
        };
    }

    void Log(string message)
    {
        if (InvokeRequired)
        {
            Invoke(new Action<string>(Log), message);
            return;
        }

        if (worker != null)
        {
            progressBar.Value = Math.Min(100, worker.Progress);
        }

        richTextBox.AppendText($"{DateTime.Now:T}: {message}{Environment.NewLine}");
    }

    private void timer_Tick(object sender, EventArgs e)
    {
        if (worker != null)
        {
            progressBar.Value = Math.Min(100, worker.Progress);
        }
        else
        {
            button_heapSort.Enabled = true;
            button_mergeSort.Enabled = true;
            button_validate.Enabled = true;
            button_heapSort.Text = "Heap sort";
            button_mergeSort.Text = "Merge sort";
            button_validate.Text = "Validate";
        }

        if (worker is HeapSorter)
        {
            button_heapSort.Enabled = true;
            button_heapSort.Text = "Cancel";
        }

        if (worker is MergeSorter)
        {
            button_mergeSort.Enabled = true;
            button_mergeSort.Text = "Cancel";
        }

        if (worker is Validator)
        {
            button_validate.Enabled = true;
            button_validate.Text = "Cancel";
        }

        if (worker!= null && worker is not HeapSorter)
        {
            button_heapSort.Enabled = false;
            button_heapSort.Text = "Heap sort";
        }

        if (worker != null && worker is not MergeSorter)
        {
            button_mergeSort.Enabled = false;
            button_mergeSort.Text = "Merge sort";
        }

        if (worker != null && worker is not Validator)
        {
            button_validate.Enabled = false;
            button_validate.Text = "Validate";
        }

        if (!fileSelected)
        {
            button_heapSort.Enabled = false;
            button_mergeSort.Enabled = false;
            button_validate.Enabled = false;
        }
    }
}
