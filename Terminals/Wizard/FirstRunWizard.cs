using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Kohl.Framework.Info;
using Kohl.Framework.Logging;
using Terminals.Configuration;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Forms.Controls;

namespace Terminals.Wizard
{
    public partial class FirstRunWizard : Form
    {
        private readonly CommonOptions co = new CommonOptions();
        private readonly DefaultCredentials dc = new DefaultCredentials();
        private readonly MethodInvoker miv;
        private readonly MasterPassword mp = new MasterPassword();
        private readonly AddExistingRDPConnections rdp = new AddExistingRDPConnections();
        private WizardForms SelectedForm = WizardForms.Intro;

        public FirstRunWizard()
        {
            this.InitializeComponent();
            this.rdp.OnDiscoveryCompleted += this.rdp_OnDiscoveryCompleted;
            this.miv = this.DiscoComplete;
        }

        private void FirstRunWizard_Load(object sender, EventArgs e)
        {
            IntroForm frm = new IntroForm {Dock = DockStyle.Fill};
            this.panel1.Controls.Add(frm);
            Settings.StartDelayedUpdate();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (this.SelectedForm == WizardForms.Intro)
            {
                this.SwitchToMasterPassword();
            }
            else if (this.SelectedForm == WizardForms.MasterPassword)
            {
                if (this.mp.StorePassword)
                {
                    this.SwitchToDefaultCredentials();
                }
                else
                {
                    this.SwitchToOptions();
                }
            }
            else if (this.SelectedForm == WizardForms.DefaultCredentials)
            {
                this.SwitchToOptionsFromCredentials();
            }
            else if (this.SelectedForm == WizardForms.Options)
            {
                this.FinishOptions();
            }
            else if (this.SelectedForm == WizardForms.Scanner)
            {
                this.Hide();
            }
        }

        private void FinishOptions()
        {
            try
            {
                this.ApplySettings();
                this.StartImportIfRequested();
            }
            catch (Exception exc)
            {
                Log.Error("Apply settings in the first run wizard failed.", exc);
            }
        }

        private void StartImportIfRequested()
        {
            if (this.co.ImportRDPConnections)
            {
                this.nextButton.Enabled = false;
                this.nextButton.Text = "Finished!";
                this.panel1.Controls.Clear();
                this.rdp.Dock = DockStyle.Fill;
                this.panel1.Controls.Add(this.rdp);
                this.rdp.StartImport();
                this.SelectedForm = WizardForms.Scanner;
            }
            else
            {
                this.rdp.CancelDiscovery();
                this.Hide();
            }
        }

        private void ApplySettings()
        {
            Settings.MinimizeToTray = this.co.MinimizeToTray;
            Settings.SingleInstance = this.co.AllowOnlySingleInstance;
            Settings.ShowConfirmDialog = this.co.WarnOnDisconnect;
            Settings.EnableCaptureToClipboard = this.co.EnableCaptureToClipboard;
            Settings.EnableCaptureToFolder = this.co.EnableCaptureToFolder;
            Settings.AutoSwitchOnCapture = this.co.AutoSwitchOnCapture;

            if (this.co.LoadDefaultShortcuts)
                Settings.SpecialCommands = SpecialCommandsWizard.LoadSpecialCommands();
        }

        private void SwitchToOptionsFromCredentials()
        {
            Settings.DefaultDomain = this.dc.DefaultDomain;
            Settings.DefaultPassword = this.dc.DefaultPassword;
            Settings.DefaultUsername = this.dc.DefaultUsername;

            this.nextButton.Enabled = true;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(this.co);
            this.SelectedForm = WizardForms.Options;
        }

        private void SwitchToOptions()
        {
            this.nextButton.Enabled = true;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(this.co);
            this.SelectedForm = WizardForms.Options;
        }

        private void SwitchToDefaultCredentials()
        {
            Settings.UpdateMasterPassword(this.mp.Password);
            this.nextButton.Enabled = true;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(this.dc);
            this.SelectedForm = WizardForms.DefaultCredentials;
        }

        private void SwitchToMasterPassword()
        {
            this.nextButton.Enabled = true;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(this.mp);
            this.SelectedForm = WizardForms.MasterPassword;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.rdp.CancelDiscovery();
            this.Hide();
        }

        private void DiscoComplete()
        {
            this.nextButton.Enabled = true;
            this.cancelButton.Enabled = false;
            this.Hide();
        }

        private void rdp_OnDiscoveryCompleted()
        {
            this.Invoke(this.miv);
        }

        private void FirstRunWizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.ShowWizard = false;
            Settings.SaveAndFinishDelayedUpdate();
            this.ImportDiscoveredFavorites();
        }

        private void ImportDiscoveredFavorites()
        {
            if (this.rdp.DiscoveredConnections.Count > 0)
            {
                String message = String.Format("Automatic Discovery was able to find {0} connections.\r\n" +
                                               "Would you like to add them to your connections list?",
                                               this.rdp.DiscoveredConnections.Count);
                if (MessageBox.Show(message, AssemblyInfo.Title() + " Confirmation", MessageBoxButtons.YesNo) ==
                    DialogResult.Yes)
                {
                    List<FavoriteConfigurationElement> favoritesToImport = this.rdp.DiscoveredConnections.ToList();
                    ImportWithDialogs managedImport = new ImportWithDialogs(this);
                    managedImport.Import(favoritesToImport);
                }
            }
        }
    }
}