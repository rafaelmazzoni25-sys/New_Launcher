// Decompiled with JetBrains decompiler
// Type: ConfigGenerator.Form1
// Assembly: ConfigGenerator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E333375F-E325-44D6-8F8A-127F1A7AB356
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Generator\ConfigGenerator.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace ConfigGenerator;

public class Form1 : Form
{
  private byte[] Xor = new byte[5]
  {
    (byte) 77,
    (byte) 252,
    (byte) 207,
    (byte) 171,
    byte.MaxValue
  };
  private const string FileName = ".\\\\mu.ini";
  private IContainer components;
  private Label S12;
  private Label Url;
  private TextBox textBoxUrl1;
  private ComboBox comboBox1;
  private Button Save;
  private Label label2;
  private TextBox txtboxName;
  private Label label3;
  private TextBox textBoxExe;
  private Label label4;
  private TextBox textBoxUrl2;
  private Label label1;
  private ComboBox comboBox2;

  public Form1() => this.InitializeComponent();

  private void Save_Click(object sender, EventArgs e)
  {
    using (BinaryWriter binaryWriter = new BinaryWriter((Stream) new FileStream(".\\\\mu.ini", FileMode.Create)))
    {
      byte[] bytes = Encoding.UTF8.GetBytes($"{this.txtboxName.Text}|{this.textBoxUrl1.Text}|{this.textBoxExe.Text}|{this.textBoxUrl2.Text}");
      for (int index = 0; index < bytes.Length; ++index)
        bytes[index] ^= this.Xor[index % 5];
      binaryWriter.Write(this.comboBox1.SelectedIndex);
      binaryWriter.Write(this.comboBox2.SelectedIndex);
      binaryWriter.Write(bytes);
      binaryWriter.Close();
      int num = (int) MessageBox.Show("Created mu.ini", "NOTICE");
    }
  }

  private void Form1_Load(object sender, EventArgs e)
  {
    this.comboBox1.SelectedIndex = 0;
    this.comboBox2.SelectedIndex = 0;
    try
    {
      using (BinaryReader binaryReader = new BinaryReader((Stream) new FileStream(".\\\\mu.ini", FileMode.Open)))
      {
        this.comboBox1.SelectedIndex = binaryReader.ReadInt32();
        this.comboBox2.SelectedIndex = binaryReader.ReadInt32();
        byte[] bytes = binaryReader.ReadBytes((int) binaryReader.BaseStream.Length - 4);
        for (int index = 0; index < bytes.Length; ++index)
          bytes[index] ^= this.Xor[index % 5];
        this.txtboxName.Text = Encoding.UTF8.GetString(bytes).Split('|')[0];
        this.textBoxUrl1.Text = Encoding.UTF8.GetString(bytes).Split('|')[1];
        this.textBoxExe.Text = Encoding.UTF8.GetString(bytes).Split('|')[2];
        this.textBoxUrl2.Text = Encoding.UTF8.GetString(bytes).Split('|')[3];
        binaryReader.Close();
      }
    }
    catch
    {
    }
  }

  private void Url_Click(object sender, EventArgs e)
  {
  }

