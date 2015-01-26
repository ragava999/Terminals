using Terminals.Connection.ScreenCapture;

namespace Terminals.Connection
{
    // .NET namespaces
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    // Terminals and framework namespaces
    using Kohl.Framework.Logging;
    using TabControl;

    public partial class PopupTerminal : Form
    {
        #region Private Fields (5)
        private readonly TerminalTabsSelectionControler mainTabsControler;
        private readonly object synLock = new object();
        private Timer closeTimer;
        private bool fullScreen;
        private Timer timerHover;
        #endregion

        #region Constructors (2)
        private PopupTerminal()
        {
            this.InitializeComponent();
        }

        public PopupTerminal(TerminalTabsSelectionControler mainTabsControler = null) : this()
        {
            this.mainTabsControler = mainTabsControler;

            if (mainTabsControler == null)
                this.AttachToolStripButton.Visible = false;
        }
        #endregion

        #region Internal Properties (1)
        internal bool CaptureButtonEnabled
        {
            set { this.CaptureToolStripButton.Enabled = value; }
        }
        #endregion

        #region Private Properties (1)
        private bool FullScreen
        {
            get { return this.fullScreen; }
            set
            {
                this.fullScreen = value;
                this.UpdateWindowByFullScreen(value);
            }
        }
        #endregion

        #region Public Methods (2)
        public void AddTerminal(TerminalTabControlItem tabControlItem)
        {
            if (tabControlItem == null)
                return;

            while (tabControlItem.Connection == null)
                Application.DoEvents();

            this.tabControl1.AddTab(tabControlItem);
            this.Text = tabControlItem.Connection.Favorite.Name;
        }

        public void UpdateTitle(string newTitle)
        {
            this.tabControl1.Items[0].Title = newTitle;
            this.Text = newTitle;
        }
        #endregion

        #region Private Methods (13)
        private void CaptureScreen()
        {
            CaptureManagerConnection.PerformScreenCapture(this.tabControl1);

            if (mainTabsControler != null)
                this.mainTabsControler.RefreshCaptureManagerAndCreateItsTab(false);
        }

        private void UpdateWindowByFullScreen(bool fullScreen)
        {
            if (fullScreen)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void timerHover_Tick(object sender, EventArgs e)
        {
            if (this.timerHover.Enabled)
            {
                this.timerHover.Enabled = false;
                this.tabControl1.ShowTabs = true;
            }
        }

        private void tabControl1_TabControlItemClosing(TabControlItemClosingEventArgs e)
        {
            if (this.tabControl1.Items.Count <= 1)
            {
                this.Close();
            }
        }

        private void tcTerminals_MouseHover(object sender, EventArgs e)
        {
            if (this.tabControl1 != null)
            {
                if (!this.tabControl1.ShowTabs)
                {
                    this.timerHover.Enabled = true;
                }
            }
        }

        private void tcTerminals_MouseLeave(object sender, EventArgs e)
        {
            this.timerHover.Enabled = false;
            if (this.FullScreen && this.tabControl1.ShowTabs && !this.tabControl1.MenuOpen)
            {
                this.tabControl1.ShowTabs = false;
            }
        }

        private void tabControl1_DoubleClick(object sender, EventArgs e)
        {
            this.FullScreen = !this.fullScreen;
        }

        private void PopupTerminal_Load(object sender, EventArgs e)
        {
            this.timerHover = new Timer();
            this.timerHover.Interval = 200;
            this.timerHover.Tick += this.timerHover_Tick;
            this.timerHover.Start();

            this.closeTimer = new Timer();
            this.closeTimer.Interval = 500;
            this.closeTimer.Tick += this.closeTimer_Tick;
            this.closeTimer.Start();
        }

        private void closeTimer_Tick(object sender, EventArgs e)
        {
            lock (this.synLock)
            {
                this.closeTimer.Enabled = false;
                List<TabControlItem> removeableTabs = new List<TabControlItem>();
                foreach (TabControlItem tab in this.tabControl1.Items)
                {
                    if (tab.Controls.Count > 0)
                    {
                        if (!((ConnectionBase) tab.Controls[0]).Connected)
                        {
                            removeableTabs.Add(tab);
                        }
                    }
                }
                try
                {
                    foreach (TabControlItem tab in removeableTabs)
                    {
                        this.tabControl1.CloseTab(tab);
                        tab.Dispose();
                    }
                }
                catch (Exception exc)
                {
                    Log.Error("Error attempting to remove tab from window", exc);
                }
                this.closeTimer.Enabled = true;
            }
        }

        private void attachToTerminalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainTabsControler == null)
                return;

            TerminalTabControlItem activeTab = this.tabControl1.SelectedItem as TerminalTabControlItem;
            if (activeTab != null)
            {
                this.mainTabsControler.AttachTabFromWindow(activeTab);
                this.tabControl1.CloseTab(activeTab);
            }
        }

        private void CaptureToolStripButton_Click(object sender, EventArgs e)
        {
            this.CaptureScreen();
        }

        private void PopupTerminal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mainTabsControler != null)
                this.mainTabsControler.UnRegisterPopUp(this);
        }

        private void PopupTerminal_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F12)
            {
                this.CaptureScreen();
            }
        }
        #endregion
    }
}