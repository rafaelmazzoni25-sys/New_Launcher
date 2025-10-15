using Launcher.Layout;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;

namespace Launcher
{
    public static class LayoutLoader
    {
        public static void ApplyLayout(Form form, string layoutPath)
        {
            if (form == null)
            {
                throw new ArgumentNullException("form");
            }

            if (string.IsNullOrEmpty(layoutPath) || !File.Exists(layoutPath))
            {
                return;
            }

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
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            LayoutDefinition layoutDefinition = serializer.Deserialize<LayoutDefinition>(input);
            if (layoutDefinition == null)
            {
                return;
            }

            if (layoutDefinition.Form != null)
            {
                ApplyFormDefinition(form, layoutDefinition.Form, layoutPath);
            }

            if (layoutDefinition.Controls == null)
            {
                return;
            }

            foreach (KeyValuePair<string, ControlDefinition> control in layoutDefinition.Controls)
            {
                ApplyControlDefinition(form, control.Key, control.Value, layoutPath);
            }
        }

        private static void ApplySvgLayout(Form form, string layoutPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(layoutPath);
            XmlElement documentElement = xmlDocument.DocumentElement;
            if (documentElement != null)
            {
                int? width = ReadIntAttribute(documentElement, "width");
                int? height = ReadIntAttribute(documentElement, "height");
                if (width.HasValue && height.HasValue)
                {
                    form.ClientSize = new Size(width.Value, height.Value);
                }

                string background = documentElement.GetAttribute("data-background");
                if (!string.IsNullOrEmpty(background))
                {
                    TryApplyBackgroundImage(form, layoutPath, background);
                }

                string caption = documentElement.GetAttribute("data-text");
                if (!string.IsNullOrEmpty(caption))
                {
                    form.Text = caption;
                }

                string visible = documentElement.GetAttribute("display");
                if (!string.IsNullOrEmpty(visible))
                {
                    form.Visible = !string.Equals(visible, "none", StringComparison.OrdinalIgnoreCase);
                }
            }

            XmlNodeList nodes = xmlDocument.SelectNodes("//*[@id]");
            if (nodes == null)
            {
                return;
            }

            foreach (XmlNode node in nodes)
            {
                XmlElement element = node as XmlElement;
                if (element == null)
                {
                    continue;
                }

                string id = element.GetAttribute("id");
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                Control control = FindControl(form, id);
                if (control == null)
                {
                    continue;
                }

                int? x = ReadIntAttribute(element, "x");
                int? y = ReadIntAttribute(element, "y");
                if (x.HasValue && y.HasValue)
                {
                    control.Location = new Point(x.Value, y.Value);
                }

                int? width = ReadIntAttribute(element, "width");
                int? height = ReadIntAttribute(element, "height");
                if (width.HasValue && height.HasValue)
                {
                    control.Size = new Size(width.Value, height.Value);
                }

                string background = element.GetAttribute("data-background");
                if (!string.IsNullOrEmpty(background))
                {
                    TryApplyBackgroundImage(control, layoutPath, background);
                }

                string image = element.GetAttribute("data-image");
                if (!string.IsNullOrEmpty(image))
                {
                    TryApplyImage(control, layoutPath, image);
                }

                string text = element.GetAttribute("data-text");
                if (!string.IsNullOrEmpty(text))
                {
                    control.Text = text;
                }

                string visible = element.GetAttribute("display");
                if (!string.IsNullOrEmpty(visible))
                {
                    control.Visible = !string.Equals(visible, "none", StringComparison.OrdinalIgnoreCase);
                }

                string enabled = element.GetAttribute("data-enabled");
                if (!string.IsNullOrEmpty(enabled))
                {
                    control.Enabled = ParseBoolean(enabled, control.Enabled);
                }

                string sizeMode = element.GetAttribute("data-sizemode");
                if (!string.IsNullOrEmpty(sizeMode))
                {
                    ApplySizeMode(control, sizeMode);
                }
            }
        }

        private static void ApplyFormDefinition(Form form, FormDefinition definition, string layoutPath)
        {
            if (definition.ClientSize != null && definition.ClientSize.Length >= 2)
            {
                form.ClientSize = new Size(definition.ClientSize[0], definition.ClientSize[1]);
            }

            if (!string.IsNullOrEmpty(definition.BackgroundImage))
            {
                TryApplyBackgroundImage(form, layoutPath, definition.BackgroundImage);
            }

            if (!string.IsNullOrEmpty(definition.Text))
            {
                form.Text = definition.Text;
            }

            if (definition.Visible.HasValue)
            {
                form.Visible = definition.Visible.Value;
            }
        }

