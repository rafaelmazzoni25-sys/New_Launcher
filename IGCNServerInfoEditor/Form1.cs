// Decompiled with JetBrains decompiler
// Type: IGCNServerInfoEditor.Form1
// Assembly: IGCNServerInfoEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 42E4CD18-0BEB-4B8E-82BE-8F1631EA8A60
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Sources\Main Source\IGCNServerInfoEditor.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace IGCNServerInfoEditor;

public class Form1 : Form
{
  private IContainer components;
  private RichTextBox richTextBox1;
  private StatusStrip statusStrip1;
  private ToolStripStatusLabel toolStripStatusLabel1;
  private MenuStrip menuStrip1;
  private ToolStripMenuItem openToolStripMenuItem;
  private ToolStripMenuItem mOpen;
  private ToolStripMenuItem mSave;
  private ToolStripSeparator toolStripSeparator1;
  private ToolStripMenuItem mExit;
  private ToolStripMenuItem aboutToolStripMenuItem;

  public Form1() => this.InitializeComponent();

  public void Cleanfile()
  {
    Form1.Mydata.bytedata = (byte[]) null;
    Form1.Mydata.filelen = 0;
    Form1.Mydata.filetyte = 0;
    Form1.Mydata.fn = (string) null;
  }

  private void BmdSave()
  {
    byte[] numArray = new byte[4]
    {
      byte.MaxValue,
      (byte) 250,
      (byte) 5,
      (byte) 136
    };
    try
    {
      if (this.richTextBox1.Text == null && !(this.richTextBox1.Text.Trim() == ""))
        return;
      Form1.Mydata.bytedata = Encoding.Default.GetBytes(this.richTextBox1.Text);
      Form1.Mydata.filelen = Form1.Mydata.bytedata.Length;
      for (int index = 0; index < Form1.Mydata.filelen; ++index)
        Form1.Mydata.bytedata[index] = (byte) ((uint) Form1.Mydata.bytedata[index] ^ (uint) numArray[index % 4]);
    }
    catch (Exception ex)
    {
      int num = (int) MessageBox.Show(ex.ToString());
    }
  }

  private void Filesopen(int filetype)
  {
    this.Cleanfile();
    OpenFileDialog openFileDialog = new OpenFileDialog();
    openFileDialog.InitialDirectory = Environment.CurrentDirectory;
    if (filetype == 1)
    {
      openFileDialog.Filter = "Text File(*.Bmd)|*.bmd|All files(*.*)|*.*";
      this.richTextBox1.Visible = false;
    }
    if (openFileDialog.ShowDialog() != DialogResult.OK)
      return;
    string fileName = openFileDialog.FileName;
    Form1.Mydata.fn = openFileDialog.SafeFileName;
    long length = new FileInfo(fileName).Length;
    FileStream fileStream = new FileStream(fileName, FileMode.Open);
    byte[] buffer = new byte[length];
    fileStream.Read(buffer, 0, (int) length);
    fileStream.Close();
    Form1.Mydata.bytedata = buffer;
    Form1.Mydata.filelen = (int) length;
    Form1.Mydata.filetyte = filetype;
    this.toolStripStatusLabel1.Text = fileName;
  }

  private void Fliesave(int filetype, int len)
  {
    SaveFileDialog saveFileDialog = new SaveFileDialog();
    byte[] numArray = new byte[4];
    if (filetype == 1)
      saveFileDialog.Filter = "Text File(*.bmd)|*.bmd|All files(*.*)|*.*";
    byte[] bytedata = Form1.Mydata.bytedata;
    saveFileDialog.FileName = Form1.Mydata.fn;
    saveFileDialog.AddExtension = true;
    saveFileDialog.Title = "Write file";
    if (saveFileDialog.ShowDialog() != DialogResult.OK)
      return;
    FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create);
    byte[] buffer = bytedata;
    fileStream.Write(buffer, 0, buffer.Length);
    fileStream.Flush();
    fileStream.Close();
    int num = (int) MessageBox.Show("Done", "File Saved!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
  }

  private void mOpen_Click(object sender, EventArgs e)
  {
    byte[] numArray = new byte[4]
    {
      byte.MaxValue,
      (byte) 250,
      (byte) 5,
      (byte) 136
    };
    this.Filesopen(1);
    byte[] bytedata = Form1.Mydata.bytedata;
    for (int index = 0; index < Form1.Mydata.filelen; ++index)
      bytedata[index] = (byte) ((uint) bytedata[index] ^ (uint) numArray[index % 4]);
    if (bytedata == null)
      return;
    this.richTextBox1.Text = Encoding.Default.GetString(bytedata);
    this.richTextBox1.Visible = true;
  }

