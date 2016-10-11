using System;
using System.Windows.Forms;

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

            try
            {
                if (server != "" && server != NTPClient.DefaultTimeServer)
                    client = NTPClient.GetTime(server);
                else
                    client = NTPClient.GetTime();

                this.propertyGrid1.SelectedObject = client;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to receive NTP lookup details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Kohl.Framework.Logging.Log.Error("Unable to receive the network time.", ex);
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            LookupButton_Click(sender, e);

            // Check if we are capable of retrieving some data from the NTP servers.
            // If not -> the min date is equal to 1900-01-01 2:00
            if (this.propertyGrid1 != null && this.propertyGrid1.SelectedObject != null && ((NTPClient)this.propertyGrid1.SelectedObject).ReferenceTimestamp > new DateTime(1900, 1, 1, 2, 00, 0))
                if (this.TimeServerTextBox.Text != "" && this.TimeServerTextBox.Text != NTPClient.DefaultTimeServer)
                    NTPClient.GetAndSetTime(this.TimeServerTextBox.Text);
                else
                    NTPClient.GetAndSetTime();
        }

        private void NetworkTime_Load(object sender, EventArgs e)
        {
            this.TimeServerTextBox.Text = NTPClient.DefaultTimeServer;
        }
    }
}