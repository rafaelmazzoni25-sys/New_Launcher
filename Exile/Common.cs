// Decompiled with JetBrains decompiler
// Type: Launcher.Exile.Common
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 232F895E-1583-4AAE-8C54-19D96214944A
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Client\Launcher.exe

using Cyclic.Redundancy.Check;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#nullable disable
namespace Launcher.Exile;

internal class Common
{
  public const int WM_NCLBUTTONDOWN = 161;
  public const int HT_CAPTION = 2;

  public static void ChangeStatus(string Key, params string[] Arguments)
  {
    Globals.MainForm.Status.Text = Languages.GetText(Key, (object[]) Arguments);
  }

  public static void ChangeProgresss(float fPercent)
  {
    float num = (float) (619.0 * ((double) fPercent / 100.0) + 2.0 * Math.Sin((double) DateTime.Now.Ticks / 100.0));
    if ((double) num < 0.0 || (double) fPercent < 1.0099999904632568)
      num = 0.0f;
    if ((double) num > 619.0 || (double) fPercent > 99.989997863769531)
      num = 619f;
    Globals.MainForm.pictureBox6.Update();
    Globals.MainForm.pictureBox6.Width = (int) num;
  }

  public static void UpdateCompleteProgress(long Value)
  {
    if (Value < 0L || Value > 100L)
      return;
    Common.ChangeProgresss((float) Value);
  }

  public static void UpdateCurrentProgress(long Value, double Speed)
  {
    if (Value < 0L || Value > 100L)
      return;
    Common.ChangeProgresss((float) Value);
  }

  public static string GetHash(string Name)
  {
    if (Name == string.Empty)
      return string.Empty;
    CRC crc = new CRC();
    string empty = string.Empty;
    using (FileStream inputStream = System.IO.File.Open(Name, FileMode.Open))
    {
      foreach (byte num in crc.ComputeHash((Stream) inputStream))
        empty += num.ToString("x2").ToUpper();
    }
    return empty;
  }

  public static void EnableStart()
  {
    Globals.MainForm.StartButton.Enabled = true;
    Globals.MainForm.pictureBox3.Enabled = true;
    Globals.MainForm.pictureBox4.Enabled = true;
    Globals.MainForm.StartButton.Cursor = Cursors.Hand;
    Globals.EnableStartBTN = true;
  }

  [DllImport("user32.dll")]
  public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

  [DllImport("user32.dll")]
  public static extern bool ReleaseCapture();
}
