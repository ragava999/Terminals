using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.TabControl;

namespace Terminals.Connection.ScreenCapture
{
    public class CaptureManagerConnection : ConnectionBase
    {
        #region Private Fields (2)
        private bool connected;
        private CaptureManagerLayout layout;
        #endregion

        #region Public Methods (6)
        public CaptureManagerConnection()
        {
            this.InitializeComponent();
        }

        public override bool Connected
        {
            get { return this.connected; }
        }

        public void RefreshView()
        {
            this.layout.RefreshView();
        }

        public override bool Connect()
        {
            this.layout.Parent = this.TerminalTabPage;
            this.Parent = this.TerminalTabPage;
            this.TerminalTabPage.Connection = this;
            return true;
        }

        public override void Disconnect()
        {
            try
            {
                this.connected = false;
            }
            catch (Exception ex)
            {
                Log.Error("Error disconnecting from the capture manager.", ex);
            }
        }

        public static void PerformScreenCapture(TabControl.TabControl tab)
        {
            TerminalTabControlItem activeTab = tab.SelectedItem as TerminalTabControlItem;
            string name = "";
            if (activeTab != null && activeTab.Favorite != null && !string.IsNullOrEmpty(activeTab.Favorite.Name))
            {
                name = activeTab.Favorite.Name + "-";
            }
            string filename = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");

            string rootPath = Settings.CaptureRoot.NormalizePath();

            string tempFile = Path.Combine(rootPath, string.Format("{0}{1}.png", name, filename));
            ScreenCapture sc = new ScreenCapture();
            Bitmap bmp = sc.CaptureControl(tab, tempFile, ImageFormatTypes.imgPNG);

            if (Settings.EnableCaptureToClipboard)
                Clipboard.SetImage(bmp);
        }
        #endregion

        #region Protected Methods (1)
        protected override void ChangeDesktopSize(DesktopSize desktopSize, System.Drawing.Size size)
        {

        }
        #endregion

        #region Private Methods (1)
        private void InitializeComponent()
        {
            this.layout = new CaptureManagerLayout();
            this.SuspendLayout();
            // 
            // networkingToolsLayout1
            // 
            this.layout.Location = new Point(0, 0);
            this.layout.Name = "layout1";
            this.layout.Size = new Size(700, 500);
            this.layout.TabIndex = 0;
            this.layout.Dock = DockStyle.Fill;
            this.ResumeLayout(false);
        }
        #endregion
    }
}