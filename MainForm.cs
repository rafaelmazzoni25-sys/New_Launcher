// Decompiled with JetBrains decompiler
// Type: Launcher.MainForm
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 232F895E-1583-4AAE-8C54-19D96214944A
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Client\Launcher.exe

using Launcher.Exile;
using Launcher.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace Launcher;

public class MainForm : Form
{
  private static Mutex _m;
  private static string InstanceName = "#launcheritself";
  private static byte[] Xor = new byte[5]
  {
    (byte) 77,
    (byte) 252,
    (byte) 207,
    (byte) 171,
    byte.MaxValue
  };
  private bool Pic1_Hover = false;
  private bool blink = true;
  private IContainer components = (IContainer) null;
  public Label Status;
  public PictureBox StartButton;
  public PictureBox ExitButton;
  public PictureBox pictureBox3;
  public PictureBox pictureBox4;
  private Label label1;
  private Label Captions;
  private Label CopyRightLabel;
  public WebBrowser webBrowser1;
  private System.Windows.Forms.Timer timer1;
  public PictureBox pictureBox5;
  public PictureBox pictureBox6;

  public MainForm()
  {
    this.InitializeComponent();
    Globals.MainForm = this;
    this.LoadExternalLayout();
  }

  private void LoadExternalLayout()
  {
    try
    {
      string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
      string jsonLayoutPath = Path.Combine(baseDirectory, "layout.json");
      string svgLayoutPath = Path.Combine(baseDirectory, "layout.svg");
      if (File.Exists(jsonLayoutPath))
      {
        LayoutLoader.ApplyLayout((Form) this, jsonLayoutPath);
      }
      else if (File.Exists(svgLayoutPath))
        LayoutLoader.ApplyLayout((Form) this, svgLayoutPath);
    }
    catch (Exception ex)
    {
      try
      {
        this.Status.Text = ex.Message;
      }
      catch
      {
      }
    }
  }

  private void MainForm_Shown(object sender, EventArgs e)
  {
    Common.ChangeStatus("CONNECTING");
    DateTime now = DateTime.Now;
    do
    {
      Application.DoEvents();
    }
    while (now.AddSeconds(1.0) > DateTime.Now);
    this.BeginInvoke((Delegate) new MainForm.DoWorkDelegate(this.DoWorkMethod));
    this.timer1.Interval = 600;
    this.timer1.Start();
  }

  public void DoWorkMethod() => Networking.CheckNetwork();

  private void StartButton_Click(object sender, EventArgs e)
  {
    MainForm._m = new Mutex(true, "#32770");
    Starter.Start();
  }

  private void ExitButton_Click(object sender, EventArgs e) => this.Close();

  private void pictureBox4_Click(object sender, EventArgs e)
  {
    if (Globals.UseSeason9)
    {
      if (CRegistry.GetValueInt("FullScreenMode") != 0)
      {
        if (!CRegistry.Update("FullScreenMode", 0))
          return;
        this.pictureBox4.BackgroundImage = (Image) Resources.windowmode;
      }
      else if (CRegistry.Update("FullScreenMode", 1))
        this.pictureBox4.BackgroundImage = (Image) Resources.windowmode_uncheck;
    }
    else if (!Globals.UseSeason12)
    {
      if (CRegistry.GetValueInt("WindowMode") != 0)
      {
        if (!CRegistry.Update("WindowMode", 0))
          return;
        this.pictureBox4.BackgroundImage = (Image) Resources.windowmode_uncheck;
      }
      else if (CRegistry.Update("WindowMode", 1))
        this.pictureBox4.BackgroundImage = (Image) Resources.windowmode;
    }
    else if (LauncherOptions.m_WindowMode == 1)
    {
      LauncherOptions.SetValue(LauncherOptions.m_DevModeIndex, 0, LauncherOptions.m_ID);
      this.pictureBox4.BackgroundImage = (Image) Resources.windowmode_uncheck;
    }
    else
    {
      LauncherOptions.SetValue(LauncherOptions.m_DevModeIndex, 1, LauncherOptions.m_ID);
      this.pictureBox4.BackgroundImage = (Image) Resources.windowmode;
    }
  }

