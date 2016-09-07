using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Kohl.Framework.Logging;
using ZedGraph;
using Timer = System.Threading.Timer;

namespace Terminals.Network.Ping
{
    public partial class Ping : UserControl, IDisposable
    {
        private bool disposing;

        public new void Dispose()
        {
            this.disposing = true;
            this.ButtonStop_Click(null, EventArgs.Empty);
            base.Dispose();
        }

        private readonly MethodInvoker DoUpdateForm;
        private readonly Byte[] buffer;
        private readonly PingOptions packetOptions;
        private readonly Object threadLocker = new Object();
        private readonly AutoResetEvent waiter = new AutoResetEvent(false);
        private Int64 counter;
        private Int32 currentDelay;
        private String destination = String.Empty;
        private String hostName = String.Empty;
        private GraphPane myPane;
        private List<PingReplyData> pingList = new List<PingReplyData>();
        private Boolean pingReady;
        private Boolean pingRunning;
        private System.Net.NetworkInformation.Ping pingSender;
        private Timer timer;

        public Ping()
        {
            this.InitializeComponent();
            this.DoUpdateForm = this.UpdateForm;

            // Create a buffer of 32 bytes of data to be transmitted.
            this.buffer = Encoding.ASCII.GetBytes(new String('.', 32));
            // Jump though 50 routing nodes tops, and don't fragment the packet
            this.packetOptions = new PingOptions(50, true);

            this.InitializeGraph();

            this.dataGridView1.SuspendLayout();

            this.dataGridView1.Columns.Add("1", "Count");
            this.dataGridView1.Columns.Add("2", "Status");
            this.dataGridView1.Columns.Add("3", "Host name");
            this.dataGridView1.Columns.Add("4", "Destination");
            this.dataGridView1.Columns.Add("5", "Bytes");
            this.dataGridView1.Columns.Add("6", "Time to live");
            this.dataGridView1.Columns.Add("7", "Roundtrip time");
            this.dataGridView1.Columns.Add("8", "Time");

            this.dataGridView1.ResumeLayout(true);
        }

        private void Ping_Load(object sender, EventArgs e)
        {
            this.TextHost.Focus();
        }

        private void Ping_Resize(object sender, EventArgs e)
        {
            this.SetSize();
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.TextHost.Text.Trim()))
            {
                this.TextHost.Focus();
                return;
            }
            
            this.TextHost.Text = this.TextHost.Text.Trim();

            this.ButtonStart.Enabled = false;
            this.TextHost.Enabled = false;
            Application.DoEvents();

            if (!this.pingRunning)
            {
                String msg = String.Empty;
                try
                {
                    IPAddress[] list = Dns.GetHostAddresses(this.TextHost.Text);
                    if (list != null)
                    {
                        this.destination = list[0].ToString();

                        IPAddress ip;
                        this.hostName = (IPAddress.TryParse(this.TextHost.Text, out ip))
                                            ? this.destination
                                            : Dns.GetHostEntry(this.TextHost.Text).HostName;

                        this.counter = 1;
                        this.currentDelay = (Int32) this.DelayNumericUpDown.Value;
                        this.pingList = new List<PingReplyData>();

                        if (this.pingSender == null)
                        {
                            this.pingSender = new System.Net.NetworkInformation.Ping();
                            this.pingSender.PingCompleted += this.pingSender_PingCompleted;
                        }

                        this.pingRunning = true;
                        this.pingReady = true;

                        // Making sure previous timer is cleared before starting a new one
                        if (this.timer != null)
                        {
                            this.timer.Dispose();
                            this.timer = null;
                        }

                        // Start thread timer to start TrySend method for every ms in the specified delay updown box
                        TimerCallback callback = this.TryPing;
                        this.timer = new Timer(callback, null, this.currentDelay, this.currentDelay);
                    }
                }
                catch (SocketException)
                {
                    msg = String.Format("Could not resolve address: {0}", this.TextHost.Text);
                    this.pingRunning = false;
                }
                catch (ArgumentException)
                {
                    msg = String.Format("Hostname or IP-Address is invalid: {0}", this.TextHost.Text);
                    this.pingRunning = false;
                }
                catch (Exception ex)
                {
                    msg = String.Format("An error occured trying to ping {0}", this.TextHost.Text);
                    Log.Info(msg, ex);
                    this.pingRunning = false;
                }
                finally
                {
                    if (!this.pingRunning)
                    {
                        MessageBox.Show(msg, "Ping Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.ResetForm();
                    }
                }
            }
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            if (this.pingRunning)
            {
                this.pingRunning = false;
                this.pingReady = false;

                if (this.pingSender != null)
                {
                    this.pingSender.PingCompleted -= this.pingSender_PingCompleted;
                    this.pingSender.SendAsyncCancel();
                    this.pingSender.Dispose();
                    this.pingSender = null;
                }

                this.timer.Dispose();
                this.timer = null;
            }

            this.ResetForm();
        }

