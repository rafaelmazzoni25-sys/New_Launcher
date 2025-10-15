// Decompiled with JetBrains decompiler
// Type: Update.Maker.Program
// Assembly: Generator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F664580-F6B4-4B6D-A5C6-C03B862A74BB
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Generator\Update Generator.exe

using System;
using System.Windows.Forms;

#nullable disable
namespace Update.Maker;

internal static class Program
{
  [STAThread]
  private static void Main()
  {
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.Run((Form) new lForm());
  }
}
