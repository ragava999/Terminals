using System.Windows.Forms;
using System;

using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Panels.OptionPanels;
using System.IO;

namespace Terminals.Panels.OptionPanels
{
    public partial class StartShutdownOptionPanel : IOptionPanel
    {
        public StartShutdownOptionPanel()
        {
            this.InitializeComponent();
            
        }

        public override void LoadSettings()
        {
            this.chkSingleInstance.Checked = Settings.SingleInstance;
            this.chkNeverShowTerminalsCheckbox.Checked = Settings.NeverShowTerminalsWindow;
            this.chkShowConfirmDialog.Checked = Settings.ShowConfirmDialog;
            this.chkSaveConnections.Checked = Settings.SaveConnectionsOnClose;
            this.txtImage.Text = Settings.ImagePath;
            this.cmbStyle.SelectedIndex = Settings.ImageStyle;
            this.picColor.BackColor = Kohl.Framework.Converters.ColorParser.FromString(Settings.DashBoardBackgroundColor, System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control));

            string file = Settings.ImagePath.NormalizePath(Kohl.Framework.Info.AssemblyInfo.DirectoryConfigFiles);

            if (!string.IsNullOrEmpty(file) && File.Exists(file))
                picImage.BackgroundImage = System.Drawing.Bitmap.FromFile(file);
        }

        public override void SaveSettings()
        {
            Settings.SingleInstance = this.chkSingleInstance.Checked;
            Settings.NeverShowTerminalsWindow = this.chkNeverShowTerminalsCheckbox.Checked;
            Settings.ShowConfirmDialog = this.chkShowConfirmDialog.Checked;
            Settings.SaveConnectionsOnClose = this.chkSaveConnections.Checked;
            Settings.ImagePath = this.txtImage.Text;
            Settings.DashBoardBackgroundColor = Kohl.Framework.Converters.ColorParser.ToString(picColor.BackColor);
            Settings.ImageStyle = this.cmbStyle.SelectedIndex;
        }

        public new IHostingForm IHostingForm { get; set; }

        private void chkSaveConnections_CheckedChanged(object sender, System.EventArgs e)
        {
            chkShowConfirmDialog.Enabled = chkShowConfirmDialog.Checked = !chkSaveConnections.Checked;
        }

        private void btnBrowse_Click(object sender, System.EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Select the background icon";

                if (string.IsNullOrEmpty(this.txtImage.Text) || !File.Exists(this.txtImage.Text))
                    dlg.InitialDirectory = Kohl.Framework.Info.AssemblyInfo.DirectoryConfigFiles;
                else
                    dlg.InitialDirectory = Path.GetDirectoryName(this.txtImage.Text);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string destFile = Path.Combine(Kohl.Framework.Info.AssemblyInfo.DirectoryConfigFiles, Path.GetFileName(dlg.FileName));

                    if (!File.Exists(destFile))
                    {
                        File.Copy(dlg.FileName, destFile);
                    }

                    picImage.BackgroundImage = System.Drawing.Bitmap.FromFile(destFile);
                    picImage.BackgroundImageLayout = (ImageLayout)this.cmbStyle.SelectedIndex;

                    destFile = destFile.GetRelativePath(Kohl.Framework.Info.AssemblyInfo.DirectoryConfigFiles);

                    this.txtImage.Text = destFile + Path.DirectorySeparatorChar + Path.GetFileName(dlg.FileName);
                    this.txtImage.SelectionStart = this.txtImage.Text.Length;
                    this.txtImage.Focus();
                }
            }
        }

        private void picDelete_Click(object sender, System.EventArgs e)
        {
            txtImage.Text = null;
            picImage.BackColor = picColor.BackColor = System.Drawing.SystemColors.Control;
            cmbStyle.SelectedIndex = 0;
            picImage.BackgroundImage = Terminals.Properties.Resources.Question;
        }

        private void picColor_Click(object sender, System.EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.Color = picColor.BackColor;
            
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                picColor.BackColor = dialog.Color;
                picImage.BackColor = dialog.Color;
            }
        }
    }
}