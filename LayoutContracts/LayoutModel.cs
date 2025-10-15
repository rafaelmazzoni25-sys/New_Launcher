using System.Collections.Generic;

namespace Launcher.Layout
{
    public class LayoutDefinition
    {
        public FormDefinition Form { get; set; }
        public Dictionary<string, ControlDefinition> Controls { get; set; }
    }

    public class FormDefinition
    {
        public int[] ClientSize { get; set; }
        public string BackgroundImage { get; set; }
        public bool? Visible { get; set; }
        public string Text { get; set; }
    }

    public class ControlDefinition
    {
        public string Type { get; set; }
        public int[] Location { get; set; }
        public int[] Size { get; set; }
        public string BackgroundImage { get; set; }
        public string Image { get; set; }
        public string Text { get; set; }
        public bool? Visible { get; set; }
        public bool? Enabled { get; set; }
        public string SizeMode { get; set; }
    }
}
