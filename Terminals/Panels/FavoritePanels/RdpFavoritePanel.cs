using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Connection.Manager;
using Terminals.Connection.Panels.FavoritePanels;
using Terminals.Connections;
using Terminals.Forms;
using Terminals.Network.Servers;
using System.Drawing;

namespace Terminals.Panels.FavoritePanels
{
    public partial class RdpFavoritePanel : FavoritePanel
    {
        public override string[] ProtocolName
        {
            get
            {
                return new[] { typeof(RDPConnection).GetProtocolName() };
            }
        }

        public List<string> RedirectedDrives { get; set; }
        public bool RedirectDevices { get; set; }

        private TerminalServerManager terminalServerManager = null;

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            this.pnlTSGWsettings.Enabled = this.radTSGWenable.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.pnlTSGWlogon.Enabled = this.chkTSGWlogin.Checked;
        }

        private void btnDrives_Click(object sender, EventArgs e)
        {
            DiskDrivesForm drivesForm = new DiskDrivesForm(((FavoriteEditor)this.ParentForm), this.RedirectedDrives, this.RedirectDevices);
            drivesForm.ShowDialog(this);
            this.RedirectedDrives = drivesForm.RedirectedDrives;
            this.RedirectDevices = drivesForm.RedirectDevices;
        }

