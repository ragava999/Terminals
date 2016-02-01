using System;
using System.ComponentModel;
using System.Windows.Forms;

using Kohl.Framework.Logging;
using Kohl.Framework.WinForms;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection;
using Terminals.Connection.Manager;
using Terminals.Connections;
using Terminals.TerminalServices;

namespace Terminals.Network.Servers
{
    public partial class TerminalServerManager : UserControl
    {
        private Session SelectedSession;
        private TerminalServer server;
        
        public TerminalServerManager()
        {
            this.InitializeComponent();
            progress.Visible = false;
        }

        public void ForceTSAdmin(string Host)
        {
            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            this.ServerNameComboBox.Text = Host;
            this.button1_Click(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connect(credentials);
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            if (this.server.IsATerminalServer)
            {
                if (this.dataGridView1.DataSource != null)
                {
                    this.SelectedSession = this.server.Sessions[e.RowIndex];
                    this.propertyGrid1.SelectedObject = this.SelectedSession.Client;
                    this.dataGridView2.DataSource = this.SelectedSession.Processes;
                }
            }
        }

        public void Connect(Kohl.Framework.Security.Credential credentials)
        {
        	progress.Visible = true;
        	splitContainer1.Visible = false;
        	
        	this.SelectedSession = null;
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;
            this.propertyGrid1.SelectedObject = null;
            
            this.server = TerminalServer.LoadServer(this.ServerNameComboBox.Text, credentials);

            progress.Visible = false;
            splitContainer1.Visible = true;
            
            if (this.server.IsATerminalServer)
            {            	
                this.dataGridView1.DataSource = this.server.Sessions;
                
                if (this.dataGridView1.Columns.Count > 0)
                	this.dataGridView1.Columns[1].Visible = false;
                
                if (this.server.Sessions == null)
                	MessageBox.Show("Terminals was unable to enumerate your server's sessions." + (this.server.Errors != null & this.server.Errors.Count > 0 ? "\n" +this.server.Errors[0] : "" ));
            }
            else
            {
				MessageBox.Show("This machine does not appear to be a terminal server.");
            }
        }
        
        Kohl.Framework.Security.Credential credentials = null;
        
        public void Connect(string server, bool headless, Kohl.Framework.Security.Credential credentials)
        {
            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            this.credentials = credentials;
            
            try
            {
                this.splitContainer1.Panel1Collapsed = headless;

                if (server != "")
                {
                    this.ServerNameComboBox.Text = server;
                    Connect(credentials);
                }
            }
            catch (Exception exc)
            {
				Log.Error("Connection failure.", exc);
            }
        }

        public bool SaveInDB { get; set; }

        private void TerminalServerManager_Load(object sender, EventArgs e)
        {
            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            if (this.DesignMode)
                return;

            this.ServerNameComboBox.Items.Clear();
            FavoriteConfigurationElementCollection favorites = Settings.GetFavorites(SaveInDB);

            if (favorites != null)
            {
                foreach (FavoriteConfigurationElement elm in favorites)
                {
                    if (typeof (RDPConnection).IsEqual(elm.Protocol))
                    {
                        this.ServerNameComboBox.Items.Add(elm.ServerName);
                    }
                }
            }
        }

        private void sendMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.SelectedSession != null)
            {
            	string input = "Please enter the message to send..";

            	if (InputBox.Show(ref input) == DialogResult.OK && !string.IsNullOrWhiteSpace(input))
                {
                    TerminalServicesAPI.SendMessage(this.SelectedSession,
                                                    
						"Message from your administrator (sent via " + Kohl.Framework.Info.AssemblyInfo.Title + ")",
                                                    input.Trim(), 0, 10, false);
                }
            }
            else
            	MessageBox.Show("Please select a session");
        }

        private void logoffSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.SelectedSession != null)
            {
                if (
					MessageBox.Show("Are you sure you want to log off the selected session?",
						"Confirmation required ...",
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    TerminalServicesAPI.LogOffSession(this.SelectedSession, false);
                }
            }
            else
            	MessageBox.Show("Please select a session");
        }

        private void rebootServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.server.IsATerminalServer)
            {
                if (
					MessageBox.Show("Are you sure you want to reboot this server?",
						"Confirmation required ...",
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    TerminalServicesAPI.ShutdownSystem(this.server, true);
                }
            }
        }

        private void shutdownServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.server.IsATerminalServer)
            {
                if (
					MessageBox.Show("Are you sure you want to shutdown this server?",
						"Confirmation required ...",
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    TerminalServicesAPI.ShutdownSystem(this.server, false);
                }
            }
        }

        private void ServerNameComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button1_Click(null, null);
            }
        }
    }
}