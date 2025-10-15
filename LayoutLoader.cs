using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;

namespace Launcher;

internal static class LayoutLoader
{
  private class LayoutDefinition
  {
    public FormDefinition Form { get; set; }
    public Dictionary<string, ControlDefinition> Controls { get; set; }
  }

  private class FormDefinition
  {
    public int[] ClientSize { get; set; }
    public string BackgroundImage { get; set; }
    public bool? Visible { get; set; }
    public string Text { get; set; }
  }

  private class ControlDefinition
  {
    public int[] Location { get; set; }
    public int[] Size { get; set; }
    public string BackgroundImage { get; set; }
    public string Image { get; set; }
    public string Text { get; set; }
    public bool? Visible { get; set; }
    public bool? Enabled { get; set; }
    public string SizeMode { get; set; }
  }

  public static void ApplyLayout(Form form, string layoutPath)
  {
    if (form == null)
      throw new ArgumentNullException(nameof (form));
    if (string.IsNullOrEmpty(layoutPath) || !File.Exists(layoutPath))
      return;
    try
    {
      string extension = Path.GetExtension(layoutPath);
      if (!string.IsNullOrEmpty(extension) && string.Compare(extension, ".json", StringComparison.OrdinalIgnoreCase) == 0)
      {
        ApplyJsonLayout(form, layoutPath);
      }
      else if (!string.IsNullOrEmpty(extension) && string.Compare(extension, ".svg", StringComparison.OrdinalIgnoreCase) == 0)
      {
        ApplySvgLayout(form, layoutPath);
      }
    }
    catch
    {
    }
  }

  private static void ApplyJsonLayout(Form form, string layoutPath)
  {
    string input = File.ReadAllText(layoutPath);
    JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
    LayoutDefinition layoutDefinition = scriptSerializer.Deserialize<LayoutDefinition>(input);
    if (layoutDefinition == null)
      return;
    if (layoutDefinition.Form != null)
      ApplyFormDefinition(form, layoutDefinition.Form, layoutPath);
    if (layoutDefinition.Controls == null)
      return;
    foreach (KeyValuePair<string, ControlDefinition> control in layoutDefinition.Controls)
      ApplyControlDefinition(form, control.Key, control.Value, layoutPath);
  }

  private static void ApplySvgLayout(Form form, string layoutPath)
  {
    XmlDocument xmlDocument = new XmlDocument();
    xmlDocument.Load(layoutPath);
    XmlElement documentElement = xmlDocument.DocumentElement;
    if (documentElement != null)
    {
      int? attribute = ReadIntAttribute(documentElement, "width");
      int? nullable = ReadIntAttribute(documentElement, "height");
      if (attribute.HasValue && nullable.HasValue)
        form.ClientSize = new Size(attribute.Value, nullable.Value);
      string attribute1 = documentElement.GetAttribute("data-background");
      if (!string.IsNullOrEmpty(attribute1))
        TryApplyBackgroundImage(form, layoutPath, attribute1);
      string attribute2 = documentElement.GetAttribute("data-text");
      if (!string.IsNullOrEmpty(attribute2))
        form.Text = attribute2;
      string attribute3 = documentElement.GetAttribute("display");
      if (!string.IsNullOrEmpty(attribute3))
        form.Visible = !string.Equals(attribute3, "none", StringComparison.OrdinalIgnoreCase);
    }
    XmlNodeList elementsByTagName = xmlDocument.SelectNodes("//*[@id]");
    if (elementsByTagName == null)
      return;
    foreach (XmlNode xmlNode in elementsByTagName)
    {
      XmlElement xmlElement = xmlNode as XmlElement;
      if (xmlElement != null)
      {
        string attribute4 = xmlElement.GetAttribute("id");
        if (!string.IsNullOrEmpty(attribute4))
        {
          Control control = FindControl(form, attribute4);
          if (control != null)
          {
            int? attribute5 = ReadIntAttribute(xmlElement, "x");
            int? attribute6 = ReadIntAttribute(xmlElement, "y");
            if (attribute5.HasValue && attribute6.HasValue)
              control.Location = new Point(attribute5.Value, attribute6.Value);
            int? attribute7 = ReadIntAttribute(xmlElement, "width");
            int? attribute8 = ReadIntAttribute(xmlElement, "height");
            if (attribute7.HasValue && attribute8.HasValue)
              control.Size = new Size(attribute7.Value, attribute8.Value);
            string attribute9 = xmlElement.GetAttribute("data-background");
            if (!string.IsNullOrEmpty(attribute9))
              TryApplyBackgroundImage(control, layoutPath, attribute9);
            string attribute10 = xmlElement.GetAttribute("data-image");
            if (!string.IsNullOrEmpty(attribute10))
              TryApplyImage(control, layoutPath, attribute10);
            string attribute11 = xmlElement.GetAttribute("data-text");
            if (!string.IsNullOrEmpty(attribute11))
              control.Text = attribute11;
            string attribute12 = xmlElement.GetAttribute("display");
            if (!string.IsNullOrEmpty(attribute12))
              control.Visible = !string.Equals(attribute12, "none", StringComparison.OrdinalIgnoreCase);
            string attribute13 = xmlElement.GetAttribute("data-enabled");
            if (!string.IsNullOrEmpty(attribute13))
              control.Enabled = ParseBoolean(attribute13, control.Enabled);
            string attribute14 = xmlElement.GetAttribute("data-sizemode");
            if (!string.IsNullOrEmpty(attribute14))
              ApplySizeMode(control, attribute14);
          }
        }
      }
    }
  }

