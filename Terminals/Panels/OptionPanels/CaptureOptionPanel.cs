using System;
using System.IO;
using System.Windows.Forms;

using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Panels.OptionPanels;

namespace Terminals.Panels.OptionPanels
{
    public partial class CaptureOptionPanel : IOptionPanel
    {
        public CaptureOptionPanel()
        {
            this.InitializeComponent();
        }

        public override void LoadSettings()
        {
            this.chkEnableCaptureToClipboard.Checked = Settings.EnableCaptureToClipboard;
            this.chkEnableCaptureToFolder.Checked = Settings.EnableCaptureToFolder;
            this.chkAutoSwitchToCaptureCheckbox.Enabled = Settings.AutoSwitchOnCapture;
            this.txtScreenCaptureFolder.Text = Settings.CaptureRoot;

            this.txtScreenCaptureFolder.SelectionStart = this.txtScreenCaptureFolder.Text.Length;
            this.UpdateCaptureToFolderControls();
        }

        public override void SaveSettings()
        {
            Settings.AutoSwitchOnCapture = this.chkAutoSwitchToCaptureCheckbox.Checked;
            Settings.EnableCaptureToClipboard = this.chkEnableCaptureToClipboard.Checked;
            Settings.EnableCaptureToFolder = this.chkEnableCaptureToFolder.Checked;
            Settings.CaptureRoot = this.txtScreenCaptureFolder.Text;
        }

        public new IHostingForm IHostingForm { get; set; }

        private void ButtonBrowseCaptureFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select the screen capture folder";
                dlg.RootFolder = Environment.SpecialFolder.MyComputer;

                string rootPath = this.txtScreenCaptureFolder.Text.NormalizePath();

                if (!rootPath.Equals(String.Empty))
                    rootPath = (rootPath.EndsWith("\\")) ? rootPath : rootPath + "\\";

                dlg.SelectedPath = (rootPath.Equals(String.Empty))
                                       ? Environment.GetFolderPath(dlg.RootFolder)
                                       : System.IO.Path.GetDirectoryName(rootPath);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    String selectedFld = dlg.SelectedPath;
                    if (!selectedFld.Equals(String.Empty))
                        selectedFld = (selectedFld.EndsWith("\\")) ? selectedFld : selectedFld + "\\";

                    this.txtScreenCaptureFolder.Text = selectedFld;
                    this.txtScreenCaptureFolder.SelectionStart = this.txtScreenCaptureFolder.Text.Length;
                    this.txtScreenCaptureFolder.Focus();
                }
            }
        }

        private void chkEnableCaptureToFolder_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateCaptureToFolderControls();
        }

        private void UpdateCaptureToFolderControls()
        {
            this.chkAutoSwitchToCaptureCheckbox.Enabled = this.chkEnableCaptureToFolder.Checked;
            this.txtScreenCaptureFolder.Enabled = this.chkEnableCaptureToFolder.Checked;
            this.ButtonBrowseCaptureFolder.Enabled = this.chkEnableCaptureToFolder.Checked;
        }
    }
}