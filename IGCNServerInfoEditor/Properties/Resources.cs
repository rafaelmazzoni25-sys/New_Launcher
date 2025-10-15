// Decompiled with JetBrains decompiler
// Type: IGCNServerInfoEditor.Properties.Resources
// Assembly: IGCNServerInfoEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 42E4CD18-0BEB-4B8E-82BE-8F1631EA8A60
// Assembly location: C:\Users\rafae\Downloads\MuOnline\Sources\Main Source\IGCNServerInfoEditor.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace IGCNServerInfoEditor.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
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
      if (IGCNServerInfoEditor.Properties.Resources.resourceMan == null)
        IGCNServerInfoEditor.Properties.Resources.resourceMan = new ResourceManager("IGCNServerInfoEditor.Properties.Resources", typeof (IGCNServerInfoEditor.Properties.Resources).Assembly);
      return IGCNServerInfoEditor.Properties.Resources.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static CultureInfo Culture
  {
    get => IGCNServerInfoEditor.Properties.Resources.resourceCulture;
    set => IGCNServerInfoEditor.Properties.Resources.resourceCulture = value;
  }

  internal static Bitmap mu2
  {
    get => (Bitmap) IGCNServerInfoEditor.Properties.Resources.ResourceManager.GetObject(nameof (mu2), IGCNServerInfoEditor.Properties.Resources.resourceCulture);
  }
}
