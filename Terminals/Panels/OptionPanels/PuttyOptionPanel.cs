namespace Terminals.Panels.OptionPanels
{
    using Configuration.Files.Main.Settings;
    using Connection.Panels.OptionPanels;
    using System;
    using System.Windows.Forms;

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
        void ButtonBrowseCaptureFolderClick(object sender, System.EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Select the Putty executable";
                dlg.Filter = "*.exe (puTTY)|putty.exe";

                if (!string.IsNullOrEmpty(txtPuttyPath.Text) && System.IO.File.Exists(txtPuttyPath.Text))
                    dlg.FileName = txtPuttyPath.Text;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtPuttyPath.Text = dlg.FileName.NormalizePath();
                }
            }
        }
    }
}