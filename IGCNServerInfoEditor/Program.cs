// Decompiled with JetBrains decompiler
// Type: IGCNServerInfoEditor.Program
// Assembly: IGCNServerInfoEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 42E4CD18-0BEB-4B8E-82BE-8F1631EA8A60
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Sources\Main Source\IGCNServerInfoEditor.exe

using System;
using System.Windows.Forms;

#nullable disable
namespace IGCNServerInfoEditor;

internal static class Program
{
  [STAThread]
  private static void Main()
  {
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.Run((Form) new Form1());
  }
}
