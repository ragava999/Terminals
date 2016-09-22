using Microsoft.Win32;
using System;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Connection.Manager;
using Terminals.Connection.Panels.FavoritePanels;
using Terminals.Connections;

namespace Terminals.Panels.FavoritePanels
{
    public partial class PuttyFavoritePanel : FavoritePanel
    {
        public override string[] ProtocolName
        {
            get
            {
                return new[] { typeof(PuttyConnection).GetProtocolName() };
            }
        }

        public PuttyFavoritePanel()
        {
            this.InitializeComponent();

            LoadPuttyStrings();
        }

        public override void FillControls(FavoriteConfigurationElement favorite)
        {
            this.cmbPuttyProtocol.Text = Enum.GetName(typeof(PuttyConnectionType), favorite.PuttyConnectionType);

            if (!string.IsNullOrEmpty(favorite.PuttySession))
            {
                if (this.cmbPuttySession.Items.Contains(favorite.PuttySession))
                {
                    this.cmbPuttySession.SelectedText = favorite.PuttySession;
                }
            }

            this.cmbPuttyCloseWindowOnExit.Text = Enum.GetName(typeof(PuttyCloseWindowOnExit),
                                                               favorite.PuttyCloseWindowOnExit);

            this.chkPuttyCompress.Checked = favorite.PuttyCompression;
            this.chkPuttyVerbose.Checked = favorite.PuttyVerbose;
            this.chkPuttyShowOptions.Checked = favorite.PuttyShowOptions;

            this.nudPuttyPasswordTimeout.Value = favorite.PuttyPasswordTimeout;

            this.cmbProxyType.Text = Enum.GetName(typeof(PuttyProxyType), favorite.PuttyProxyType);
            this.txtProxyHost.Text = favorite.PuttyProxyHost;
            this.nudProxyPort.Value = favorite.PuttyProxyPort;

            this.chkPuttyEnableX11.Checked = favorite.PuttyEnableX11;
            this.chkPuttyDontAddDomainToUserName.Checked = favorite.PuttyDontAddDomainToUserName;
        }

        public override void FillFavorite(FavoriteConfigurationElement favorite)
        {
            favorite.PuttyConnectionType =
                (PuttyConnectionType)Enum.Parse(typeof(PuttyConnectionType), this.cmbPuttyProtocol.Text, true);

            favorite.PuttySession = this.cmbPuttySession.Text;
            favorite.PuttyCloseWindowOnExit =
                (PuttyCloseWindowOnExit)
                Enum.Parse(typeof(PuttyCloseWindowOnExit), this.cmbPuttyCloseWindowOnExit.Text, true);

            favorite.PuttyCompression = this.chkPuttyCompress.Checked;
            favorite.PuttyVerbose = this.chkPuttyVerbose.Checked;
            favorite.PuttyShowOptions = this.chkPuttyShowOptions.Checked;

            favorite.PuttyPasswordTimeout = (int)this.nudPuttyPasswordTimeout.Value;

            favorite.PuttyProxyType =
                (PuttyProxyType)
                Enum.Parse(typeof(PuttyProxyType), this.cmbProxyType.Text, true);

            favorite.PuttyProxyHost = txtProxyHost.Text;
            favorite.PuttyProxyPort = (int)nudProxyPort.Value;

            favorite.PuttyEnableX11 = this.chkPuttyEnableX11.Checked;
            favorite.PuttyDontAddDomainToUserName = this.chkPuttyDontAddDomainToUserName.Checked;
        }

        private static bool DeletePuttySession(string name)
        {
            try
            {
                Registry.CurrentUser.OpenSubKey("Software\\SimonTatham\\PuTTY\\Sessions\\").DeleteSubKey(name, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void PuttySessionDelete_Click(object sender, EventArgs e)
        {
            if (this.cmbPuttySession.SelectedIndex > -1)
            {
                DeletePuttySession(this.cmbPuttySession.SelectedText);
            }
        }

        private static string[] GetPuttySessions()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\SimonTatham\\PuTTY\\Sessions");

                if (key != null)
                {
                    string[] keys = key.GetSubKeyNames();
                    if (keys.Length > 0)
                    {
                        return keys;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public void LoadPuttyStrings()
        {
            this.cmbPuttyProtocol.Items.AddRange(Enum.GetNames(typeof(PuttyConnectionType)));
            this.cmbPuttyProtocol.SelectedIndex = 0;

            this.cmbProxyType.Items.AddRange(Enum.GetNames(typeof(PuttyProxyType)));
            this.cmbProxyType.SelectedIndex = 0;

            this.cmbPuttyCloseWindowOnExit.Items.AddRange(Enum.GetNames(typeof(PuttyCloseWindowOnExit)));
            this.cmbPuttyCloseWindowOnExit.SelectedIndex = 0;

            string[] keys = GetPuttySessions();

            if (keys != null)
            {
                this.cmbPuttySession.Items.AddRange(keys);
            }

            this.btnPuttySessionExport.Visible = false;
            this.btnPuttySessionImport.Visible = false;
        }
    }
}