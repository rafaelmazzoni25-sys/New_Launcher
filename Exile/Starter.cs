// Decompiled with JetBrains decompiler
// Type: Launcher.Exile.Starter
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 232F895E-1583-4AAE-8C54-19D96214944A
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Client\Launcher.exe

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace Launcher.Exile;

internal class Starter
{
  public static void Start()
  {
    if (!System.IO.File.Exists(Globals.BinaryName))
    {
      int num1 = (int) MessageBox.Show(Languages.GetText("MISSINGBINARY", (object) Globals.BinaryName), Globals.Caption);
    }
    else
    {
      try
      {
        Process.Start(Globals.BinaryName);
        Globals.MainForm.WindowState = FormWindowState.Minimized;
        Thread.Sleep(5000);
        Application.Exit();
      }
      catch (Exception ex)
      {
        int num2 = (int) MessageBox.Show(Languages.GetText("UNKNOWNERROR", (object) ex.Message), Globals.Caption);
        Application.Exit();
      }
    }
  }
}
