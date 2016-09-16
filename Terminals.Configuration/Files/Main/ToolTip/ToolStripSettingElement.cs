using System;
using System.Configuration;

namespace Terminals.Configuration.Files.Main.ToolTip
{
    [Serializable]
    public class ToolStripSettingElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("visible", IsRequired = true)]
        public bool Visible
        {
            get { return Convert.ToBoolean(this["visible"]); }
            set { this["visible"] = value; }
        }

        [ConfigurationProperty("row", IsRequired = true)]
        public int Row
        {
            get { return Convert.ToInt32(this["row"]); }
            set { this["row"] = value; }
        }

        [ConfigurationProperty("dock", IsRequired = true)]
        public string Dock
        {
            get { return (string) this["dock"]; }
            set { this["dock"] = value; }
        }

        [ConfigurationProperty("left", IsRequired = true)]
        public int Left
        {
            get { return Convert.ToInt32(this["left"]); }
            set { this["left"] = value; }
        }

        [ConfigurationProperty("top", IsRequired = true)]
        public int Top
        {
            get { return Convert.ToInt32(this["top"]); }
            set { this["top"] = value; }
        }
    }
}