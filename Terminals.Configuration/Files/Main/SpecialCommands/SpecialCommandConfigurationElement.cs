using System;
using System.Configuration;
using System.Diagnostics;

namespace Terminals.Configuration.Files.Main.SpecialCommands
{
    public class SpecialCommandConfigurationElement : ConfigurationElement
    {
        public SpecialCommandConfigurationElement()
        {
        }

        public SpecialCommandConfigurationElement(string name)
        {
            this.Name = name;
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("executable", IsRequired = false)]
        public string Executable
        {
            get { return (string) this["executable"]; }
            set { this["executable"] = value; }
        }


        [ConfigurationProperty("arguments", IsRequired = false)]
        public string Arguments
        {
            get { return (string) this["arguments"]; }
            set { this["arguments"] = value; }
        }

        [ConfigurationProperty("workingFolder", IsRequired = false)]
        public string WorkingFolder
        {
            get { return (string) this["workingFolder"]; }
            set { this["workingFolder"] = value; }
        }

        [ConfigurationProperty("thumbnail", IsRequired = false)]
        public string Thumbnail
        {
            get { return (string) this["thumbnail"]; }
            set { this["thumbnail"] = value; }
        }

        public void Launch()
        {
            Process p = new Process();
            string exe = this.Executable;
            if (exe.Contains("%"))
            {
                exe = exe.Replace("%systemroot%", Environment.GetEnvironmentVariable("systemroot"));
            }
            p.StartInfo = new ProcessStartInfo(exe, this.Arguments);
            p.StartInfo.WorkingDirectory = this.WorkingFolder;

            try
            {
                p.Start();
            }
            catch (Exception ex)
            {
                Kohl.Framework.Logging.Log.Warn(ex.Message);
            }
        }
    }
}