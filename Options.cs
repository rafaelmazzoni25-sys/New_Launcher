// Decompiled with JetBrains decompiler
// Type: Launcher.Options
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 232F895E-1583-4AAE-8C54-19D96214944A
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Client\Launcher.exe

using Launcher.Exile;
using Launcher.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace Launcher;

public class Options : Form
{
  private IContainer components = (IContainer) null;
  private PictureBox SaveButton;
  private PictureBox CloseButton;
  private ComboBox m_Resolution;
  private ComboBox comboBox2;
  private TextBox textBox1;
  private CheckBox checkBox1;
  private CheckBox checkBox2;
  private RadioButton radioButton1;
  private RadioButton radioButton2;
  private Label CopyRightLabel;
  private Label label1;
  private Label label2;
  private Label label3;
  public ComboBox m_Resolution_S12;
  private Label label4;

  public Options() => this.InitializeComponent();

  private void SaveButton_Click(object sender, EventArgs e)
  {
    this.SaveButton.Cursor = Cursors.Hand;
    this.DialogResult = DialogResult.OK;
    if (Globals.UseSeason9)
    {
      if (this.checkBox2.Checked)
        CRegistry.Update("MusicOn", 1);
      else
        CRegistry.Update("MusicOn", 0);
      if (this.checkBox1.Checked)
        CRegistry.Update("SoundOn", 1);
      else
        CRegistry.Update("SoundOn", 0);
      if (this.textBox1.Text != null)
        CRegistry.Update("UserID", this.textBox1.Text);
      else
        CRegistry.Update("UserID", string.Empty);
      CRegistry.Update("DisplayDeviceModeIndex", this.m_Resolution.SelectedIndex);
      CRegistry.Update("LangSelection", "Eng");
      if (this.radioButton1.Checked)
      {
        CRegistry.Update("DisplayColorBit", 0);
      }
      else
      {
        if (!this.radioButton2.Checked)
          return;
        CRegistry.Update("DisplayColorBit", 1);
      }
    }
    else if (!Globals.UseSeason12)
    {
      if (this.checkBox2.Checked)
        CRegistry.Update("MusicOnOFF", 1);
      else
        CRegistry.Update("MusicOnOFF", 0);
      if (this.checkBox1.Checked)
        CRegistry.Update("SoundOnOFF", 1);
      else
        CRegistry.Update("SoundOnOFF", 0);
      if (this.textBox1.Text != null)
        CRegistry.Update("ID", this.textBox1.Text);
      else
        CRegistry.Update("ID", string.Empty);
      CRegistry.Update("Resolution", this.m_Resolution.SelectedIndex + 1);
      CRegistry.Update("LangSelection", "Eng");
      if (this.radioButton1.Checked)
      {
        CRegistry.Update("ColorDepth", 0);
      }
      else
      {
        if (!this.radioButton2.Checked)
          return;
        CRegistry.Update("ColorDepth", 1);
      }
    }
    else
      LauncherOptions.SetValue(this.m_Resolution_S12.SelectedIndex + 1, LauncherOptions.m_WindowMode, this.textBox1.Text);
  }

  private void CloseButton_Click(object sender, EventArgs e)
  {
    this.CloseButton.Cursor = Cursors.Hand;
    this.DialogResult = DialogResult.Cancel;
  }

  private void Options_MouseDown(object sender, MouseEventArgs e)
  {
    if (e.Button != MouseButtons.Left)
      return;
    Common.ReleaseCapture();
    Common.SendMessage(this.Handle, 161, 2, 0);
  }

  private void CloseButton_MouseDown(object sender, MouseEventArgs e)
  {
    this.CloseButton.BackgroundImage = (Image) Resources.exit_3;
  }

  private void CloseButton_MouseHover(object sender, EventArgs e)
  {
    this.CloseButton.BackgroundImage = (Image) Resources.exit_2;
  }

  private void CloseButton_MouseLeave(object sender, EventArgs e)
  {
    this.CloseButton.BackgroundImage = (Image) Resources.exit_1;
  }