        private static void ApplyControlDefinition(Form form, string controlName, ControlDefinition definition, string layoutPath)
        {
            if (definition == null)
            {
                return;
            }

            Control control = FindControl(form, controlName);
            if (control == null && !string.IsNullOrEmpty(definition.Type))
            {
                control = CreateControl(form, controlName, definition);
            }

            if (control == null)
            {
                return;
            }

            if (definition.Location != null && definition.Location.Length >= 2)
            {
                control.Location = new Point(definition.Location[0], definition.Location[1]);
            }

            if (definition.Size != null && definition.Size.Length >= 2)
            {
                control.Size = new Size(definition.Size[0], definition.Size[1]);
            }

            if (!string.IsNullOrEmpty(definition.BackgroundImage))
            {
                TryApplyBackgroundImage(control, layoutPath, definition.BackgroundImage);
            }

            if (!string.IsNullOrEmpty(definition.Image))
            {
                TryApplyImage(control, layoutPath, definition.Image);
            }

            if (!string.IsNullOrEmpty(definition.Text))
            {
                control.Text = definition.Text;
            }

            if (definition.Visible.HasValue)
            {
                control.Visible = definition.Visible.Value;
            }

            if (definition.Enabled.HasValue)
            {
                control.Enabled = definition.Enabled.Value;
            }

            if (!string.IsNullOrEmpty(definition.SizeMode))
            {
                ApplySizeMode(control, definition.SizeMode);
            }
        }

        private static Control CreateControl(Form form, string controlName, ControlDefinition definition)
        {
            Control control = InstantiateControl(definition.Type);
            if (control == null)
            {
                return null;
            }

            control.Name = controlName;
            control.Text = !string.IsNullOrEmpty(definition.Text) ? definition.Text : control.Text;

            if (definition.Size != null && definition.Size.Length >= 2)
            {
                control.Size = new Size(definition.Size[0], definition.Size[1]);
            }

            if (definition.Location != null && definition.Location.Length >= 2)
            {
                control.Location = new Point(definition.Location[0], definition.Location[1]);
            }

            form.Controls.Add(control);
            control.BringToFront();
            return control;
        }

        private static Control InstantiateControl(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            switch (typeName.Trim().ToLowerInvariant())
            {
                case "label":
                    return new Label();
                case "button":
                    return new Button();
                case "picturebox":
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox.BorderStyle = BorderStyle.FixedSingle;
                    return pictureBox;
                case "webbrowser":
                    return new WebBrowser();
                case "textbox":
                    return new TextBox();
                case "panel":
                    Panel panel = new Panel();
                    panel.BorderStyle = BorderStyle.FixedSingle;
                    return panel;
                default:
                    Type type = Type.GetType(typeName, false, true);
                    if (type != null && typeof(Control).IsAssignableFrom(type))
                    {
                        return (Control)Activator.CreateInstance(type);
                    }
                    break;
            }

            return null;
        }

        private static void ApplySizeMode(Control control, string sizeModeValue)
        {
            PictureBox pictureBox = control as PictureBox;
            if (pictureBox == null)
            {
                return;
            }

            try
            {
                PictureBoxSizeMode mode = (PictureBoxSizeMode)Enum.Parse(typeof(PictureBoxSizeMode), sizeModeValue, true);
                pictureBox.SizeMode = mode;
            }
            catch
            {
            }
        }

        private static Control FindControl(Form form, string controlName)
        {
            Control[] controls = form.Controls.Find(controlName, true);
            return controls.Length > 0 ? controls[0] : null;
        }

        private static void TryApplyBackgroundImage(Control control, string layoutPath, string imagePath)
        {
            Image image = LoadImage(layoutPath, imagePath);
            if (image == null)
            {
                return;
            }

            Image previous = control.BackgroundImage;
            control.BackgroundImage = image;
            if (previous != null)
            {
                previous.Dispose();
            }
        }

        private static void TryApplyImage(Control control, string layoutPath, string imagePath)
        {
            PictureBox pictureBox = control as PictureBox;
            if (pictureBox == null)
            {
                return;
            }

            Image image = LoadImage(layoutPath, imagePath);
            if (image == null)
            {
                return;
            }

            Image previous = pictureBox.Image;
            pictureBox.Image = image;
            if (previous != null)
            {
                previous.Dispose();
            }
        }

        private static Image LoadImage(string layoutPath, string imagePath)
        {
            string resolvedPath = ResolvePath(layoutPath, imagePath);
            if (string.IsNullOrEmpty(resolvedPath) || !File.Exists(resolvedPath))
            {
                return null;
            }

            try
            {
                using (FileStream stream = new FileStream(resolvedPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        CopyStream(stream, memory);
                        memory.Position = 0;
                        using (Image image = Image.FromStream(memory))
                        {
                            return new Bitmap(image);
                        }
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        private static string ResolvePath(string layoutPath, string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return null;
            }

            if (Path.IsPathRooted(imagePath))
            {
                return imagePath;
            }

            string directoryName = Path.GetDirectoryName(layoutPath);
            if (string.IsNullOrEmpty(directoryName))
            {
                return imagePath;
            }

            return Path.GetFullPath(Path.Combine(directoryName, imagePath));
        }

        private static int? ReadIntAttribute(XmlElement element, string attributeName)
        {
            string attribute = element.GetAttribute(attributeName);
            if (string.IsNullOrEmpty(attribute))
            {
                return null;
            }

            double result;
            if (!double.TryParse(attribute, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                return null;
            }

            return (int)Math.Round(result);
        }

        private static bool ParseBoolean(string value, bool defaultValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "1", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (string.Equals(value, "false", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "0", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return defaultValue;
        }

        private static void CopyStream(Stream source, Stream destination)
        {
            byte[] buffer = new byte[4096];
            int count;
            while ((count = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, count);
            }
        }
    }
}
