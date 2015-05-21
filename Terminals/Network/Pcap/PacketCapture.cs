using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Be.Windows.Forms;
using Kohl.Framework.Info;

using Kohl.Framework.Logging;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.WinPcap;
using Terminals.Properties;

namespace Terminals.Network.Pcap
{
    public partial class PacketCapture : UserControl, IDisposable
    {
        private WinPcapDeviceList devices;
        private bool disposing;
        private List<RawCapture> newpackets = new List<RawCapture>();
        private List<RawCapture> packets = new List<RawCapture>();
        private WinPcapDevice selectedDevice;
        private MethodInvoker stopUpdater;
        private MethodInvoker updater;

        public PacketCapture()
        {
            this.InitializeComponent();
            
        }

        public new void Dispose()
        {
            this.disposing = true;
            this.StopCaptureButton_Click(null, EventArgs.Empty);
        }

        private void PacketCapture_Load(object sender, EventArgs e)
        {
            try
            {
                this.promiscuousCheckbox.Enabled = true;
                this.StopCaptureButton.Enabled = false;
                this.AmberPicture.Visible = true;
                this.GreenPicture.Visible = false;
                this.RedPicture.Visible = false;
                this.updater = this.UpdateUI;
                this.stopUpdater = this.PcapStopped;
                this.devices = WinPcapDeviceList.Instance;

                foreach (WinPcapDevice device in this.devices)
                {
                    this.comboBox1.Items.Add(device.Description);
                }

                if (this.devices.Count > 0) this.comboBox1.SelectedIndex = 1;

                this.webBrowser1.DocumentStream = new MemoryStream(Encoding.Default.GetBytes(Resources.Filtering));
            }
            catch (Exception exc)
            {
                this.Enabled = false;
                if (exc is BadImageFormatException)
                {
					Log.Info("Terminals packet capture is not configured to work with this system (Bad Image Format Exception)", exc);
					MessageBox.Show("Terminals packet capture is not configured to work with this system (Bad Image Format Exception)", AssemblyInfo.Title(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (exc is DllNotFoundException)
                {
                    Log.Info("Network.Pcap.PacketCapture_Info2", exc);
                    if (
                        MessageBox.Show(
							"It appears that WinPcap is not installed.  In order to use this feature within Terminals you must first install that product. " +
							"Do you wish to visit the download location right now?",
							"Download WinPcap?", MessageBoxButtons.OKCancel) ==
                        DialogResult.OK)
                    {
                        Process.Start("http://www.winpcap.org/install/default.htm");
                    }
                }
                else
                {
                    Log.Info("Network.Pcap.PacketCapture_Info3", exc);
                }
            }
            this.PacketCapture_Resize(null, null);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectedDevice = this.devices[this.comboBox1.SelectedIndex];
            this.propertyGrid1.SelectedObject = this.selectedDevice;
        }

        private void StartCapture(object state)
        {
            WinPcapDevice device = (WinPcapDevice) state;
            device.Open(this.promiscuousCheckbox.Checked ? DeviceMode.Promiscuous : DeviceMode.Normal);

            try
            {
                device.Filter = this.FilterTextBox.Text;
            }
            catch (Exception exc)
            {
				MessageBox.Show(string.Format("Failed to set the filter {0}.",
                                              this.FilterTextBox.Text));
                Log.Info(
					string.Format("Failed to set the filter {0}.", this.FilterTextBox.Text),
                    exc);
            }

            device.StartCapture();
        }

        private void CaptureButton_Click(object sender, EventArgs e)
        {
            // If no device is visible.
            if (this.selectedDevice == null)
                return;

            this.promiscuousCheckbox.Enabled = false;
            this.CaptureButton.Enabled = false;
            this.StopCaptureButton.Enabled = true;
            this.AmberPicture.Visible = false;
            this.GreenPicture.Visible = true;
            this.RedPicture.Visible = false;
            this.listBox1.Items.Clear();
            lock (this.packets)
            {
                this.packets = new List<RawCapture>();
                this.newpackets = new List<RawCapture>();

                this.selectedDevice.OnPacketArrival += this.SelectedDeviceOnPacketArrival;
                this.selectedDevice.OnCaptureStopped += this.SelectedDeviceOnCaptureStopped;
            }

            ThreadPool.QueueUserWorkItem(this.StartCapture, this.selectedDevice);
        }

        private void SelectedDeviceOnCaptureStopped(object sender, CaptureStoppedEventStatus status)
        {
            if (this.disposing)
                return;

            this.Invoke(this.stopUpdater);
        }

        private void SelectedDeviceOnPacketArrival(object sender, CaptureEventArgs e)
        {
            if (this.disposing)
                return;

            lock (this.packets)
            {
                this.packets.Add(e.Packet);
                this.newpackets.Add(e.Packet);
            }

            if (!this.disposing)
                this.Invoke(this.updater);
        }

        private void PcapStopped()
        {
            this.CaptureButton.Enabled = true;
            this.StopCaptureButton.Enabled = false;
            this.RedPicture.Visible = false;
            this.GreenPicture.Visible = false;
            this.AmberPicture.Visible = true;
            this.promiscuousCheckbox.Enabled = true;
        }

        private void UpdateUI()
        {
            lock (this.packets)
            {
                this.GreenPicture.Visible = false;
                Application.DoEvents();
                foreach (RawCapture packet in this.newpackets)
                {
                    this.listBox1.Items.Add(packet);
                    this.newpackets = new List<RawCapture>();
                }
                Application.DoEvents();
                this.GreenPicture.Visible = true;
            }
        }

        private void StopCapture(object state)
        {
            PcapDevice device = (PcapDevice) state;
            device.StopCaptureTimeout = new TimeSpan(0, 0, 0, 0, 500);
            try
            {
                device.StopCapture();
            }
            catch
            {
				Log.Warn("Terminals needed more than 500 milliseconds to abort the PCap thread.");
            }

            device.Close();
        }

        private void StopCaptureButton_Click(object sender, EventArgs e)
        {
            // If no device is visible.
            if (this.selectedDevice == null)
                return;

            this.CaptureButton.Enabled = true;
            this.StopCaptureButton.Enabled = false;
            this.RedPicture.Visible = true;
            this.GreenPicture.Visible = false;
            this.AmberPicture.Visible = false;
            this.promiscuousCheckbox.Enabled = true;

            ThreadPool.QueueUserWorkItem(this.StopCapture, this.selectedDevice);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex > 0)
            {
                RawCapture packet = this.packets[this.listBox1.SelectedIndex];
                DynamicByteProvider provider = new DynamicByteProvider(packet.Data);
                this.hexBox1.ByteProvider = provider;
                this.textBox1.Text = Encoding.Default.GetString(packet.Data);
                this.treeView1.Nodes.Clear();
				TreeNode header = this.treeView1.Nodes.Add("Header");
				header.Nodes.Add(string.Format("Length: {0}",
                                               packet.Data.Length));

                StringBuilder sb = new StringBuilder();
                foreach (byte b in packet.Data)
                {
                    sb.Append(b.ToString("00"));
                    sb.Append(" ");
                }
				header.Nodes.Add(string.Format("Data: {0}", sb));
				header.Nodes.Add(string.Format("Data length: {0}",
                                               packet.Data.Length.ToString()));
				header.Nodes.Add(string.Format("Date: {0}",
                                               packet.Timeval.Date.ToString()));
				header.Nodes.Add(string.Format("Microseconds: {0}",
                                               packet.Timeval.MicroSeconds.ToString()));
				header.Nodes.Add(string.Format("Seconds: {0}",
                                               packet.Timeval.Seconds.ToString()));

                this.treeView1.ExpandAll();
            }
        }

        private void PacketCapture_Resize(object sender, EventArgs e)
        {
            this.hexBox1.BytesPerLine = this.hexBox1.Width > 640 ? 16 : 8;
        }
    }
}