  private void CloseButton_MouseUp(object sender, MouseEventArgs e)
  {
    this.CloseButton.BackgroundImage = (Image) Resources.exit_1;
  }

  private void SaveButton_MouseDown(object sender, MouseEventArgs e)
  {
    this.SaveButton.BackgroundImage = (Image) Resources.accept_3;
  }

  private void SaveButton_MouseHover(object sender, EventArgs e)
  {
    this.SaveButton.BackgroundImage = (Image) Resources.accept_2;
  }

  private void SaveButton_MouseLeave(object sender, EventArgs e)
  {
    this.SaveButton.BackgroundImage = (Image) Resources.accept_1;
  }

  private void SaveButton_MouseUp(object sender, MouseEventArgs e)
  {
    this.SaveButton.BackgroundImage = (Image) Resources.accept_1;
  }

  private void Options_Load(object sender, EventArgs e)
  {
    this.label1.Text = Languages.GetText("S_ACCOUNT");
    this.radioButton1.Text = Languages.GetText("S_COLOR16");
    this.radioButton2.Text = Languages.GetText("S_COLOR32");
    this.checkBox1.Text = Languages.GetText("S_SOUND");
    this.checkBox2.Text = Languages.GetText("S_MUSIC");
    this.label3.Text = Languages.GetText("S_LANGUAGE");
    this.label2.Text = Languages.GetText("S_RESOLUTION");
    this.label4.Text = Languages.GetText("S_RESOLUTION");
    if (Globals.UseSeason9)
    {
      int valueInt1 = CRegistry.GetValueInt("MusicOn");
      int valueInt2 = CRegistry.GetValueInt("SoundOn");
      int valueInt3 = CRegistry.GetValueInt("DisplayDeviceModeIndex");
      int valueInt4 = CRegistry.GetValueInt("DisplayColorBit");
      string valueString1 = CRegistry.GetValueString("UserID");
      string valueString2 = CRegistry.GetValueString("LangSelection");
      if (valueInt1 == 1)
        this.checkBox2.Checked = true;
      if (valueInt2 == 1)
        this.checkBox1.Checked = true;
      this.textBox1.Text = valueString1;
      this.m_Resolution.SelectedIndex = valueInt3 - 1 < 0 ? 0 : valueInt3;
      switch (valueString2)
      {
        case "Eng":
          this.comboBox2.Text = "English";
          break;
        case "Spn":
          this.comboBox2.Text = "Spanish";
          break;
        case "Por":
          this.comboBox2.Text = "Portuguese";
          break;
      }
      if (valueInt4 == 0)
      {
        this.radioButton1.Checked = true;
      }
      else
      {
        if (valueInt4 != 1)
          return;
        this.radioButton2.Checked = true;
      }
    }
    else if (!Globals.UseSeason12)
    {
      int valueInt5 = CRegistry.GetValueInt("MusicOnOff");
      int valueInt6 = CRegistry.GetValueInt("SoundOnOff");
      int valueInt7 = CRegistry.GetValueInt("Resolution");
      int valueInt8 = CRegistry.GetValueInt("ColorDepth");
      string valueString3 = CRegistry.GetValueString("ID");
      string valueString4 = CRegistry.GetValueString("LangSelection");
      if (valueInt5 == 1)
        this.checkBox2.Checked = true;
      if (valueInt6 == 1)
        this.checkBox1.Checked = true;
      this.textBox1.Text = valueString3;
      this.m_Resolution.SelectedIndex = valueInt7 - 1 < 0 ? 0 : valueInt7 - 1;
      switch (valueString4)
      {
        case "Eng":
          this.comboBox2.Text = "English";
          break;
        case "Spn":
          this.comboBox2.Text = "Spanish";
          break;
        case "Por":
          this.comboBox2.Text = "Portuguese";
          break;
      }
      if (valueInt8 == 0)
      {
        this.radioButton1.Checked = true;
      }
      else
      {
        if (valueInt8 != 1)
          return;
        this.radioButton2.Checked = true;
      }
    }
    else
    {
      this.label4.Visible = true;
      this.m_Resolution_S12.Visible = true;
      this.m_Resolution.Visible = false;
      this.label2.Visible = false;
      this.label3.Visible = false;
      this.comboBox2.Visible = false;
      this.radioButton1.Visible = false;
      this.radioButton2.Visible = false;
      this.checkBox1.Visible = false;
      this.checkBox2.Visible = false;
      this.textBox1.Location = new Point(170, 120);
      this.label1.Location = new Point(104, 122);
      this.m_Resolution_S12.SelectedIndex = LauncherOptions.m_DevModeIndex - 1 < 0 ? 0 : LauncherOptions.m_DevModeIndex - 1;
      this.textBox1.Text = LauncherOptions.m_ID;
    }
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Options));
    this.SaveButton = new PictureBox();
    this.CloseButton = new PictureBox();
    this.m_Resolution = new ComboBox();
    this.comboBox2 = new ComboBox();
    this.textBox1 = new TextBox();
    this.checkBox1 = new CheckBox();
    this.checkBox2 = new CheckBox();
    this.radioButton1 = new RadioButton();
    this.radioButton2 = new RadioButton();
    this.CopyRightLabel = new Label();
    this.label1 = new Label();
    this.label2 = new Label();
    this.label3 = new Label();
    this.m_Resolution_S12 = new ComboBox();
    this.label4 = new Label();
    ((ISupportInitialize) this.SaveButton).BeginInit();
    ((ISupportInitialize) this.CloseButton).BeginInit();
    this.SuspendLayout();
    this.SaveButton.BackColor = Color.Transparent;
    this.SaveButton.BackgroundImage = (Image) Resources.accept_1;
    this.SaveButton.Cursor = Cursors.Hand;
    this.SaveButton.Location = new Point(170, 412);
    this.SaveButton.Name = "SaveButton";
    this.SaveButton.Size = new Size(108, 43);
    this.SaveButton.TabIndex = 11;
    this.SaveButton.TabStop = false;
    this.SaveButton.Click += new EventHandler(this.SaveButton_Click);
    this.SaveButton.MouseDown += new MouseEventHandler(this.SaveButton_MouseDown);
    this.SaveButton.MouseLeave += new EventHandler(this.SaveButton_MouseLeave);
    this.SaveButton.MouseHover += new EventHandler(this.SaveButton_MouseHover);
    this.SaveButton.MouseUp += new MouseEventHandler(this.SaveButton_MouseUp);
    this.CloseButton.BackColor = Color.Transparent;
    this.CloseButton.BackgroundImage = (Image) Resources.exit_1;
    this.CloseButton.Cursor = Cursors.Hand;
    this.CloseButton.Location = new Point(426, 2);
    this.CloseButton.Name = "CloseButton";
    this.CloseButton.Size = new Size(18, 18);
    this.CloseButton.TabIndex = 10;
    this.CloseButton.TabStop = false;
    this.CloseButton.Click += new EventHandler(this.CloseButton_Click);
    this.CloseButton.MouseDown += new MouseEventHandler(this.CloseButton_MouseDown);
    this.CloseButton.MouseLeave += new EventHandler(this.CloseButton_MouseLeave);
    this.CloseButton.MouseHover += new EventHandler(this.CloseButton_MouseHover);
    this.CloseButton.MouseUp += new MouseEventHandler(this.CloseButton_MouseUp);
    this.m_Resolution.DropDownStyle = ComboBoxStyle.DropDownList;
    this.m_Resolution.FormattingEnabled = true;
    if (!Globals.UseSeason9)
      this.m_Resolution.Items.AddRange(new object[7]
      {
        (object) "1: 800x600 (4:3)",
        (object) "2: 1024x768 (4:3)",
        (object) "3: 1280x1024 (4:3)",
        (object) "4: 1366x768 (4:3)",
        (object) "5: 1440x900 (16:10)",
        (object) "6: 1600x1050 (16:10)",
        (object) "7: 1920x1080 (16:9)"
      });
    else
      this.m_Resolution.Items.AddRange(new object[11]
      {
        (object) "1: 800x600",
        (object) "2: 1024x768",
        (object) "3: 1152x864",
        (object) "4: 1280x720",
        (object) "5: 1280x800",
        (object) "6: 1280x960",
        (object) "7: 1400x1050",
        (object) "8: 1400x900",
        (object) "9: 1600x900",
        (object) "10: 1680x1050",
        (object) "11: 1920x1080"
      });
    this.m_Resolution.Location = new Point(61, 229);
    this.m_Resolution.Name = "m_Resolution";
    this.m_Resolution.Size = new Size(122, 21);
    this.m_Resolution.TabIndex = 20;
    this.comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBox2.FormattingEnabled = true;
    this.comboBox2.Items.AddRange(new object[1]
    {
      (object) "English"
    });
    this.comboBox2.Location = new Point(240 /*0xF0*/, 229);
    this.comboBox2.Name = "comboBox2";
    this.comboBox2.Size = new Size(122, 21);
    this.comboBox2.TabIndex = 21;
    this.textBox1.Font = new Font("Microsoft Sans Serif", 10.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
    this.textBox1.Location = new Point(170, 70);
    this.textBox1.MaxLength = 20;
    this.textBox1.Name = "textBox1";
    this.textBox1.Size = new Size(190, 23);
    this.textBox1.TabIndex = 22;
    this.checkBox1.AutoSize = true;
    this.checkBox1.BackColor = Color.Transparent;
    this.checkBox1.ForeColor = Color.CornflowerBlue;
    this.checkBox1.Location = new Point(92, 169);
    this.checkBox1.Name = "checkBox1";
    this.checkBox1.Size = new Size(57, 17);
    this.checkBox1.TabIndex = 23;
    this.checkBox1.Text = "Sound";
    this.checkBox1.UseVisualStyleBackColor = false;
    this.checkBox2.AutoSize = true;
    this.checkBox2.BackColor = Color.Transparent;
    this.checkBox2.ForeColor = Color.CornflowerBlue;
    this.checkBox2.Location = new Point(235, 169);
    this.checkBox2.Name = "checkBox2";
    this.checkBox2.Size = new Size(54, 17);
    this.checkBox2.TabIndex = 24;
    this.checkBox2.Text = "Music";
    this.checkBox2.UseVisualStyleBackColor = false;
    this.radioButton1.AutoSize = true;
    this.radioButton1.BackColor = Color.Transparent;
    this.radioButton1.ForeColor = Color.CornflowerBlue;
    this.radioButton1.Location = new Point(92, 124);
    this.radioButton1.Name = "radioButton1";
    this.radioButton1.Size = new Size(104, 17);
    this.radioButton1.TabIndex = 25;
    this.radioButton1.TabStop = true;
    this.radioButton1.Text = "Min Color (16 bit)";
    this.radioButton1.UseVisualStyleBackColor = false;
    this.radioButton2.AutoSize = true;
    this.radioButton2.BackColor = Color.Transparent;
    this.radioButton2.ForeColor = Color.CornflowerBlue;
    this.radioButton2.Location = new Point(235, 124);
    this.radioButton2.Name = "radioButton2";
    this.radioButton2.Size = new Size(107, 17);
    this.radioButton2.TabIndex = 26;
    this.radioButton2.TabStop = true;
    this.radioButton2.Text = "Max Color (32 bit)";
    this.radioButton2.UseVisualStyleBackColor = false;
    this.CopyRightLabel.AutoSize = true;
    this.CopyRightLabel.BackColor = Color.Transparent;
    this.CopyRightLabel.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
    this.CopyRightLabel.ForeColor = SystemColors.AppWorkspace;
    this.CopyRightLabel.Location = new Point(12, 9);
    this.CopyRightLabel.Name = "CopyRightLabel";
    this.CopyRightLabel.Size = new Size(121, 13);
    this.CopyRightLabel.TabIndex = 42;
    this.CopyRightLabel.Text = "Coded by MyHeart (RZ)";
    this.label1.AutoSize = true;
    this.label1.BackColor = Color.Transparent;
    this.label1.Font = new Font("Tahoma", 11.25f);
    this.label1.ForeColor = Color.DarkOrange;
    this.label1.Location = new Point(104, 72);
    this.label1.Name = "label1";
    this.label1.Size = new Size(60, 18);
    this.label1.TabIndex = 43;
    this.label1.Text = "Account";
    this.label2.AutoSize = true;
    this.label2.BackColor = Color.Transparent;
    this.label2.Font = new Font("Tahoma", 11.25f);
    this.label2.ForeColor = Color.DarkOrange;
    this.label2.Location = new Point(58, 208 /*0xD0*/);
    this.label2.Name = "label2";
    this.label2.Size = new Size(73, 18);
    this.label2.TabIndex = 44;
    this.label2.Text = "Resolution";
    this.label3.AutoSize = true;
    this.label3.BackColor = Color.Transparent;
    this.label3.Font = new Font("Tahoma", 11.25f);
    this.label3.ForeColor = Color.DarkOrange;
    this.label3.Location = new Point(237, 208 /*0xD0*/);
    this.label3.Name = "label3";
    this.label3.Size = new Size(71, 18);
    this.label3.TabIndex = 45;
    this.label3.Text = "Language";
    this.m_Resolution_S12.DropDownStyle = ComboBoxStyle.DropDownList;
    this.m_Resolution_S12.FormattingEnabled = true;
    this.m_Resolution_S12.Items.AddRange(new object[10]
    {
      (object) "1: 800x600 (4:3)",
      (object) "2: 1024x768 (4:3)",
      (object) "3: 1152x864 (4:3)",
      (object) "4: 1280x720 (16:9)",
      (object) "5: 1280x800 (16:10)",
      (object) "6: 1280x960 (4:3)",
      (object) "7: 1600x900 (16:9)",
      (object) "8: 1680x1050 (16:10)",
      (object) "9: 1920x1080 (16:9)",
      (object) "10: 1440x900 (16:10)"
    });
    this.m_Resolution_S12.Location = new Point(170, 192 /*0xC0*/);
    this.m_Resolution_S12.Name = "m_Resolution_S12";
    this.m_Resolution_S12.Size = new Size(190, 21);
    this.m_Resolution_S12.TabIndex = 46;
    this.m_Resolution_S12.Visible = false;
    this.label4.AutoSize = true;
    this.label4.BackColor = Color.Transparent;
    this.label4.Font = new Font("Tahoma", 11.25f);
    this.label4.ForeColor = Color.DarkOrange;
    this.label4.Location = new Point(91, 191);
    this.label4.Name = "label4";
    this.label4.Size = new Size(73, 18);
    this.label4.TabIndex = 47;
    this.label4.Text = "Resolution";
    this.label4.Visible = false;
    this.AutoScaleDimensions = new SizeF(6f, 13f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.BackgroundImage = (Image) Resources.setting_back;
    this.BackgroundImageLayout = ImageLayout.None;
    this.ClientSize = new Size(446, 461);
    this.Controls.Add((Control) this.label4);
    this.Controls.Add((Control) this.m_Resolution_S12);
    this.Controls.Add((Control) this.label3);
    this.Controls.Add((Control) this.label2);
    this.Controls.Add((Control) this.label1);
    this.Controls.Add((Control) this.CopyRightLabel);
    this.Controls.Add((Control) this.radioButton2);
    this.Controls.Add((Control) this.radioButton1);
    this.Controls.Add((Control) this.checkBox2);
    this.Controls.Add((Control) this.checkBox1);
    this.Controls.Add((Control) this.textBox1);
    this.Controls.Add((Control) this.comboBox2);
    this.Controls.Add((Control) this.m_Resolution);
    this.Controls.Add((Control) this.SaveButton);
    this.Controls.Add((Control) this.CloseButton);
    this.FormBorderStyle = FormBorderStyle.None;
    this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
    this.Name = nameof (Options);
    this.StartPosition = FormStartPosition.CenterParent;
    this.Text = nameof (Options);
    this.TransparencyKey = Color.Magenta;
    this.Load += new EventHandler(this.Options_Load);
    this.MouseDown += new MouseEventHandler(this.Options_MouseDown);
    ((ISupportInitialize) this.SaveButton).EndInit();
    ((ISupportInitialize) this.CloseButton).EndInit();
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
