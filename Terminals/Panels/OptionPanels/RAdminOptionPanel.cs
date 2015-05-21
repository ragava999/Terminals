using Terminals.Connection.Panels.OptionPanels;
using System;

using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Panels.OptionPanels
{
    public partial class RAdminOptionPanel : OptionPanel
    {
        public RAdminOptionPanel()
        {
            this.InitializeComponent();
            
        }

        public override void LoadSettings()
        {
            this.txtRAdminDefaultPort.Text = Settings.RAdminDefaultPort.ToString();
            this.txtRAdminPath.Text = Settings.RAdminProgramPath;
        }

        public override void SaveSettings()
        {
            Settings.RAdminDefaultPort = Convert.ToUInt16(this.txtRAdminDefaultPort.Text);
            Settings.RAdminProgramPath = this.txtRAdminPath.Text;
        }
    }
}