        private void btnBrowseShare_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select Desktop Share:";
                dialog.ShowNewFolderButton = false;
                dialog.SelectedPath = @"\\" + ((FavoriteEditor)this.ParentForm).Server;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    this.txtDesktopShare.Text = dialog.SelectedPath;
            }
        }

        private void SecuritySettingsEnabledCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            this.panel2.Enabled = this.SecuritySettingsEnabledCheckbox.Checked;
        }

        public RdpFavoritePanel()
        {
            InitializeComponent();

            chkTSGWlogin.Checked = this.pnlTSGWlogon.Enabled = false;
            this.pnlTSGWlogon.FillCredentials(null);
            
            // move following line down to default value only once smart card access worked out.
            this.cmbTSGWLogonMethod.SelectedIndex = 0;

            this.cmbResolution.SelectedIndex = 0;
            this.cmbColors.SelectedIndex = 1;
            this.cmbSounds.SelectedIndex = 2;

            this.LocalResourceGroupBox.Enabled = false;
            this.groupBox1.Enabled = false;
            this.chkConnectToConsole.Enabled = false;

            this.RedirectedDrives = new List<String>();
            this.RedirectDevices = false;

            this.groupBox1.Enabled = true;
            this.chkConnectToConsole.Enabled = true;
            this.LocalResourceGroupBox.Enabled = true;
        }
        
        private void RDPSubTabPage_SelectedIndexChanged(object sender, EventArgs e)
        {
			if (this.RDPSubTabPage.SelectedTab == this.tabPage10)
			{
				if (terminalServerManager != null)
				{
		            this.terminalServerManager.Connect(((FavoriteEditor)this.ParentForm).Server, true, ((FavoriteEditor)this.ParentForm).Credentials);
					this.terminalServerManager.Invalidate();
				}
			}
        }

        public override void FillControls(FavoriteConfigurationElement favorite)
        {
            this.RedirectedDrives = favorite.RedirectedDrives;
            this.chkSerialPorts.Checked = favorite.RedirectPorts;
            this.chkPrinters.Checked = favorite.RedirectPrinters;
            this.chkRedirectClipboard.Checked = favorite.RedirectClipboard;
            this.RedirectDevices = favorite.RedirectDevices;
            this.chkRedirectSmartcards.Checked = favorite.RedirectSmartCards;
            this.cmbSounds.SelectedIndex = (Int32)favorite.Sounds;
            
            this.txtDesktopShare.Text = favorite.DesktopShare;

            this.widthUpDown.Value = favorite.DesktopSizeWidth;
            this.heightUpDown.Value = favorite.DesktopSizeHeight;

            switch (favorite.TsgwUsageMethod)
            {
                case 0:
                    this.radTSGWdisable.Checked = true;
                    this.chkTSGWlocalBypass.Checked = false;
                    break;
                case 1:
                    this.radTSGWenable.Checked = true;
                    this.chkTSGWlocalBypass.Checked = false;
                    break;
                case 2:
                    this.radTSGWenable.Checked = true;
                    this.chkTSGWlocalBypass.Checked = true;
                    break;
                case 4:
                    this.radTSGWdisable.Checked = true;
                    this.chkTSGWlocalBypass.Checked = true;
                    break;
            }

            this.txtTSGWServer.Text = favorite.TsgwHostname;

			// A seperate login hasn't been specified - so clear the TSGW credentials
			if (!favorite.TsgwSeparateLogin)
				favorite.TsgwDomain = favorite.TsgwUsername = favorite.TsgwPassword = favorite.TsgwXmlCredentialSetName = string.Empty;
			
			// Use the tswg credentials
			else
			{
				if (favorite.TsgwXmlCredentialSetName == Terminals.Forms.Controls.CredentialPanel.Custom)
	            {
	            	this.pnlTSGWlogon.FillControls(new FavoriteConfigurationElement() { DomainName = favorite.TsgwDomain, UserName = favorite.TsgwUsername, Password = favorite.TsgwPassword  });
				}
	            this.pnlTSGWlogon.FillCredentials(favorite.TsgwXmlCredentialSetName);

	            // if the credential store isn't either set/filled or set to the 
				// storename "custom" than check if the values contain garbage
	            //  -> if so clear the store and set seperate login to false.
	            if (string.IsNullOrEmpty(favorite.TsgwXmlCredentialSetName) || favorite.TsgwXmlCredentialSetName == Terminals.Forms.Controls.CredentialPanel.Custom)
		            // if there are no credentials in the store and no custom ones -> clear the the TSGW credentials and disable the checkbox
		            if (string.IsNullOrEmpty(favorite.TsgwDomain) && string.IsNullOrEmpty(favorite.TsgwUsername) && string.IsNullOrEmpty(favorite.TsgwPassword))
		            {
		            	favorite.TsgwXmlCredentialSetName = string.Empty;
		            	favorite.TsgwSeparateLogin = false;
		            }
			}

            if (terminalServerManager == null)
            {
                this.terminalServerManager = new TerminalServerManager()
                {
                    SaveInDB = ((FavoriteEditor)this.ParentForm).SaveInDB,
                    Dock = DockStyle.Fill,
                    Location = new Point(0, 0),
                    Name = "terminalServerManager1",
                    Size = new Size(748, 309),
                    TabIndex = 0
                };

                this.tabPage10.Controls.Add(this.terminalServerManager);
            }

            this.chkTSGWlogin.Checked = favorite.TsgwSeparateLogin;
            this.cmbTSGWLogonMethod.SelectedIndex = favorite.TsgwCredsSource;

            this.cmbResolution.SelectedIndex = (Int32)favorite.DesktopSize;
            this.cmbColors.SelectedIndex = (Int32)favorite.Colors;
            this.chkConnectToConsole.Checked = favorite.ConnectToConsole;

            //extended settings
            this.ShutdownTimeoutTextBox.Text = favorite.ShutdownTimeout.ToString();
            this.OverallTimeoutTextbox.Text = favorite.OverallTimeout.ToString();
            this.SingleTimeOutTextbox.Text = favorite.ConnectionTimeout.ToString();
            this.IdleTimeoutMinutesTextBox.Text = favorite.IdleTimeout.ToString();
            this.SecurityWorkingFolderTextBox.Text = favorite.SecurityWorkingFolder;
            this.SecuriytStartProgramTextbox.Text = favorite.SecurityStartProgram;
            this.SecurityStartFullScreenCheckbox.Checked = favorite.SecurityFullScreen;
            this.SecuritySettingsEnabledCheckbox.Checked = favorite.EnableSecuritySettings;
            this.GrabFocusOnConnectCheckbox.Checked = favorite.GrabFocusOnConnect;
            this.EnableEncryptionCheckbox.Checked = favorite.EnableEncryption;
            this.DisableWindowsKeyCheckbox.Checked = favorite.DisableWindowsKey;
            this.DetectDoubleClicksCheckbox.Checked = favorite.DoubleClickDetect;
            this.DisplayConnectionBarCheckbox.Checked = favorite.DisplayConnectionBar;
            this.DisableControlAltDeleteCheckbox.Checked = favorite.DisableControlAltDelete;
            this.AcceleratorPassthroughCheckBox.Checked = favorite.AcceleratorPassthrough;
            this.EnableCompressionCheckbox.Checked = favorite.EnableCompression;
            this.EnableBitmapPersistanceCheckbox.Checked = favorite.BitmapPeristence;
            this.EnableTLSAuthenticationCheckbox.Checked = favorite.EnableTlsAuthentication;
            this.EnableNLAAuthenticationCheckbox.Checked = favorite.EnableNlaAuthentication;
            this.AllowBackgroundInputCheckBox.Checked = favorite.AllowBackgroundInput;

            this.chkDisableCursorShadow.Checked = false;
            this.chkDisableCursorBlinking.Checked = false;
            this.chkDisableFullWindowDrag.Checked = false;
            this.chkDisableMenuAnimations.Checked = false;
            this.chkDisableThemes.Checked = false;
            this.chkDisableWallpaper.Checked = false;

            if (favorite.PerformanceFlags > 0)
            {
                this.chkDisableCursorShadow.Checked = favorite.DisableCursorShadow;
                this.chkDisableCursorBlinking.Checked = favorite.DisableCursorBlinking;
                this.chkDisableFullWindowDrag.Checked = favorite.DisableFullWindowDrag;
                this.chkDisableMenuAnimations.Checked = favorite.DisableMenuAnimations;
                this.chkDisableThemes.Checked = favorite.DisableTheming;
                this.chkDisableWallpaper.Checked = favorite.DisableWallPaper;
                this.AllowFontSmoothingCheckbox.Checked = favorite.EnableFontSmoothing;
                this.AllowDesktopCompositionCheckbox.Checked = favorite.EnableDesktopComposition;
            }
        }

        public override void FillFavorite(FavoriteConfigurationElement favorite)
        {
            favorite.DesktopShare = this.txtDesktopShare.Text;

            favorite.DesktopSize = (DesktopSize)this.cmbResolution.SelectedIndex;
            favorite.Colors = (Colors)this.cmbColors.SelectedIndex;
            favorite.ConnectToConsole = this.chkConnectToConsole.Checked;
            favorite.DisableWallPaper = this.chkDisableWallpaper.Checked;
            favorite.DisableCursorBlinking = this.chkDisableCursorBlinking.Checked;
            favorite.DisableCursorShadow = this.chkDisableCursorShadow.Checked;
            favorite.DisableFullWindowDrag = this.chkDisableFullWindowDrag.Checked;
            favorite.DisableMenuAnimations = this.chkDisableMenuAnimations.Checked;
            favorite.DisableTheming = this.chkDisableThemes.Checked;

            favorite.RedirectedDrives = this.RedirectedDrives;
            favorite.RedirectPorts = this.chkSerialPorts.Checked;
            favorite.RedirectPrinters = this.chkPrinters.Checked;
            favorite.RedirectClipboard = this.chkRedirectClipboard.Checked;
            favorite.RedirectDevices = this.RedirectDevices;
            favorite.RedirectSmartCards = this.chkRedirectSmartcards.Checked;
            favorite.Sounds = (RemoteSounds)this.cmbSounds.SelectedIndex;

            favorite.TsgwHostname = this.txtTSGWServer.Text;
            
            FavoriteConfigurationElement tswgFavorite = new FavoriteConfigurationElement();
            
            // TSGW            
            this.pnlTSGWlogon.FillFavorite(tswgFavorite);
            favorite.TsgwXmlCredentialSetName = this.pnlTSGWlogon.SelectedCredentialSet == null || string.IsNullOrWhiteSpace(this.pnlTSGWlogon.SelectedCredentialSet.Name) ? Terminals.Forms.Controls.CredentialPanel.Custom : this.pnlTSGWlogon.SelectedCredentialSet.Name;

			favorite.TsgwSeparateLogin = this.chkTSGWlogin.Checked;
            
			if (!favorite.TsgwSeparateLogin)
			{
				favorite.TsgwDomain = favorite.TsgwUsername = favorite.TsgwPassword = string.Empty;
			}
			else
			{
				favorite.TsgwDomain = tswgFavorite.Credential.DomainName;
				favorite.TsgwUsername = tswgFavorite.Credential.UserName;
				favorite.TsgwPassword = tswgFavorite.Credential.Password;

				if (favorite.TsgwXmlCredentialSetName == Terminals.Forms.Controls.CredentialPanel.Custom)
				{
					if (string.IsNullOrEmpty (favorite.TsgwDomain) && string.IsNullOrEmpty (favorite.TsgwUsername) && string.IsNullOrEmpty (favorite.TsgwPassword))
					{
						favorite.TsgwSeparateLogin = false;
						favorite.TsgwXmlCredentialSetName = string.Empty;
					}
				}
			}
            
            favorite.TsgwCredsSource = this.cmbTSGWLogonMethod.SelectedIndex;

            //extended settings
            if (this.ShutdownTimeoutTextBox.Text.Trim() != String.Empty)
            {
                favorite.ShutdownTimeout = Convert.ToInt32(this.ShutdownTimeoutTextBox.Text.Trim());
            }

            if (this.OverallTimeoutTextbox.Text.Trim() != String.Empty)
            {
                favorite.OverallTimeout = Convert.ToInt32(this.OverallTimeoutTextbox.Text.Trim());
            }

            if (this.SingleTimeOutTextbox.Text.Trim() != String.Empty)
            {
                favorite.ConnectionTimeout = Convert.ToInt32(this.SingleTimeOutTextbox.Text.Trim());
            }

            if (this.IdleTimeoutMinutesTextBox.Text.Trim() != String.Empty)
            {
                favorite.IdleTimeout = Convert.ToInt32(this.IdleTimeoutMinutesTextBox.Text.Trim());
            }

            favorite.EnableSecuritySettings = this.SecuritySettingsEnabledCheckbox.Checked;

            if (this.SecuritySettingsEnabledCheckbox.Checked)
            {
                favorite.SecurityWorkingFolder = this.SecurityWorkingFolderTextBox.Text;
                favorite.SecurityStartProgram = this.SecuriytStartProgramTextbox.Text;
                favorite.SecurityFullScreen = this.SecurityStartFullScreenCheckbox.Checked;
            }

            favorite.GrabFocusOnConnect = this.GrabFocusOnConnectCheckbox.Checked;
            favorite.EnableEncryption = this.EnableEncryptionCheckbox.Checked;
            favorite.DisableWindowsKey = this.DisableWindowsKeyCheckbox.Checked;
            favorite.DoubleClickDetect = this.DetectDoubleClicksCheckbox.Checked;
            favorite.DisplayConnectionBar = this.DisplayConnectionBarCheckbox.Checked;
            favorite.DisableControlAltDelete = this.DisableControlAltDeleteCheckbox.Checked;
            favorite.AcceleratorPassthrough = this.AcceleratorPassthroughCheckBox.Checked;
            favorite.EnableCompression = this.EnableCompressionCheckbox.Checked;
            favorite.BitmapPeristence = this.EnableBitmapPersistanceCheckbox.Checked;
            favorite.EnableTlsAuthentication = this.EnableTLSAuthenticationCheckbox.Checked;
            favorite.EnableNlaAuthentication = this.EnableNLAAuthenticationCheckbox.Checked;
            favorite.AllowBackgroundInput = this.AllowBackgroundInputCheckBox.Checked;

            favorite.EnableFontSmoothing = this.AllowFontSmoothingCheckbox.Checked;
            favorite.EnableDesktopComposition = this.AllowDesktopCompositionCheckbox.Checked;

            if (this.radTSGWenable.Checked)
            {
                favorite.TsgwUsageMethod = this.chkTSGWlocalBypass.Checked ? 2 : 1;
            }
            else
            {
                favorite.TsgwUsageMethod = this.chkTSGWlocalBypass.Checked ? 4 : 0;
            }

            favorite.DesktopSizeWidth = (Int32)this.widthUpDown.Value;
            favorite.DesktopSizeHeight = (Int32)this.heightUpDown.Value;
        }

        private void cmbResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbResolution.Text == "Custom")
                this.customSizePanel.Visible = true;
            else
                this.customSizePanel.Visible = false;
        }
    }
}