  private void pictureBox3_Click(object sender, EventArgs e)
  {
    Options options = new Options();
    int num = (int) options.ShowDialog();
    options.Dispose();
  }

  private void pictureBox6_Click(object sender, EventArgs e)
  {
    this.WindowState = FormWindowState.Minimized;
  }

  private void MainForm_MouseDown(object sender, MouseEventArgs e)
  {
    if (e.Button != MouseButtons.Left)
      return;
    Common.ReleaseCapture();
    Common.SendMessage(this.Handle, 161, 2, 0);
  }

  private static bool IsSingleInstance()
  {
    try
    {
      Mutex.OpenExisting(MainForm.InstanceName);
    }
    catch
    {
      MainForm._m = new Mutex(true, MainForm.InstanceName);
      return true;
    }
    return false;
  }

  private void MainForm_Load(object sender, EventArgs e)
  {
    if (!MainForm.IsSingleInstance() && MessageBox.Show(Languages.GetText("ALREADYRUNNING"), Globals.Caption, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
      this.Close();
    try
    {
      using (BinaryReader binaryReader = new BinaryReader((Stream) new FileStream(".\\\\mu.ini", FileMode.Open)))
      {
        int num = binaryReader.ReadInt32();
        Globals.Language = binaryReader.ReadInt32();
        byte[] bytes = binaryReader.ReadBytes((int) binaryReader.BaseStream.Length - 4);
        for (int index = 0; index < bytes.Length; ++index)
          bytes[index] ^= MainForm.Xor[index % 5];
        binaryReader.Close();
        switch (num)
        {
          case 1:
            Globals.UseSeason9 = true;
            break;
          case 2:
            Globals.UseSeason12 = true;
            break;
        }
        Globals.ServerURL = Encoding.UTF8.GetString(bytes).Split('|')[1];
        Globals.BinaryName = Encoding.UTF8.GetString(bytes).Split('|')[2];
        this.webBrowser1.Url = new Uri(Encoding.UTF8.GetString(bytes).Split('|')[3]);
      }
    }
    catch
    {
    }
    switch (Globals.Language)
    {
      case 2:
        this.label1.Location = new Point(738, 543);
        break;
      case 3:
        this.label1.Location = new Point(748, 543);
        break;
    }
    this.label1.Text = Languages.GetText("WINDOW_MODE");
    if (Globals.UseSeason12)
    {
      LauncherOptions.GetValue();
      if (LauncherOptions.m_WindowMode != 1)
        return;
      this.pictureBox4.BackgroundImage = (Image) Resources.windowmode;
    }
    else
    {
      CRegistry.CreateKey();
      int num = 0;
      if (Globals.UseSeason9)
      {
        if (CRegistry.GetValueInt("FullScreenMode") == 0)
          num = 1;
      }
      else
        num = CRegistry.GetValueInt("WindowMode");
      if (num == 1)
        this.pictureBox4.BackgroundImage = (Image) Resources.windowmode;
      if (Globals.Language == 0)
      {
        switch (CRegistry.GetValueString("LangSelection"))
        {
          case "Eng":
            Globals.Language = 1;
            break;
          case "Spn":
            Globals.Language = 2;
            break;
          case "Por":
            Globals.Language = 3;
            break;
        }
      }
    }
  }

  private void StartButton_MouseDown(object sender, MouseEventArgs e)
  {
    this.StartButton.BackgroundImage = (Image) Resources.start_3;
  }

  private void StartButton_MouseHover(object sender, EventArgs e)
  {
    this.StartButton.BackgroundImage = (Image) Resources.start_2;
    this.Pic1_Hover = true;
  }

  private void StartButton_MouseLeave(object sender, EventArgs e)
  {
    this.StartButton.BackgroundImage = (Image) Resources.start_1;
    this.Pic1_Hover = false;
  }

  private void StartButton_MouseUp(object sender, MouseEventArgs e)
  {
    this.StartButton.BackgroundImage = (Image) Resources.start_1;
  }

  private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
  {
    this.pictureBox3.BackgroundImage = (Image) Resources.setting_3;
  }

  private void pictureBox3_MouseUp(object sender, MouseEventArgs e)
  {
    this.pictureBox3.BackgroundImage = (Image) Resources.setting_1;
  }

  private void ExitButton_MouseDown(object sender, MouseEventArgs e)
  {
    this.ExitButton.BackgroundImage = (Image) Resources.exit_3;
  }

  private void ExitButton_MouseHover(object sender, EventArgs e)
  {
    this.ExitButton.BackgroundImage = (Image) Resources.exit_2;
  }

  private void ExitButton_MouseLeave(object sender, EventArgs e)
  {
    this.ExitButton.BackgroundImage = (Image) Resources.exit_1;
  }

  private void ExitButton_MouseUp(object sender, MouseEventArgs e)
  {
    this.ExitButton.BackgroundImage = (Image) Resources.exit_1;
  }

  private void pictureBox3_MouseHover(object sender, EventArgs e)
  {
    this.pictureBox3.BackgroundImage = (Image) Resources.setting_2;
  }

  private void pictureBox3_MouseLeave(object sender, EventArgs e)
  {
    this.pictureBox3.BackgroundImage = (Image) Resources.setting_1;
  }

  private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
  {
    this.webBrowser1.Visible = true;
  }

  private void timer1_Tick(object sender, EventArgs e)
  {
    if (this.Pic1_Hover || !Globals.EnableStartBTN)
      return;
    if (this.blink)
      this.StartButton.BackgroundImage = (Image) Resources.start_2;
    else
      this.StartButton.BackgroundImage = (Image) Resources.start_1;
    this.blink = !this.blink;
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new System.ComponentModel.Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (MainForm));
    this.Status = new Label();
    this.StartButton = new PictureBox();
    this.ExitButton = new PictureBox();
    this.pictureBox3 = new PictureBox();
    this.pictureBox4 = new PictureBox();
    this.label1 = new Label();
    this.Captions = new Label();
    this.CopyRightLabel = new Label();
    this.webBrowser1 = new WebBrowser();
    this.timer1 = new System.Windows.Forms.Timer(this.components);
    this.pictureBox5 = new PictureBox();
    this.pictureBox6 = new PictureBox();
    ((ISupportInitialize) this.StartButton).BeginInit();
    ((ISupportInitialize) this.ExitButton).BeginInit();
    ((ISupportInitialize) this.pictureBox3).BeginInit();
    ((ISupportInitialize) this.pictureBox4).BeginInit();
    ((ISupportInitialize) this.pictureBox5).BeginInit();
    ((ISupportInitialize) this.pictureBox6).BeginInit();
    this.SuspendLayout();
    this.Status.AutoSize = true;
    this.Status.BackColor = Color.Transparent;
    this.Status.Font = new Font("Tahoma", 8.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
    this.Status.ForeColor = Color.White;
    this.Status.Location = new Point(176 /*0xB0*/, 503);
    this.Status.Name = "Status";
    this.Status.Size = new Size(42, 14);
    this.Status.TabIndex = 6;
    this.Status.Text = "Status";
    this.StartButton.BackColor = Color.Transparent;
    this.StartButton.BackgroundImage = (Image) Resources.start_4;
    this.StartButton.Cursor = Cursors.Hand;
    this.StartButton.Enabled = false;
    this.StartButton.Location = new Point(840, 503);
    this.StartButton.Name = "StartButton";
    this.StartButton.Size = new Size(146, 53);
    this.StartButton.TabIndex = 7;
    this.StartButton.TabStop = false;
    this.StartButton.Click += new EventHandler(this.StartButton_Click);
    this.StartButton.MouseDown += new MouseEventHandler(this.StartButton_MouseDown);
    this.StartButton.MouseLeave += new EventHandler(this.StartButton_MouseLeave);
    this.StartButton.MouseHover += new EventHandler(this.StartButton_MouseHover);
    this.StartButton.MouseUp += new MouseEventHandler(this.StartButton_MouseUp);
    this.ExitButton.BackColor = Color.Transparent;
    this.ExitButton.BackgroundImage = (Image) Resources.exit_1;
    this.ExitButton.Cursor = Cursors.Hand;
    this.ExitButton.Location = new Point(968, 3);
    this.ExitButton.Name = "ExitButton";
    this.ExitButton.Size = new Size(18, 18);
    this.ExitButton.TabIndex = 8;
    this.ExitButton.TabStop = false;
    this.ExitButton.Click += new EventHandler(this.ExitButton_Click);
    this.ExitButton.MouseDown += new MouseEventHandler(this.ExitButton_MouseDown);
    this.ExitButton.MouseLeave += new EventHandler(this.ExitButton_MouseLeave);
    this.ExitButton.MouseHover += new EventHandler(this.ExitButton_MouseHover);
    this.ExitButton.MouseUp += new MouseEventHandler(this.ExitButton_MouseUp);
    this.pictureBox3.BackColor = Color.Transparent;
    this.pictureBox3.BackgroundImage = (Image) Resources.setting_1;
    this.pictureBox3.Cursor = Cursors.Hand;
    this.pictureBox3.Enabled = false;
    this.pictureBox3.Location = new Point(947, 3);
    this.pictureBox3.Name = "pictureBox3";
    this.pictureBox3.Size = new Size(18, 18);
    this.pictureBox3.TabIndex = 9;
    this.pictureBox3.TabStop = false;
    this.pictureBox3.Click += new EventHandler(this.pictureBox3_Click);
    this.pictureBox3.MouseDown += new MouseEventHandler(this.pictureBox3_MouseDown);
    this.pictureBox3.MouseLeave += new EventHandler(this.pictureBox3_MouseLeave);
    this.pictureBox3.MouseHover += new EventHandler(this.pictureBox3_MouseHover);
    this.pictureBox3.MouseUp += new MouseEventHandler(this.pictureBox3_MouseUp);
    this.pictureBox4.BackColor = Color.Transparent;
    this.pictureBox4.BackgroundImage = (Image) Resources.windowmode_uncheck;
    this.pictureBox4.Cursor = Cursors.Hand;
    this.pictureBox4.Enabled = false;
    this.pictureBox4.Location = new Point(818, 541);
    this.pictureBox4.Name = "pictureBox4";
    this.pictureBox4.Size = new Size(16 /*0x10*/, 15);
    this.pictureBox4.TabIndex = 10;
    this.pictureBox4.TabStop = false;
    this.pictureBox4.Click += new EventHandler(this.pictureBox4_Click);
    this.label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
    this.label1.AutoSize = true;
    this.label1.BackColor = Color.Transparent;
    this.label1.ForeColor = Color.LightSteelBlue;
    this.label1.Location = new Point(760, 543);
    this.label1.Name = "label1";
    this.label1.Size = new Size(52, 13);
    this.label1.TabIndex = 13;
    this.label1.Text = "Winmode";
    this.label1.TextAlign = ContentAlignment.TopRight;
    this.Captions.AutoSize = true;
    this.Captions.BackColor = Color.Transparent;
    this.Captions.Font = new Font("Tahoma", 9f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
    this.Captions.ForeColor = Color.LightSteelBlue;
    this.Captions.Location = new Point(26, 3);
    this.Captions.Name = "Captions";
    this.Captions.Size = new Size(86, 14);
    this.Captions.TabIndex = 14;
    this.Captions.Text = "MU Launcher";
    this.CopyRightLabel.AutoSize = true;
    this.CopyRightLabel.BackColor = Color.Transparent;
    this.CopyRightLabel.Font = new Font("Tahoma", 8.3f);
    this.CopyRightLabel.ForeColor = SystemColors.AppWorkspace;
    this.CopyRightLabel.Location = new Point(108, 543);
    this.CopyRightLabel.Name = "CopyRightLabel";
    this.CopyRightLabel.Size = new Size(136, 14);
    this.CopyRightLabel.TabIndex = 41;
    this.CopyRightLabel.Text = "Coded by MyHeart (RZ)";
    this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
    this.webBrowser1.Location = new Point(5, 23);
    this.webBrowser1.MinimumSize = new Size(20, 20);
    this.webBrowser1.Name = "webBrowser1";
    this.webBrowser1.ScrollBarsEnabled = false;
    this.webBrowser1.Size = new Size(979, 475);
    this.webBrowser1.TabIndex = 44;
    this.webBrowser1.Url = new Uri("http://127.0.0.1/update/index.php", UriKind.Absolute);
    this.webBrowser1.Visible = false;
    this.webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
    this.timer1.Tick += new EventHandler(this.timer1_Tick);
    this.pictureBox5.BackgroundImage = (Image) Resources.BITMAP154_1;
    this.pictureBox5.BackgroundImageLayout = ImageLayout.Stretch;
    this.pictureBox5.Location = new Point(179, 524);
    this.pictureBox5.Name = "pictureBox5";
    this.pictureBox5.Size = new Size(619, 11);
    this.pictureBox5.TabIndex = 47;
    this.pictureBox5.TabStop = false;
    this.pictureBox6.BackgroundImageLayout = ImageLayout.Center;
    this.pictureBox6.Image = (Image) Resources.BITMAP155_1;
    this.pictureBox6.Location = new Point(179, 524);
    this.pictureBox6.Name = "pictureBox6";
    this.pictureBox6.Size = new Size(0, 11);
    this.pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
    this.pictureBox6.TabIndex = 50;
    this.pictureBox6.TabStop = false;
    this.AutoScaleDimensions = new SizeF(6f, 13f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.BackgroundImage = (Image) Resources.BITMAP129_1;
    this.ClientSize = new Size(990, 560);
    this.Controls.Add((Control) this.pictureBox6);
    this.Controls.Add((Control) this.pictureBox5);
    this.Controls.Add((Control) this.webBrowser1);
    this.Controls.Add((Control) this.CopyRightLabel);
    this.Controls.Add((Control) this.Captions);
    this.Controls.Add((Control) this.label1);
    this.Controls.Add((Control) this.pictureBox4);
    this.Controls.Add((Control) this.pictureBox3);
    this.Controls.Add((Control) this.ExitButton);
    this.Controls.Add((Control) this.StartButton);
    this.Controls.Add((Control) this.Status);
    this.FormBorderStyle = FormBorderStyle.None;
    this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
    this.MaximizeBox = false;
    this.MaximumSize = new Size(990, 560);
    this.MinimumSize = new Size(990, 560);
    this.Name = nameof (MainForm);
    this.ShowIcon = false;
    this.StartPosition = FormStartPosition.CenterScreen;
    this.Text = "Autoupdate";
    this.TransparencyKey = Color.Magenta;
    this.Load += new EventHandler(this.MainForm_Load);
    this.Shown += new EventHandler(this.MainForm_Shown);
    this.MouseDown += new MouseEventHandler(this.MainForm_MouseDown);
    ((ISupportInitialize) this.StartButton).EndInit();
    ((ISupportInitialize) this.ExitButton).EndInit();
    ((ISupportInitialize) this.pictureBox3).EndInit();
    ((ISupportInitialize) this.pictureBox4).EndInit();
    ((ISupportInitialize) this.pictureBox5).EndInit();
    ((ISupportInitialize) this.pictureBox6).EndInit();
    this.ResumeLayout(false);
    this.PerformLayout();
  }

  public delegate void DoWorkDelegate();
}
