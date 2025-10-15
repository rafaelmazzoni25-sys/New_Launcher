// Decompiled with JetBrains decompiler
// Type: Launcher.Exile.LauncherOptions
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 232F895E-1583-4AAE-8C54-19D96214944A
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Client\Launcher.exe

using System;

#nullable disable
namespace Launcher.Exile;

internal class LauncherOptions
{
  public static int m_DevModeIndex = 0;
  public static int m_WindowMode = 0;
  public static string m_ID = string.Empty;
  private static readonly string m_FileName = "LauncherOption.if";

  public static void GetValue()
  {
    try
    {
      foreach (string readAllLine in System.IO.File.ReadAllLines(LauncherOptions.m_FileName))
      {
        if (readAllLine.Contains("DevModeIndex:"))
          LauncherOptions.m_DevModeIndex = Convert.ToInt32(readAllLine.Split(':')[1]);
        if (readAllLine.Contains("WindowMode:"))
          LauncherOptions.m_WindowMode = Convert.ToInt32(readAllLine.Split(':')[1]);
        if (readAllLine.Contains("ID:"))
          LauncherOptions.m_ID = readAllLine.Split(':')[1];
      }
    }
    catch
    {
    }
  }

  public static void SetValue(int DevModeIndex, int WindowMode, string ID)
  {
    try
    {
      LauncherOptions.m_DevModeIndex = DevModeIndex;
      LauncherOptions.m_WindowMode = WindowMode;
      LauncherOptions.m_ID = ID;
      string[] contents = new string[3]
      {
        $"DevModeIndex:{DevModeIndex}",
        $"WindowMode:{WindowMode}",
        $"ID:{ID}"
      };
      System.IO.File.WriteAllLines(LauncherOptions.m_FileName, contents);
    }
    catch
    {
    }
  }
}
