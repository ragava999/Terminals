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
            this.SelectedSession = null;
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;
            this.propertyGrid1.SelectedObject = null;
            Application.DoEvents();
            this.server = TerminalServer.LoadServer(this.ServerNameComboBox.Text);

            if (this.server.IsATerminalServer)
            {
                this.dataGridView1.DataSource = this.server.Sessions;
                this.dataGridView1.Columns[1].Visible = false;
            }
            else
            {
				MessageBox.Show("This machine does not appear to be a terminal server.");
            }
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

        public void Connect(string server, bool headless)
        {
            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            try
            {
                this.splitContainer1.Panel1Collapsed = headless;

                if (server != "")
                {
                    this.ServerNameComboBox.Text = server;
                    this.button1_Click(null, null);
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