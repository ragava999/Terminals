namespace Terminals.Panels.OptionPanels
{
    using Configuration.Files.Main.Settings;
    using Connection.Panels.OptionPanels;
    using System;
    using System.Windows.Forms;

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
        void ButtonBrowseCaptureFolderClick(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Select the RAdmin executable";
                dlg.Filter = "*.exe (RAdmin)|Radmin.exe";

                if (!string.IsNullOrEmpty(txtRAdminPath.Text) && System.IO.File.Exists(txtRAdminPath.Text))
                    dlg.FileName = txtRAdminPath.Text;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtRAdminPath.Text = dlg.FileName.NormalizePath();
                }
            }
        }
    }
}