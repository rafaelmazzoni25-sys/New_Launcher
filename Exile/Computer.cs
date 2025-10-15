// Decompiled with JetBrains decompiler
// Type: Launcher.Exile.Computer
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 232F895E-1583-4AAE-8C54-19D96214944A
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Client\Launcher.exe

using System.Diagnostics;

#nullable disable
namespace Launcher.Exile;

internal class Computer
{
  public static long Compute(long Size) => Size * 100L / Globals.fullSize;

  public static double ComputeDownloadSize(double Size) => Size / 1024.0 / 1024.0;

  public static double ComputeDownloadSpeed(double Size, Stopwatch stopWatch)
  {
    return Size / 1024.0 / stopWatch.Elapsed.TotalSeconds;
  }
}
