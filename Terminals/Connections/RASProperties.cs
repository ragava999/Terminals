using Terminals.Connection.Manager;
using Terminals.Connection.Panels.OptionPanels;

namespace Terminals.Connections
{
    using System;
    using System.Windows.Forms;
    using DotRas;

    using Kohl.Framework.Localization;
    using Kohl.Framework.Logging;

    using Connection;

    public partial class RasProperties : UserControl
    {
        private DateTime connectedTime = DateTime.MinValue;
        private RASConnection rasConnection;

        private Timer timer;

        public RasProperties(IHostingForm parentForm, RASConnection rasConnection)
        {
            this.RasConnection = rasConnection;
            this.ParentForm = parentForm;
            this.InitializeComponent();

            if (parentForm.InvokeRequired)
                parentForm.Invoke(new MethodInvoker(delegate
                {
                    this.timer = new Timer { Interval = 2000 };
                    this.timer.Tick += this.timer_Tick;
                    this.timer.Start();
                }));
            else
            {
                this.timer = new Timer { Interval = 2000 };
                this.timer.Tick += this.timer_Tick;
                this.timer.Start();
            }
        }

        private void DisposeTimer()
        {
            if (timer != null)
            {
                if (timer.Enabled)
                    timer.Enabled = false;

                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }

        private RASConnection RasConnection
        {
            get { return this.rasConnection; }

            set
            {
                this.rasConnection = value;
                this.rasConnection.OnLog += this.RasConnectionOnLog;
            }
        }

        private new IHostingForm ParentForm { get; set; }

        public RasEntry RasEntry { private get; set; }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.UpdateStats();
        }

        private static readonly object Locker = new object();

        private void UpdateStats()
        {
            this.InvokeIfNecessary(() =>
            {
                lock (Locker)
                {
                    // If we have 500 Entries delete the list
                    if (lbDetails1 != null && this.lbDetails1.Items.Count >= 500)
                        this.lbDetails1.Items.Clear();

                    this.BringToFront();

                    if (this.RasConnection.Connected)
                    {
                        if (this.connectedTime == DateTime.MinValue)
                            this.connectedTime = DateTime.Now;

                        this.Info(Localization.Text("Connection.RASProperties.UpdateStats_Connected"));
                        this.Info(string.Format(Localization.Text("Connection.RASProperties.UpdateStats_ServerName"), this.RasConnection.Favorite.ServerName));
                        this.Info(string.Format(Localization.Text("Connection.RASProperties.UpdateStats_Host"), this.RasEntry.PhoneNumber));
                        this.Info(string.Format(Localization.Text("Connection.RASProperties.UpdateStats_IPAddress"), this.RasEntry.IPAddress));

                        TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - this.connectedTime.Ticks);

                        this.Info(string.Format(Localization.Text("Connection.RASProperties.UpdateStats_ConnectionDuration"), ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                    }
                    else
                    {
                        this.Info(Localization.Text("Connection.RASProperties.UpdateStats_NotConnected"));
                    }
                }
            });
        }

        public void Error(string entry, Exception exception = null)
        {
            if (exception != null)
                Log.Error(entry, exception);
            else
                Log.Error(entry);

            RasConnectionOnLog("ERROR: " + entry);
        }

        public void Warn(string entry, Exception exception = null)
        {
            if (exception != null)
                Log.Error(entry, exception);
            else
                Log.Error(entry);

            RasConnectionOnLog("WARNING: " + entry);
        }

        public void Info(string entry)
        {
            Log.Info(entry);
            RasConnectionOnLog("INFORMATION: " + entry);
        }

        private void RasConnectionOnLog(string entry)
        {
            if (lbDetails1 == null)
                return;

            if (!string.IsNullOrEmpty(entry))
            {
                if (this.lbDetails1.InvokeRequired)
                    this.lbDetails1.Invoke(new MethodInvoker(() => this.RasConnectionOnLog(entry)));
                else
                    //this.lbDetails1.TopIndex = this.lbDetails1.Items.Add(string.Format("{0} - {1}", DateTime.Now.ToString("hh:mm"), entry));
                    this.lbDetails1.Items.Insert(0, string.Format("{0} - {1}", DateTime.Now.ToString("hh:mm:ss"), entry));
            }
        }
    }
}