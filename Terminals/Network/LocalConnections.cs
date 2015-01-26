using System;
using System.Windows.Forms;
using Kohl.Framework.Localization;
using Metro.TransportLayer.Tcp;

namespace Terminals.Network
{
    public partial class LocalConnections : UserControl
    {
        public LocalConnections()
        {
            this.InitializeComponent();
            Localization.SetLanguage(this);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TcpConnection[] connections = TcpConnectionManager.GetCurrentTcpConnections();
            //this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = connections;
        }

        private void LocalConnections_Load(object sender, EventArgs e)
        {
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            this.timer1_Tick(null, null);
            this.timer1.Enabled = true;
            this.timer1.Start();
        }
    }
}