        private void TextHost_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
                if (!this.pingRunning)
                    this.ButtonStart.PerformClick();
            }
        }

        private void TryPing(Object state)
        {
            if (this.pingRunning && this.pingReady)
            {
                this.SendPing();
            }
        }

        private void SendPing()
        {
            try
            {
                this.pingSender.SendAsync(this.destination, 2000, this.buffer, this.packetOptions, this.waiter);
                this.waiter.WaitOne(0);

                lock (this.timer)
                {
                    if (this.DelayNumericUpDown.Value != this.currentDelay)
                    {
                        this.currentDelay = (Int32) this.DelayNumericUpDown.Value;
                        this.timer.Change(this.currentDelay, this.currentDelay);
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // Error: An asynchronous call is already in progress
                // Overflow of SendAsync calls. Just let it go, or does someone know how to handle this better?
            }
            catch (Exception ex)
            {
                Log.Info(String.Empty, ex);
            }
        }

        private void pingSender_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            if (this.disposing)
                return;

            try
            {
                if (Thread.CurrentThread.Name == null)
                    Thread.CurrentThread.Name = "Ping Completed";

                ((AutoResetEvent) e.UserState).Set();

                if (e.Reply.Status == IPStatus.Success)
                {
                    lock (this.threadLocker)
                    {
                        PingReplyData pd = new PingReplyData(
                            this.counter++,
                            "Reply from: ",
                            this.hostName,
                            this.destination,
                            e.Reply.Buffer.Length,
                            e.Reply.Options.Ttl,
                            e.Reply.RoundtripTime,
                            DateTime.Now.ToLongTimeString());

                        lock (this.pingList)
                        {
                            this.pingList.Add(pd);
                        }

						if (!this.IsDisposed && !this.Disposing)
							this.InvokeIfNecessary(() => this.Invoke(this.DoUpdateForm));
                        this.pingReady = true;
                    }
                }
                else if (!e.Cancelled)
                {
                    String status = String.Empty;
                    switch (e.Reply.Status)
                    {
                        case IPStatus.TimedOut:
                            status = "Request timed out.";
                            break;

                        case IPStatus.DestinationHostUnreachable:
                            status = "Destination host unreachable.";
                            break;
                    }

                    lock (this.threadLocker)
                    {
                        this.pingSender.SendAsyncCancel();
                        PingReplyData pd = new PingReplyData(
                            this.counter++, status, String.Empty, String.Empty, 0, 0, 0, DateTime.Now.ToLongTimeString());

                        lock (this.pingList)
                        {
                            this.pingList.Add(pd);
                        }
                        
						if (!this.IsDisposed && !this.Disposing)
							this.InvokeIfNecessary(() => this.Invoke(this.DoUpdateForm));
                        this.pingReady = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Info("Error on Ping.PingCompleted", ex);
            }
            finally
            {
                ((AutoResetEvent) e.UserState).Set();
            }
        }

        public void ForcePing(String hostName)
        {
            this.TextHost.Text = hostName;
            this.ButtonStart.PerformClick();
        }

        /// <summary>
        ///     Update form control with new data.
        /// </summary>
        private void UpdateForm()
        {
            PingReplyData data = this.pingList[this.pingList.Count - 1];

            this.dataGridView1.Rows.Add(new object[] { data.Count, data.Status, data.Hostname, data.Destination, data.Bytes, data.TimeToLive, data.RoundTripTime, data.Time });

            if (this.dataGridView1.Rows.Count > 1)
                this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView1.Rows.Count - 1;

            this.UpdateGraph();
        }

        /// <summary>
        ///     Reset the form control to start properties.
        /// </summary>
        private void ResetForm()
        {
            this.ButtonStart.Enabled = true;
            this.TextHost.Enabled = true;
            this.TextHost.Focus();
            this.TextHost.SelectAll();
        }

        private void InitializeGraph()
        {
            this.myPane = this.ZGraph.GraphPane;
            // Set the titles and axis labels
            this.myPane.Title.Text = "Ping results";
            this.myPane.XAxis.Title.Text = "Counter";
            this.myPane.YAxis.Title.Text = "Time, Milliseconds";

            // Show the x axis grid
            this.myPane.XAxis.MajorGrid.IsVisible = true;

            // Make the Y axis scale red
            this.myPane.YAxis.Scale.FontSpec.FontColor = Color.Blue;
            this.myPane.YAxis.Title.FontSpec.FontColor = Color.Blue;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            this.myPane.YAxis.MajorTic.IsOpposite = false;
            this.myPane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            this.myPane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            this.myPane.YAxis.Scale.Align = AlignP.Inside;

            // Fill the axis background with a gradient
            this.myPane.Chart.Fill = new Fill(Color.White, Color.LightGray, 45.0f);

            // Add a text box with instructions
            TextObj text = new TextObj(
                "Zoom: left mouse & drag\nPan: middle mouse & drag\nContext Menu: right mouse",
                0.02f, 0.15f, CoordType.ChartFraction, AlignH.Left, AlignV.Bottom)
                               {
                                   FontSpec =
                                       {
                                           Size = 8,
                                           StringAlignment = StringAlignment.Near
                                       }
                               };
            this.myPane.GraphObjList.Add(text);

            // Enable scrollbars if needed
            this.ZGraph.IsShowHScrollBar = true;
            this.ZGraph.IsShowVScrollBar = true;

            // OPTIONAL: Show tooltips when the mouse hovers over a point
            this.ZGraph.IsShowPointValues = true;
            this.ZGraph.PointValueEvent += this.MyPointValueHandler;

            // OPTIONAL: Add a custom context menu item
            //this.ZGraph.ContextMenuBuilder += new ZedGraphControl.ContextMenuBuilderEventHandler(this.MyContextMenuBuilder);

            // Size the control to fit the window
            this.SetSize();
        }

        private void UpdateGraph()
        {
            // Make up some data points based on the Sine function
            PointPairList list = new PointPairList();
            PointPairList avgList = new PointPairList();
            Int32 x = 1;
            Int64 yMax = 0;
            Int64 sum = 0;

            foreach (PingReplyData p in this.pingList)
            {
                if (p.RoundTripTime > yMax)
                    yMax = p.RoundTripTime;

                list.Add(x, p.RoundTripTime);

                sum += p.RoundTripTime;
                avgList.Add(x, (Int32) (sum/x));
                x++;
            }

            this.myPane.Title.Text = String.Format("Ping results for {0}", this.TextHost.Text);

            // Manually set the axis range
            this.myPane.YAxis.Scale.Min = 0;
            this.myPane.YAxis.Scale.Max = yMax;
            this.myPane.XAxis.Scale.Min = 0;
            this.myPane.XAxis.Scale.Max = x;

            this.myPane.CurveList.Clear();
            LineItem myCurve = this.myPane.AddCurve(this.TextHost.Text, list, Color.Blue, SymbolType.Diamond);
            this.myPane.AddCurve("Average", avgList, Color.Red, SymbolType.Diamond);

            // Fill the symbols with white
            myCurve.Symbol.Fill = new Fill(Color.White);
            // Associate this curve with the Y2 axis
            myCurve.IsY2Axis = true;

            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            this.ZGraph.AxisChange();
            // Make sure the Graph gets redrawn
            this.ZGraph.Invalidate();
        }

        private void SetSize()
        {
            this.ZGraph.Location = new Point(10, 10);
            // Leave a small margin around the outside of the control
            this.ZGraph.Size = new Size(this.ClientRectangle.Width - 20, this.ClientRectangle.Height - 20);
        }

        /// <summary>
        ///     Display customized tooltips when the mouse hovers over a point
        /// </summary>
        private string MyPointValueHandler(ZedGraphControl control, GraphPane pane, CurveItem curve, int iPt)
        {
            try
            {
                // Get the PointPair that is under the mouse
                PointPair pt = curve[iPt];

                return String.Format("{0} is {1:f2} milliseconds at {2:f1}", curve.Label.Text, pt.Y, pt.X);
            }
            catch (Exception ex)
            {
                Log.Info(String.Empty, ex);
            }

            return String.Empty;
        }
    }
}