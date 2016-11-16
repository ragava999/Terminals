namespace Terminals
{
    using CommandLine;
    using Configuration.Files.Credentials;
    using Configuration.Files.History;
    using Configuration.Files.Main.Favorites;
    using Configuration.Files.Main.Groups;
    using Configuration.Files.Main.Settings;
    using Configuration.Files.Main.SpecialCommands;
    using Connection;
    using Connection.Manager;
    using Connection.Native;
    using Connection.Panels.OptionPanels;
    using Connection.ScreenCapture;
    using Connection.TabControl;
    using Connections;
    using ExportImport;
    using Forms;
    using Forms.Controls;
    using Forms.Credentials;
    using Forms.Render;
    using Kohl.Framework.Info;
    using Kohl.Framework.Logging;
    using Kohl.Framework.WinForms;
    using Network.Services;
    using Panels;
    using Properties;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using VncSharp;
    using Wizard;

    public partial class MainForm : Form, IHostingForm
    {
        #region Persist the main window state
        private void PersistWindowState()
        {
            Log.InsideMethod();

            new PersistWindowState
            {
                Parent = this,
                AllowSaveMinimized = false,
                RegistryPath = AssemblyInfo.RegistryPath
                // LoadStateEvent += new PersistWindowState.WindowStateDelegate(LoadState),
                // SaveStateEvent += new PersistWindowState.WindowStateDelegate(SaveState)
            };
        }

        #region Don't delete this
        /*
        //private int m_data = 34;

        private void LoadState(object sender, RegistryKey key)
        { 
            // get additional state information from registry
            //m_data = (int)key.GetValue("m_data", m_data);
        }

        private void SaveState(object sender, RegistryKey key)
        {
            // save additional state information to registry
            //key.SetValue("m_data", m_data);
        }
        */
        #endregion

        #region Nested Classes
        private class MainFormFullScreenSwitch
        {
            private readonly MainForm mainForm;
            private Boolean showOnAllScreens;
            private Boolean isFavoriteToolbarVisible = true;
            private Boolean isFullScreen;

            private Boolean isSpecialCommandsToolStripVisible = true;
            private Boolean isStandardToolbarVisible = true;

            public MainFormFullScreenSwitch(MainForm mainForm)
            {
                Log.InsideMethod();

                this.mainForm = mainForm;
            }

            public bool FullScreen
            {
                get { return this.isFullScreen; }
                set { this.SwitchFullScreen(value); }
            }

            private void SwitchFullScreen(Boolean newfullScreen)
            {
                Log.InsideMethod();

                this.mainForm.SuspendLayout();

                //Save windows state before we do a fullscreen so we can restore it
                if (newfullScreen)
                    this.mainForm.toolStripContainer.SaveLayout();

                this.SetFullScreen(newfullScreen);
                this.mainForm.menuLoader.UpdateSwitchFullScreenMenuItemsVisibility(this.isFullScreen);

                if (!newfullScreen)
                    this.LoadWindowState();

                this.isFullScreen = newfullScreen;

                this.mainForm.DisalbeSizeChanged(true);
                this.mainForm.ResumeLayout();
                this.mainForm.DisalbeSizeChanged(false);
            }

            public void LoadWindowState()
            {
                Log.InsideMethod();

                Program.Caption = AssemblyInfo.AboutText;
                this.mainForm.Text = Program.Caption;
                this.mainForm.HideShowFavoritesPanel(Settings.ShowFavoritePanel);
                this.mainForm.toolStripContainer.LoadToolStripsState();
            }

            private void SetFullScreen(Boolean fullScreen)
            {
                Log.InsideMethod();

                this.BackUpToolBarsVisibility(fullScreen);
                this.HideToolBar(fullScreen);

                if (fullScreen)
                    this.mainForm.menuStrip.Visible = false;
                else
                {
                    this.mainForm.TopMost = false;
                    this.mainForm.FormBorderStyle = FormBorderStyle.Sizable;
                    this.mainForm.menuStrip.Visible = true;
                }

                this.mainForm.tcTerminals.ShowTabs = !fullScreen;
                this.mainForm.tcTerminals.ShowBorder = !fullScreen;

                this.mainForm.PerformLayout();
            }

            public void CheckForMultiMonitorUse()
            {
                Log.InsideMethod();

                this.showOnAllScreens = false;

                this.mainForm.showInDualScreensToolStripMenuItem.Text = "Show on &multi screens";

                if (Screen.AllScreens.Length > 1)
                {
                    this.mainForm.showInDualScreensToolStripMenuItem.Enabled = true;

                    //Lazy check to see if we are using dual screens
                    int w = this.mainForm.Width / Screen.PrimaryScreen.Bounds.Width;

                    if (this.mainForm.Width > Screen.FromControl(this.mainForm).Bounds.Width || w > 2)  //Screen.PrimaryScreen.Bounds.Width || w > 2)
                    {
                        this.showOnAllScreens = true;
                        this.mainForm.showInDualScreensToolStripMenuItem.Text = "Show on &single screen";
                    }
                }
                else
                {
                    this.mainForm.showInDualScreensToolStripMenuItem.ToolTipText = "You only have one screen";
                    this.mainForm.showInDualScreensToolStripMenuItem.Enabled = false;
                }
            }

            public void ToggleMultiMonitor()
            {
                Log.InsideMethod();

                Screen[] screenArr = Screen.AllScreens;
                Int32 with = 0;

                if (!this.showOnAllScreens)
                {
                    if (this.mainForm.WindowState == FormWindowState.Maximized)
                        this.mainForm.WindowState = FormWindowState.Normal;

                    with += screenArr.Sum(screen => screen.Bounds.Width);

                    this.mainForm.showInDualScreensToolStripMenuItem.Text = "Show on &single screen";
                    this.mainForm.BringToFront();
                }
                else
                {
                    with = Screen.PrimaryScreen.Bounds.Width;
                    this.mainForm.showInDualScreensToolStripMenuItem.Text = "Show on &multi screens";
                }

                this.mainForm.Top = 0;
                this.mainForm.Left = 0;
                this.mainForm.Height = Screen.PrimaryScreen.Bounds.Height;
                this.mainForm.Width = with;
                this.showOnAllScreens = !this.showOnAllScreens;
            }

            private void BackUpToolBarsVisibility(bool fullScreen)
            {
                Log.InsideMethod();

                if (fullScreen)
                {
                    this.isStandardToolbarVisible = this.mainForm.toolbarStd.Visible;
                    this.isSpecialCommandsToolStripVisible = this.mainForm.SpecialCommandsToolStrip.Visible;
                    this.isFavoriteToolbarVisible = this.mainForm.favoriteToolBar.Visible;
                }
            }

            private void HideToolBar(Boolean fullScreen)
            {
                Log.InsideMethod();

                if (!fullScreen)
                {
                    this.mainForm.toolbarStd.Visible = this.isStandardToolbarVisible;
                    this.mainForm.SpecialCommandsToolStrip.Visible = this.isSpecialCommandsToolStripVisible;
                    this.mainForm.favoriteToolBar.Visible = this.isFavoriteToolbarVisible;
                }
                else
                {
                    this.mainForm.toolbarStd.Visible = false;
                    this.mainForm.SpecialCommandsToolStrip.Visible = false;
                    this.mainForm.favoriteToolBar.Visible = false;
                }
            }
        }
        #endregion

        private void LoadWindowState()
        {
            Log.InsideMethod();

            if (this.fullScreenSwitch != null)
                this.fullScreenSwitch.LoadWindowState();
        }
        #endregion

        #region Delegates (1)
        public delegate void ReleaseIsAvailable(FavoriteConfigurationElement ReleaseFavorite);
        #endregion

        #region Private Fields

        private const String FULLSCREEN_ERROR_MSG = "Screen properties not available for RDP";

        private static int lastOpenLogFileProcessId = 0;
        private static Boolean isNewTerminalsReleaseAvailable;
        private static MainForm mainForm;

        private bool switching = false;

        private readonly MethodInvoker specialCommandsMethodInvoker;
        private readonly FavoritesMenuLoader menuLoader;
        private ToolTip currentToolTip;
        private TabControlItem currentToolTipItem;
        private FormWindowState originalFormWindowState;
        private MethodInvoker openTerminalsReleasePageMethodInvoker;
        private readonly MainFormFullScreenSwitch fullScreenSwitch;
        private readonly TerminalTabsSelectionControler terminalsControler;
        public static event ReleaseIsAvailable OnReleaseIsAvailable;

        #endregion

        #region Constuctor (1)
        public MainForm()
        {
            try
            {
                Log.InsideMethod();

                this.fullScreenSwitch = new MainFormFullScreenSwitch(this);

                this.PersistWindowState();

                mainForm = this;
                this.specialCommandsMethodInvoker = this.LoadSpecialCommands;

                this.ShowWizardAndReloadSpecialCommands();
                Settings.StartDelayedUpdate();

                // Set default font type by Windows theme to use for all controls on form
                this.Font = SystemFonts.IconTitleFont;

                // main designer procedure
                this.InitializeComponent();

                if (!MachineInfo.IsUnixOrMac)
                    // Set notifyicon icon from embedded png image
                    this.MainWindowNotifyIcon.Icon = Resources.terminalsicon;

                this.terminalsControler = new TerminalTabsSelectionControler(this, this.tcTerminals);

                this.menuLoader = new FavoritesMenuLoader(this.favoritesToolStripMenuItem,
                                                          this.tscConnectTo, this.serverToolStripMenuItem_Click,
                                                          this.favoriteToolBar, this.QuickContextMenu,
                                                          this.QuickContextMenu_ItemClicked);

                this.favoriteToolBar.Visible = this.toolStripMenuItemShowHideFavoriteToolbar.Checked;

                this.AssignToolStripsToContainer();

                this.tcTerminals.MouseDown += this.tcTerminals_MouseDown;
                this.tcTerminals.MouseUp += this.tcTerminals_MouseUp;
                this.tcTerminals.MouseClick += this.tcTerminals_MouseClick;
                this.QuickContextMenu.ItemClicked += this.QuickContextMenu_ItemClicked;

                DataDispatcher.AssignSynchronizationObject(this);

                this.SetLogLevelToDebugToolStripMenuItem.Checked = Log.IsDebugLogLevel();
                this.SetLogLevelToInfoToolStripMenuItem.Checked = !this.SetLogLevelToDebugToolStripMenuItem.Checked;
            }
            catch (Exception exc)
            {
                Log.Error("Error loading the main form", exc);
            }
        }
        #endregion

        #region Properties (7)
        public bool FullScreen { get { return this.fullScreenSwitch.FullScreen; } set { this.fullScreenSwitch.FullScreen = value; } }

        public static Boolean ReleaseAvailable
        {
            private get { return isNewTerminalsReleaseAvailable; }

            set
            {
                isNewTerminalsReleaseAvailable = value;
                if (isNewTerminalsReleaseAvailable)
                {
                    FavoriteConfigurationElement release = FavoritesFactory.GetOrCreateReleaseFavorite();

                    //Thread.Sleep(5000);
                    if (OnReleaseIsAvailable != null)
                        OnReleaseIsAvailable(release);
                }
            }
        }

        public static string ReleaseDescription { private get; set; }

        private Boolean IsMouseDown { get; set; }
        private Point MouseDownLocation { get; set; }

        private ConnectionBase CurrentConnection
        {
            get
            {
                if (this.terminalsControler.HasSelected)
                    return this.terminalsControler.Selected.Connection;

                return null;
            }
        }
        #endregion

        #region Public methods (9)
        public void UpdateControls()
        {
            Log.InsideMethod();

            this.tcTerminals.ShowToolTipOnTitle = Settings.ShowInformationToolTips;
            bool hasSelectedTerminal = this.terminalsControler.HasSelected;
            this.addTerminalToGroupToolStripMenuItem.Enabled = hasSelectedTerminal;
            this.tsbConnect.Enabled = (this.tscConnectTo.Text != String.Empty);
            this.tsbConnectToConsole.Enabled = (this.tscConnectTo.Text != String.Empty);
            this.saveTerminalsAsGroupToolStripMenuItem.Enabled = (this.tcTerminals.Items.Count > 0);
            this.vncActionButton.Visible = false;
            this.VMRCAdminSwitchButton.Visible = false;
            this.VMRCViewOnlyButton.Visible = false;
            RefreshToolStripButtons();

            if (this.CurrentConnection == null)
                return;

            if (this.CurrentConnection as VMRCConnection != null)
            {
                this.VMRCAdminSwitchButton.Visible = true;
                this.VMRCViewOnlyButton.Visible = true;
            }

            if (this.CurrentConnection as VNCConnection != null)
            {
                this.vncActionButton.Visible = true;
            }
        }

        public void DetachTabToNewWindow(TerminalTabControlItem tabControlToOpen)
        {
            Log.InsideMethod();

            this.terminalsControler.DetachTabToNewWindow(tabControlToOpen);
        }

        public void RemoveAndUnSelect(TerminalTabControlItem toRemove)
        {
            Log.InsideMethod();

            this.terminalsControler.RemoveAndUnSelect(toRemove);
        }

        public static MainForm GetMainForm()
        {
            Log.InsideMethod();

            return mainForm;
        }

        public void Connect(String connectionName, Boolean forceConsole, Boolean forceNewWindow, bool isDatabaseFavorite, CredentialSet credential = null, bool waitForEnd = false)
        {
            Log.InsideMethod();

            FavoriteConfigurationElement favorite = FavoritesFactory.GetFavoriteUpdatedCopy(connectionName,
                                                                                            forceConsole, forceNewWindow,
                                                                                            credential, isDatabaseFavorite);

            if (favorite != null)
            {
                ConnectionHistory.Instance.RecordHistoryItem(connectionName);
                this.SendNativeMessageToFocus();
                this.CreateTerminalTab(favorite, waitForEnd);
            }
            else
                this.CreateNewTerminal(waitForEnd, connectionName);
        }

        public void OpenNetworkingTools(string action, string host)
        {
            Log.InsideMethod();

            TerminalTabControlItem terminalTabPage = new TerminalTabControlItem("Networking Tools", "Networking Tools");
            try
            {
                terminalTabPage.AllowDrop = false;
                terminalTabPage.ToolTipText = "Networking Tools";
                terminalTabPage.Favorite = null;
                terminalTabPage.DoubleClick += this.TerminalTabPage_DoubleClick;
                this.terminalsControler.AddAndSelect(terminalTabPage);
                this.tcTerminals_SelectedIndexChanged(this, EventArgs.Empty);
                using (NetworkingToolsConnection conn = new NetworkingToolsConnection())
                {
                    conn.TerminalTabPage = terminalTabPage;
                    conn.ParentForm = this;
                    terminalTabPage.Connection = conn;
                    conn.Connect();
                    conn.BringToFront();
                    conn.Update();
                    this.UpdateControls();
                    conn.Execute(action, host);
                }
            }
            catch (Exception exc)
            {
                Log.Error("Open networking tools failure", exc);
                this.terminalsControler.RemoveAndUnSelect(terminalTabPage);
                terminalTabPage.Dispose();
            }
        }

        public void ShowManageTerminalForm(FavoriteConfigurationElement favorite)
        {
            Log.InsideMethod();

            using (FavoriteEditor frmNewTerminal = new FavoriteEditor(favorite))
            {
                if (this.favsList1 != null)
                    frmNewTerminal.SaveInDB = this.favsList1.SaveInDB;

                TerminalFormDialogResult result = frmNewTerminal.ShowDialog();

                if (result != TerminalFormDialogResult.Cancel)
                {
                    if (result == TerminalFormDialogResult.SaveAndConnect)
                        this.CreateTerminalTab(frmNewTerminal.Favorite, false);
                }
            }
        }

        public void TerminalTabPage_DoubleClick(object sender, EventArgs e)
        {
            Log.InsideMethod();

            if (this.terminalsControler.HasSelected)
            {
                this.tsbDisconnect.PerformClick();
            }
        }

        public void DisalbeSizeChanged(bool disable)
        {
            Log.InsideMethod();

            if (disable)
                this.tcTerminals.SizeChanged -= new System.EventHandler(this.tcTerminals_SizeChanged);
            else
                this.tcTerminals.SizeChanged += new System.EventHandler(this.tcTerminals_SizeChanged);
        }
        #endregion

        #region Private methods (126)

        private void LoadContextMenu()
        {
            Log.InsideMethod();

            this.menuLoader.ToggleShowHideFavoritesFromQuickMenu();
        }

        private void ToggleGrabInput()
        {
            Log.InsideMethod();

            if (this.CurrentConnection != null && this.CurrentConnection is RDPConnection)
            {
                ((RDPConnection)this.CurrentConnection).FullScreen = !((RDPConnection)this.CurrentConnection).FullScreen;
            }
        }

        private void CreateTerminalTab(FavoriteConfigurationElement favorite, bool waitForEnd)
        {
            Log.InsideMethod();

            this.CallExecuteBeforeConnectedFromSettings();
            this.CallExecuteBeforeConnectedFromFavorite(favorite);
            this.TryConnectTabPage(favorite, waitForEnd);
        }

        private void ApplyControlsEnableAndVisibleState()
        {
            Log.InsideMethod();

            if (!MachineInfo.IsUnixOrMac)
                this.MainWindowNotifyIcon.Visible = Settings.MinimizeToTray;

            if (!Settings.MinimizeToTray && !this.Visible)
                this.Visible = true;

            this.lockToolbarsToolStripMenuItem.Checked = Settings.ToolbarsLocked;
            this.MainMenuStrip.GripStyle = Settings.ToolbarsLocked
                                               ? ToolStripGripStyle.Hidden
                                               : ToolStripGripStyle.Visible;

            this.tcTerminals.ShowToolTipOnTitle = Settings.ShowInformationToolTips;
            if (this.terminalsControler.HasSelected)
                this.terminalsControler.Selected.ToolTipText =
                    this.terminalsControler.Selected.Favorite.GetToolTipText();

            this.groupsToolStripMenuItem.Visible = Settings.EnableGroupsMenu;
            this.tsbTags.Checked = Settings.ShowFavoritePanel;
            this.pnlTagsFavorites.Width = 7;

            this.HideShowFavoritesPanel(Settings.ShowFavoritePanel);
            this.UpdateCaptureButtonEnabled();
            this.ApplyTheme();
        }

        private void ApplyTheme()
        {
            Log.InsideMethod();

            if (Settings.Office2007BlueFeel)
                ToolStripManager.Renderer = Office2007Renderer.GetRenderer(RenderColors.Blue);
            else if (Settings.Office2007BlackFeel)
                ToolStripManager.Renderer = Office2007Renderer.GetRenderer(RenderColors.Black);
            else
                if (!MachineInfo.IsUnixOrMac)
            {
                ToolStripManager.Renderer = new ToolStripProfessionalRenderer();
            }
            else
            {
                ToolStripManager.RenderMode = ToolStripManagerRenderMode.System;
            }

            if (!MachineInfo.IsUnixOrMac)
                // Update the old treeview theme to the new theme from Win Vista and up
                WindowsApi.SetWindowTheme(this.menuStrip.Handle, "Explorer", null);
        }

        private void ShowWizardAndReloadSpecialCommands()
        {
            Log.InsideMethod();

            if (Settings.ShowWizard)
            {
                //settings file doesnt exist, wizard!
                using (FirstRunWizard wzrd = new FirstRunWizard())
                    wzrd.ShowDialog(this);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(this.ReloadSpecialCommands, null);
            }
        }

        private void SendNativeMessageToFocus()
        {
            Log.InsideMethod();

            if (!this.Visible)
            {
                this.Show();

                if (!MachineInfo.IsUnixOrMac)
                {
                    if (this.WindowState == FormWindowState.Minimized)
                        WindowsApi.ShowWindow(new HandleRef(this, this.Handle), 9);

                    WindowsApi.SetForegroundWindow(new HandleRef(this, this.Handle));
                }
            }
        }

        /// <summary>
        ///     Assignes toolbars and menu items to toolstrip container.
        ///     They arent moved to the container because of designer
        /// </summary>
        private void AssignToolStripsToContainer()
        {
            Log.InsideMethod();

            this.toolStripContainer.toolbarStd = this.toolbarStd;
            this.toolStripContainer.standardToolbarToolStripMenuItem = this.standardToolbarToolStripMenuItem;
            this.toolStripContainer.favoriteToolBar = this.favoriteToolBar;
            this.toolStripContainer.toolStripMenuItemShowHideFavoriteToolbar = this.toolStripMenuItemShowHideFavoriteToolbar;
            this.toolStripContainer.SpecialCommandsToolStrip = this.SpecialCommandsToolStrip;
            this.toolStripContainer.shortcutsToolStripMenuItem = this.shortcutsToolStripMenuItem;
            this.toolStripContainer.menuStrip = this.menuStrip;
            this.toolStripContainer.AssignToolStripsLocationChangedEventHandler();
        }

        private void CloseTabControlItem()
        {
            Log.InsideMethod();

            this.Text = AssemblyInfo.AboutText;

            if (Settings.LetTerminalsAutomaticallyManageRevertFromFullScreenMode)
                if (this.tcTerminals.Items.Count == 0)
                    this.fullScreenSwitch.FullScreen = false;
        }

        public void RemoveTabPage(TabControlItem tabControlToRemove)
        {
            Log.InsideMethod();

            this.tcTerminals.RemoveTab(tabControlToRemove);
            this.CloseTabControlItem();
        }

        private void TryConnectTabPage(FavoriteConfigurationElement favorite, bool waitForEnd)
        {
            Log.InsideMethod();

            try
            {
                ConnectionManager.CreateConnection(favorite, this, waitForEnd, this.AssignEventsToConnectionTab(favorite));
            }
            catch (Exception exc)
            {
                Log.Error("Error creating the connection.", exc);
                this.terminalsControler.UnSelect();
            }
        }

        private TerminalTabControlItem AssignEventsToConnectionTab(FavoriteConfigurationElement favorite)
        {
            Log.InsideMethod();

            TerminalTabControlItem terminalTabPage = ConnectionManager.CreateTerminalTabPageByFavoriteName(favorite);

            // This might happen if the user is not allowed to
            // use all available connections e.g. 
            // if the user has a freeware version.
            if (terminalTabPage == null)
            {
                return null;
            }

            terminalTabPage.AllowDrop = true;
            terminalTabPage.DragOver += this.terminalTabPage_DragOver;
            terminalTabPage.DragEnter += this.terminalTabPage_DragEnter;
            //terminalTabPage.ToolTipText = favorite.GetToolTipText();
            terminalTabPage.Favorite = favorite;
            terminalTabPage.DoubleClick += this.TerminalTabPage_DoubleClick;
            this.terminalsControler.AddAndSelect(terminalTabPage);
            this.UpdateControls();

            return terminalTabPage;
        }

        private void CallExecuteBeforeConnectedFromFavorite(FavoriteConfigurationElement favorite)
        {
            Log.InsideMethod();

            if (favorite.ExecuteBeforeConnect && !string.IsNullOrEmpty(favorite.ExecuteBeforeConnectCommand))
            {
                try
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo(favorite.ExecuteBeforeConnectCommand,
                                                                             favorite.ExecuteBeforeConnectArgs)
                    {
                        WorkingDirectory = favorite.ExecuteBeforeConnectInitialDirectory
                    };

                    Process process = Process.Start(processStartInfo);

                    if (favorite.ExecuteBeforeConnectWaitForExit)
                    {
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Execute before connect failed. Please have a look at your favorites configuration and check the 'Execute' tab.", ex);
                }
            }
        }

        private void CallExecuteBeforeConnectedFromSettings()
        {
            Log.InsideMethod();

            if (Settings.ExecuteBeforeConnect && !string.IsNullOrEmpty(Settings.ExecuteBeforeConnectCommand))
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo(Settings.ExecuteBeforeConnectCommand,
                                                                         Settings.ExecuteBeforeConnectArgs)
                {
                    WorkingDirectory = Settings.ExecuteBeforeConnectInitialDirectory
                };
                Process process = Process.Start(processStartInfo);
                if (Settings.ExecuteBeforeConnectWaitForExit)
                {
                    process.WaitForExit();
                }
            }
        }

        private void LoadSpecialCommands()
        {
            Log.InsideMethod();

            this.SpecialCommandsToolStrip.Items.Clear();

            foreach (SpecialCommandConfigurationElement cmd in Settings.SpecialCommands)
            {
                this.SpecialCommandsToolStrip.Items.Add(new ToolStripMenuItem(cmd.Name)
                {
                    DisplayStyle = ToolStripItemDisplayStyle.Image,
                    ToolTipText = cmd.Name,
                    Text = cmd.Name,
                    Tag = cmd,
                    Image = this.LoadThumbnail(cmd),
                    ImageTransparentColor = Color.Magenta,
                    Overflow = ToolStripItemOverflow.AsNeeded
                });
            }
        }

        private Image LoadThumbnail(SpecialCommandConfigurationElement cmd)
        {
            Log.InsideMethod();

            Image img = Resources.application_xp_terminal;

            try
            {
                if (!string.IsNullOrEmpty(cmd.Thumbnail))
                {
                    img = Kohl.Framework.Drawing.IconHandler.Base64ToImage(cmd.Thumbnail);
                }
            }
            catch (Exception exc)
            {
                Log.Error("Could not LoadThumbnail for file:" + cmd.Thumbnail, exc);
            }

            return img;
        }

        private void ShowCredentialsManager()
        {
            Log.InsideMethod();
            CredentialManager mgr = new CredentialManager();
            mgr.ShowDialog();
        }

        private void OpenSavedConnections()
        {
            Log.InsideMethod();
            if (this.favsList1 != null)
                foreach (string name in Settings.SavedConnections)
                {
                    this.Connect(name, false, false, favsList1.SaveInDB, waitForEnd: true);
                }

            Settings.ClearSavedConnectionsList();
        }

        private void HideShowFavoritesPanel(bool show)
        {
            Log.InsideMethod();

            if (Settings.EnableFavoritesPanel)
            {
                if (show)
                {
                    this.splitContainer1.Panel1MinSize = 15;

                    // If we start Terminals minimized than ignore the "FavoritePanelWidth" because it wouuld be bigger than the splitter width.
                    if (Settings.FavoritePanelWidth < this.splitContainer1.Width)
                        this.splitContainer1.SplitterDistance = Settings.FavoritePanelWidth;

                    this.splitContainer1.Panel1Collapsed = false;
                    this.splitContainer1.IsSplitterFixed = false;
                    this.pnlHideTagsFavorites.Show();
                    this.pnlShowTagsFavorites.Hide();
                }
                else
                {
                    this.splitContainer1.Panel1MinSize = 9;
                    this.splitContainer1.SplitterDistance = 9;
                    this.splitContainer1.IsSplitterFixed = true;
                    this.pnlHideTagsFavorites.Hide();
                    this.pnlShowTagsFavorites.Show();
                }

                Settings.ShowFavoritePanel = show;
                this.tsbTags.Checked = show;
            }
            else
            {
                //just hide it completely
                this.splitContainer1.Panel1Collapsed = true;
                this.splitContainer1.Panel1MinSize = 0;
                this.splitContainer1.SplitterDistance = 0;
            }
        }

        private void LoadGroups()
        {
            Log.InsideMethod();

            GroupConfigurationElementCollection serversGroups = Settings.GetGroups();
            Int32 seperatorIndex = this.groupsToolStripMenuItem.DropDownItems.IndexOf(this.groupsSeparator);
            for (Int32 i = this.groupsToolStripMenuItem.DropDownItems.Count - 1; i > seperatorIndex; i--)
            {
                this.groupsToolStripMenuItem.DropDownItems.RemoveAt(i);
            }

            this.addTerminalToGroupToolStripMenuItem.DropDownItems.Clear();
            foreach (GroupConfigurationElement serversGroup in serversGroups)
            {
                this.AddGroup(serversGroup);
            }

            this.addTerminalToGroupToolStripMenuItem.Enabled = false;
            this.saveTerminalsAsGroupToolStripMenuItem.Enabled = false;
        }

        private void AddGroup(GroupConfigurationElement group)
        {
            Log.InsideMethod();

            ToolStripMenuItem groupToolStripMenuItem = new ToolStripMenuItem(group.Name) { Name = @group.Name };
            groupToolStripMenuItem.Click += this.groupToolStripMenuItem_Click;
            this.groupsToolStripMenuItem.DropDownItems.Add(groupToolStripMenuItem);
            ToolStripMenuItem groupAddToolStripMenuItem = new ToolStripMenuItem(group.Name) { Name = @group.Name };
            groupAddToolStripMenuItem.Click += this.groupAddToolStripMenuItem_Click;
            this.addTerminalToGroupToolStripMenuItem.DropDownItems.Add(groupAddToolStripMenuItem);
        }

        public void SetGrabInput(bool grab)
        {
            Log.InsideMethod();

            if (this.CurrentConnection != null)
            {
                if (grab && !this.CurrentConnection.ContainsFocus)
                    this.CurrentConnection.Focus();

                try
                {
                    if (this.CurrentConnection is RDPConnection)
                        ((RDPConnection)this.CurrentConnection).FullScreen = grab;
                }
                catch (Exception exc)
                {
                    Log.Error(FULLSCREEN_ERROR_MSG, exc);
                }
            }
        }

        private void CreateNewTerminal(bool waitForEnd = false, String name = null)
        {
            Log.InsideMethod();

            if (this.favsList1 != null)
                using (FavoriteEditor frmNewTerminal = new FavoriteEditor(name, this.favsList1.SelectedTag))
                {
                    frmNewTerminal.SaveInDB = this.favsList1.SaveInDB;

                    TerminalFormDialogResult result = frmNewTerminal.ShowDialog();
                    if (result != TerminalFormDialogResult.Cancel)
                    {
                        if (result == TerminalFormDialogResult.SaveAndConnect)
                            this.CreateTerminalTab(frmNewTerminal.Favorite, waitForEnd);
                    }
                }
        }

        private void QuickConnect(String server, Int32 port, Boolean connectToConsole, string protocol, string url, bool isDatabaseFavorite)
        {
            Log.InsideMethod();

            if (port == 0)
                port = ConnectionManager.GetPort(protocol);

            FavoriteConfigurationElement favorite = FavoritesFactory.GetOrCreateQuickConnectFavorite(server,
                                                                                                     connectToConsole,
                                                                                                     port, protocol, url, isDatabaseFavorite);

            this.CreateTerminalTab(favorite, false);
        }

        public void HandleCommandLineActions(CommandLineArgs commandLineArgs)
        {
            Log.InsideMethod();

            Boolean connectToConsole = commandLineArgs.Console;
            this.fullScreenSwitch.FullScreen = commandLineArgs.Fullscreen;

            if (commandLineArgs.HasUrlDefined)
                this.QuickConnect(commandLineArgs.UrlServer, commandLineArgs.UrlPort, connectToConsole, commandLineArgs.ProtcolName, commandLineArgs.UrlServer, commandLineArgs.UseDbFavorite);
            else if (commandLineArgs.HasMachineDefined)
                this.QuickConnect(commandLineArgs.MachineName, commandLineArgs.Port, connectToConsole, commandLineArgs.ProtcolName, null, commandLineArgs.UseDbFavorite);
            else
                this.ConnectToFavorites(commandLineArgs, connectToConsole);

            Updates.UpdateManager.CheckAndPerformUpgradeIfAllowedInSettingsAndBinaryIsNewer(this, commandLineArgs);
        }

        private void ConnectToFavorites(CommandLineArgs commandLineArgs, bool connectToConsole)
        {
            Log.InsideMethod();

            if (this.favsList1 != null)
                if (commandLineArgs.Favorites.Length > 0)
                    foreach (String favoriteName in commandLineArgs.Favorites)
                        this.Connect(favoriteName, connectToConsole, false, favsList1.SaveInDB, waitForEnd: true);
        }

        private void SaveActiveConnections()
        {
            Log.InsideMethod();

            Settings.CreateSavedConnectionsList((from TabControlItem item in this.tcTerminals.Items select item.Name).ToArray());
        }

        private void OpenReleasePage()
        {
            Log.InsideMethod();

            if (!Settings.NeverShowTerminalsWindow)
                this.Connect(FavoritesFactory.TerminalsReleasesFavoriteName, false, false, false);
        }

        private void ReloadSpecialCommands(Object state)
        {
            Log.InsideMethod();

            // start creating the icons
            LoadSpecialCommands(false);
            Settings.SaveAndFinishDelayedUpdate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Log.InsideMethod();

            this.openTerminalsReleasePageMethodInvoker = this.OpenReleasePage;
            this.Text = AssemblyInfo.AboutText;
            OnReleaseIsAvailable += this.MainForm_OnReleaseIsAvailable;

            this.ApplyControlsEnableAndVisibleState();

            this.LoadGroups();
            this.UpdateControls();

            this.fullScreenSwitch.LoadWindowState();

            // 1) It is highly important to call the method in Form.Load()
            // due to that fact that for example the RDP connection can't be
            // loaded in the constructor (the main form is not ready) -> and
            // the rdp session would crash afterwards.
            // 2) Moreover it is important to load the connections with the right
            // size ... this can only be done if the form and splitter settings have
            // been restored ... otherwise the connection window would be either
            // a bit smaller or huger (i.e. some content would be cutted - missed!).
            this.OpenSavedConnections();

            SetUpgradeStatusInToolStripBar();
        }
        
        protected override void WndProc(ref Message msg)
        {
            try
            {
                if (msg.Msg == 0x21)  // mouse click
                {
                    TerminalTabControlItem selectedTab = this.terminalsControler.Selected;
                    if (selectedTab != null)
                    {
                        Rectangle r = selectedTab.RectangleToScreen(selectedTab.ClientRectangle);
                        if (r.Contains(MousePosition))
                        {
                            SetGrabInput(true);
                        }
                        else
                        {
                            TabControlItem item = tcTerminals.GetTabItemByPoint(tcTerminals.PointToClient(MousePosition));
                            if (item == selectedTab)
                                SetGrabInput(true); //Grab input if clicking on currently selected tab
                        }
                    }
                    /*else
                    {
                        SetGrabInput(false);
                    }*/
                }

                base.WndProc(ref msg);
            }
            catch (Exception ex)
            {
                Log.Debug("WnProc failure", ex);
            }
        }

        private void SetUpgradeStatusInToolStripBar()
        {
            // Give the update procedure some time.
            for (int i = 0; i <= 100; i++)
            {
                Thread.Sleep(50);
                Application.DoEvents();
            }

            if (Updates.UpdateManager.IsUpdateInProgress)
            {
                this.updateToolStripItem.Enabled = false;
                this.updateToolStripItem.Text = "Upgrade is in progress ... ";

                while (Updates.UpdateManager.IsUpdateInProgress)
                {
                    Thread.Sleep(50);
                    Application.DoEvents();
                }

                // actually not needed, but if some error occurs during the upgrade process
                // we want to reset our environment.
                this.updateToolStripItem.Text = "Checking for a new &release online";
                this.updateToolStripItem.Enabled = true;
                this.updateToolStripItem.Visible = false;
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            Log.InsideMethod();

            if (e.KeyCode == Keys.Cancel)
                this.ToggleGrabInput();
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            Log.InsideMethod();

            //handle global keyup events
            if (e.Control && e.KeyCode == Keys.F12)
            {
                CaptureManagerConnection.PerformScreenCapture(this.tcTerminals);
                this.terminalsControler.RefreshCaptureManagerAndCreateItsTab(false);
            }
            else if (e.KeyCode == Keys.F4)
            {
                if (!this.tscConnectTo.Focused)
                    this.tscConnectTo.Focus();
            }
            else if (e.KeyCode == Keys.F3)
            {
                this.ShowQuickConnect();
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            Log.InsideMethod();

            if (this.fullScreenSwitch.FullScreen)
                this.tcTerminals.ShowTabs = false;

            this.fullScreenSwitch.CheckForMultiMonitorUse();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.InsideMethod();

            if (this.favsList1 != null)
                this.favsList1.SaveState();

            if (this.fullScreenSwitch.FullScreen)
                this.fullScreenSwitch.FullScreen = false;

            if (!MachineInfo.IsUnixOrMac)
                this.MainWindowNotifyIcon.Visible = false;

            this.CloseOpenedConnections(e);
            this.toolStripContainer.SaveLayout();

            if (!e.Cancel)
            {
                try
                {
                    SingleInstanceApplication.Instance.Close();
                }
                catch { }

                // Deregister the form closing event
                this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);

                // force the application to exit
                // this prevents a ghost "Terminals.exe" to
                // remain/stay in memory.
                Application.Exit();
            }
        }

        private void CloseOpenedConnections(FormClosingEventArgs args)
        {
            Log.InsideMethod();

            if (this.tcTerminals.Items.Count > 0)
            {
                if (Settings.ShowConfirmDialog)
                    this.SaveConnectonsIfRequested(args);

                if (args.Cancel == true)
                    return;

                if (Settings.SaveConnectionsOnClose)
                    this.SaveActiveConnections();

                // Close all active connections
                foreach (TerminalTabControlItem item in this.tcTerminals.Items)
                {
                    // Save the connection for later disposal
                    ConnectionBase con = item.Connection;

                    // Dispose the TabPage
                    item.Dispose();

                    // Disconnect the connection
                    // -> i.e. kill all external remaining processes spawned by Terminals e.g. Radmin.exe, Putty.exe, etc.
                    // -> i.e. Release all unmanaged resources e.g. File handles (Text connection, autoit connection)
                    con.Disconnect();
                }
            }
        }

        private void SaveConnectonsIfRequested(FormClosingEventArgs args)
        {
            Log.InsideMethod();

            SaveActiveConnectionsForm frmSaveActiveConnections = new SaveActiveConnectionsForm();
            if (frmSaveActiveConnections.ShowDialog() == DialogResult.OK)
            {
                Settings.ShowConfirmDialog = frmSaveActiveConnections.PromptNextTime;
                if (frmSaveActiveConnections.OpenConnectionsNextTime)
                    this.SaveActiveConnections();
            }
            else
                args.Cancel = true;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            Log.InsideMethod();

            if (this.WindowState == FormWindowState.Minimized)
            {
                if (Settings.MinimizeToTray) this.Visible = false;
            }
            else
            {
                this.originalFormWindowState = this.WindowState;
            }
        }

        private void tcTerminals_MouseUp(object sender, MouseEventArgs e)
        {
            Log.InsideMethod();

            this.Cursor = Cursors.Default;
            this.IsMouseDown = false;
        }

        private void tcTerminals_MouseDown(object sender, MouseEventArgs e)
        {
            Log.InsideMethod();

            this.MouseDownLocation = MousePosition;
            this.IsMouseDown = true;
        }

        private void tcTerminals_MouseClick(object sender, MouseEventArgs e)
        {
            Log.InsideMethod();

            if (e.Button == MouseButtons.Right)
            {
                if (this.tcTerminals != null && sender != null)
                    this.QuickContextMenu.Show(this.tcTerminals, e.Location);
            }
        }

        private void QuickContextMenu_Opening(object sender, CancelEventArgs e)
        {
            Log.InsideMethod();

            this.tcTerminals_MouseClick(null, new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0));
            e.Cancel = false;
        }

        private void QuickContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Log.InsideMethod();

            ToolStripItem clickedItem = e.ClickedItem;
            if (clickedItem.Text == "Restore" ||
                clickedItem.Name == FavoritesMenuLoader.COMMAND_RESTORESCREEN ||
                clickedItem.Name == FavoritesMenuLoader.COMMAND_FULLSCREEN)
            {
                this.fullScreenSwitch.FullScreen = !this.fullScreenSwitch.FullScreen;
            }
            else if (clickedItem.Name == FavoritesMenuLoader.COMMAND_CREDENTIALMANAGER)
            {
                this.ShowCredentialsManager();
            }
            else if (clickedItem.Name == FavoritesMenuLoader.COMMAND_ORGANIZEFAVORITES)
            {
                this.manageConnectionsToolStripMenuItem_Click(null, null);
            }
            else if (clickedItem.Name == FavoritesMenuLoader.COMMAND_OPTIONS)
            {
                this.optionsToolStripMenuItem_Click(null, null);
            }
            else if (clickedItem.Name == FavoritesMenuLoader.COMMAND_NETTOOLS)
            {
                this.TsbNetworkingTools_Click(null, null);
            }
            else if (clickedItem.Name == FavoritesMenuLoader.COMMAND_CAPTUREMANAGER)
            {
                this.terminalsControler.RefreshCaptureManagerAndCreateItsTab(true);
            }
            else if (clickedItem.Name == FavoritesMenuLoader.COMMAND_EXIT)
            {
                this.Close();
            }
            else if (clickedItem.Name == FavoritesMenuLoader.COMMAND_SHOWMENU)
            {
                Boolean visible = !this.menuStrip.Visible;
                this.menuStrip.Visible = visible;
                this.menubarToolStripMenuItem.Checked = visible;
            }
            else if (clickedItem.Name == FavoritesMenuLoader.COMMAND_SPECIAL)
            {
                return;
            }
            else if (clickedItem.Name == FavoritesMenuLoader.QUICK_CONNECT)
            {
                ShowQuickConnect();
            }
            else if (clickedItem.Name == FavoritesMenuLoader.COMMAND_DETACH)
            {
                this.terminalsControler.DetachTabToNewWindow();
            }
            else
            {
                this.OnFavoriteTrayToolsStripClick(e);
            }

            this.QuickContextMenu.Hide();
        }

        private void ShowQuickConnect()
        {
            Log.InsideMethod();

            if (this.favsList1 == null)
                return;

            QuickConnect qc = new QuickConnect { StartPosition = FormStartPosition.CenterParent, SaveInDB = favsList1.SaveInDB };

            if (qc.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(qc.ConnectionName))
            {
                this.Connect(qc.ConnectionName, false, false, favsList1.SaveInDB, waitForEnd: true);
            }
        }

        private void OnFavoriteTrayToolsStripClick(ToolStripItemClickedEventArgs e)
        {
            Log.InsideMethod();

            string tag = e.ClickedItem.Tag as String;

            if (tag != null)
            {
                String itemName = e.ClickedItem.Text;
                if (tag == FavoritesMenuLoader.FAVORITE)
                    if (this.favsList1 != null)
                        this.Connect(itemName, false, false, favsList1.SaveInDB, waitForEnd: false);

                if (tag == FavoritesMenuLoader.TAG)
                {
                    ToolStripMenuItem parent = e.ClickedItem as ToolStripMenuItem;
                    this.ConnectToAllFavoritesUnderTag(parent);
                }
            }
        }

        private void ConnectToAllFavoritesUnderTag(ToolStripMenuItem parent)
        {
            Log.InsideMethod();

            if (parent.DropDownItems.Count > 0)
            {
                DialogResult result = this.AskUserIfWantsConnectToAll(parent);
                if (result == DialogResult.OK)
                {
                    if (this.favsList1 != null)
                        foreach (ToolStripMenuItem button in parent.DropDownItems)
                        {
                            this.Connect(button.Text, false, false, favsList1.SaveInDB, waitForEnd: true);
                        }
                }
            }
        }

        private DialogResult AskUserIfWantsConnectToAll(ToolStripMenuItem parent)
        {
            Log.InsideMethod();

            String message = String.Format("Are you sure you want to connect to all these {0} connections?",
                                           parent.DropDownItems.Count);
            return MessageBox.Show(message, "Confirmation", MessageBoxButtons.OKCancel);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();

            this.Close();
        }

        private void groupAddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();

            GroupConfigurationElement group = Settings.GetGroups()[((ToolStripMenuItem)sender).Text];
            String selectedTitle = this.terminalsControler.Selected.Title;
            group.FavoriteAliases.Add(new FavoriteAliasConfigurationElement(selectedTitle));
            Settings.DeleteGroup(group.Name);
            Settings.AddGroup(group);
        }

        private void groupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();

            if (this.favsList1 == null)
                return;

            FavoriteConfigurationElementCollection favorites = Settings.GetFavorites(favsList1.SaveInDB);
            String groupName = ((ToolStripItem)sender).Text;
            GroupConfigurationElement serversGroup = Settings.GetGroups()[groupName];
            foreach (FavoriteAliasConfigurationElement favoriteAlias in serversGroup.FavoriteAliases)
            {
                FavoriteConfigurationElement favorite = favorites[favoriteAlias.Name];
                this.CreateTerminalTab(favorite, true);
            }
        }

        private void serverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();

            if (this.favsList1 == null)
                return;

            String connectionName = ((ToolStripItem)sender).Text;
            FavoriteConfigurationElement favorite = Settings.GetOneFavorite(connectionName, favsList1.SaveInDB);
            this.CreateTerminalTab(favorite, false);
        }

        private void terminalTabPage_DragEnter(object sender, DragEventArgs e)
        {
            Log.InsideMethod();
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop, false) ? DragDropEffects.Copy : DragDropEffects.None;

            if (e.Data != null)
                this.CurrentConnection.DoDragDrop(e.Data, e.AllowedEffect);
        }

        private void terminalTabPage_DragOver(object sender, DragEventArgs e)
        {
            Log.InsideMethod();
            this.terminalsControler.Select(sender as TerminalTabControlItem);

            if (e.Data != null)
                this.CurrentConnection.DoDragDrop(e.Data, e.AllowedEffect);
        }

        private void newTerminalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.CreateNewTerminal();
        }

        private void tsbConnect_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            string connectionName = this.tscConnectTo.Text;

            if (this.favsList1 != null)
                if (connectionName != String.Empty)
                {
                    this.Connect(connectionName, false, false, favsList1.SaveInDB, waitForEnd: false);
                }
        }

        private void tsbConnectToConsole_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            string connectionName = this.tscConnectTo.Text;

            if (this.favsList1 != null)
                if (connectionName != String.Empty)
                {
                    this.Connect(connectionName, true, false, favsList1.SaveInDB, waitForEnd: false);
                }
        }

        private void tscConnectTo_KeyDown(object sender, KeyEventArgs e)
        {
            Log.InsideMethod();
            if (e.KeyCode == Keys.Enter)
            {
                if (this.favsList1 == null)
                    return;

                FavoriteConfigurationElement fav = Settings.GetFavorites(favsList1.SaveInDB)
                                                           .ToList()
                                                           .FirstOrDefault(f => f.Name == tscConnectTo.Text);
                if (fav != null)
                {
                    this.Connect(fav.Name, false, false, fav.IsDatabaseFavorite, waitForEnd: false);
                    tscConnectTo.Text = null;
                }
            }
        }

        private void tsbDisconnect_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            try
            {
                TerminalTabControlItem tabToClose = this.terminalsControler.Selected;
                if (this.tcTerminals.Items.Contains(tabToClose))
                    this.tcTerminals.CloseTab(tabToClose);
            }
            catch (Exception exc)
            {
                Log.Error("Disconnecting a tab threw an exception", exc);
            }
        }

        private void tcTerminals_SelectedIndexChanged(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.UpdateControls();
        }

        private void newTerminalToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.CreateNewTerminal();
        }

        private void tsbGrabInput_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.ToggleGrabInput();
        }

        private void tcTerminals_TabControlItemClosing(TabControlItemClosingEventArgs e)
        {
            Log.InsideMethod();

            Boolean cancel = false;
            if (this.CurrentConnection != null && this.CurrentConnection.Connected)
            {
                Boolean close = false;
                if (Settings.WarnOnConnectionClose)
                {
                    close =
                        (MessageBox.Show(this, "Are you sure that you want to disconnect from the active terminal?",
                                         AssemblyInfo.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                         DialogResult.Yes);
                }
                else
                {
                    close = true;
                }

                if (close)
                {
                    if (this.CurrentConnection != null)
                    {
                        this.CurrentConnection.Disconnect();
                        // Close tabitem functions handled under each connection disconnect methods.
                        cancel = true;
                    }

                    this.Text = AssemblyInfo.AboutText;
                }
                else
                {
                    cancel = true;
                }
            }

            e.Cancel = cancel;
        }

        private void RefreshToolStripButtons()
        {
            if (this.tcTerminals.Items.Count > 0)
            {
                tsbDisconnect.Enabled = this.disconnectToolStripMenuItem.Enabled = this.CurrentConnection != null;
            }
            else
            {
                tsbDisconnect.Enabled = this.disconnectToolStripMenuItem.Enabled = false;
            }
        }

        private void tcTerminals_TabControlItemSelectionChanged(TabControlItemChangedEventArgs e)
        {
            Log.InsideMethod();
            this.UpdateControls();

            if (this.tcTerminals.Items.Count > 0)
            {
                this.SetGrabInput(true);

                if (e.Item.Selected && Settings.ShowInformationToolTips)
                    this.Text = e.Item.ToolTipText.Replace("\r\n", "; ");
            }
        }

        // Enable lazy search load
        System.Windows.Forms.Timer timer = null;

        /// <summary>
        /// Enable lazy search load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tscConnectTo_TextChanged(object sender, EventArgs e)
        {
            Log.InsideMethod();
            if (timer == null)
            {
                timer = new System.Windows.Forms.Timer();
                timer.Tick += timer_Tick;
            }

            if (string.IsNullOrEmpty(tscConnectTo.Text) || !tscConnectTo.Items.Contains(tscConnectTo.Text))
            {
                tsbConnect.Enabled = tsbConnectToConsole.Enabled = false;
            }

            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Stop();
            timer.Start();
        }

        private void tscConnectTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            tsbConnect.Enabled = tsbConnectToConsole.Enabled = !string.IsNullOrEmpty(this.tscConnectTo.Text);
        }

        /// <summary>
        /// Enable lazy search load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            Log.InsideMethod();
            timer.Stop();

            if (this.favsList1 == null)
                return;

            this.favsList1.SaveState();
            this.ApplyControlsEnableAndVisibleState();
            this.favsList1.LoadFavorites(tscConnectTo.Text);
        }

        private void tcTerminals_MouseHover(object sender, EventArgs e)
        {
            Log.InsideMethod();
            if (this.tcTerminals != null && !this.tcTerminals.ShowTabs)
                this.timerHover.Enabled = true;
        }

        private void tcTerminals_MouseLeave(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.timerHover.Enabled = false;
            if (this.fullScreenSwitch.FullScreen && this.tcTerminals.ShowTabs && !this.tcTerminals.MenuOpen)
                this.tcTerminals.ShowTabs = false;

            if (this.currentToolTipItem != null && !currentToolTipItem.IsDisposed && !currentToolTipItem.Disposing)
            {
                this.currentToolTip.Hide(this.currentToolTipItem);
                this.currentToolTip.Active = false;
            }
        }

        private void tcTerminals_TabControlItemClosed(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.CloseTabControlItem();
        }

        private void tcTerminals_DoubleClick(object sender, EventArgs e)
        {
            Log.InsideMethod();
            // Deaktivated
            //switching = true;
            //this.fullScreenSwitch.FullScreen = !this.fullScreenSwitch.FullScreen;
        }

        private void tsbFullScreen_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            switching = true;
            this.fullScreenSwitch.FullScreen = !this.fullScreenSwitch.FullScreen;
            this.UpdateControls();
        }

        private void tcTerminals_MenuItemsLoaded(object sender, EventArgs e)
        {
            Log.InsideMethod();
            foreach (ToolStripItem item in this.tcTerminals.Menu.Items)
            {
                item.Image = Resources.smallterm;
            }

            if (this.fullScreenSwitch.FullScreen)
            {
                ToolStripSeparator sep = new ToolStripSeparator();
                this.tcTerminals.Menu.Items.Add(sep);
                ToolStripMenuItem item = new ToolStripMenuItem("Restore", null, this.tcTerminals_DoubleClick);
                this.tcTerminals.Menu.Items.Add(item);
                item = new ToolStripMenuItem("Minimize", null, this.Minimize);
                this.tcTerminals.Menu.Items.Add(item);
            }
        }

        private void manageConnectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            OrganizeFavoritesForm conMgr = new OrganizeFavoritesForm { MainForm = this };

            if (this.favsList1 != null)
                conMgr.SaveInDB = favsList1.SaveInDB;

            conMgr.ShowDialog();
        }

        private void saveTerminalsAsGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            using (NewGroupForm frmNewGroup = new NewGroupForm())
            {
                if (frmNewGroup.ShowDialog() == DialogResult.OK)
                {
                    GroupConfigurationElement serversGroup = new GroupConfigurationElement
                    {
                        Name =
                                                                         frmNewGroup
                                                                         .txtGroupName.Text
                    };
                    foreach (TabControlItem tabControlItem in this.tcTerminals.Items)
                    {
                        serversGroup.FavoriteAliases.Add(new FavoriteAliasConfigurationElement(tabControlItem.Title));
                    }

                    Settings.AddGroup(serversGroup);
                    this.LoadGroups();
                }
            }
        }

        private void organizeGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            using (OrganizeGroupsForm frmOrganizeGroups = new OrganizeGroupsForm())
            {
                if (this.favsList1 != null)
                    frmOrganizeGroups.SaveInDB = favsList1.SaveInDB;

                frmOrganizeGroups.ShowDialog();
                this.LoadGroups();
            }
        }

        private void tcTerminals_TabControlMouseOnTitle(TabControlMouseOnTitleEventArgs e)
        {
            Log.InsideMethod();
            if (Settings.ShowInformationToolTips)
            {
                if (this.currentToolTip == null)
                {
                    this.currentToolTip = new ToolTip { Active = false };
                }
                else if ((this.currentToolTipItem != null) && (this.currentToolTipItem != e.Item) && !currentToolTipItem.IsDisposed && !currentToolTipItem.Disposing)
                {
                    this.currentToolTip.Hide(this.currentToolTipItem);
                    this.currentToolTip.Active = false;
                }

                if (!this.currentToolTip.Active)
                {
                    this.currentToolTip = new ToolTip
                    {
                        ToolTipTitle = "Connection Information",
                        ToolTipIcon = ToolTipIcon.Info,
                        UseFading = true,
                        UseAnimation = true,
                        IsBalloon = false
                    };
                    this.currentToolTip.Show(e.Item.ToolTipText, e.Item, (int)e.Item.StripRect.X, 2);
                    this.currentToolTipItem = e.Item;
                    this.currentToolTip.Active = true;
                }
            }
        }

        private void tcTerminals_TabControlMouseLeftTitle(TabControlMouseOnTitleEventArgs e)
        {
            Log.InsideMethod();
            if (this.currentToolTipItem != null && !currentToolTipItem.IsDisposed && !currentToolTipItem.Disposing)
            {
                this.currentToolTip.Hide(this.currentToolTipItem);
                this.currentToolTip.Active = false;
            }
        }

        private void timerHover_Tick(object sender, EventArgs e)
        {
            Log.InsideMethod();
            if (this.timerHover.Enabled)
            {
                this.timerHover.Enabled = false;
                this.tcTerminals.ShowTabs = true;
            }
        }

        private void organizeFavoritesToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            OrganizeFavoritesToolbarForm frmOrganizeFavoritesToolbar = new OrganizeFavoritesToolbarForm();
            frmOrganizeFavoritesToolbar.ShowDialog();
        }

        /// <summary>
        ///     Disable capture button when function is disabled in options
        /// </summary>
        private void UpdateCaptureButtonEnabled()
        {
            Log.InsideMethod();

            Boolean enableCapture = Settings.EnabledCaptureToFolderAndClipBoard;

            if (Kohl.Framework.Info.MachineInfo.IsUnixOrMac)
                enableCapture = false;

            this.CaptureScreenToolStripButton.Enabled = enableCapture;
            this.captureTerminalScreenToolStripMenuItem.Enabled = enableCapture;
            this.terminalsControler.UpdateCaptureButtonOnDetachedPopUps();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            using (AboutForm frmAbout = new AboutForm())
            {
                frmAbout.ShowDialog();
            }
        }

        private void tsbTags_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.HideShowFavoritesPanel(this.tsbTags.Checked);
        }

        private void pbShowTags_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.HideShowFavoritesPanel(true);
        }

        private void pbHideTags_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.HideShowFavoritesPanel(false);
        }

        private void Minimize(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.originalFormWindowState = this.WindowState;
            this.WindowState = FormWindowState.Minimized;
        }

        private void MainWindowNotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            Log.InsideMethod();
            if (e.Button == MouseButtons.Left)
            {
                if (Settings.MinimizeToTray)
                {
                    this.Visible = !this.Visible;
                    if (this.Visible && this.WindowState == FormWindowState.Minimized)
                        this.WindowState = this.originalFormWindowState;
                }
                else
                {
                    if (this.WindowState == FormWindowState.Normal)
                    {
                        this.originalFormWindowState = this.WindowState;
                        this.WindowState = FormWindowState.Minimized;
                    }
                    else
                    {
                        this.WindowState = this.originalFormWindowState;
                    }
                }
            }
        }

        private void CaptureScreenToolStripButton_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            CaptureManagerConnection.PerformScreenCapture(this.tcTerminals);
            this.terminalsControler.RefreshCaptureManager(false);

            if (Settings.EnableCaptureToFolder && Settings.AutoSwitchOnCapture)
                this.terminalsControler.RefreshCaptureManagerAndCreateItsTab(false);
        }

        private void captureTerminalScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.CaptureScreenToolStripButton_Click(null, null);
        }

        private void VMRCAdminSwitchButton_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            if (this.CurrentConnection != null)
            {
                VMRCConnection vmrc = this.CurrentConnection as VMRCConnection;
                if (vmrc != null)
                {
                    vmrc.AdminDisplay();
                }
            }
        }

        private void VMRCViewOnlyButton_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            if (this.CurrentConnection != null)
            {
                VMRCConnection vmrc = this.CurrentConnection as VMRCConnection;
                if (vmrc != null)
                {
                    vmrc.ViewOnlyMode = !vmrc.ViewOnlyMode;
                }
            }

            this.UpdateControls();
        }

        private void standardToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.AddShowStrip(this.toolbarStd, this.standardToolbarToolStripMenuItem, !this.toolbarStd.Visible);
        }

        private void toolStripMenuItemShowHideFavoriteToolbar_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.AddShowStrip(this.favoriteToolBar, this.toolStripMenuItemShowHideFavoriteToolbar,
                              !this.favoriteToolBar.Visible);
        }

        private void shortcutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.AddShowStrip(this.SpecialCommandsToolStrip, this.shortcutsToolStripMenuItem,
                              !this.SpecialCommandsToolStrip.Visible);
        }

        private void menubarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.AddShowStrip(this.menuStrip, this.menubarToolStripMenuItem, !this.menuStrip.Visible);
        }

        private void AddShowStrip(ToolStrip strip, ToolStripMenuItem menu, Boolean visible)
        {
            Log.InsideMethod();
            if (!Settings.ToolbarsLocked)
            {
                strip.Visible = visible;
                menu.Checked = visible;
            }
            else
            {
                MessageBox.Show("Please unlock the toolbars before you are able to change them.");
            }
        }

        private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.shortcutsToolStripMenuItem.Checked = this.SpecialCommandsToolStrip.Visible;
            this.toolStripMenuItemShowHideFavoriteToolbar.Checked = this.favoriteToolBar.Visible;
            this.standardToolbarToolStripMenuItem.Checked = this.toolbarStd.Visible;
        }

        private void ToolStripOrganizeShortucts_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            using (OrganizeShortcuts org = new OrganizeShortcuts())
            {
                org.ShowDialog(this);
            }

            this.Invoke(this.specialCommandsMethodInvoker);
        }

        private void ShortcutsContextMenu_MouseClick(object sender, MouseEventArgs e)
        {
            Log.InsideMethod();
            this.ToolStripOrganizeShortucts_Click(null, null);
        }

        // todo assign missing SpecialCommandsToolStrip_MouseClick
        private void SpecialCommandsToolStrip_MouseClick(object sender, MouseEventArgs e)
        {
            Log.InsideMethod();
            if (e.Button == MouseButtons.Right)
                this.ShortcutsContextMenu.Show(e.X, e.Y);
        }

        private void SpecialCommandsToolStrip_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
        {
            Log.InsideMethod();
            SpecialCommandConfigurationElement elm = e.ClickedItem.Tag as SpecialCommandConfigurationElement;
            if (elm != null)
                elm.Launch();
        }

        private void TsbNetworkingTools_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.OpenNetworkingTools(null, null);
        }

        private void networkingToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.TsbNetworkingTools_Click(null, null);
        }

        private void toolStripMenuItemCaptureManager_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.terminalsControler.RefreshCaptureManagerAndCreateItsTab(true);
        }

        private void toolStripButtonCaptureManager_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            Boolean origval = Settings.AutoSwitchOnCapture;
            if (!Settings.EnableCaptureToFolder || !Settings.AutoSwitchOnCapture)
            {
                Settings.AutoSwitchOnCapture = true;
            }

            this.terminalsControler.RefreshCaptureManagerAndCreateItsTab(true);
            Settings.AutoSwitchOnCapture = origval;
        }

        private void sendALTKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            if (sender != null && (sender as ToolStripMenuItem) != null)
            {
                String key = (sender as ToolStripMenuItem).Text;
                if (this.CurrentConnection != null)
                {
                    VNCConnection vnc = this.CurrentConnection as VNCConnection;
                    if (vnc != null)
                    {
                        if (key == this.sendALTF4KeyToolStripMenuItem.Text)
                        {
                            vnc.SendSpecialKeys(SpecialKeys.AltF4);
                        }
                        else if (key == this.sendALTKeyToolStripMenuItem.Text)
                        {
                            vnc.SendSpecialKeys(SpecialKeys.Alt);
                        }
                        else if (key == this.sendCTRLESCKeysToolStripMenuItem.Text)
                        {
                            vnc.SendSpecialKeys(SpecialKeys.CtrlEsc);
                        }
                        else if (key == this.sendCTRLKeyToolStripMenuItem.Text)
                        {
                            vnc.SendSpecialKeys(SpecialKeys.Ctrl);
                        }
                        else if (key == this.sentCTRLALTDELETEKeysToolStripMenuItem.Text)
                        {
                            vnc.SendSpecialKeys(SpecialKeys.CtrlAltDel);
                        }
                    }
                }
            }
        }

        private void TsbFixDesktopSize_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            if (this.terminalsControler.HasSelected)
            {
                TerminalTabControlItem terminalTabPage = this.terminalsControler.Selected;
                if (terminalTabPage.Connection != null)
                {
                    terminalTabPage.Connection.ChangeDesktopSize();
                }
            }
        }

        private void pbShowTagsFavorites_MouseMove(object sender, MouseEventArgs e)
        {
            Log.InsideMethod();
            if (Settings.AutoExapandTagsPanel)
                this.HideShowFavoritesPanel(true);
        }

        private void lockToolbarsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.toolStripContainer.SaveLayout();
            this.lockToolbarsToolStripMenuItem.Checked = !this.lockToolbarsToolStripMenuItem.Checked;
            Settings.ToolbarsLocked = this.lockToolbarsToolStripMenuItem.Checked;
            this.toolStripContainer.ChangeLockState();
        }

        private void MainForm_OnReleaseIsAvailable(FavoriteConfigurationElement releaseFavorite)
        {
            Log.InsideMethod();
            this.Invoke(this.openTerminalsReleasePageMethodInvoker);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            if (!this.updateToolStripItem.Visible)
            {
                if (ReleaseAvailable && this.updateToolStripItem != null)
                {
                    this.updateToolStripItem.Visible = ReleaseAvailable;
                    if (!string.IsNullOrEmpty(ReleaseDescription))
                    {
                        this.updateToolStripItem.Text = String.Format("Upgrade to {0} now", ReleaseDescription);
                    }
                }
            }
        }

        private void TsbComputerManagement_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            Process.Start("mmc.exe", "compmgmt.msc /a /computer=.");
        }

        private void rebuildTagsIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();

            if (this.favsList1 != null)
                Settings.RebuildTagIndex(favsList1.SaveInDB);

            this.LoadGroups();
            this.UpdateControls();
        }

        private void viewInNewWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();

            if (this.CurrentConnection != null)
            {
                this.terminalsControler.DetachTabToNewWindow();
            }
        }

        private void rebuildShortcutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadSpecialCommands(true);
        }

        private void LoadSpecialCommands(bool reload)
        {
            Log.InsideMethod();

            if (Settings.SpecialCommands.Count == 0 || reload)
            {
                Settings.SpecialCommands.Clear();
                Settings.SpecialCommands = SpecialCommandsWizard.LoadSpecialCommands();
            }

            while (!this.Created)
            {
                Thread.Sleep(50);
                Application.DoEvents();
            }

            this.Invoke(this.specialCommandsMethodInvoker);
        }

        private void rebuildToolbarsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.fullScreenSwitch.LoadWindowState();
        }

        private void openLogFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            Process.Start(Log.CurrentLogFolder);
            Log.Info("The log file folder has been opened.");
        }

        private void openLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            if (lastOpenLogFileProcessId > 0)
            {
                Process lastOpenLogFileProcess =
                (from process in Process.GetProcesses().ToList<Process>()
                 where process.Id == lastOpenLogFileProcessId
                 select process).FirstOrDefault<Process>();

                if (lastOpenLogFileProcess != null && lastOpenLogFileProcess.Handle != IntPtr.Zero && !lastOpenLogFileProcess.HasExited)
                    try
                    {
                        lastOpenLogFileProcess.Kill();
                        Log.Info("Kill last notepad containing the log.");
                    }
                    catch
                    {
                        Log.Warn("Unable to kill last notepad containing the log.");
                    }
            }

            if (MachineInfo.IsMac)
                lastOpenLogFileProcessId = Process.Start("open", "-a TextEdit " + Log.CurrentLogFile).Id;
            else
                lastOpenLogFileProcessId = Process.Start("notepad.exe", Log.CurrentLogFile).Id;

            Log.Info("Notepad has been started containing the current log file.");
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Log.InsideMethod();

            if (this.splitContainer1.Panel1.Width > 15)
                Settings.FavoritePanelWidth = this.splitContainer1.Panel1.Width;

            ChangeSize();
            //if (this.CurrentConnection != null)
            //this.CurrentConnection.ChangeDesktopSize();
        }

        private void credentialManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.ShowCredentialsManager();
        }

        private void CredentialManagementToolStripButton_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.ShowCredentialsManager();
        }

        private void exportConnectionsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            ExportFrom ei = new ExportFrom();
            ei.Show();
        }

        private void toolStripMenuItemImport_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();

            OrganizeFavoritesForm conMgr = new OrganizeFavoritesForm { MainForm = this };

            if (this.favsList1 != null)
                conMgr.SaveInDB = favsList1.SaveInDB;

            conMgr.CallImport();
            conMgr.ShowDialog();
        }

        private void showInDualScreensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();
            this.fullScreenSwitch.ToggleMultiMonitor();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.InsideMethod();

            using (OptionEditor frmOptions = new OptionEditor(this))
            {
                if (frmOptions.ShowDialog() == DialogResult.OK)
                {
                    if (frmOptions.FavoritesTreeViewOptionsChanged)
                    {
                        if (this.favsList1 == null)
                            return;

                        this.favsList1.SaveState();
                        this.ApplyControlsEnableAndVisibleState();
                        this.pnlTagsFavorites.Controls.Remove(this.favsList1);
                        this.favsList1.Dispose();
                        this.favsList1 = new FavsList { Dock = DockStyle.Fill, TabIndex = 2 };
                        this.pnlTagsFavorites.Controls.Add(this.favsList1);

                        LoadContextMenu();
                    }
                    ReloadTerminalsBackgroundImage();
                }
            }
        }

        private void ReloadTerminalsBackgroundImage()
        {
            Log.InsideMethod();

            tcTerminals.BackColor = Kohl.Framework.Converters.ColorParser.FromString(Settings.DashBoardBackgroundColor, Color.FromKnownColor(KnownColor.Control));
            tcTerminals.BackgroundImageLayout = (ImageLayout)Settings.ImageStyle;

            string backgroundImage = Settings.ImagePath.NormalizePath(Kohl.Framework.Info.AssemblyInfo.DirectoryConfigFiles);

            if (string.IsNullOrEmpty(backgroundImage))
                tcTerminals.BackgroundImage = null;

            if (File.Exists(backgroundImage))
                tcTerminals.BackgroundImage = System.Drawing.Bitmap.FromFile(backgroundImage);
        }

        private void tcTerminals_SizeChanged(object sender, EventArgs e)
        {
            //ChangeSize();
        }
        #endregion
        
        private void ChangeSize()
        {
            Log.InsideMethod();
            ReloadTerminalsBackgroundImage();

            // we are chaning from full screen to normal screen
            if (switching && fullScreenSwitch.FullScreen)
            {
                //this.terminalsControler.Resize(null, false);
                this.terminalsControler.Resize(new Size(this.tcTerminals.Width, this.tcTerminals.Height - this.tcTerminals.HeaderHeight * 3), true);
                switching = false;
                return;
            }

            // we are chaning from normal screen to full screen
            if (switching && !fullScreenSwitch.FullScreen)
            {
                //this.terminalsControler.Resize(this.tcTerminals.Size, false);
                this.terminalsControler.Resize(new Size(this.tcTerminals.Width, this.tcTerminals.Height + this.tcTerminals.HeaderHeight), true);
                switching = false;
                return;
            }

            // Normal resize
            this.terminalsControler.Resize(null);
            switching = false;
        }

        void EmptyLogFileToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(Log.CurrentLogFile))
                    File.WriteAllText(Log.CurrentLogFile, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terminals was unable to clear the contents of the file" + Environment.NewLine + ex.Message, "Error");
            }
        }

        void SetLogLevelToInfoToolStripMenuItemClick(object sender, EventArgs e)
        {
            Kohl.Framework.Logging.Log.SetLogLevel(false);
            this.SetLogLevelToDebugToolStripMenuItem.Checked = false;
            this.SetLogLevelToInfoToolStripMenuItem.Checked = true;
        }

        void SetLogLevelToDebugToolStripMenuItemClick(object sender, EventArgs e)
        {
            Kohl.Framework.Logging.Log.SetLogLevel(true);
            this.SetLogLevelToDebugToolStripMenuItem.Checked = true;
            this.SetLogLevelToInfoToolStripMenuItem.Checked = false;
        }

        private void updateToolStripItem_Click(object sender, EventArgs e)
        {
            // Force the upgrade
            Thread upgradeThread = new Thread(() => { Updates.UpdateManager.PerformUpgradeIfNewer(new CommandLineArgs() { AutomaticallyUpdate = true }, true); });

            upgradeThread.Start();

            this.updateToolStripItem.Enabled = false;
            this.updateToolStripItem.Text = "Upgrade is in progress ... ";

            while (upgradeThread.IsAlive)
            {
                Thread.Sleep(50);
                Application.DoEvents();
            }

            // actually not needed, but if some error occurs during the upgrade process
            // we want to reset our environment.
            this.updateToolStripItem.Text = "Checking for a new &release online";
            this.updateToolStripItem.Enabled = true;
            this.updateToolStripItem.Visible = false;
        }
    }
}
