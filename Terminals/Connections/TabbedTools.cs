using Kohl.Framework.Logging;
using System;
using System.Drawing;
using System.Windows.Forms;
using Terminals.Network.Pcap;

namespace Terminals.Connections
{
    public partial class TabbedTools : UserControl, IDisposable
    {
        public delegate void TabChanged(object sender, TabControlEventArgs e);

        private readonly PacketCapture packetCapture1;

        public TabbedTools()
        {
            this.InitializeComponent();

            try
            {
                this.packetCapture1 = new PacketCapture();

                this.PcapTabPage.Controls.Add(this.packetCapture1);
                this.PcapTabPage.Location = new Point(4, 22);
                this.PcapTabPage.Name = "PcapTabPage";
                this.PcapTabPage.Padding = new Padding(3);
                this.PcapTabPage.Size = new Size(886, 309);
                this.PcapTabPage.TabIndex = 15;
                this.PcapTabPage.Text = "Packets";
                this.PcapTabPage.UseVisualStyleBackColor = true;

                this.packetCapture1.Dock = DockStyle.Fill;
                this.packetCapture1.Location = new Point(3, 3);
                this.packetCapture1.Name = "packetCapture1";
                this.packetCapture1.Size = new Size(880, 303);
                this.packetCapture1.TabIndex = 0;
            }
            catch (Exception ex)
            {
                this.PcapTabPage.Controls.Clear();
                Label l = new Label
                {
                    Text = "The software \"WinPCap\" is either not installed or not supported on this version of windows.",
                    Dock = DockStyle.Top
                };
                this.PcapTabPage.Controls.Add(l);
                Log.Info(l.Text, ex);
            }
        }

        public new void Dispose()
        {
            base.Dispose();
            this.packetCapture1.Dispose();
        }

        public event TabChanged OnTabChanged;

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (this.OnTabChanged != null)
                this.OnTabChanged(sender, e);
        }

        public void HideTab(Int32 index)
        {
            if (this.tabControl1.TabCount > index)
                this.tabControl1.TabPages[index].Hide();
        }

        public void Execute(String action, String host)
        {
            switch (action)
            {
                case "Ping":
                    this.tabControl1.SelectedTab = this.tabControl1.TabPages[0];
                    this.ping1.ForcePing(host);

                    break;
                case "DNS":
                    this.tabControl1.SelectedTab = this.tabControl1.TabPages[6];
                    this.dnsLookup1.ForceDNS(host);
                    break;

                case "Trace":
                    this.tabControl1.SelectedTab = this.tabControl1.TabPages[1];
                    this.traceRoute1.ForceTrace(host);
                    break;

                case "TSAdmin":
                    this.tabControl1.SelectedTab = this.tabControl1.TabPages[10];
                    this.terminalServerManager1.ForceTSAdmin(host);
                    break;

                default:
                    break;
            }
        }
    }
}