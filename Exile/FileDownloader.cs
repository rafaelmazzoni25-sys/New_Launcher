// Decompiled with JetBrains decompiler
// Type: Launcher.Exile.FileDownloader
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 232F895E-1583-4AAE-8C54-19D96214944A
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Client\Launcher.exe

using Launcher.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;

#nullable disable
namespace Launcher.Exile;

internal class FileDownloader
{
  private static int curFile;
  private static long lastBytes;
  private static long currentBytes;
  private static Stopwatch stopWatch = new Stopwatch();

  public static void DownloadFile()
  {
    if (Globals.OldFiles.Count <= 0)
    {
      Common.ChangeStatus("CHECKCOMPLETE");
      Common.UpdateCompleteProgress(100L);
      Globals.MainForm.StartButton.BackgroundImage = (Image) Resources.start_1;
      Common.EnableStart();
    }
    else if (FileDownloader.curFile >= Globals.OldFiles.Count)
    {
      Common.ChangeStatus("DOWNLOADCOMPLETE");
      Globals.MainForm.StartButton.BackgroundImage = (Image) Resources.start_1;
      Common.EnableStart();
    }
    else
    {
      if (Globals.OldFiles[FileDownloader.curFile].Contains("\\"))
        Directory.CreateDirectory(Path.GetDirectoryName(Globals.OldFiles[FileDownloader.curFile]));
      WebClient webClient = new WebClient();
      webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(FileDownloader.webClient_DownloadProgressChanged);
      webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(FileDownloader.webClient_DownloadFileCompleted);
      FileDownloader.stopWatch.Start();
      webClient.DownloadFileAsync(new Uri(Globals.ServerURL + Globals.OldFiles[FileDownloader.curFile]), Globals.OldFiles[FileDownloader.curFile].Remove(Globals.OldFiles[FileDownloader.curFile].Length - 4, 4));
    }
  }

  private static void webClient_DownloadProgressChanged(
    object sender,
    DownloadProgressChangedEventArgs e)
  {
    FileDownloader.currentBytes = FileDownloader.lastBytes + e.BytesReceived;
    Common.ChangeStatus("DOWNLOADFILE", Globals.OldFiles[FileDownloader.curFile].Remove(Globals.OldFiles[FileDownloader.curFile].Length - 4, 4));
    Common.UpdateCurrentProgress((long) e.ProgressPercentage, Computer.ComputeDownloadSpeed((double) e.BytesReceived, FileDownloader.stopWatch));
  }

  private static void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
  {
    FileDownloader.lastBytes = FileDownloader.currentBytes;
    Common.UpdateCompleteProgress(100L);
    ++FileDownloader.curFile;
    FileDownloader.stopWatch.Reset();
    FileDownloader.DownloadFile();
  }
}
