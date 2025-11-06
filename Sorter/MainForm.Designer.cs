namespace Sorter;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        openFileDialog = new OpenFileDialog();
        button_heapSort = new Button();
        button_openFile = new Button();
        label_fileName = new Label();
        button_mergeSort = new Button();
        comboBox_runSizeUnit = new ComboBox();
        textBox_runSize = new TextBox();
        label1 = new Label();
        richTextBox = new RichTextBox();
        progressBar = new ProgressBar();
        button_validate = new Button();
        timer = new System.Windows.Forms.Timer(components);
        label2 = new Label();
        comboBox_bufferSizeUnit = new ComboBox();
        textBox_bufferSize = new TextBox();
        checkBox_ramIndex = new CheckBox();
        label3 = new Label();
        textBox_threads = new TextBox();
        SuspendLayout();
        // 
        // openFileDialog
        // 
        openFileDialog.FileName = "data.txt";
        // 
        // button_heapSort
        // 
        button_heapSort.Enabled = false;
        button_heapSort.Location = new Point(114, 206);
        button_heapSort.Name = "button_heapSort";
        button_heapSort.Size = new Size(96, 33);
        button_heapSort.TabIndex = 0;
        button_heapSort.Text = "Heap sort";
        button_heapSort.UseVisualStyleBackColor = true;
        button_heapSort.Click += button_heapSort_Click;
        // 
        // button_openFile
        // 
        button_openFile.Location = new Point(12, 12);
        button_openFile.Name = "button_openFile";
        button_openFile.Size = new Size(96, 33);
        button_openFile.TabIndex = 1;
        button_openFile.Text = "Open file";
        button_openFile.UseVisualStyleBackColor = true;
        button_openFile.Click += button_openFile_Click;
        // 
        // label_fileName
        // 
        label_fileName.AutoSize = true;
        label_fileName.Location = new Point(114, 21);
        label_fileName.Name = "label_fileName";
        label_fileName.Size = new Size(92, 15);
        label_fileName.TabIndex = 2;
        label_fileName.Text = "File not selected";
        // 
        // button_mergeSort
        // 
        button_mergeSort.Enabled = false;
        button_mergeSort.Location = new Point(12, 206);
        button_mergeSort.Name = "button_mergeSort";
        button_mergeSort.Size = new Size(96, 33);
        button_mergeSort.TabIndex = 3;
        button_mergeSort.Text = "Merge sort";
        button_mergeSort.UseVisualStyleBackColor = true;
        button_mergeSort.Click += button_mergeSort_Click;
        // 
        // comboBox_runSizeUnit
        // 
        comboBox_runSizeUnit.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBox_runSizeUnit.FormattingEnabled = true;
        comboBox_runSizeUnit.Items.AddRange(new object[] { "GB", "MB", "KB" });
        comboBox_runSizeUnit.Location = new Point(81, 108);
        comboBox_runSizeUnit.Name = "comboBox_runSizeUnit";
        comboBox_runSizeUnit.Size = new Size(41, 23);
        comboBox_runSizeUnit.TabIndex = 5;
        // 
        // textBox_runSize
        // 
        textBox_runSize.Location = new Point(12, 108);
        textBox_runSize.Name = "textBox_runSize";
        textBox_runSize.Size = new Size(63, 23);
        textBox_runSize.TabIndex = 4;
        textBox_runSize.Text = "100";
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(12, 90);
        label1.Name = "label1";
        label1.Size = new Size(136, 15);
        label1.TabIndex = 6;
        label1.Text = "(Merge sort) Run length:";
        // 
        // richTextBox
        // 
        richTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        richTextBox.Location = new Point(12, 274);
        richTextBox.Name = "richTextBox";
        richTextBox.Size = new Size(360, 193);
        richTextBox.TabIndex = 8;
        richTextBox.Text = "";
        // 
        // progressBar
        // 
        progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        progressBar.Location = new Point(12, 245);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(360, 23);
        progressBar.TabIndex = 7;
        // 
        // button_validate
        // 
        button_validate.Enabled = false;
        button_validate.Location = new Point(216, 206);
        button_validate.Name = "button_validate";
        button_validate.Size = new Size(96, 33);
        button_validate.TabIndex = 9;
        button_validate.Text = "Validate";
        button_validate.UseVisualStyleBackColor = true;
        button_validate.Click += button_validate_Click;
        // 
        // timer
        // 
        timer.Enabled = true;
        timer.Tick += timer_Tick;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(12, 48);
        label2.Name = "label2";
        label2.Size = new Size(138, 15);
        label2.TabIndex = 12;
        label2.Text = "Write/read buffer length:";
        // 
        // comboBox_bufferSizeUnit
        // 
        comboBox_bufferSizeUnit.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBox_bufferSizeUnit.FormattingEnabled = true;
        comboBox_bufferSizeUnit.Items.AddRange(new object[] { "GB", "MB", "KB" });
        comboBox_bufferSizeUnit.Location = new Point(81, 66);
        comboBox_bufferSizeUnit.Name = "comboBox_bufferSizeUnit";
        comboBox_bufferSizeUnit.Size = new Size(41, 23);
        comboBox_bufferSizeUnit.TabIndex = 11;
        // 
        // textBox_bufferSize
        // 
        textBox_bufferSize.Location = new Point(12, 66);
        textBox_bufferSize.Name = "textBox_bufferSize";
        textBox_bufferSize.Size = new Size(63, 23);
        textBox_bufferSize.TabIndex = 10;
        textBox_bufferSize.Text = "1";
        // 
        // checkBox_ramIndex
        // 
        checkBox_ramIndex.AutoSize = true;
        checkBox_ramIndex.Location = new Point(12, 181);
        checkBox_ramIndex.Name = "checkBox_ramIndex";
        checkBox_ramIndex.Size = new Size(188, 19);
        checkBox_ramIndex.TabIndex = 13;
        checkBox_ramIndex.Text = "(Heap sort) Store index in RAM";
        checkBox_ramIndex.UseVisualStyleBackColor = true;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(12, 134);
        label3.Name = "label3";
        label3.Size = new Size(203, 15);
        label3.TabIndex = 16;
        label3.Text = "(Merge sort) Max concurrent threads:";
        // 
        // textBox_threads
        // 
        textBox_threads.Location = new Point(12, 152);
        textBox_threads.Name = "textBox_threads";
        textBox_threads.Size = new Size(63, 23);
        textBox_threads.TabIndex = 14;
        textBox_threads.Text = "5";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(384, 479);
        Controls.Add(label3);
        Controls.Add(textBox_threads);
        Controls.Add(checkBox_ramIndex);
        Controls.Add(label2);
        Controls.Add(comboBox_bufferSizeUnit);
        Controls.Add(textBox_bufferSize);
        Controls.Add(button_validate);
        Controls.Add(richTextBox);
        Controls.Add(progressBar);
        Controls.Add(label1);
        Controls.Add(comboBox_runSizeUnit);
        Controls.Add(textBox_runSize);
        Controls.Add(button_mergeSort);
        Controls.Add(label_fileName);
        Controls.Add(button_openFile);
        Controls.Add(button_heapSort);
        Name = "MainForm";
        Text = "Sorter";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private OpenFileDialog openFileDialog;
    private Button button_heapSort;
    private Button button_openFile;
    private Label label_fileName;
    private Button button_mergeSort;
    private ComboBox comboBox_runSizeUnit;
    private TextBox textBox_runSize;
    private Label label1;
    private RichTextBox richTextBox;
    private ProgressBar progressBar;
    private Button button_validate;
    private System.Windows.Forms.Timer timer;
    private Label label2;
    private ComboBox comboBox_bufferSizeUnit;
    private TextBox textBox_bufferSize;
    private CheckBox checkBox_ramIndex;
    private Label label3;
    private TextBox textBox_threads;
}
