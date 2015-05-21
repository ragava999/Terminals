namespace Terminals.Connection
{
    // .NET namespaces
    using System;
    using System.ComponentModel;
	using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    // Terminals and framework namespaces
    using Configuration.Files.Main.Favorites;
    using TabControl;
    using Terminals.Connection.Panels.OptionPanels;

    public abstract class ConnectionBase : Control
    {
		public ConnectionBase()
		{
            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
				System.AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            }
		}
		
		private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, System.ResolveEventArgs args)
		{
			return DependencyResolver.ResolveAssembly(sender, args);
	    }
		
        #region Public Properties (5)
        public FavoriteConfigurationElement Favorite { get; set; }

        public TerminalTabControlItem TerminalTabPage { get; set; }

        public IHostingForm ParentForm { get; set; }

        public Size MaxSize
        {
            get
            {
                return GetMaxSize(this);
            }
        }
        #endregion

        #region Public Abstract Properties (1)
        public abstract bool Connected { get; }
        #endregion

        #region Public Abstract Methods (2)
        public abstract bool Connect();
        public abstract void Disconnect();
        #endregion

        public void AfterConnectPlugins()
        {
            // Get all types that implement the IAfterConnectSupport interface
            var types = Manager.ConnectionManager.GetIAfterConnectSupportTypes();

            // Filter the current active type out.
            // e.g.
            // HTTP connection might have an AutoIt connection afterwards
            // AutoIt connection can't have AutoIt afterwards -> avoiding recursion
            types = (from type in types
                     where this.GetType().FullName != type.FullName
                     select type).ToList<Type>();

            foreach (Type type in types)
            {
                FavoriteConfigurationElement favoriteCopy = (FavoriteConfigurationElement)Favorite.Clone();

                Connection conn = Manager.ConnectionManager.GetConnection(Manager.ConnectionManager.GetProtocolName(type));
                ((IAfterConnectSupport)conn).IsInAfterConnectMode = true;
                conn.Favorite = favoriteCopy;
                conn.TerminalTabPage = this.TerminalTabPage;

                if (((IAfterConnectSupport)conn).IsAfterConnectEnabled)
                    Manager.ConnectionManager.CreateConnection(Favorite, this.ParentForm, false, TerminalTabPage, conn);
            }
        }

        #region Public Fields (1)
        public const Int32 WM_LEAVING_FULLSCREEN = 0x4ff;
        #endregion

        #region Private Delegates (1)
        private delegate void InvokeCloseTabPage(TabControlItem tabPage, bool checkNullException = true);
        #endregion

        #region Public Methods (5)
        public void ChangeDesktopSize(System.Drawing.Size? size = null, DesktopSize? desktopSize = null)
        {
            if (this.Favorite == null && !desktopSize.HasValue)
                return;

            if (!(size.HasValue && desktopSize.HasValue))
            {
                if (desktopSize.HasValue)
                    if (size.HasValue)
                        size = GetSize(this, desktopSize.Value, size.Value.Width, size.Value.Height);
                    else
                        size = GetSize(this, desktopSize.Value, this.Favorite.DesktopSizeWidth, this.Favorite.DesktopSizeHeight);
                else
                    if (size == null)
                        size = GetSize();

                if (size == null)
                    return;
            }

            this.ChangeDesktopSize(desktopSize.HasValue ? desktopSize.Value : this.Favorite.DesktopSize, size.Value);
        }

        public static Size GetSize(ConnectionBase connection, FavoriteConfigurationElement favorite)
        {
            switch (favorite.DesktopSize)
            {
                case DesktopSize.FullScreen:
                    return GetMaxSize(connection);
                case DesktopSize.FitToWindow:
                case DesktopSize.AutoScale:
                    return new Size(connection.TerminalTabPage.Width, connection.TerminalTabPage.Height);
                default:
                    return new Size(favorite.DesktopSizeWidth, favorite.DesktopSizeHeight);
            }
        }

        public static Size GetMaxSize(Control connection)
        {
            if (connection.InvokeRequired)
            {
                Size size = new Size();
                connection.Invoke(new MethodInvoker(delegate { size = new Size(Screen.FromControl(connection).Bounds.Width - 13, Screen.FromControl(connection).Bounds.Height - 1); }));
                return size;
            }
            else
                return new Size(Screen.FromControl(connection).Bounds.Width - 13, Screen.FromControl(connection).Bounds.Height - 1);
        }

        public Size GetSize()
        {
            return GetSize(this, this.Favorite);
        }

        public static Size GetSize(ConnectionBase connection, DesktopSize desktopSize, int desktopSizeWidth, int desktopSizeHeight)
        {
            return GetSize(connection, new FavoriteConfigurationElement { DesktopSize = desktopSize, DesktopSizeWidth = desktopSizeWidth, DesktopSizeHeight = desktopSizeHeight });
        }
        #endregion

        #region Protected Methods (2)
        protected void InvokeIfNecessary(MethodInvoker code)
        {
            ((Control)ParentForm).InvokeIfNecessary(code);
        }

        protected void CloseTabPage(bool checkNullException = true)
        {
            CloseTabPage(this.TerminalTabPage, checkNullException);
        }
        #endregion

        #region Protected User32 Windows API functions (1)
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        protected static extern int PostMessage(HandleRef hwnd, int msg, IntPtr wparam, IntPtr lparam);
        #endregion

        #region Protected Abstract Methods (1)
        protected abstract void ChangeDesktopSize(DesktopSize desktopSize, System.Drawing.Size size);
        #endregion

        #region Private Methods (1)
        private void CloseTabPage(object tabObject, bool checkNullException = true)
        {
            // If the handle has been disposed -> the control is non existent
            // -> we don't need to close it again -> Exception will be thrown -> return.
            try
            {
                // The TabControlItem is of base type Panel.
                // If the control is no panel return.
                if (!(tabObject is Panel))
                    return;

                // The following check is not needed in case of the AutoIt connection.
                if (checkNullException)
                    // Check for a valid and existing handle -> if Zero or exception return.
                    // otherwise continue.
                    if (((Panel)tabObject).Handle == IntPtr.Zero)
                        return;
            }
            catch
            {
                return;
            }

            if (!(tabObject is TabControlItem))
                return;

            if (this.ParentForm.InvokeRequired)
            {
                InvokeCloseTabPage d = this.CloseTabPage;
                this.TerminalTabPage.Invoke(d, new object[] { this.TerminalTabPage, checkNullException });
                return;
            }

            TabControlItem tabPage = (TabControlItem)tabObject;

            bool wasSelected = tabPage.Selected;
            this.ParentForm.RemoveTabPage(tabPage);
            if (wasSelected)
                PostMessage(new HandleRef(this, this.Handle), WM_LEAVING_FULLSCREEN, IntPtr.Zero,
                                    IntPtr.Zero);

            this.ParentForm.UpdateControls();
        }
        #endregion
    }
}