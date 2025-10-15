// Decompiled with JetBrains decompiler
// Type: Launcher.Exile.Languages
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 232F895E-1583-4AAE-8C54-19D96214944A
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Client\Launcher.exe

using System.Collections.Generic;

#nullable disable
namespace Launcher.Exile;

internal class Languages
{
  private static Dictionary<string, string> TextEng = new Dictionary<string, string>()
  {
    {
      "UNKNOWNERROR",
      "Cannot connect to server: \n{0}"
    },
    {
      "MISSINGBINARY",
      "This file is corrupt or missing {0}."
    },
    {
      "CANNOTSTART",
      "Unable to start Main.exe, please run Lancher as Administrator."
    },
    {
      "NONETWORK",
      "The update server is not responding or is not running right now."
    },
    {
      "CONNECTING",
      "Connecting to server."
    },
    {
      "LISTDOWNLOAD",
      "Analyzing patch info.."
    },
    {
      "CHECKFILE",
      "Initializing.."
    },
    {
      "DOWNLOADFILE",
      "Downloading: {0}"
    },
    {
      "COMPLETEPROGRESS",
      "Updated: {0}%"
    },
    {
      "CURRENTPROGRESS",
      "Downloading: {0}%  |  {1} kb/s"
    },
    {
      "CHECKCOMPLETE",
      "Ready to start."
    },
    {
      "DOWNLOADCOMPLETE",
      "Ready to start."
    },
    {
      "DOWNLOADSPEED",
      "{0} kb/s"
    },
    {
      "ALREADYRUNNING",
      "Another Autoupdate is running.\nDo you want to continue?"
    },
    {
      "S_ACCOUNT",
      "Account"
    },
    {
      "S_COLOR16",
      "Min Color (16bit)"
    },
    {
      "S_COLOR32",
      "Max Color (32bit)"
    },
    {
      "S_SOUND",
      "Sound"
    },
    {
      "S_MUSIC",
      "Music"
    },
    {
      "S_RESOLUTION",
      "Resolution"
    },
    {
      "S_LANGUAGE",
      "Language"
    },
    {
      "WINDOW_MODE",
      "Winmode"
    }
  };
  private static Dictionary<string, string> TextSpn = new Dictionary<string, string>()
  {
    {
      "UNKNOWNERROR",
      "No es posible conectar con el servidor: \n{0}"
    },
    {
      "MISSINGBINARY",
      "Este archivo está dañado o falta {0}."
    },
    {
      "CANNOTSTART",
      "No se puede iniciar Main.exe, ejecute Lancher como administrador."
    },
    {
      "NONETWORK",
      "El servidor de actualización no responde o no se está ejecutando ahora."
    },
    {
      "CONNECTING",
      "Conectando al servidor."
    },
    {
      "LISTDOWNLOAD",
      "Analizando la información del parche.."
    },
    {
      "CHECKFILE",
      "Inicializando.."
    },
    {
      "DOWNLOADFILE",
      "Descargando: {0}"
    },
    {
      "COMPLETEPROGRESS",
      "Actualizado: {0}%"
    },
    {
      "CURRENTPROGRESS",
      "Descargando: {0}%  |  {1} kb/s"
    },
    {
      "CHECKCOMPLETE",
      "Listo para empezar."
    },
    {
      "DOWNLOADCOMPLETE",
      "Listo para empezar."
    },
    {
      "DOWNLOADSPEED",
      "{0} kb/s"
    },
    {
      "ALREADYRUNNING",
      "Se está ejecutando otra actualización automática.\n¿Quieres continuar?"
    },
    {
      "S_ACCOUNT",
      "Cuenta"
    },
    {
      "S_COLOR16",
      "Color mínimo (16bit)"
    },
    {
      "S_COLOR32",
      "Color máximo (32bit)"
    },
    {
      "S_SOUND",
      "Sonar"
    },
    {
      "S_MUSIC",
      "Música"
    },
    {
      "S_RESOLUTION",
      "Resolución"
    },
    {
      "S_LANGUAGE",
      "Idioma"
    },
    {
      "WINDOW_MODE",
      "Modo ventana"
    }
  };
  private static Dictionary<string, string> TextPor = new Dictionary<string, string>()
  {
    {
      "UNKNOWNERROR",
      "Não é possível se conectar ao servidor: \n{0}"
    },
    {
      "MISSINGBINARY",
      "Este arquivo está corrompido ou ausente {0}."
    },
    {
      "CANNOTSTART",
      "Não é possível iniciar o Main.exe, por favor execute o Lancher como Administrador."
    },
    {
      "NONETWORK",
      "O servidor de atualização não está respondendo ou não está em execução no momento."
    },
    {
      "CONNECTING",
      "Conectando ao servidor."
    },
    {
      "LISTDOWNLOAD",
      "Analisando a informação do patch .."
    },
    {
      "CHECKFILE",
      "Inicializando.."
    },
    {
      "DOWNLOADFILE",
      "Downloading: {0}"
    },
    {
      "COMPLETEPROGRESS",
      "Atualizada: {0}%"
    },
    {
      "CURRENTPROGRESS",
      "Downloading: {0}%  |  {1} kb/s"
    },
    {
      "CHECKCOMPLETE",
      "Pronto para começar."
    },
    {
      "DOWNLOADCOMPLETE",
      "Pronto para começar."
    },
    {
      "DOWNLOADSPEED",
      "{0} kb/s"
    },
    {
      "ALREADYRUNNING",
      "Outro Autoupdate está sendo executado.\nDeseja continuar?"
    },
    {
      "S_ACCOUNT",
      "Conta"
    },
    {
      "S_COLOR16",
      "Min Color (16bit)"
    },
    {
      "S_COLOR32",
      "Max Color (32bit)"
    },
    {
      "S_SOUND",
      "Som"
    },
    {
      "S_MUSIC",
      "Música"
    },
    {
      "S_RESOLUTION",
      "Resolução"
    },
    {
      "S_LANGUAGE",
      "Língua"
    },
    {
      "WINDOW_MODE",
      "Modo janela"
    }
  };

  public static string GetText(string Key, params object[] Arguments)
  {
    if (Globals.Language == 2)
    {
      foreach (KeyValuePair<string, string> keyValuePair in Languages.TextSpn)
      {
        if (keyValuePair.Key == Key)
          return string.Format(keyValuePair.Value, Arguments);
      }
    }
    if (Globals.Language == 3)
    {
      foreach (KeyValuePair<string, string> keyValuePair in Languages.TextPor)
      {
        if (keyValuePair.Key == Key)
          return string.Format(keyValuePair.Value, Arguments);
      }
    }
    else
    {
      foreach (KeyValuePair<string, string> keyValuePair in Languages.TextEng)
      {
        if (keyValuePair.Key == Key)
          return string.Format(keyValuePair.Value, Arguments);
      }
    }
    return (string) null;
  }
}
