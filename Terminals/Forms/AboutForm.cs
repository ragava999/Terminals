using System;
using System.Diagnostics;
using System.Windows.Forms;
using Kohl.Framework.Info;
using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            this.InitializeComponent();
        }

        private void lblTerminals_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(AssemblyInfo.Url);
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            this.lblVersion.Text = AssemblyInfo.AboutText;
            DateTime dt = AssemblyInfo.BuildDate;
            this.textBox1.Text =
                string.Format(
                    "{0}\r\nConfig File:\r\n{1}\r\n\r\n{2}\r\n\r\n{3}\r\n{4}\r\n{5}\r\n{6}\r\n{7}\r\n{8}\r\n{9}\r\n{10}\r\n\r\n{11}\r\n\r\n{12}",
                    this.textBox1.Text, Settings.ConfigurationFileLocation,
                    string.Format("This version of " + AssemblyInfo.Title() + " has been built for you on {0} at {1}",
                                  dt.ToLongDateString(), dt.ToLongTimeString()),
                    String.Format("Version:\t\t\t{0}", AssemblyInfo.Version),
                    String.Format("Current directory:\t\t{0}", Environment.CurrentDirectory),
                    String.Format("Machine name:\t\t{0}", Environment.MachineName),
                    String.Format("Your Operating system:\t{0}", Environment.OSVersion),
                    String.Format("Number of processors:\t{0}", Environment.ProcessorCount),
                    String.Format("User interactive:\t\t{0}", Environment.UserInteractive.ToString().ToLower()),
                    String.Format(".NET Framework version:\t{0}", Environment.Version),
                    String.Format("Working set:\t\t{0} MB", Environment.WorkingSet/1024/1024),
                    String.Format("Command line arguments:\t{0}",
                                  string.IsNullOrEmpty(Environment.CommandLine.Trim())
                                      ? "None"
                                      : Environment.CommandLine),
                    String.Format("Author:\t\t\t{0}", AssemblyInfo.Author)
                    );

            /*
            string ret = Kohl.Framework.MachineInfo.Build;
            string l = Kohl.Framework.MachineInfo.BuildGuid;
            string c = Kohl.Framework.MachineInfo.ClientOrServer;
            string c1 = Kohl.Framework.MachineInfo.CompanyName;
            string c2 = Kohl.Framework.MachineInfo.EditionID;
            string c3 = Kohl.Framework.MachineInfo.ExternalIp;
            string c31 = Kohl.Framework.MachineInfo.HardwareID;
            string c4 = Kohl.Framework.MachineInfo.HostName;
            string[] c5 = Kohl.Framework.MachineInfo.IPAddresses;
            DateTime c6 = Kohl.Framework.MachineInfo.InstallDate;
            bool c7 = Kohl.Framework.MachineInfo.InternetConnection;
            bool c8 = Kohl.Framework.MachineInfo.Is64BitOperatingSystem;
            bool c91 = Kohl.Framework.MachineInfo.Is64BitProcess;
            string c9 = Kohl.Framework.MachineInfo.MachineDomain;
            System.Collections.IDictionary c10 = Kohl.Framework.MachineInfo.MachineEnvironmentVariables;
            System.Collections.IDictionary c11 = Kohl.Framework.MachineInfo.ProcessEnvironmentVariables;
            string l1 = Kohl.Framework.MachineInfo.ProductId;
            string l11 = Kohl.Framework.MachineInfo.ProcessorId;
            string c12 = Kohl.Framework.MachineInfo.ProductName;
            string c13 = Kohl.Framework.MachineInfo.RegisteredOwner;
            string c14 = Kohl.Framework.MachineInfo.StartupPath;
            System.Collections.IDictionary c24 = Kohl.Framework.MachineInfo.UserEnvironmentVariables;
            string c35 = Kohl.Framework.MachineInfo.UserName;
            string c46 = Kohl.Framework.MachineInfo.UserNameAlias;
            string c47 = Kohl.Framework.MachineInfo.UserSID;*/
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}