  private void textBox3_TextChanged(object sender, EventArgs e)
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
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Form1));
    this.S12 = new Label();
    this.Url = new Label();
    this.textBoxUrl1 = new TextBox();
    this.comboBox1 = new ComboBox();
    this.Save = new Button();
    this.label2 = new Label();
    this.txtboxName = new TextBox();
    this.label3 = new Label();
    this.textBoxExe = new TextBox();
    this.label4 = new Label();
    this.textBoxUrl2 = new TextBox();
    this.label1 = new Label();
    this.comboBox2 = new ComboBox();
    this.SuspendLayout();
    this.S12.AutoSize = true;
    this.S12.Location = new Point(9, 41);
    this.S12.Name = "S12";
    this.S12.Size = new Size(74, 13);
    this.S12.TabIndex = 0;
    this.S12.Text = "Client Version:";
    this.Url.AutoSize = true;
    this.Url.Location = new Point(9, 68);
    this.Url.Name = "Url";
    this.Url.Size = new Size(61, 13);
    this.Url.TabIndex = 2;
    this.Url.Text = "Update Url:";
    this.Url.Click += new EventHandler(this.Url_Click);
    this.textBoxUrl1.Location = new Point(77, 65);
    this.textBoxUrl1.Name = "textBoxUrl1";
    this.textBoxUrl1.Size = new Size(198, 20);
    this.textBoxUrl1.TabIndex = 3;
    this.textBoxUrl1.Text = "http://127.0.0.1/update/";
    this.comboBox1.FormattingEnabled = true;
    this.comboBox1.Items.AddRange(new object[3]
    {
      (object) "Season 6",
      (object) "Season 9",
      (object) "Season 12"
    });
    this.comboBox1.Location = new Point(99, 38);
    this.comboBox1.Name = "comboBox1";
    this.comboBox1.Size = new Size(176 /*0xB0*/, 21);
    this.comboBox1.TabIndex = 4;
    this.Save.Location = new Point(99, 169);
    this.Save.Name = "Save";
    this.Save.Size = new Size(75, 23);
    this.Save.TabIndex = 5;
    this.Save.Text = "Save";
    this.Save.UseVisualStyleBackColor = true;
    this.Save.Click += new EventHandler(this.Save_Click);
    this.label2.AutoSize = true;
    this.label2.Location = new Point(9, 15);
    this.label2.Name = "label2";
    this.label2.Size = new Size(38, 13);
    this.label2.TabIndex = 7;
    this.label2.Text = "Name:";
    this.txtboxName.Enabled = false;
    this.txtboxName.Location = new Point(77, 12);
    this.txtboxName.Name = "txtboxName";
    this.txtboxName.Size = new Size(198, 20);
    this.txtboxName.TabIndex = 8;
    this.txtboxName.Text = "Mu Launcher";
    this.label3.AutoSize = true;
    this.label3.Location = new Point(9, 120);
    this.label3.Name = "label3";
    this.label3.Size = new Size(57, 13);
    this.label3.TabIndex = 9;
    this.label3.Text = "Exe name:";
    this.textBoxExe.Location = new Point(77, 117);
    this.textBoxExe.Name = "textBoxExe";
    this.textBoxExe.Size = new Size(198, 20);
    this.textBoxExe.TabIndex = 10;
    this.textBoxExe.Text = "main.exe";
    this.textBoxExe.TextChanged += new EventHandler(this.textBox3_TextChanged);
    this.label4.AutoSize = true;
    this.label4.Location = new Point(9, 94);
    this.label4.Name = "label4";
    this.label4.Size = new Size(51, 13);
    this.label4.TabIndex = 11;
    this.label4.Text = "Page Url:";
    this.textBoxUrl2.Location = new Point(77, 91);
    this.textBoxUrl2.Name = "textBoxUrl2";
    this.textBoxUrl2.Size = new Size(198, 20);
    this.textBoxUrl2.TabIndex = 12;
    this.textBoxUrl2.Text = "http://127.0.0.1/update/index.php";
    this.label1.AutoSize = true;
    this.label1.Location = new Point(9, 145);
    this.label1.Name = "label1";
    this.label1.Size = new Size(106, 13);
    this.label1.TabIndex = 13;
    this.label1.Text = "Launcher Language:";
    this.comboBox2.Enabled = false;
    this.comboBox2.FormattingEnabled = true;
    this.comboBox2.Items.AddRange(new object[4]
    {
      (object) "Auto",
      (object) "Eng",
      (object) "Spn",
      (object) "Por"
    });
    this.comboBox2.Location = new Point(121, 142);
    this.comboBox2.Name = "comboBox2";
    this.comboBox2.Size = new Size(154, 21);
    this.comboBox2.TabIndex = 14;
    this.AutoScaleDimensions = new SizeF(6f, 13f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.ClientSize = new Size(287, 201);
    this.Controls.Add((Control) this.comboBox2);
    this.Controls.Add((Control) this.label1);
    this.Controls.Add((Control) this.textBoxUrl2);
    this.Controls.Add((Control) this.label4);
    this.Controls.Add((Control) this.textBoxExe);
    this.Controls.Add((Control) this.label3);
    this.Controls.Add((Control) this.txtboxName);
    this.Controls.Add((Control) this.label2);
    this.Controls.Add((Control) this.Save);
    this.Controls.Add((Control) this.comboBox1);
    this.Controls.Add((Control) this.textBoxUrl1);
    this.Controls.Add((Control) this.Url);
    this.Controls.Add((Control) this.S12);
    this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
    this.Name = nameof (Form1);
    this.StartPosition = FormStartPosition.CenterScreen;
    this.Text = "Config Generator";
    this.Load += new EventHandler(this.Form1_Load);
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
