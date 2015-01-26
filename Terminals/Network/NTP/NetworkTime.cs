using System;
using System.Windows.Forms;
using Kohl.Framework.Localization;

namespace Terminals.Network.NTP
{
    public partial class NetworkTime : UserControl
    {
        public NetworkTime()
        {
            this.InitializeComponent();
        }

        private void LookupButton_Click(object sender, EventArgs e)
        {
            this.propertyGrid1.SelectedObject = null;
            Application.DoEvents();
            NTPClient client = null;
            string server = this.TimeServerTextBox.Text;
            if (server != "" && server != NTPClient.DefaultTimeServer)
                client = NTPClient.GetTime(server);
            else
                client = NTPClient.GetTime();

            this.propertyGrid1.SelectedObject = client;

            Localization.SetLanguage(this);
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            this.propertyGrid1.SelectedObject = null;
            Application.DoEvents();
            NTPClient client = null;
            string server = this.TimeServerTextBox.Text;
            if (server != "" && server != NTPClient.DefaultTimeServer)
                client = NTPClient.GetTime(server);
            else
                client = NTPClient.GetTime();

            this.propertyGrid1.SelectedObject = client;
        }

        private void NetworkTime_Load(object sender, EventArgs e)
        {
            this.TimeServerTextBox.Text = NTPClient.DefaultTimeServer;
        }
    }
}