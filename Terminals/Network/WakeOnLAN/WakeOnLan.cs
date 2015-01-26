using System;
using System.Windows.Forms;
using Kohl.Framework.Localization;
using Kohl.Framework.Logging;

namespace Terminals.Network.WakeOnLAN
{
    public partial class WakeOnLan : UserControl
    {
        public WakeOnLan()
        {
            this.InitializeComponent();
            Localization.SetLanguage(this);
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            try
            {
                MagicPacket wakeUpPacket = new MagicPacket(this.MACTextbox.Text);

                int byteSend = wakeUpPacket.WakeUp();

                this.ResultsLabel.Text = string.Format("{0} bytes sent to {1}", byteSend, wakeUpPacket.macAddress);
            }
            catch (Exception exc)
            {
                Log.Info("Error sending Magic Packet", exc);
                MessageBox.Show("There was an error sending the Magic Packet" + exc.Message);
            }
        }

        private void MACTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SendButton_Click(null, null);
            }
        }
    }
}