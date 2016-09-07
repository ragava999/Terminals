using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows.Forms;

using Kohl.Framework.Logging;
using Metro;
using Metro.Scanning;

namespace Terminals.Network.PortScanner
{
    public partial class PortScanner : UserControl
    {
        private readonly MethodInvoker miv;
        private readonly object resultsLock = new object();
        private int Counter;
        private List<ScanResult> Results;
        private IPAddress endPointAddress;
        private int portCount;

        private List<TcpSynScanner> scanners;

        public PortScanner()
        {
            this.InitializeComponent();
            this.miv = this.UpdateConnections;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            this.scanners = new List<TcpSynScanner>();
            this.Results = new List<ScanResult>();

            this.StartButton.Enabled = false;
            this.StopButton.Enabled = true;

            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ScanSubnet), null);
            this.ScanSubnet();
        }

        private void ScanSubnet()
        {
            string startPort = this.pa.Text;
            string endPort = this.pb.Text;
            int iStartPort = 0;
            int iEndPort = 0;

            if (int.TryParse(startPort, out iStartPort) && int.TryParse(endPort, out iEndPort))
            {
                if (iStartPort > iEndPort)
                {
                    int iPortTemp = iStartPort;
                    iStartPort = iEndPort;
                    iEndPort = iPortTemp;
                }

                ushort[] ports = new ushort[iEndPort - iStartPort + 1];
                int counter = 0;

                for (int y = iStartPort; y <= iEndPort; y++)
                {
                    ports[counter] = (ushort) y;
                    counter++;
                }

                this.portCount = ports.Length;
                string initial = string.Format("{0}.{1}.{2}.", this.a.Text, this.b.Text, this.c.Text);
                string start = this.d.Text;
                string end = this.e.Text;
                int iStart = 0;
                int iEnd = 0;

                if (int.TryParse(start, out iStart) && int.TryParse(end, out iEnd))
                {
                    if (iStart > iEnd)
                    {
                        int iTemp = iStart;
                        iStart = iEnd;
                        iEnd = iTemp;
                    }

                    for (int x = iStart; x <= iEnd; x++)
                    {
                        IPAddress finalAddress;

                        if (IPAddress.TryParse(initial + x.ToString(), out finalAddress))
                        {
                            try
                            {
                                ThreadPool.QueueUserWorkItem(this.ScanMachine, new object[] {finalAddress, ports});
                            }
                            catch (Exception exc)
                            {
								Log.Error("Error scanning the IP subset.", exc);
                            }
                        }
                    }
                }
            }
        }

        private void ScanMachine(object state)
        {
            try
            {
                object[] states = (object[]) state;
                IPAddress address = (IPAddress) states[0];
                ushort[] ports = (ushort[]) states[1];

                TcpSynScanner scanner = new TcpSynScanner(new IPEndPoint(this.endPointAddress, 0));
                scanner.PortReply += this.scanner_PortReply;
                scanner.ScanComplete += this.scanner_ScanComplete;
                this.scanners.Add(scanner);
                scanner.StartScan(address, ports, 1000, 100, true);
                this.Counter = this.Counter + ports.Length;
            }
            catch (Exception ex)
            {
				Log.Warn("Error occured while trying to scan the target machine.", ex);
				MessageBox.Show("Error occured while trying to scan the target machine." + Environment.NewLine + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!this.IsDisposed)
                this.Invoke(this.miv);
        }

        private void PortScanner_Load(object sender, EventArgs eargs)
        {
            NetworkInterfaceList nil = new NetworkInterfaceList();
            try
            {
                foreach (NetworkInterface face in nil.Interfaces)
                {
                    if (face.IsEnabled && !face.isLoopback)
                    {
                        this.endPointAddress = face.Address;
                        string[] parts = this.endPointAddress.ToString().Split('.');
                        this.a.Text = parts[0];
                        this.b.Text = parts[1];
                        this.c.Text = parts[2];
                        this.d.Text = parts[3];
                        this.e.Text = parts[3];
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
				Log.Error("Error connecting to the network interfaces.", exc);
            }
        }

        private void scanner_ScanComplete()
        {
            if (this.Counter > 0) this.Counter = this.Counter - this.portCount;
            this.Invoke(this.miv);
            this.StartButton.Enabled = true;
        }

        private void scanner_PortReply(IPEndPoint remoteEndPoint, TcpPortState state)
        {
            this.Counter--;
            ScanResult r = new ScanResult
                               {
                                   RemoteEndPoint = new IPEndPoint(remoteEndPoint.Address, remoteEndPoint.Port),
                                   State = state
                               };

            lock (this.resultsLock)
            {
                this.Results.Add(r);
            }

            this.Invoke(this.miv);
        }

        private void UpdateConnections()
        {
            this.resultsGridView.Rows.Clear();

            if (this.resultsGridView.Columns == null || this.resultsGridView.Columns.Count == 0)
            {
                this.resultsGridView.Columns.Add(
					"End point",
					"End point");
                this.resultsGridView.Columns.Add(
					"State",
					"State");
            }

            lock (this.resultsLock)
            {
                foreach (ScanResult result in this.Results)
                {
                    if (result.State == TcpPortState.Opened)
                    {
                        this.resultsGridView.Rows.Add(new object[] {result.RemoteEndPoint, result.State});
                    }
                }
            }

            if (this.Counter <= 0)
            {
                this.StopButton.Enabled = false;
                this.StartButton.Enabled = true;
                this.Counter = 0;
            }

            this.ScanResultsLabel.Text =
                string.Format(
					"Outstanding requests: {0}",
                    this.Counter);
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            this.StartButton.Enabled = true;
            this.StopButton.Enabled = false;

            foreach (TcpSynScanner scanner in this.scanners)
            {
                if (scanner.Running) scanner.CancelScan();
            }

            this.Invoke(this.miv);
        }

        private void a_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.OemPeriod)
            {
                this.b.Focus();
            }
        }

        private void b_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.OemPeriod)
            {
                this.c.Focus();
            }
        }

        private void c_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.OemPeriod)
            {
                this.d.Focus();
            }
        }

        private void d_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.OemPeriod)
            {
                this.e.Focus();
            }
        }

        private void e_KeyUp(object sender, KeyEventArgs earg)
        {
            if (earg.KeyCode == Keys.Enter || earg.KeyCode == Keys.OemPeriod)
            {
                this.pa.Focus();
            }
        }

        private void pa_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.OemPeriod)
            {
                this.pb.Focus();
            }
        }

        private void pb_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.StartButton_Click(null, null);
            }
        }

        private void copyRemoteAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.resultsGridView.SelectedCells.Count > 0 &&
                this.resultsGridView.SelectedCells[0].RowIndex <= this.resultsGridView.Rows.Count)
            {
                string ip =
                    this.resultsGridView.Rows[this.resultsGridView.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
                if (ip != null && ip.IndexOf(":") > 0)
                {
                    ip = ip.Substring(0, ip.IndexOf(":"));
                    Clipboard.SetText(ip, TextDataFormat.Text);
                }
            }
        }
    }
}