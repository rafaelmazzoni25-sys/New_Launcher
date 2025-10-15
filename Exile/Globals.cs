// Decompiled with JetBrains decompiler
// Type: Launcher.Exile.Globals
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 232F895E-1583-4AAE-8C54-19D96214944A
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Client\Launcher.exe

using System.Collections.Generic;

#nullable disable
namespace Launcher.Exile;

internal class Globals
{
  public static string ServerURL = string.Empty;
  public static string PatchlistName = "update.txt";
  public static string BinaryName = "Main.exe";
  public static string Caption = "Launcher";
  public static bool EnableStartBTN = false;
  public static MainForm MainForm;
  public static bool UseSeason12 = false;
  public static bool UseSeason9 = false;
  public static List<Globals.File> Files = new List<Globals.File>();
  public static List<string> OldFiles = new List<string>();
  public static long fullSize;
  public static long completeSize;
  public static int Language = 1;

  public struct File
  {
    public string Name;
    public string Hash;
  }
}
