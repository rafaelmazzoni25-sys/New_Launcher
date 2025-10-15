// Decompiled with JetBrains decompiler
// Type: Launcher.Exile.CRegistry
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 232F895E-1583-4AAE-8C54-19D96214944A
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Client\Launcher.exe

using Microsoft.Win32;

#nullable disable
namespace Launcher.Exile;

internal class CRegistry
{
  private static string BaseWzKey = "Software\\Webzen\\Mu\\Config";

  public static void CreateKey()
  {
    try
    {
      using (RegistryKey subKey = Registry.CurrentUser.CreateSubKey(CRegistry.BaseWzKey))
        subKey.Close();
    }
    catch
    {
    }
  }

  public static bool Update(string Subkey, int Value)
  {
    try
    {
      using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(CRegistry.BaseWzKey, true))
      {
        if (registryKey != null)
        {
          registryKey.SetValue(Subkey, (object) Value, RegistryValueKind.DWord);
          registryKey.Close();
          return true;
        }
      }
    }
    catch
    {
    }
    return false;
  }

  public static bool Update(string Subkey, string Value)
  {
    try
    {
      using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(CRegistry.BaseWzKey, true))
      {
        if (registryKey != null)
        {
          registryKey.SetValue(Subkey, (object) Value, RegistryValueKind.String);
          registryKey.Close();
          return true;
        }
      }
    }
    catch
    {
    }
    return false;
  }

  public static int GetValueInt(string Subkey)
  {
    try
    {
      using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(CRegistry.BaseWzKey, true))
      {
        if (registryKey != null)
        {
          int valueInt = (int) registryKey.GetValue(Subkey);
          registryKey.Close();
          return valueInt;
        }
      }
    }
    catch
    {
    }
    return 0;
  }

  public static string GetValueString(string Subkey)
  {
    try
    {
      using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(CRegistry.BaseWzKey, true))
      {
        if (registryKey != null)
        {
          string valueString = registryKey.GetValue(Subkey).ToString();
          registryKey.Close();
          return valueString;
        }
      }
    }
    catch
    {
    }
    return string.Empty;
  }
}
