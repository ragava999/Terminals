using Terminals.Connection.Panels.OptionPanels;

using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Panels.OptionPanels
{
    public partial class PuttyOptionPanel : OptionPanel
    {
        public PuttyOptionPanel()
        {
            this.InitializeComponent();
            
        }

        public override void LoadSettings()
        {
            this.txtPuttyPath.Text = Settings.PuttyProgramPath;
        }

        public override void SaveSettings()
        {
            Settings.PuttyProgramPath = this.txtPuttyPath.Text;
        }
    }
}