  private void mSave_Click(object sender, EventArgs e)
  {
    if (Form1.Mydata.filelen != 0)
    {
      if (Form1.Mydata.filetyte != 1)
        return;
      this.BmdSave();
      this.Fliesave(Form1.Mydata.filetyte, Form1.Mydata.filelen);
    }
    else
    {
      int num = (int) MessageBox.Show("Unable to save，Please re-load！");
    }
  }

  private void aboutToolStripMenuItem_Click(object sender, EventArgs e) => new About().Show();

  private void mExit_Click(object sender, EventArgs e) => this.Close();

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Form1));
    this.richTextBox1 = new RichTextBox();
    this.statusStrip1 = new StatusStrip();
    this.toolStripStatusLabel1 = new ToolStripStatusLabel();
    this.menuStrip1 = new MenuStrip();
    this.openToolStripMenuItem = new ToolStripMenuItem();
    this.mOpen = new ToolStripMenuItem();
    this.mSave = new ToolStripMenuItem();
    this.toolStripSeparator1 = new ToolStripSeparator();
    this.mExit = new ToolStripMenuItem();
    this.aboutToolStripMenuItem = new ToolStripMenuItem();
    this.statusStrip1.SuspendLayout();
    this.menuStrip1.SuspendLayout();
    this.SuspendLayout();
    this.richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.richTextBox1.Location = new Point(4, 27);
    this.richTextBox1.Name = "richTextBox1";
    this.richTextBox1.Size = new Size(508, 296);
    this.richTextBox1.TabIndex = 0;
    this.richTextBox1.Text = "";
    this.statusStrip1.Items.AddRange(new ToolStripItem[1]
    {
      (ToolStripItem) this.toolStripStatusLabel1
    });
    this.statusStrip1.Location = new Point(0, 326);
    this.statusStrip1.Name = "statusStrip1";
    this.statusStrip1.Size = new Size(514, 22);
    this.statusStrip1.TabIndex = 6;
    this.statusStrip1.Text = "statusStrip1";
    this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
    this.toolStripStatusLabel1.Size = new Size(0, 17);
    this.menuStrip1.Items.AddRange(new ToolStripItem[2]
    {
      (ToolStripItem) this.openToolStripMenuItem,
      (ToolStripItem) this.aboutToolStripMenuItem
    });
    this.menuStrip1.Location = new Point(0, 0);
    this.menuStrip1.Name = "menuStrip1";
    this.menuStrip1.Size = new Size(514, 24);
    this.menuStrip1.TabIndex = 5;
    this.menuStrip1.Text = "menuStrip1";
    this.openToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[4]
    {
      (ToolStripItem) this.mOpen,
      (ToolStripItem) this.mSave,
      (ToolStripItem) this.toolStripSeparator1,
      (ToolStripItem) this.mExit
    });
    this.openToolStripMenuItem.Name = "openToolStripMenuItem";
    this.openToolStripMenuItem.Size = new Size(37, 20);
    this.openToolStripMenuItem.Text = "File";
    this.mOpen.Name = "mOpen";
    this.mOpen.Size = new Size(103, 22);
    this.mOpen.Text = "Open";
    this.mOpen.Click += new EventHandler(this.mOpen_Click);
    this.mSave.Name = "mSave";
    this.mSave.Size = new Size(103, 22);
    this.mSave.Text = "Save";
    this.mSave.Click += new EventHandler(this.mSave_Click);
    this.toolStripSeparator1.Name = "toolStripSeparator1";
    this.toolStripSeparator1.Size = new Size(100, 6);
    this.mExit.Name = "mExit";
    this.mExit.Size = new Size(103, 22);
    this.mExit.Text = "Exit";
    this.mExit.Click += new EventHandler(this.mExit_Click);
    this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
    this.aboutToolStripMenuItem.Size = new Size(52, 20);
    this.aboutToolStripMenuItem.Text = "About";
    this.aboutToolStripMenuItem.Click += new EventHandler(this.aboutToolStripMenuItem_Click);
    this.AutoScaleDimensions = new SizeF(6f, 13f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.ClientSize = new Size(514, 348);
    this.Controls.Add((Control) this.statusStrip1);
    this.Controls.Add((Control) this.menuStrip1);
    this.Controls.Add((Control) this.richTextBox1);
    this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
    this.Name = nameof (Form1);
    this.Text = "ServerInfo Editor";
    this.statusStrip1.ResumeLayout(false);
    this.statusStrip1.PerformLayout();
    this.menuStrip1.ResumeLayout(false);
    this.menuStrip1.PerformLayout();
    this.ResumeLayout(false);
    this.PerformLayout();
  }

  public class Mydata
  {
    public static byte[] bytedata;
    public static int filelen;
    public static int filetyte;
    public static string fn;
  }
}
