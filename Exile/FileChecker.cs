﻿// Decompiled with JetBrains decompiler
// Type: Launcher.Exile.FileChecker
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 232F895E-1583-4AAE-8C54-19D96214944A
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Client\Launcher.exe

using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

#nullable disable
namespace Launcher.Exile;

internal class FileChecker
{
  private static BackgroundWorker backgroundWorker = new BackgroundWorker();

  public static void CheckFiles()
  {
    FileChecker.backgroundWorker.WorkerReportsProgress = true;
    FileChecker.backgroundWorker.DoWork += new DoWorkEventHandler(FileChecker.backgroundWorker_DoWork);
    FileChecker.backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(FileChecker.backgroundWorker_ProgressChanged);
    FileChecker.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FileChecker.backgroundWorker_RunWorkerCompleted);
    if (FileChecker.backgroundWorker.IsBusy)
    {
      int num = (int) MessageBox.Show(Languages.GetText("UNKNOWNERROR", (object) "CheckFiles isBusy"), Globals.Caption);
      Application.Exit();
    }
    else
      FileChecker.backgroundWorker.RunWorkerAsync();
  }

  private static void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
  {
    foreach (Globals.File file in Globals.Files)
    {
      ++Globals.fullSize;
      FileChecker.backgroundWorker.ReportProgress(0, (object) Path.GetFileName(file.Name));
      if (file.Name.ToLower().Contains("launcher.exe") || file.Name.ToLower().Contains("mu.exe"))
      {
        ++Globals.completeSize;
        FileChecker.backgroundWorker.ReportProgress(1);
      }
      else if (!System.IO.File.Exists(file.Name.Remove(file.Name.Length - 4, 4)) || Common.GetHash(file.Name.Remove(file.Name.Length - 4, 4)) != file.Hash)
      {
        Globals.OldFiles.Add(file.Name);
      }
      else
      {
        ++Globals.completeSize;
        FileChecker.backgroundWorker.ReportProgress(1);
      }
    }
  }

  private static void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
  {
    if (e.ProgressPercentage != 0)
      return;
    Common.ChangeStatus("CHECKFILE");
  }

  private static void backgroundWorker_RunWorkerCompleted(
    object sender,
    RunWorkerCompletedEventArgs e)
  {
    FileDownloader.DownloadFile();
  }

  private enum State
  {
    REPORT_NAME,
    REPORT_PROGRESS,
  }
}
