namespace Terminals.Connections
{
    // .NET namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading;

    // Terminals and framework namespaces

    using Kohl.Framework.Logging;
    using Configuration.Files.Main.Favorites;

    // Aliases
    using Timer = System.Windows.Forms.Timer;

    public abstract class ExternalConnection : Connection.Connection
    {
        #region Private User32 Windows API functions (1)

        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hwnd, uint msg, int wParam, int lParam);

        #endregion

        #region Protected User32 Windows API functions (9)

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        protected static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("user32.dll")]
        protected static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        protected static extern IntPtr SendMessage(HandleRef hWnd, uint msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll")]
        protected static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPStr)] string lParam);

        [DllImport("user32.dll")]
        protected static extern IntPtr SendMessage(IntPtr hwnd, int msg, int wParam, StringBuilder sb);

        [DllImport("user32.dll")]
        protected static extern IntPtr SendMessage(HandleRef hWnd, uint msg, int wParam, StringBuilder sb);

        [DllImport("user32.dll", SetLastError = true)]
        protected static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

        [DllImport("user32.dll", SetLastError = false)]
        protected static extern IntPtr GetDlgItem(IntPtr hDlg, int nIdDlgItem);

        [DllImport("user32.dll")]
        protected static extern int BringWindowToTop(IntPtr hwnd);

        #endregion

        #region Private User32 Windows API constants (4)

        private const int GWL_STYLE = (-16);
        private const int WM_CLOSE = 0x10;
        private const int WS_MAXIMIZE = 0x01000000;
        private const int WS_VISIBLE =  0x10000000;

        #endregion

        #region Private User32 Windows API functions (3)

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        #endregion

        #region Private Delegates (2)

        private delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        private delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        #endregion

        #region Protected Enums (3)

        /// <summary>
        ///     Defines the algorithm on how to wait for the process to be ready for hWnd detection.
        /// </summary>
        protected enum SleepType
        {
            /// <summary>
            ///     Wait for the proccess to be idle.
            /// </summary>
            WaitForIdleInput,

            /// <summary>
            ///     Neither wait for the process nor sleep.
            /// </summary>
            NoSleep,

            /// <summary>
            ///     Sleeps the specified amout of milliseconds.
            /// </summary>
            MilliSeconds,

            /// <summary>
            ///     Waits for idle input plus waits for the specified amount of time in msecs.
            /// </summary>
            WaitForIdleInputSleep,

            /// <summary>
            ///     Waits for the process to guess the main hWnd.
            /// </summary>
            WaitForProcessMainWindowIdle,

            /// <summary>
            ///     Waits for the process to guess the main hWnd and sleeps
            /// </summary>
            WaitForProcessMainWindowIdleSleep
        }

        protected enum ShowWindowCommands
        {
            /// <summary>
            ///     Hides the window and activates another window.
            /// </summary>
            Hide = 0,

            /// <summary>
            ///     Activates and displays a window. If the window is minimized or
            ///     maximized, the system restores it to its original size and position.
            ///     An application should specify this flag when displaying the window
            ///     for the first time.
            /// </summary>
            Normal = 1,

            /// <summary>
            ///     Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2,

            /// <summary>
            ///     Maximizes the specified window.
            /// </summary>
            Maximize = 3, // is this the right value?
            /// <summary>
            ///     Activates the window and displays it as a maximized window.
            /// </summary>
            ShowMaximized = 3,

            /// <summary>
            ///     Displays a window in its most recent size and position. This value
            ///     is similar to <see cref="ShowWindowCommands.Normal" />, except
            ///     the window is not actived.
            /// </summary>
            ShowNoActivate = 4,

            /// <summary>
            ///     Activates the window and displays it in its current size and position.
            /// </summary>
            Show = 5,

            /// <summary>
            ///     Minimizes the specified window and activates the next top-level
            ///     window in the Z order.
            /// </summary>
            Minimize = 6,

            /// <summary>
            ///     Displays the window as a minimized window. This value is similar to
            ///     <see cref="ShowWindowCommands.ShowMinimized" />, except the
            ///     window is not activated.
            /// </summary>
            ShowMinNoActive = 7,

            /// <summary>
            ///     Displays the window in its current size and position. This value is
            ///     similar to <see cref="ShowWindowCommands.Show" />, except the
            ///     window is not activated.
            /// </summary>
            ShowNA = 8,

            /// <summary>
            ///     Activates and displays the window. If the window is minimized or
            ///     maximized, the system restores it to its original size and position.
            ///     An application should specify this flag when restoring a minimized window.
            /// </summary>
            Restore = 9,

            /// <summary>
            ///     Sets the show state based on the SW_* value specified in the
            ///     STARTUPINFO structure passed to the CreateProcess function by the
            ///     program that started the application.
            /// </summary>
            ShowDefault = 10,

            /// <summary>
            ///     <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
            ///     that owns the window is not responding. This flag should only be
            ///     used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimize = 11
        }

        protected enum WindowsMessages : uint
        {
            BM_CLICK = 0x00F5,
            WM_NULL = 0x00U,
            WM_CREATE = 0x01U,
            WM_DESTROY = 0x02U,
            WM_MOVE = 0x03U,
            WM_SIZE = 0x05U,
            WM_ACTIVATE = 0x06U,
            WM_SETFOCUS = 0x07U,
            WM_KILLFOCUS = 0x08U,
            WM_ENABLE = 0x0AU,
            WM_SETREDRAW = 0x0BU,
            WM_SETTEXT = 0x0CU,
            WM_GETTEXT = 0x0DU,
            WM_GETTEXTLENGTH = 0x0EU,
            WM_PAINT = 0x0FU,
            WM_CLOSE = 0x10U,
            WM_QUERYENDSESSION = 0x11U,
            WM_QUIT = 0x12U,
            WM_QUERYOPEN = 0x13U,
            WM_ERASEBKGND = 0x14U,
            WM_SYSCOLORCHANGE = 0x15U,
            WM_ENDSESSION = 0x16U,
            WM_SYSTEMERROR = 0x17U,
            WM_SHOWWINDOW = 0x18U,
            WM_CTLCOLOR = 0x19U,
            WM_WININICHANGE = 0x1AU,
            WM_SETTINGCHANGE = 0x1AU,
            WM_DEVMODECHANGE = 0x1BU,
            WM_ACTIVATEAPP = 0x1CU,
            WM_FONTCHANGE = 0x1DU,
            WM_TIMECHANGE = 0x1EU,
            WM_CANCELMODE = 0x1FU,
            WM_SETCURSOR = 0x20U,
            WM_MOUSEACTIVATE = 0x21U,
            WM_CHILDACTIVATE = 0x22U,
            WM_QUEUESYNC = 0x23U,
            WM_GETMINMAXINFO = 0x24U,
            WM_PAINTICON = 0x26U,
            WM_ICONERASEBKGND = 0x27U,
            WM_NEXTDLGCTL = 0x28U,
            WM_SPOOLERSTATUS = 0x2AU,
            WM_DRAWITEM = 0x2BU,
            WM_MEASUREITEM = 0x2CU,
            WM_DELETEITEM = 0x2DU,
            WM_VKEYTOITEM = 0x2EU,
            WM_CHARTOITEM = 0x2FU,

            WM_SETFONT = 0x30U,
            WM_GETFONT = 0x31U,
            WM_SETHOTKEY = 0x32U,
            WM_GETHOTKEY = 0x33U,
            WM_QUERYDRAGICON = 0x37U,
            WM_COMPAREITEM = 0x39U,
            WM_COMPACTING = 0x41U,
            WM_WINDOWPOSCHANGING = 0x46U,
            WM_WINDOWPOSCHANGED = 0x47U,
            WM_POWER = 0x48U,
            WM_COPYDATA = 0x4AU,
            WM_CANCELJOURNAL = 0x4BU,
            WM_NOTIFY = 0x4EU,
            WM_INPUTLANGCHANGEREQUEST = 0x50U,
            WM_INPUTLANGCHANGE = 0x51U,
            WM_TCARD = 0x52U,
            WM_HELP = 0x53U,
            WM_USERCHANGED = 0x54U,
            WM_NOTIFYFORMAT = 0x55U,
            WM_CONTEXTMENU = 0x7BU,
            WM_STYLECHANGING = 0x7CU,
            WM_STYLECHANGED = 0x7DU,
            WM_DISPLAYCHANGE = 0x7EU,
            WM_GETICON = 0x7FU,
            WM_SETICON = 0x80U,

            WM_NCCREATE = 0x81U,
            WM_NCDESTROY = 0x82U,
            WM_NCCALCSIZE = 0x83U,
            WM_NCHITTEST = 0x84U,
            WM_NCPAINT = 0x85U,
            WM_NCACTIVATE = 0x86U,
            WM_GETDLGCODE = 0x87U,
            WM_NCMOUSEMOVE = 0xA0U,
            WM_NCLBUTTONDOWN = 0xA1U,
            WM_NCLBUTTONUP = 0xA2U,
            WM_NCLBUTTONDBLCLK = 0xA3U,
            WM_NCRBUTTONDOWN = 0xA4U,
            WM_NCRBUTTONUP = 0xA5U,
            WM_NCRBUTTONDBLCLK = 0xA6U,
            WM_NCMBUTTONDOWN = 0xA7U,
            WM_NCMBUTTONUP = 0xA8U,
            WM_NCMBUTTONDBLCLK = 0xA9U,

            WM_KEYFIRST = 0x100U,
            WM_KEYDOWN = 0x100U,
            WM_KEYUP = 0x101U,
            WM_CHAR = 0x102U,
            WM_DEADCHAR = 0x103U,
            WM_SYSKEYDOWN = 0x104U,
            WM_SYSKEYUP = 0x105U,
            WM_SYSCHAR = 0x106U,
            WM_SYSDEADCHAR = 0x107U,
            WM_KEYLAST = 0x108U,

            WM_IME_STARTCOMPOSITION = 0x10DU,
            WM_IME_ENDCOMPOSITION = 0x10EU,
            WM_IME_COMPOSITION = 0x10FU,
            WM_IME_KEYLAST = 0x10FU,

            WM_INITDIALOG = 0x110U,
            WM_COMMAND = 0x111U,
            WM_SYSCOMMAND = 0x112U,
            WM_TIMER = 0x113U,
            WM_HSCROLL = 0x114U,
            WM_VSCROLL = 0x115U,
            WM_INITMENU = 0x116U,
            WM_INITMENUPOPUP = 0x117U,
            WM_MENUSELECT = 0x11FU,
            WM_MENUCHAR = 0x120U,
            WM_ENTERIDLE = 0x121U,

            WM_CTLCOLORMSGBOX = 0x132U,
            WM_CTLCOLOREDIT = 0x133U,
            WM_CTLCOLORLISTBOX = 0x134U,
            WM_CTLCOLORBTN = 0x135U,
            WM_CTLCOLORDLG = 0x136U,
            WM_CTLCOLORSCROLLBAR = 0x137U,
            WM_CTLCOLORSTATIC = 0x138U,

            WM_MOUSEFIRST = 0x200U,
            WM_MOUSEMOVE = 0x200U,
            WM_LBUTTONDOWN = 0x201U,
            WM_LBUTTONUP = 0x202U,
            WM_LBUTTONDBLCLK = 0x203U,
            WM_RBUTTONDOWN = 0x204U,
            WM_RBUTTONUP = 0x205U,
            WM_RBUTTONDBLCLK = 0x206U,
            WM_MBUTTONDOWN = 0x207U,
            WM_MBUTTONUP = 0x208U,
            WM_MBUTTONDBLCLK = 0x209U,
            WM_MOUSEWHEEL = 0x20AU,
            WM_MOUSEHWHEEL = 0x20EU,

            WM_PARENTNOTIFY = 0x210U,
            WM_ENTERMENULOOP = 0x211U,
            WM_EXITMENULOOP = 0x212U,
            WM_NEXTMENU = 0x213U,
            WM_SIZING = 0x214U,
            WM_CAPTURECHANGED = 0x215U,
            WM_MOVING = 0x216U,
            WM_POWERBROADCAST = 0x218U,
            WM_DEVICECHANGE = 0x219U,

            WM_MDICREATE = 0x220U,
            WM_MDIDESTROY = 0x221U,
            WM_MDIACTIVATE = 0x222U,
            WM_MDIRESTORE = 0x223U,
            WM_MDINEXT = 0x224U,
            WM_MDIMAXIMIZE = 0x225U,
            WM_MDITILE = 0x226U,
            WM_MDICASCADE = 0x227U,
            WM_MDIICONARRANGE = 0x228U,
            WM_MDIGETACTIVE = 0x229U,
            WM_MDISETMENU = 0x230U,
            WM_ENTERSIZEMOVE = 0x231U,
            WM_EXITSIZEMOVE = 0x232U,
            WM_DROPFILES = 0x233U,
            WM_MDIREFRESHMENU = 0x234U,

            WM_IME_SETCONTEXT = 0x281U,
            WM_IME_NOTIFY = 0x282U,
            WM_IME_CONTROL = 0x283U,
            WM_IME_COMPOSITIONFULL = 0x284U,
            WM_IME_SELECT = 0x285U,
            WM_IME_CHAR = 0x286U,
            WM_IME_KEYDOWN = 0x290U,
            WM_IME_KEYUP = 0x291U,

            WM_MOUSEHOVER = 0x2A1U,
            WM_NCMOUSELEAVE = 0x2A2U,
            WM_MOUSELEAVE = 0x2A3U,

            WM_CUT = 0x300U,
            WM_COPY = 0x301U,
            WM_PASTE = 0x302U,
            WM_CLEAR = 0x303U,
            WM_UNDO = 0x304U,

            WM_RENDERFORMAT = 0x305U,
            WM_RENDERALLFORMATS = 0x306U,
            WM_DESTROYCLIPBOARD = 0x307U,
            WM_DRAWCLIPBOARD = 0x308U,
            WM_PAINTCLIPBOARD = 0x309U,
            WM_VSCROLLCLIPBOARD = 0x30AU,
            WM_SIZECLIPBOARD = 0x30BU,
            WM_ASKCBFORMATNAME = 0x30CU,
            WM_CHANGECBCHAIN = 0x30DU,
            WM_HSCROLLCLIPBOARD = 0x30EU,
            WM_QUERYNEWPALETTE = 0x30FU,
            WM_PALETTEISCHANGING = 0x310U,
            WM_PALETTECHANGED = 0x311U,

            WM_HOTKEY = 0x312U,
            WM_PRINT = 0x317U,
            WM_PRINTCLIENT = 0x318U,

            WM_HANDHELDFIRST = 0x358U,
            WM_HANDHELDLAST = 0x35FU,
            WM_PENWINFIRST = 0x380U,
            WM_PENWINLAST = 0x38FU,
            WM_COALESCE_FIRST = 0x390U,
            WM_COALESCE_LAST = 0x39FU,
            WM_DDE_FIRST = 0x3E0U,
            WM_DDE_INITIATE = 0x3E0U,
            WM_DDE_TERMINATE = 0x3E1U,
            WM_DDE_ADVISE = 0x3E2U,
            WM_DDE_UNADVISE = 0x3E3U,
            WM_DDE_ACK = 0x3E4U,
            WM_DDE_DATA = 0x3E5U,
            WM_DDE_REQUEST = 0x3E6U,
            WM_DDE_POKE = 0x3E7U,
            WM_DDE_EXECUTE = 0x3E8U,
            WM_DDE_LAST = 0x3E8U,

            WM_USER = 0x400U,
            WM_APP = 0x8000U
        }

        #endregion

        #region Private Enums (1)

        private enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        #endregion

        #region Protected Delegates (1)

        protected delegate void Procedure();

        #endregion

        #region Private Fields (7)

        private static readonly object EmbedWindowLock = new object();
        protected readonly Process process = new Process();
        private readonly ProcessStartInfo processStartInfo = new ProcessStartInfo();
        private bool connected;
        private bool disconnecting;
        private IntPtr hWndRedrawWindow;
        private Timer timer;

        #endregion

        #region Protected Fields (2)

        protected IntPtr HWnd = IntPtr.Zero;
        protected Procedure RedrawExternalWindow;

        #endregion

        #region Protected Properties (1)

        protected IntPtr HWndRedrawWindow
        {
            private get
            {
                if (this.hWndRedrawWindow == IntPtr.Zero)
                {
                    return this.HWnd;
                }

                return this.hWndRedrawWindow;
            }
            set { this.hWndRedrawWindow = value; }
        }

        #endregion

        #region Public Abstract Properties (4)

        protected abstract string ProgramPath { get; }
        protected abstract string Arguments { get; }
        protected abstract string WorkingDirectory { get; }
        protected abstract bool UseShellExecute { get; }

        #endregion

        #region Public Virtual Properties (6)

        protected virtual string RunAsUser
        {
            get { return string.Empty; }
        }

        protected virtual string RunAsDomain
        {
            get { return string.Empty; }
        }

        protected virtual string RunAsPassword
        {
            get { return string.Empty; }
        }

        protected virtual int Sleep
        {
            get { return 0; }
        }

        protected virtual SleepType SleepMethod
        {
            get { return SleepType.WaitForIdleInput; }
        }

        protected virtual bool EnableRedrawExternalWindow
        {
            get { return true; }
        }

        #endregion

        #region Public Override Properties (1)

        public override bool Connected
        {
            get { return this.connected; }
        }

        #endregion

        #region Private DLL Imports (4)

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);

        [DllImport("user32.dll")]
        private static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        #endregion

        #region Protected Methods (6)

        /// <summary>
        ///     Used to embed connections that derive from <see cref="ExternalConnection" />.
        /// </summary>
        /// <param name="hWnd"> The control to be embedded in the tab page. </param>
        protected Panel EmbedWindow(IntPtr hWnd)
        {
            return this.EmbedWindow(hWnd, this.TerminalTabPage);
        }

        protected static IntPtr GetDialog(IntPtr parentHWnd, string className, string windowTitle)
        {
            return GetDialog(parentHWnd, className, windowTitle, true, -1);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            this.ResizeClient();
            base.OnSizeChanged(e);
        }

        protected override void OnResize(EventArgs e)
        {
            this.ResizeClient();
            base.OnResize(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            // Stop the application
            if (this.HWnd != IntPtr.Zero)
            {
                // Post a colse message
                PostMessage(this.HWnd, WM_CLOSE, 0, 0);

                // Delay for it to get the message
                Thread.Sleep(1000);

                // Clear public handle
                this.HWnd = IntPtr.Zero;
            }

            base.OnHandleDestroyed(e);
        }

        protected virtual void PerformPostAction(Process process)
        {
        }

        #endregion

        #region Private Methods (10)

        private void ResizeClient()
        {
            // Resize the application
            if (this.HWnd != IntPtr.Zero)
            {
                MoveWindow(this.HWnd, 0, 0, this.TerminalTabPage.Size.Width, this.TerminalTabPage.Size.Height, true);
            }
        }

        private static IntPtr GetDialog(IntPtr parentHWnd, string className, IntPtr windowTitle, bool loop = true, int attempts = 1000)
        {
            IntPtr hDialog = IntPtr.Zero;

            int sleep = 0;

            do
            {
                hDialog = FindWindowEx(parentHWnd, hDialog, className, windowTitle);
                Thread.Sleep(1);
                sleep++;

                if (loop)
                {
                    if (hDialog != IntPtr.Zero || sleep == attempts)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            } while (true);

            return hDialog;
        }

        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                EnumWindowProc childProc = EnumWindow;
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        /// <summary>
        ///     Used to embed connections that derive from <see cref="ExternalConnection" />.
        /// </summary>
        /// <param name="hWnd"> The control to be embedded in the tab page. </param>
        /// <param name="terminalTabPage">The terminal tab page that will be the parent of the external hWnd.</param>
        private Panel EmbedWindow(IntPtr hWnd, Control terminalTabPage)
        {
            lock (EmbedWindowLock)
            {
                terminalTabPage.InvokeIfNecessary(() => terminalTabPage.Controls.Clear());

                Panel panel = null;

                terminalTabPage.InvokeIfNecessary(() =>
                {
                    panel = new Panel
                    {
                        Size =
                            new Size(terminalTabPage.Size.Width,
                                    terminalTabPage.Size.Height)
                    };
                    panel.BringToFront();
                    SetParent(hWnd, panel.Handle);
                });

                SetWindowLong(hWnd, GWL_STYLE, WS_VISIBLE + WS_MAXIMIZE);
                MoveWindow(hWnd, 0, 0, terminalTabPage.Size.Width, terminalTabPage.Size.Height, true);

                terminalTabPage.InvokeIfNecessary(() => terminalTabPage.Controls.Add(panel));

                return panel;
            }
        }

        private static List<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            List<IntPtr> handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                                                 {
                                                     handles.Add(hWnd);
                                                     return true;
                                                 }, IntPtr.Zero);

            return handles;
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
                throw new InvalidCastException("GCHandle target could not be cast to List&lt;IntPtr&gt;.");

            list.Add(handle);
            return true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Check if the process has being destroyed
                // the process.HasExited Property is not reliable
                // In the case of VMWare ThinApp/Thinstall wrong results are reported.
                bool destroyed = false;

                // If the connection method has successfully opened an EXE application
                if (this.connected)
                {
                    List<IntPtr> list = GetChildWindows(this.TerminalTabPage.Controls[0].Handle);

                    // If our panel doesn't contain any subhandles (i.e. no windows) ->
                    // This is an indicator that the application has been destroyed
                    // e.g. by taskkill /F /IM RAdmin.exe
                    if (!list.Any())
                        destroyed = true;
                }

                if (destroyed || this.disconnecting)
                {
                    if (this.timer != null)
                        this.timer.Stop();

                    if (!this.disconnecting)
                        this.Disconnect();
                }
                else
                {
                    // Only enable redrawing if we want to
                    if (this.EnableRedrawExternalWindow)
                    {
                        this.RedrawExternalWindow();
                    }
                }
            }
            catch
            {
                if (this.timer != null)
                    this.timer.Stop();
            }
        }

        /// <summary>
        ///     Stops the main thread for the specified amount of milliseconds.
        /// </summary>
        private void ProcessSleep()
        {
            if (this.Sleep > 0 || this.Sleep < Int32.MaxValue)
            {
                Application.DoEvents();
                Thread.Sleep(this.Sleep);
                Application.DoEvents();
            }
        }

        /// <summary>
        ///     Custom method to detect the main window handle.
        /// </summary>
        /// <param name="process"> </param>
        /// <returns> </returns>
        private bool ProcessWaitForProcessMainWindowIdle(Process process)
        {
            int counter = 0;
            int times = 0;
            do
            {
                if (process.MainWindowHandle != IntPtr.Zero || this.HWnd != IntPtr.Zero)
                    break;

                process.Refresh();

                // FALLBACK
                // Time out reached, Windows can't guess the main window
                // ->
                // The value you get out of Process.MainWindowHandle is unfortunately a guess.
                // There is no API function available to a program that lets it tell Windows
                // "this is my main window". The rule it uses is documented, it is the first
                // window that's created by a process when it gets started. That causes trouble
                // if that first window is, say, a login window or a splash screen.
                // Not much you can do about this, you have to know more about how the program
                // behaves to find that real main window. Enumerating windows with EnumThreadWindows()
                // could help you find it, as long as the first window was created on the same thread
                // as the main window. A more elaborate EnumWindows() will be necessary if that is not
                // the case.
                if (times == 100)
                {
                    bool found = true;
                    counter = 0;
                    for (int i = 0; i < 5 && this.HWnd == IntPtr.Zero; i++)
                    {
                        List<IntPtr> handles = EnumerateProcessWindowHandles(process.Id);

                        foreach (IntPtr hWnd in handles)
                        {
                            StringBuilder message = new StringBuilder(1000);
                            SendMessage(hWnd, (int)WindowsMessages.WM_GETTEXT, message.Capacity, message);

                            // Check for a main window
                            if (this.IsMainWindow(hWnd) && found)
                            {
                                this.HWnd = hWnd;
                                break;
                            }
                            found = false;

                            // Fallback there seems to be no main hWnd -> take the first visible one that has a caption
                            if (!found)
                            {
                                if (!string.IsNullOrEmpty(message.ToString()) && hWnd != IntPtr.Zero &&
                                    IsWindowVisible(hWnd))
                                {
                                    this.HWnd = hWnd;
                                    break;
                                }
                            }
                        }

                        // Nothing found yet -> fallback failed ->
                        // Try again 4 times (i.e. 5 times in total)
                        // Afterwards abort with error.
                        for (int j = 0; j < 30; j++)
                        {
                            Thread.Sleep(5);
                            Application.DoEvents();
                            Thread.Sleep(5);
                        }
                    }

                    if (this.HWnd == IntPtr.Zero)
                    {
                        Log.Error("The process' main window hasn't been found. Therefore Terminals was not able to create a connection.");
                        return this.connected = false;
                    }
                }

                if (counter == 10)
                {
                    // Prevent a long running external app from shooting the app's main process thread.
                    times++;
                    counter = 0;
                    Application.DoEvents();
                }
                else
                {
                    Thread.Sleep(3);
                }

                counter++;
            } while (true);

            return true;
        }

        /// <summary>
        ///     Same method that uses .NET to detect the main window handle.
        /// </summary>
        /// <param name="handle"> </param>
        /// <returns> </returns>
        private bool IsMainWindow(IntPtr handle)
        {
            return (!(GetWindow(handle, GetWindow_Cmd.GW_OWNER) != IntPtr.Zero) && IsWindowVisible(handle));
        }

        #endregion

        #region Public Methods (5)

        public bool EmbedWindow()
        {
            // Embed the application
            if (this.HWnd != IntPtr.Zero)
            {
                this.EmbedWindow(this.HWnd);
                return true;
            }
            return false;
        }

        public override bool Connect()
        {
            this.connected = false;

            try
            {
                this.disconnecting = false;

                // If we want to run the program as a differnt user
                if (!string.IsNullOrEmpty(this.RunAsUser) && !string.IsNullOrEmpty(this.RunAsPassword))
                {
                    // Only use the domain if there is one
                    if (!string.IsNullOrEmpty(this.RunAsDomain))
                    {
                        this.processStartInfo.Domain = this.RunAsDomain;
                    }

                    this.processStartInfo.UserName = this.RunAsUser;

                    // Create a secure string for the runas password
                    SecureString sec = new SecureString();

                    foreach (char character in this.RunAsPassword)
                    {
                        sec.AppendChar(character);
                    }

                    this.processStartInfo.Password = sec;
                }

                if (string.IsNullOrEmpty(this.ProgramPath) || (this.UseShellExecute && !File.Exists(this.ProgramPath)))
                {
                    Log.Error("Please configure the path for the file to be opened (e.g. word document) or executable to be run.");
                    return false;
                }

                this.processStartInfo.FileName = this.ProgramPath;
                this.processStartInfo.Arguments = this.Arguments;
                this.processStartInfo.WorkingDirectory = string.IsNullOrEmpty(this.WorkingDirectory)
                                                             ? new FileInfo(this.ProgramPath).Directory.FullName
                                                             : this.WorkingDirectory;
                this.processStartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                this.processStartInfo.UseShellExecute = this.UseShellExecute;
                this.processStartInfo.CreateNoWindow = false;

                // Only overwrite the delegate if it hasn't been defined i.e. if NULL
                if (this.RedrawExternalWindow == null)
                {
                    this.RedrawExternalWindow = delegate
                                                    {
                                                        if (this.HWndRedrawWindow != IntPtr.Zero)
                                                        {
                                                            /*
                                                            RedrawWindow(HWndRedrawWindow, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.RDW_FRAME | RedrawWindowFlags.RDW_UPDATENOW | RedrawWindowFlags.RDW_INVALIDATE);
                                                            */
                                                            SetWindowLong(this.HWndRedrawWindow, GWL_STYLE, WS_VISIBLE + WS_MAXIMIZE);
                                                            Application.DoEvents();
                                                        }
                                                    };
                }

                this.process.StartInfo = this.processStartInfo;
                this.process.Start();

                switch (this.SleepMethod)
                {
                    case SleepType.WaitForIdleInputSleep:
                        this.process.WaitForInputIdle();
                        this.ProcessSleep();
                        break;

                    case SleepType.WaitForIdleInput:
                        this.process.WaitForInputIdle();
                        break;

                    case SleepType.MilliSeconds:
                        this.ProcessSleep();
                        break;
                    case SleepType.WaitForProcessMainWindowIdleSleep:
                        this.ProcessSleep();
                        if (!this.ProcessWaitForProcessMainWindowIdle(this.process))
                            return false;
                        break;
                    case SleepType.WaitForProcessMainWindowIdle:
                        if (!this.ProcessWaitForProcessMainWindowIdle(this.process))
                            return false;
                        break;
                    default:
                        // not needed ... don't sleep
                        break;
                }

                if (this.process.MainWindowHandle != IntPtr.Zero)
                {
                    this.HWnd = this.process.MainWindowHandle;
                }

                this.EmbedWindow(this.HWnd).BringToFront();
                this.BringToFront();

                this.PerformPostAction(this.process);

                // That doesn't work with VMWare ThinApp/Thinstall packages like RAdmin.
                //    process.Exited += new EventHandler(Process_Exited);
                //    process.Disposed += new EventHandler(Process_Disposed);
                // -> The workaround is to enumerate the client hWnds of the panel and check for their existance.
                // Yet another problem -> the Timer class needs to run on the applications main thread!

                this.InvokeIfNecessary(() =>
                {
                    this.timer = new Timer { Enabled = true, Interval = 230 };
                    this.timer.Tick += this.Timer_Tick;
                    this.timer.Start();
                });
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("The following error occured while trying to create the {0} connection.", this.Favorite.Protocol), ex);
                return this.connected = false;
            }

            return this.connected = true;
        }

        public override void Disconnect()
        {
            try
            {
                this.disconnecting = true;
                this.connected = false;

                if (this.process != null && !this.process.HasExited)
                {
                    this.process.Kill();
                }
                else
                {
                    // If there is a main HWnd use this to close the window
                    if (this.HWnd != IntPtr.Zero)
                    {
                        // close the window using API
                        SendMessage(this.HWnd, 0x0112 /* WM_SYSCOMMAND */, 0xF060 /* SC_CLOSE */, null);
                    }
                    // If not try to enumerate the panel and check if there are client hWnds alive if so close them
                    // This is a workaround for VMWare ThinApp/Thinstall packages like RAdmin.
                    else
                    {
                        List<IntPtr> list = GetChildWindows(this.TerminalTabPage.Controls[0].Handle);
                        foreach (IntPtr ptr in list)
                        {
                            // close the window using API
                            SendMessage(ptr, 0x0112 /* WM_SYSCOMMAND */, 0xF060 /* SC_CLOSE */, null);
                        }
                    }
                }

                this.CloseTabPage();
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Unable to disconnect form the {0} connection named \"{1}\".", this.Favorite.Protocol, this.Favorite.Name), ex);
            }
        }

        protected override void ChangeDesktopSize(DesktopSize desktopSize, System.Drawing.Size size)
        {
            this.ResizeClient();
        }

        public static IntPtr GetDialog(IntPtr parentHWnd, string className, string windowTitle, bool loop = true, int attempts = 1000)
        {
            IntPtr ptr = Marshal.StringToHGlobalAnsi(windowTitle);
            IntPtr hDialog = GetDialog(parentHWnd, className, ptr, loop, attempts);
            Marshal.FreeHGlobal(ptr);
            return hDialog;
        }

        #endregion
    }
}