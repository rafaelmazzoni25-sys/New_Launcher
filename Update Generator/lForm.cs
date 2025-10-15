// Decompiled with JetBrains decompiler
// Type: Update.Maker.lForm
// Assembly: Generator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F664580-F6B4-4B6D-A5C6-C03B862A74BB
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Generator\Update Generator.exe

using Cyclic.Redundancy.Check;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

#nullable disable
namespace Update.Maker;

public class lForm : Form
{
  private string[] Files;
  private IContainer components;
  private TextBox Result;
  private Button browseButton;
  private ProgressBar Progress;
  private Button saveButton;
  private BackgroundWorker backgroundWorker;
  private FolderBrowserDialog folderBrowserDialog;
  private TextBox filePath;
  private Button removeButton;
  private SaveFileDialog saveFileDialog;
  private GroupBox groupBox1;
  private GroupBox groupBox2;

  public lForm() => this.InitializeComponent();

  private void browseButton_Click(object sender, EventArgs e)
  {
    Directory.CreateDirectory(".\\\\update");
    this.StartBrowsing();
  }

  private void saveButton_Click(object sender, EventArgs e) => this.SaveList();

  private void removeButton_Click(object sender, EventArgs e)
  {
    this.RemoveFromPath(this.filePath.Text);
  }

  private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
  {
    this.Files = this.GetFiles(e.Argument);
    for (int index = 0; index < this.Files.Length; ++index)
      this.backgroundWorker.ReportProgress(index + 1, (object) this.GetFileData(this.Files[index]));
  }

  private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
  {
    this.UpdateResult(e.UserState);
    this.UpdateProgressBar(this.ComputeProgress(e.ProgressPercentage));
  }

  private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
  {
    this.EnableButtons();
  }

  private void DisableButtons()
  {
    this.Progress.Value = 0;
    this.Result.Clear();
    this.saveButton.Enabled = false;
    this.removeButton.Enabled = false;
    this.browseButton.Enabled = false;
  }

  private void EnableButtons()
  {
    this.saveButton.Enabled = true;
    this.removeButton.Enabled = true;
    this.browseButton.Enabled = true;
  }

  public string[] GetFiles(object Path)
  {
    return Directory.GetFiles(Path.ToString(), "*.*", SearchOption.AllDirectories);
  }

  public int GetFilesCount(string[] Files) => Files.Length;

  public string GetFileData(string File)
  {
    FileInfo fileInfo = new FileInfo(File);
    return $"{File}.rar;{this.GetHash(File)};{(object) fileInfo.Length}";
  }

  private string GetHash(string Name)
  {
    if (Name == string.Empty)
      return (string) null;
    CRC crc = new CRC();
    string empty = string.Empty;
    try
    {
      using (FileStream inputStream = File.Open(Name, FileMode.Open))
      {
        foreach (byte num in crc.ComputeHash((Stream) inputStream))
          empty += num.ToString("x2").ToUpper();
      }
    }
    catch
    {
      int num = (int) MessageBox.Show("Can't open: " + Name);
    }
    return empty;
  }

  private void UpdateResult(object Data)
  {
    if (this.Result.IsDisposed)
      return;
    string path = Data.ToString().Replace("\\", "/").Split(';')[0].Replace(this.filePath.Text, string.Empty);
    if (path.Contains("/"))
      Directory.CreateDirectory("./update/" + Path.GetDirectoryName(path));
    File.Copy(Data.ToString().Replace("\\", "/").Split(';')[0].Replace(".rar", string.Empty), $"{Directory.GetCurrentDirectory()}/update/{path}", true);
    this.Result.AppendText(Data.ToString().Replace("\\", "/").Replace(this.filePath.Text, string.Empty) + Environment.NewLine);
  }

  private int ComputeProgress(int Percent) => 100 * Percent / this.Files.Length;

  private void UpdateProgressBar(int Percent)
  {
    if (Percent < 0 || Percent > 100 || this.Progress.IsDisposed)
      return;
    this.Progress.Value = Percent;
  }

  private void RemoveFromPath(string Remove)
  {
    if (Remove == string.Empty)
      return;
    this.Result.Text = this.Result.Text.Replace(Remove, string.Empty);
    this.filePath.Text = this.filePath.Text.Replace(Remove, string.Empty);
  }

  private void StartBrowsing()
  {
    if (this.folderBrowserDialog.ShowDialog() != DialogResult.OK)
      return;
    this.DisableButtons();
    this.filePath.Text = this.folderBrowserDialog.SelectedPath.Replace("\\", "/") + "/";
    if (this.backgroundWorker.IsBusy)
      return;
    this.backgroundWorker.RunWorkerAsync((object) this.folderBrowserDialog.SelectedPath);
  }

  private void SaveList()
  {
    this.saveFileDialog.FileName = "update.txt";
    this.saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\update\\";
    this.saveFileDialog.RestoreDirectory = true;
    this.saveFileDialog.Filter = "Text files (*.txt)|*.txt|Every file (*.*)|*.*";
    if (this.saveFileDialog.ShowDialog() != DialogResult.OK)
      return;
    using (StreamWriter streamWriter = new StreamWriter(this.saveFileDialog.FileName))
      streamWriter.Write(this.Result.Text);
  }

