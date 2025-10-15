// Decompiled with JetBrains decompiler
// Type: Update.Maker.Properties.Resources
// Assembly: Generator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F664580-F6B4-4B6D-A5C6-C03B862A74BB
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Tools\MyHart Tools\Launcher Free\Generator\Update Generator.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace Update.Maker.Properties;

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
      if (Update.Maker.Properties.Resources.resourceMan == null)
        Update.Maker.Properties.Resources.resourceMan = new ResourceManager("Update.Maker.Properties.Resources", typeof (Update.Maker.Properties.Resources).Assembly);
      return Update.Maker.Properties.Resources.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static CultureInfo Culture
  {
    get => Update.Maker.Properties.Resources.resourceCulture;
    set => Update.Maker.Properties.Resources.resourceCulture = value;
  }
}
