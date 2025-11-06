namespace TestFileGenerator;

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
        GenerateButton = new Button();
        progressBar = new ProgressBar();
        saveFileDialog = new SaveFileDialog();
        textBox_fileSize = new TextBox();
        comboBox_sizeUnit = new ComboBox();
        label1 = new Label();
        richTextBox = new RichTextBox();
        label2 = new Label();
        textBox_probability = new TextBox();
        label3 = new Label();
        textBox_maxLength = new TextBox();
        textBox_maxNumber = new TextBox();
        label4 = new Label();
        timer = new System.Windows.Forms.Timer(components);
        SuspendLayout();
        // 
        // GenerateButton
        // 
        GenerateButton.Location = new Point(12, 188);
        GenerateButton.Name = "GenerateButton";
        GenerateButton.Size = new Size(110, 34);
        GenerateButton.TabIndex = 5;
        GenerateButton.Text = "Generate";
        GenerateButton.UseVisualStyleBackColor = true;
        GenerateButton.Click += GenerateButton_Click;
        // 
        // progressBar
        // 
        progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        progressBar.Location = new Point(12, 228);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(360, 23);
        progressBar.TabIndex = 1;
        // 
        // saveFileDialog
        // 
        saveFileDialog.FileName = "data.txt";
        // 
        // textBox_fileSize
        // 
        textBox_fileSize.Location = new Point(12, 27);
        textBox_fileSize.Name = "textBox_fileSize";
        textBox_fileSize.Size = new Size(63, 23);
        textBox_fileSize.TabIndex = 0;
        textBox_fileSize.Text = "100";
        // 
        // comboBox_sizeUnit
        // 
        comboBox_sizeUnit.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBox_sizeUnit.FormattingEnabled = true;
        comboBox_sizeUnit.Items.AddRange(new object[] { "GB", "MB", "KB" });
        comboBox_sizeUnit.Location = new Point(81, 27);
        comboBox_sizeUnit.Name = "comboBox_sizeUnit";
        comboBox_sizeUnit.Size = new Size(41, 23);
        comboBox_sizeUnit.TabIndex = 1;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(12, 9);
        label1.Name = "label1";
        label1.Size = new Size(50, 15);
        label1.TabIndex = 4;
        label1.Text = "File size:";
        // 
        // richTextBox
        // 
        richTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        richTextBox.Location = new Point(12, 257);
        richTextBox.Name = "richTextBox";
        richTextBox.Size = new Size(360, 192);
        richTextBox.TabIndex = 6;
        richTextBox.Text = "";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(12, 53);
        label2.Name = "label2";
        label2.Size = new Size(137, 15);
        label2.TabIndex = 6;
        label2.Text = "String repeat probability:";
        // 
        // textBox_probability
        // 
        textBox_probability.Location = new Point(12, 71);
        textBox_probability.Name = "textBox_probability";
        textBox_probability.Size = new Size(77, 23);
        textBox_probability.TabIndex = 2;
        textBox_probability.Text = "0.2";
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(12, 97);
        label3.Name = "label3";
        label3.Size = new Size(102, 15);
        label3.TabIndex = 8;
        label3.Text = "Max string length:";
        // 
        // textBox_maxLength
        // 
        textBox_maxLength.Location = new Point(12, 115);
        textBox_maxLength.Name = "textBox_maxLength";
        textBox_maxLength.Size = new Size(77, 23);
        textBox_maxLength.TabIndex = 3;
        textBox_maxLength.Text = "100";
        // 
        // textBox_maxNumber
        // 
        textBox_maxNumber.Location = new Point(12, 159);
        textBox_maxNumber.Name = "textBox_maxNumber";
        textBox_maxNumber.Size = new Size(77, 23);
        textBox_maxNumber.TabIndex = 4;
        textBox_maxNumber.Text = "999999999";
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(12, 141);
        label4.Name = "label4";
        label4.Size = new Size(77, 15);
        label4.TabIndex = 10;
        label4.Text = "Max number:";
        // 
        // timer
        // 
        timer.Enabled = true;
        timer.Tick += timer_Tick;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(384, 461);
        Controls.Add(textBox_maxNumber);
        Controls.Add(label4);
        Controls.Add(textBox_maxLength);
        Controls.Add(label3);
        Controls.Add(textBox_probability);
        Controls.Add(label2);
        Controls.Add(richTextBox);
        Controls.Add(label1);
        Controls.Add(comboBox_sizeUnit);
        Controls.Add(textBox_fileSize);
        Controls.Add(progressBar);
        Controls.Add(GenerateButton);
        Name = "MainForm";
        Text = "Test file generator";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button GenerateButton;
    private ProgressBar progressBar;
    private SaveFileDialog saveFileDialog;
    private TextBox textBox_fileSize;
    private ComboBox comboBox_sizeUnit;
    private Label label1;
    private RichTextBox richTextBox;
    private Label label2;
    private TextBox textBox_probability;
    private Label label3;
    private TextBox textBox_maxLength;
    private TextBox textBox_maxNumber;
    private Label label4;
    private System.Windows.Forms.Timer timer;
}
