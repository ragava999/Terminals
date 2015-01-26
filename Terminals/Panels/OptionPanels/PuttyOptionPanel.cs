using Terminals.Connection.Panels.OptionPanels;
using Kohl.Framework.Localization;
using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Panels.OptionPanels
{
    public partial class PuttyOptionPanel : OptionPanel
    {
        public PuttyOptionPanel()
        {
            this.InitializeComponent();
            Localization.SetLanguage(this);
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