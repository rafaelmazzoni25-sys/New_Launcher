// Decompiled with JetBrains decompiler
// Type: Launcher.Exile.ListDownloader
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 232F895E-1583-4AAE-8C54-19D96214944A
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Client\Launcher.exe

using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;

#nullable disable
namespace Launcher.Exile;

internal class ListDownloader
{
  public static void DownloadList()
  {
    BackgroundWorker backgroundWorker = new BackgroundWorker();
    Common.ChangeStatus("LISTDOWNLOAD");
    backgroundWorker.DoWork += new DoWorkEventHandler(ListDownloader.backgroundWorker_DoWork);
    backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ListDownloader.backgroundWorker_RunWorkerCompleted);
    if (backgroundWorker.IsBusy)
    {
      int num = (int) MessageBox.Show(Languages.GetText("UNKNOWNERROR", (object) "DownloadList isBusy"), Globals.Caption);
      Application.Exit();
    }
    else
      backgroundWorker.RunWorkerAsync();
  }

  private static void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
  {
    StreamReader streamReader = new StreamReader(new WebClient().OpenRead(Globals.ServerURL + Globals.PatchlistName));
    while (!streamReader.EndOfStream)
      ListProcessor.AddFile(streamReader.ReadLine());
  }

  private static void backgroundWorker_RunWorkerCompleted(
    object sender,
    RunWorkerCompletedEventArgs e)
  {
    FileChecker.CheckFiles();
  }
}
