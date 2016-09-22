using Metro.TransportLayer.Tcp;
using System;
using System.Windows.Forms;

namespace Terminals.Network
{
    public partial class LocalConnections : UserControl
    {
        public LocalConnections()
        {
            this.InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TcpConnection[] connections = TcpConnectionManager.GetCurrentTcpConnections();
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