  private static void ApplyFormDefinition(Form form, FormDefinition definition, string layoutPath)
  {
    if (definition.ClientSize != null && definition.ClientSize.Length >= 2)
      form.ClientSize = new Size(definition.ClientSize[0], definition.ClientSize[1]);
    if (!string.IsNullOrEmpty(definition.BackgroundImage))
      TryApplyBackgroundImage(form, layoutPath, definition.BackgroundImage);
    if (!string.IsNullOrEmpty(definition.Text))
      form.Text = definition.Text;
    if (definition.Visible.HasValue)
      form.Visible = definition.Visible.Value;
  }

  private static void ApplyControlDefinition(Form form, string controlName, ControlDefinition definition, string layoutPath)
  {
    if (definition == null)
      return;
    Control control = FindControl(form, controlName);
    if (control == null)
      return;
    if (definition.Location != null && definition.Location.Length >= 2)
      control.Location = new Point(definition.Location[0], definition.Location[1]);
    if (definition.Size != null && definition.Size.Length >= 2)
      control.Size = new Size(definition.Size[0], definition.Size[1]);
    if (!string.IsNullOrEmpty(definition.BackgroundImage))
      TryApplyBackgroundImage(control, layoutPath, definition.BackgroundImage);
    if (!string.IsNullOrEmpty(definition.Image))
      TryApplyImage(control, layoutPath, definition.Image);
    if (!string.IsNullOrEmpty(definition.Text))
      control.Text = definition.Text;
    if (definition.Visible.HasValue)
      control.Visible = definition.Visible.Value;
    if (definition.Enabled.HasValue)
      control.Enabled = definition.Enabled.Value;
    if (!string.IsNullOrEmpty(definition.SizeMode))
      ApplySizeMode(control, definition.SizeMode);
  }

  private static void ApplySizeMode(Control control, string sizeModeValue)
  {
    PictureBox pictureBox = control as PictureBox;
    if (pictureBox == null)
      return;
    try
    {
      PictureBoxSizeMode pictureBoxSizeMode = (PictureBoxSizeMode) Enum.Parse(typeof (PictureBoxSizeMode), sizeModeValue, true);
      pictureBox.SizeMode = pictureBoxSizeMode;
    }
    catch
    {
    }
  }

  private static Control FindControl(Form form, string controlName)
  {
    Control[] controlArray = form.Controls.Find(controlName, true);
    return controlArray.Length != 0 ? controlArray[0] : (Control) null;
  }

  private static void TryApplyBackgroundImage(Control control, string layoutPath, string imagePath)
  {
    Image image = LoadImage(layoutPath, imagePath);
    if (image == null)
      return;
    Image backgroundImage = control.BackgroundImage;
    control.BackgroundImage = image;
    if (backgroundImage == null)
      return;
    backgroundImage.Dispose();
  }

  private static void TryApplyImage(Control control, string layoutPath, string imagePath)
  {
    PictureBox pictureBox = control as PictureBox;
    if (pictureBox == null)
      return;
    Image image = LoadImage(layoutPath, imagePath);
    if (image == null)
      return;
    Image image1 = pictureBox.Image;
    pictureBox.Image = image;
    if (image1 == null)
      return;
    image1.Dispose();
  }

  private static Image LoadImage(string layoutPath, string imagePath)
  {
    string resolvedPath = ResolvePath(layoutPath, imagePath);
    if (string.IsNullOrEmpty(resolvedPath) || !File.Exists(resolvedPath))
      return (Image) null;
    try
    {
      using (FileStream fileStream = new FileStream(resolvedPath, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          CopyStream((Stream) fileStream, (Stream) memoryStream);
          memoryStream.Position = 0L;
          using (Image image = Image.FromStream((Stream) memoryStream))
            return (Image) new Bitmap(image);
        }
      }
    }
    catch
    {
    }
    return (Image) null;
  }

  private static string ResolvePath(string layoutPath, string imagePath)
  {
    if (string.IsNullOrEmpty(imagePath))
      return (string) null;
    if (Path.IsPathRooted(imagePath))
      return imagePath;
    string directoryName = Path.GetDirectoryName(layoutPath);
    return directoryName == null ? imagePath : Path.GetFullPath(Path.Combine(directoryName, imagePath));
  }

  private static int? ReadIntAttribute(XmlElement element, string attributeName)
  {
    string attribute = element.GetAttribute(attributeName);
    if (string.IsNullOrEmpty(attribute))
      return new int?();
    double result;
    if (!double.TryParse(attribute, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      return new int?();
    return new int?((int) Math.Round(result));
  }

  private static bool ParseBoolean(string value, bool defaultValue)
  {
    if (string.IsNullOrEmpty(value))
      return defaultValue;
    if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "1", StringComparison.OrdinalIgnoreCase))
      return true;
    return string.Equals(value, "false", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "0", StringComparison.OrdinalIgnoreCase) ? false : defaultValue;
  }

  private static void CopyStream(Stream source, Stream destination)
  {
    byte[] buffer = new byte[4096];
    int count;
    while ((count = source.Read(buffer, 0, buffer.Length)) > 0)
      destination.Write(buffer, 0, count);
  }
}
