// Decompiled with JetBrains decompiler
// Type: ConfigGenerator.Properties.Resources
// Assembly: ConfigGenerator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E333375F-E325-44D6-8F8A-127F1A7AB356
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Generator\ConfigGenerator.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace ConfigGenerator.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
  private static ResourceManager resourceMan;
  private static CultureInfo resourceCulture;

  internal Resources()
  {
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static ResourceManager ResourceManager
  {
    get
    {
      if (ConfigGenerator.Properties.Resources.resourceMan == null)
        ConfigGenerator.Properties.Resources.resourceMan = new ResourceManager("ConfigGenerator.Properties.Resources", typeof (ConfigGenerator.Properties.Resources).Assembly);
      return ConfigGenerator.Properties.Resources.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static CultureInfo Culture
  {
    get => ConfigGenerator.Properties.Resources.resourceCulture;
    set => ConfigGenerator.Properties.Resources.resourceCulture = value;
  }
}
