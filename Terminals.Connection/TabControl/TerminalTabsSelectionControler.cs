using Terminals.Connection.Manager;
using Terminals.Connection.Panels.OptionPanels;
using Terminals.Connection.ScreenCapture;

namespace Terminals.Connection.TabControl
{
    // .NET namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    // Terminals and framework namespaces
    using Kohl.Framework.Localization;
    using Kohl.Framework.Logging;
    using Configuration.Files.Main.Favorites;
    using Configuration.Files.Main.Settings;

    /// <summary>
    ///     Adapter between all windows (including main window) and TabControl
    /// </summary>
    public class TerminalTabsSelectionControler
    {
        #region Private Fields (3)
        private readonly List<PopupTerminal> detachedWindows = new List<PopupTerminal>();
        private readonly IHostingForm mainForm;
        private readonly TabControl mainTabControl;
        #endregion

        #region Constructors (1)
        public TerminalTabsSelectionControler(IHostingForm mainForm, TabControl tabControl)
        {
            this.mainTabControl = tabControl;
            this.mainForm = mainForm;
            FavoritesDataDispatcher.Instance.FavoritesChanged += this.OnFavoritesChanged;
        }
        #endregion

        #region Public Properties (2)
        /// <summary>
        ///     Gets the actualy selected TabControl even if it is not in main window
        /// </summary>
        public TerminalTabControlItem Selected
        {
            get { return this.mainTabControl.SelectedItem as TerminalTabControlItem; }
        }

        public bool HasSelected
        {
            get { return this.mainTabControl.SelectedItem != null; }
        }
        #endregion

        #region Public Methods (11)
        /// <summary>
        ///     Markes the selected terminal as selected. If it is in mainTabControl,
        ///     then directly selects it, otherwise marks the selected window
        /// </summary>
        /// <param name="toSelect"> new terminal tabControl to assign as selected </param>
        public void Select(TerminalTabControlItem toSelect)
        {
            this.mainTabControl.SelectedItem = toSelect;
        }

        /// <summary>
        ///     Clears the selection of currently manipulated TabControl.
        ///     This has the same result like to call Select(null).
        /// </summary>
        public void UnSelect()
        {
            this.Select(null);
        }

        public void AddAndSelect(TerminalTabControlItem toAdd)
        {
            //this.mainTabControl.Items.Add(toAdd);
            this.mainTabControl.AddTab(toAdd);
            this.Select(toAdd);
        }

        public void RemoveAndUnSelect(TerminalTabControlItem toRemove)
        {
            this.mainTabControl.Items.Remove(toRemove);
            this.UnSelect();
        }

        /// <summary>
        ///     Releases actualy selected tab to the new window
        /// </summary>
        public void DetachTabToNewWindow()
        {
            this.DetachTabToNewWindow(this.Selected);
        }

        public void DetachTabToNewWindow(TerminalTabControlItem tabControlToOpen)
        {
            if (tabControlToOpen != null)
            {
                this.mainTabControl.Items.SuspendEvents();

                PopupTerminal pop = new PopupTerminal(this);
                this.mainTabControl.RemoveTab(tabControlToOpen);
                pop.AddTerminal(tabControlToOpen);

                this.mainTabControl.Items.ResumeEvents();
                this.detachedWindows.Add(pop);
                pop.Show();
            }
        }

        public void AttachTabFromWindow(TerminalTabControlItem tabControlToAttach)
        {
            this.mainTabControl.AddTab(tabControlToAttach);
            PopupTerminal popupTerminal = tabControlToAttach.FindForm() as PopupTerminal;

            if (popupTerminal != null)
                this.UnRegisterPopUp(popupTerminal);
        }

        public void UnRegisterPopUp(PopupTerminal popupTerminal)
        {
            if (this.detachedWindows.Contains(popupTerminal))
                this.detachedWindows.Remove(popupTerminal);
        }

        public void RefreshCaptureManagerAndCreateItsTab(bool openManagerTab)
        {
            Boolean createNew = !this.RefreshCaptureManager(true);

            if (createNew) // capture manager wasnt found
                if (!openManagerTab && (!Settings.EnableCaptureToFolder || !Settings.AutoSwitchOnCapture))
                    createNew = false;

            if (createNew)
                this.CreateCaptureManagerTab();
        }

        /// <summary>
        ///     Updates the CaptureManager tabcontrol, focuses it and updates its content.
        /// </summary>
        /// <param name="setFocus"> If true, focuses the capture manager Tab; otherwise nothting </param>
        /// <returns> true, Tab exists and was updated, otherwise false. </returns>
        public void Resize(System.Drawing.Size? newSize, bool? fromFullScreen = null)
        {
            foreach (TerminalTabControlItem tab in this.mainTabControl.Items)
            {
                if (fromFullScreen.HasValue)
                    // we are chaning from full screen to normal screen
                    if (fromFullScreen.Value)
                        tab.Connection.ChangeDesktopSize(newSize, DesktopSize.AutoScale);
                    // we are chaning from normal screen to full screen
                    else
                        tab.Connection.ChangeDesktopSize(newSize, DesktopSize.FullScreen);
                else
                    tab.Connection.ChangeDesktopSize(newSize);
            }
        }

