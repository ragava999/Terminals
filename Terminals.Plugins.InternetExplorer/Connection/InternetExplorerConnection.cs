namespace Terminals.Plugins.InternetExplorer.Connection
{
    // .NET namespaces
    using System;
    using System.Drawing;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    // Terminals namespaces
    using Configuration.Files.Main.Favorites;
    using MainSettings = Configuration.Files.Main.Settings;
    
    using Kohl.Framework.Info;
    using Panels.FavoritePanels;

    public partial class InternetExplorerConnection : Terminals.Connection.Connection
    {
        private bool connected = false;

        public override bool IsPortRequired
        {
            get
            {
                return false;
            }
        }

        protected override Image[] images
        {
            get { return new Image[] { Properties.Resources.InternetExplorer }; }
        }

        public InternetExplorerConnection()
        {
            InitializeComponent();
        }

        public override bool Connected
        {
            get { return connected; }
        }

        #region Private User32 Windows API constants (4)

        private const int GWL_STYLE = (-16);
        private const int WM_CLOSE = 0x10;
        private const int WS_MAXIMIZE = 0x01000000;
        private const int WS_VISIBLE = 0x10000000;

        #endregion

        #region Private User32 Windows API functions (3)

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        #endregion

        private static readonly object EmbedWindowLock = new object();

        private Panel Embed(IntPtr hWnd)
        {
            lock (EmbedWindowLock)
            {
                Panel panel = null;

                TerminalTabPage.InvokeIfNecessary(() => TerminalTabPage.Controls.Clear());              

                TerminalTabPage.InvokeIfNecessary(() =>
                {
                    panel = new Panel
                    {
                        Size =
                            new Size(TerminalTabPage.Size.Width,
                                    TerminalTabPage.Size.Height)
                    };
                    panel.BringToFront();
                    SetParent(hWnd, panel.Handle);
                });

                SetWindowLong(hWnd, GWL_STYLE, WS_VISIBLE + WS_MAXIMIZE);
                MoveWindow(hWnd, 0, 0, TerminalTabPage.Size.Width, TerminalTabPage.Size.Height, true);

                TerminalTabPage.InvokeIfNecessary(() => TerminalTabPage.Controls.Add(panel));
                return panel;
            }
        }

        public override bool Connect()
        {
            try
            {
                Favorite.InternetExplorerUrl("http://wwww.kohl.bz");

                Process process = new Process();
                process.StartInfo.Arguments = "-k -noframemerging " + Favorite.InternetExplorerUrl();
                process.StartInfo.FileName = @"C:\Program Files (x86)\Internet Explorer\iexplore.exe";
                process.Start();
                process.WaitForInputIdle();
                IntPtr ptr = process.MainWindowHandle;

                Embed(ptr);

                return connected = true;
            }
            catch
            {
                return connected = false;
            }
        }

        public override void Disconnect()
        {

            this.CloseTabPage();
            connected = false;
        }

        protected override void ChangeDesktopSize(DesktopSize size, System.Drawing.Size siz)
        {
            
        }
    }
}