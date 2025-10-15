// Decompiled with JetBrains decompiler
// Type: IGCNServerInfoEditor.About
// Assembly: IGCNServerInfoEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 42E4CD18-0BEB-4B8E-82BE-8F1631EA8A60
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Sources\Main Source\IGCNServerInfoEditor.exe

using IGCNServerInfoEditor.Properties;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace IGCNServerInfoEditor;

public class About : Form
{
  private IContainer components;
  private Label label1;
  private PictureBox pictureBox1;

  public About() => this.InitializeComponent();

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.label1 = new Label();
    this.pictureBox1 = new PictureBox();
    ((ISupportInitialize) this.pictureBox1).BeginInit();
    this.SuspendLayout();
    this.label1.AutoSize = true;
    this.label1.Location = new Point(12, 29);
    this.label1.Name = "label1";
    this.label1.Size = new Size(85, 13);
    this.label1.TabIndex = 0;
    this.label1.Text = "Made By Ashllay";
    this.pictureBox1.BackgroundImage = (Image) Resources.mu2;
    this.pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
    this.pictureBox1.Location = new Point(103, 12);
    this.pictureBox1.Name = "pictureBox1";
    this.pictureBox1.Size = new Size(49, 45);
    this.pictureBox1.TabIndex = 1;
    this.pictureBox1.TabStop = false;
    this.AutoScaleDimensions = new SizeF(6f, 13f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.ClientSize = new Size(164, 69);
    this.Controls.Add((Control) this.pictureBox1);
    this.Controls.Add((Control) this.label1);
    this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (About);
    this.Text = nameof (About);
    ((ISupportInitialize) this.pictureBox1).EndInit();
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