        /// <summary>
        ///     Updates the CaptureManager tabcontrol, focuses it and updates its content.
        /// </summary>
        /// <param name="setFocus"> If true, focuses the capture manager Tab; otherwise nothting </param>
        /// <returns> true, Tab exists and was updated, otherwise false. </returns>
        public Boolean RefreshCaptureManager(Boolean setFocus)
        {
            foreach (TerminalTabControlItem tab in this.mainTabControl.Items)
            {
                if (tab.Title == Localization.Text("CaptureManager", typeof(TerminalTabsSelectionControler)))
                {
                    CaptureManagerConnection conn = (CaptureManagerConnection)tab.Connection;
                    conn.RefreshView();
                    if (setFocus && Settings.EnableCaptureToFolder && Settings.AutoSwitchOnCapture)
                    {
                        conn.BringToFront();
                        conn.Update();
                        this.Select(tab);
                    }

                    return true;
                }
            }

            return false;
        }
        
        public void UpdateCaptureButtonOnDetachedPopUps()
        {
            bool newEnable = Settings.EnabledCaptureToFolderAndClipBoard;
            foreach (PopupTerminal detachedWindow in this.detachedWindows)
            {
                detachedWindow.CaptureButtonEnabled = newEnable;
            }
        }
        #endregion

        #region Private Methods (6)
        private void CreateCaptureManagerTab()
        {
            string captureTitle = Localization.Text("CaptureManager", typeof(TerminalTabsSelectionControler));
            TerminalTabControlItem terminalTabPage = null;

            if (mainForm.InvokeRequired)
                mainForm.Invoke(new MethodInvoker(delegate { terminalTabPage = new TerminalTabControlItem(captureTitle, captureTitle); }));
            else
                terminalTabPage = new TerminalTabControlItem(captureTitle, captureTitle);

            try
            {
                terminalTabPage.AllowDrop = false;
                terminalTabPage.ToolTipText = captureTitle;
                terminalTabPage.Favorite = null;
                terminalTabPage.DoubleClick += this.mainForm.TerminalTabPage_DoubleClick;
                this.AddAndSelect(terminalTabPage);
                this.mainForm.UpdateControls();

                ConnectionBase conn = new CaptureManagerConnection();

                if (conn.InvokeRequired)
                    conn.Invoke(new MethodInvoker(delegate { conn.TerminalTabPage = terminalTabPage; conn.ParentForm = this.mainForm; }));
                else
                {
                    conn.TerminalTabPage = terminalTabPage;
                    conn.ParentForm = this.mainForm;
                }

                conn.Connect();
                conn.BringToFront();
                conn.Update();

                this.mainForm.UpdateControls();
            }
            catch (Exception exc)
            {
                Log.Error("Error loading the Capture Manager Tab Page", exc);
                this.RemoveAndUnSelect(terminalTabPage);
                terminalTabPage.Dispose();
            }
        }

        private void OnFavoritesChanged(FavoritesChangedEventArgs args)
        {
            foreach (KeyValuePair<string, FavoriteConfigurationElement> updated in args.Updated)
            {
                // only renamed items
                if (updated.Key == updated.Value.Name) 
                    continue;

                // dont update the rest of properties, because it doesnt reflect opened session
                this.UpdateDetachedWindowTitle(updated);
                this.UpdateAttachedTabTitle(updated);
            }
        }

        private void UpdateAttachedTabTitle(KeyValuePair<string, FavoriteConfigurationElement> updated)
        {
            TabControlItem attachedTab = this.FindAttachedTab(updated);
            if (attachedTab != null)
                attachedTab.Title = updated.Value.Name;
        }

        private TabControlItem FindAttachedTab(KeyValuePair<string, FavoriteConfigurationElement> updated)
        {
            return this.mainTabControl.Items.Cast<TerminalTabControlItem>().FirstOrDefault(tab => tab.Favorite.Name == updated.Key);
        }

        private void UpdateDetachedWindowTitle(KeyValuePair<string, FavoriteConfigurationElement> updated)
        {
            PopupTerminal detached = this.FindDetachedWindowByTitle(updated.Key);
            if (detached != null)
                detached.UpdateTitle(updated.Value.Name);
        }

        private PopupTerminal FindDetachedWindowByTitle(string oldName)
        {
            return this.detachedWindows.FirstOrDefault(window => window.Text == oldName);
        }
        #endregion
    }
}