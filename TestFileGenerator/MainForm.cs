using System.Globalization;
using static TestFileGenerator.Generator;

namespace TestFileGenerator;

public partial class MainForm : Form
{
    bool inProgress = false;
    CancellationTokenSource cancellation = new();
    GenerationResult result;
    int progress = 0;

    public MainForm()
    {
        InitializeComponent();
        comboBox_sizeUnit.SelectedIndex = 0;
    }

    private void GenerateButton_Click(object sender, EventArgs e)
    {
        if (inProgress)
        {
            cancellation.Cancel();
            GenerateButton.Text = "Generate";
            Log("Cancellation requested...");
            return;
        }

        if (saveFileDialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        if (!ulong.TryParse(textBox_fileSize.Text, out ulong fileSize))
        {
            MessageBox.Show("Invalid file size");
            return;
        }

        if (!double.TryParse(textBox_probability.Text, CultureInfo.InvariantCulture, out double repeatProbability))
        {
            MessageBox.Show("Invalid repeat probability");
            return;
        }

        if (repeatProbability < 0 || repeatProbability > 1)
        {
            MessageBox.Show("Repeat probability must be between 0 and 1");
            return;
        }

        if (!int.TryParse(textBox_maxLength.Text, out int maxStringLength))
        {
            MessageBox.Show("Invalid max string length");
            return;
        }

        if (!int.TryParse(textBox_maxNumber.Text, out int maxNumber))
        {
            MessageBox.Show("Invalid max number");
            return;
        }

        if (maxNumber <= 0)
        {
            MessageBox.Show("Max number must be greater than 0");
            return;
        }

        ulong unitMultiplier = comboBox_sizeUnit.SelectedIndex switch
        {
            0 => 1_073_741_824UL,  // GB
            1 => 1_048_576UL,      // MB
            2 => 1_024UL,          // KB
            _ => 1UL
        };

        fileSize *= unitMultiplier;

        inProgress = true;
        GenerateButton.Text = "Cancel";
        cancellation = new CancellationTokenSource();
        Log("Generating a file...");

        Task.Run(() => result = Generate(saveFileDialog.FileName, fileSize, repeatProbability, maxStringLength, maxNumber, ref progress, cancellation.Token))
             .ContinueWith(t =>
             {
                 inProgress = false;
                 GenerateButton.Text = "Generate";

                 switch (result)
                 {
                     case GenerationResult.Success:
                         progress = 100;
                         Log("File generated successfully");
                         break;
                     case GenerationResult.Canceled:
                         progress = 0;
                         Log("File generation cancelled");
                         break;
                     case GenerationResult.Error:
                         progress = 0;
                         Log("An error occurred during file generation");
                         break;
                 }
             }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    void Log(string message)
    {
        if (InvokeRequired)
        {
            Invoke(new Action<string>(Log), message);
            return;
        }
        richTextBox.AppendText($"{DateTime.Now:T}: {message}{Environment.NewLine}");
    }

    private void timer_Tick(object sender, EventArgs e)
    {
        progressBar.Value = progress;
    }
}