  private void Progress_Click(object sender, EventArgs e)
  {
  }

  private void filePath_TextChanged(object sender, EventArgs e)
  {
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (lForm));
    this.Result = new TextBox();
    this.browseButton = new Button();
    this.Progress = new ProgressBar();
    this.saveButton = new Button();
    this.backgroundWorker = new BackgroundWorker();
    this.folderBrowserDialog = new FolderBrowserDialog();
    this.filePath = new TextBox();
    this.removeButton = new Button();
    this.saveFileDialog = new SaveFileDialog();
    this.groupBox1 = new GroupBox();
    this.groupBox2 = new GroupBox();
    this.groupBox1.SuspendLayout();
    this.SuspendLayout();
    this.Result.BorderStyle = BorderStyle.FixedSingle;
    this.Result.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
    this.Result.Location = new Point(224 /*0xE0*/, 19);
    this.Result.Multiline = true;
    this.Result.Name = "Result";
    this.Result.ScrollBars = ScrollBars.Vertical;
    this.Result.Size = new Size(473, 280);
    this.Result.TabIndex = 0;
    this.browseButton.Location = new Point(25, 28);
    this.browseButton.Name = "browseButton";
    this.browseButton.Size = new Size(154, 47);
    this.browseButton.TabIndex = 1;
    this.browseButton.Text = "1. Select Folder";
    this.browseButton.UseVisualStyleBackColor = true;
    this.browseButton.Click += new EventHandler(this.browseButton_Click);
    this.Progress.BackColor = SystemColors.Control;
    this.Progress.Location = new Point(224 /*0xE0*/, 305);
    this.Progress.Name = "Progress";
    this.Progress.Size = new Size(457, 19);
    this.Progress.TabIndex = 2;
    this.Progress.Click += new EventHandler(this.Progress_Click);
    this.saveButton.Enabled = false;
    this.saveButton.Location = new Point(25, 92);
    this.saveButton.Name = "saveButton";
    this.saveButton.Size = new Size(154, 44);
    this.saveButton.TabIndex = 3;
    this.saveButton.Text = "2. Save List";
    this.saveButton.UseVisualStyleBackColor = true;
    this.saveButton.Click += new EventHandler(this.saveButton_Click);
    this.backgroundWorker.WorkerReportsProgress = true;
    this.backgroundWorker.DoWork += new DoWorkEventHandler(this.backgroundWorker_DoWork);
    this.backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
    this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
    this.filePath.Location = new Point(25, 227);
    this.filePath.MaxLength = 256 /*0x0100*/;
    this.filePath.Name = "filePath";
    this.filePath.Size = new Size(154, 20);
    this.filePath.TabIndex = 4;
    this.filePath.Visible = false;
    this.filePath.TextChanged += new EventHandler(this.filePath_TextChanged);
    this.removeButton.Enabled = false;
    this.removeButton.Location = new Point(71, 253);
    this.removeButton.Name = "removeButton";
    this.removeButton.Size = new Size(58, 23);
    this.removeButton.TabIndex = 5;
    this.removeButton.Text = "Remove";
    this.removeButton.UseVisualStyleBackColor = true;
    this.removeButton.Visible = false;
    this.removeButton.Click += new EventHandler(this.removeButton_Click);
    this.groupBox1.Controls.Add((Control) this.groupBox2);
    this.groupBox1.Controls.Add((Control) this.Result);
    this.groupBox1.Controls.Add((Control) this.removeButton);
    this.groupBox1.Controls.Add((Control) this.saveButton);
    this.groupBox1.Controls.Add((Control) this.Progress);
    this.groupBox1.Controls.Add((Control) this.browseButton);
    this.groupBox1.Controls.Add((Control) this.filePath);
    this.groupBox1.Location = new Point(15, 5);
    this.groupBox1.Name = "groupBox1";
    this.groupBox1.Size = new Size(703, 332);
    this.groupBox1.TabIndex = 100;
    this.groupBox1.TabStop = false;
    this.groupBox2.Location = new Point(203, 10);
    this.groupBox2.Name = "groupBox2";
    this.groupBox2.Size = new Size(5, 314);
    this.groupBox2.TabIndex = 6;
    this.groupBox2.TabStop = false;
    this.AutoScaleDimensions = new SizeF(6f, 13f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.ClientSize = new Size(730, 349);
    this.Controls.Add((Control) this.groupBox1);
    this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
    this.MaximizeBox = false;
    this.Name = nameof (lForm);
    this.ShowIcon = false;
    this.StartPosition = FormStartPosition.CenterScreen;
    this.Text = "Update Generator";
    this.groupBox1.ResumeLayout(false);
    this.groupBox1.PerformLayout();
    this.ResumeLayout(false);
  }
}
