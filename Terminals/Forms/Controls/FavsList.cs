namespace Terminals.Forms.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Kohl.Framework.Info;

    using Kohl.Framework.Logging;
    using Kohl.Framework.WinForms;

    using Configuration.Files.Credentials;
    using Configuration.Files.Main.Favorites;
    using Configuration.Files.Main.Settings;
    using Connection.Native;
    using ExportImport;
    using Credentials;
    using Network.WakeOnLAN;

    public partial class FavsList : UserControl
    {
        public static CredentialSet CredSet = null;
        private MainForm mainForm;

        public FavsList()
        {
            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                this.InitializeComponent();

                CredSet = new CredentialSet();

                // Update the old treeview theme to the new theme from Win Vista and up
                WindowsApi.SetWindowTheme(this.favsTree.Handle, "Explorer", null);
                WindowsApi.SetWindowTheme(this.dbTreeView1.Handle, "Explorer", null);
                WindowsApi.SetWindowTheme(this.historyTreeView.Handle, "Explorer", null);

                this.historyTreeView.DoubleClick += this.HistoryTreeView_DoubleClick;

                this.LoadFavorites();
                this.LoadState();
            }
        }

        public bool SaveInDB
        {
            get
            {
                return (TreeView == dbTreeView1);
            }
        }

        private TreeViewBase TreeView
        {
            get
            {
                if (tabControl1.SelectedIndex == 0)
                    return favsTree;

                return dbTreeView1;
            }
        }

        #region Private methods

        private MainForm GetMainForm()
        {
            return this.mainForm ?? (this.mainForm = MainForm.GetMainForm());
        }

        private void Connect(TreeNode SelectedNode, bool AllChildren, bool Console, bool NewWindow)
        {
            if (AllChildren)
            {
                foreach (TreeNode node in SelectedNode.Nodes)
                {
                    FavoriteConfigurationElement fav = (node.Tag as FavoriteConfigurationElement);
                    if (fav != null)
                    {
                        this.GetMainForm().Connect(fav.Name, Console, NewWindow, SaveInDB, waitForEnd:true);
                    }
                }
            }
            else
            {
                FavoriteConfigurationElement fav = this.TreeView.SelectedFavorite;
                if (fav != null)
                {
                    this.GetMainForm().Connect(fav.Name, Console, NewWindow, SaveInDB, waitForEnd: false);
                }
            }

            this.contextMenuStrip1.Close();
            this.contextMenuStrip2.Close();
        }

        #endregion

        #region Private event handler methods

        private void FavsList_Load(object sender, EventArgs e)
        {
            this.dbTreeView1.NodeMouseClick += this.TreeView_NodeMouseClick;
            this.favsTree.NodeMouseClick += this.TreeView_NodeMouseClick;
        }

        private void HistoryTreeView_DoubleClick(object sender, EventArgs e)
        {
            this.StartConnection(this.historyTreeView);
        }

        private void pingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FavoriteConfigurationElement fav = this.TreeView.SelectedFavorite;
            if (fav != null)
                this.GetMainForm().OpenNetworkingTools("Ping", fav.ServerName);
        }

        private void dNSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FavoriteConfigurationElement fav = this.TreeView.SelectedFavorite;
            if (fav != null)
                this.GetMainForm().OpenNetworkingTools("DNS", fav.ServerName);
        }

        private void traceRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FavoriteConfigurationElement fav = this.TreeView.SelectedFavorite;
            if (fav != null)
                this.GetMainForm().OpenNetworkingTools("Trace", fav.ServerName);
        }

        private void tSAdminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FavoriteConfigurationElement fav = this.TreeView.SelectedFavorite;
            if (fav != null)
                this.GetMainForm().OpenNetworkingTools("TSAdmin", fav.ServerName);
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FavoriteConfigurationElement fav = this.TreeView.SelectedFavorite;

            if (fav != null)
            {
                this.GetMainForm().ShowManageTerminalForm(fav);
            }
        }

        private void ShutdownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            String msg = String.Empty;

            FavoriteConfigurationElement fav = this.TreeView.SelectedFavorite;
            if (fav != null)
            {
                MagicPacket.ShutdownCommands shutdownStyle;
                if (menuItem.Equals(this.shutdownToolStripMenuItem))
                {
                    msg = String.Format("Are you sure you want to shutdown this machine: {0}", fav.ServerName);
                    shutdownStyle = MagicPacket.ShutdownCommands.ForcedShutdown;
                }
                else if (menuItem.Equals(this.rebootToolStripMenuItem))
                {
                    msg = String.Format("Are you sure you want to reboot this machine: {0}", fav.ServerName);
                    shutdownStyle = MagicPacket.ShutdownCommands.ForcedReboot;
                }
                else
                {
                    return;
                }

                if (MessageBox.Show(msg, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    try
                    {
                        if (MagicPacket.ForceShutdown(fav.ServerName, shutdownStyle, fav.Credential) == 0)
                        {
                            MessageBox.Show(AssemblyInfo.Title + " successfully sent the shutdown command.");
                            return;
                        }
                    }
                    catch (ManagementException ex)
                    {
                        Log.Info(ex.ToString(), ex);
                        MessageBox.Show("Terminals was not able to shutdown the machine remotely.\r\nPlease check the log file.");
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Terminals was not able to shutdown the machine remotely.\r\n\r\nAccess is Denied.");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Terminals was not able to shutdown the machine remotely.");
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Connect(this.TreeView.SelectedNode, false, this.consoleToolStripMenuItem.Checked,
                         this.newWindowToolStripMenuItem.Checked);
        }

        private void connectToAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Connect(this.TreeView.SelectedNode, true, this.consoleAllToolStripMenuItem.Checked,
                         this.newWindowAllToolStripMenuItem.Checked);
        }

        private void computerManagementMMCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FavoriteConfigurationElement fav = this.TreeView.SelectedFavorite;
            if (fav != null)
            {
                Process.Start("mmc.exe", "compmgmt.msc /a /computer=" + fav.ServerName);
            }
        }

        private void systemInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FavoriteConfigurationElement fav = this.TreeView.SelectedFavorite;
            if (fav != null)
            {
                String programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                //if(programFiles.Contains("(x86)")) programFiles = programFiles.Replace(" (x86)","");
                String path = String.Format(@"{0}\common files\Microsoft Shared\MSInfo\msinfo32.exe", programFiles);
                if (File.Exists(path))
                {
                    Process.Start(String.Format("\"{0}\"", path), String.Format("/computer {0}", fav.ServerName));
                }
            }
        }

        private void setCredentialByTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String tagName = this.TreeView.SelectedNode.Name;

            string input = "Set Credential by Tag\r\n\r\nThis will replace the credential used for all Favorites within this tag.\r\n\r\nUse at your own risk!";
            
            if (InputBox.Show(ref input, "Change Credential" + " - " + tagName) == DialogResult.OK)
            {
                CredentialSet credentialSet = StoredCredentials.GetByName(input);

                if (credentialSet == null)
                {
                    MessageBox.Show("The credential you specified does not exist.");
                    return;
                }

                this.GetMainForm().Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                List<FavoriteConfigurationElement> selectedFavorites = this.GetSelectedFavorites();
                Settings.ApplyCredentialsForAllSelectedFavorites(selectedFavorites, credentialSet.Name);

                this.GetMainForm().Cursor = Cursors.Default;
                Application.DoEvents();
                MessageBox.Show("Set Credential by Tag Complete.");
            }
        }

        private List<FavoriteConfigurationElement> GetSelectedFavorites()
        {
            this.TreeView.LoadAllFavoritesUnderTag(this.TreeView.GetTagNodeByName(this.SelectedTag));
            return this.TreeView.SelectedNode.Nodes.Cast<TreeNode>()
                       .Select(node => node.Tag as FavoriteConfigurationElement)
                       .ToList();
        }

        private void setPasswordByTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String tagName = this.TreeView.SelectedNode.Name;
            string input = "Set Password by Tag\r\n\r\nThis will replace the password for all Favorites within this tag.\r\n\r\nUse at your own risk!";
                
            if (InputBox.Show(ref input, "Change Password" + " - " + tagName, '*') == DialogResult.OK)
            {
                this.GetMainForm().Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                List<FavoriteConfigurationElement> selectedFavorites = this.GetSelectedFavorites();
                Settings.SetPasswordToAllSelectedFavorites(selectedFavorites, input);

                this.GetMainForm().Cursor = Cursors.Default;
                Application.DoEvents();
                MessageBox.Show("Set Password by Tag Complete.");
            }
        }

        private void setDomainByTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String tagName = this.TreeView.SelectedNode.Name;
            string input = "Set Domain by Tag\r\n\r\nThis will replace the Domain for all Favorites within this tag.\r\n\r\nUse at your own risk!";
                
            if (InputBox.Show(ref input, "Change Domain" + " - " + tagName) == DialogResult.OK)
            {
                this.GetMainForm().Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                List<FavoriteConfigurationElement> selectedFavorites = this.GetSelectedFavorites();
                Settings.ApplyDomainNameToAllSelectedFavorites(selectedFavorites, input);

                this.GetMainForm().Cursor = Cursors.Default;
                Application.DoEvents();
                MessageBox.Show("Set Domain by Tag Complete.");
            }
        }

        private void setUsernameByTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String tagName = this.TreeView.SelectedNode.Name;
            string input = "Set Username by Tag\r\n\r\nThis will replace the Username for all Favorites within this tag.\r\n\r\nUse at your own risk!";
            
            if (InputBox.Show(ref input, "Change Username" + " - " + tagName) == DialogResult.OK)
            {
                this.GetMainForm().Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                List<FavoriteConfigurationElement> selectedFavorites = this.GetSelectedFavorites();
                Settings.ApplyUserNameToAllSelectedFavorites(selectedFavorites, input);

                this.GetMainForm().Cursor = Cursors.Default;
                Application.DoEvents();
                MessageBox.Show("Set Username by Tag Complete.");
            }
        }

        private void deleteAllFavoritesByTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String tagName = this.TreeView.SelectedNode.Name;
                
            if (MessageBox.Show("Delete all Favorites by Tag\r\n\r\nThis will DELETE all Favorites within this tag.\r\n\r\nUse at your own risk!", "Delete all Favorites by Tag" + " - " + tagName, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.GetMainForm().Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                List<FavoriteConfigurationElement> selectedFavorites = this.GetSelectedFavorites();
                Settings.DeleteFavorites(selectedFavorites);

                this.GetMainForm().Cursor = Cursors.Default;
                Application.DoEvents();
                MessageBox.Show("Delete all Favorites by Tag Complete.");
            }
        }

        private void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.consoleToolStripMenuItem.Checked = false;
                this.newWindowToolStripMenuItem.Checked = false;
                this.consoleAllToolStripMenuItem.Checked = false;
                this.newWindowAllToolStripMenuItem.Checked = false;

                this.TreeView.SelectedNode = e.Node;

                this.TreeView.ContextMenuStrip = this.TreeView.SelectedFavorite != null ? this.contextMenuStrip1 : this.contextMenuStrip2;
            }
        }

        private void TreeView_DoubleClick(object sender, EventArgs e)
        {
            this.StartConnection(this.TreeView);
        }

        private void TreeView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
        }

        private void TreeView_DragDrop(object sender, DragEventArgs e)
        {
            String[] files = e.Data.GetData(DataFormats.FileDrop) as String[];
            if (files != null)
            {
                List<FavoriteConfigurationElement> favoritesToImport = Integrations.Importers.ImportFavorites(files);
                ImportWithDialogs managedImport = new ImportWithDialogs(this.ParentForm);
                managedImport.Import(favoritesToImport);
            }
        }

        private void StartConnection(TreeView tv)
        {
            // connections are always under some parent node in History and in Favorites
            if (tv.SelectedNode != null && tv.SelectedNode.Level > 0)
            {
                MainForm mainForm = this.GetMainForm();
                mainForm.Connect(tv.SelectedNode.Name, false, false, SaveInDB);
            }
        }

        private void historyTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.StartConnection(this.historyTreeView);
        }

        private void connectAsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            this.connectAsToolStripMenuItem.DropDownItems.Clear();
            this.connectAsToolStripMenuItem.DropDownItems.Add(this.userConnectToolStripMenuItem);

            List<CredentialSet> list = StoredCredentials.Items;

            foreach (CredentialSet s in list)
            {
                this.connectAsToolStripMenuItem.DropDownItems.Add(s.Name, null, this.connectAsCred_Click);
            }
        }

        private void connectAsCred_Click(object sender, EventArgs e)
        {
            FavoriteConfigurationElement fav = this.TreeView.SelectedFavorite;
            if (fav != null)
            {
                this.GetMainForm().Connect(fav.Name, this.consoleToolStripMenuItem.Checked,
                                           this.newWindowToolStripMenuItem.Checked, fav.IsDatabaseFavorite,
                                           StoredCredentials.GetByName(sender.ToString()), true);
            }
        }

        private void userConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form usrForm = new UserSelectForm();
            usrForm.ShowDialog(this.GetMainForm());
            if (CredSet != null)
            {
                FavoriteConfigurationElement fav = this.TreeView.SelectedFavorite;
                if (fav != null)
                {
                    this.GetMainForm()
                        .Connect(fav.Name, this.consoleToolStripMenuItem.Checked,
                                 this.newWindowToolStripMenuItem.Checked, fav.IsDatabaseFavorite, CredSet, false);
                }
            }
        }

        private void displayWindow_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Show();
        }

        private void displayAllWindow_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip2.Show();
        }

        #endregion

        public void SaveState()
        {
            Settings.StartDelayedUpdate();

            // Get the expanded favorites nodes in a thread safe manner
            if (this.TreeView.InvokeRequired)
            {
                this.TreeView.Invoke((System.Threading.ThreadStart)delegate { Settings.ExpandedFavoriteNodes = GetExpandedFavoriteNodes(this.TreeView); });
            }
            else
                Settings.ExpandedFavoriteNodes = GetExpandedFavoriteNodes(this.TreeView);

            // Get the expanded history nodes in a thread safe way
            if (this.historyTreeView.InvokeRequired)
            {
                this.historyTreeView.Invoke((System.Threading.ThreadStart)delegate { Settings.ExpandedHistoryNodes = GetExpandedFavoriteNodes(this.historyTreeView); });
            }
            else
                Settings.ExpandedHistoryNodes = GetExpandedFavoriteNodes(this.historyTreeView);

            Settings.SaveAndFinishDelayedUpdate();
        }

        /// <summary>
        /// Returns the currently selected node; otherwise null if nothing has been selected.
        /// </summary>
        public string SelectedTag
        {
            get
            {
                string result = this.TreeView.FindSelectedTagNodeName();

                if (string.IsNullOrEmpty(result))
                    /*
                    // If we have selected a node that has no parent it must be a TAG
                    if (this.TreeView.SelectedNode != null && this.TreeView.SelectedNode.Parent == null)
                    */
                    // There's a better way: If we have selected a node that has child nodes it must be a TAG
                    // This doesn't break the logik if a TAG might have multiple subtags in future.
                    if (this.TreeView.SelectedNode != null && this.TreeView.Nodes != null && this.TreeView.Nodes.Count > 0)
                        result = this.TreeView.SelectedNode.Name;

                // Return null if we've selected untagged favorites or the node itself.
                if (result == Settings.UNTAGGED_NODENAME)
                    return null;

                // Just to return one state not two - not string.Empty and NULL.
                // NULL is enough.
                if (string.IsNullOrEmpty(result))
                    return null;

                return result;
            }
        }

        private static string GetExpandedFavoriteNodes(TreeView treeView)
        {
            return string.Join("%%", (from TreeNode treeNode in treeView.Nodes where treeNode.IsExpanded select treeNode.Name).ToArray());
        }

        /// <summary>
        /// Load the favorits from DB and file.
        /// </summary>
        /// <param name="filter"></param>
        public void LoadFavorites(string filter = null)
        {
            if (this.favsTree != null)
                this.favsTree.Load(filter);

            try
            {
                if (dbTreeView1 != null)
                    dbTreeView1.Load(filter);
            }
            catch (Exception ex)
            {
                Log.Error("Error loading connections from database", ex);
            }
        }

        public void LoadState()
        {
            ExpandTreeView(Settings.ExpandedFavoriteNodes, this.TreeView);
            ExpandTreeView(Settings.ExpandedHistoryNodes, this.historyTreeView);
        }

        private static void ExpandTreeView(string savedNodesToExpand, TreeView treeView)
        {
            List<string> nodesToExpand = new List<string>();
            if (!string.IsNullOrEmpty(savedNodesToExpand))
                nodesToExpand.AddRange(Regex.Split(savedNodesToExpand, "%%"));

            if (nodesToExpand != null && nodesToExpand.Count > 0)
            {
                foreach (TreeNode treeNode in treeView.Nodes)
                {
                    if (nodesToExpand.Contains(treeNode.Name))
                        treeNode.Expand();
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete the SELECTED favorite?",
                                                  "Delete selected?", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                this.TreeView.SelectedFavorite.IsDatabaseFavorite = SaveInDB;

                List<FavoriteConfigurationElement> selectedFavorites = new List<FavoriteConfigurationElement>
                                                                           {
                                                                               this
                                                                                   .TreeView
                                                                                   .SelectedFavorite
                                                                           };
                Settings.DeleteFavorites(selectedFavorites);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<FavoriteConfigurationElement> selectedFavorites = new List<FavoriteConfigurationElement>
                                                                       {
                                                                           this
                                                                               .TreeView
                                                                               .SelectedFavorite
                                                                               .Copy(SaveInDB)
                                                                       };
            Settings.AddFavorites(selectedFavorites);
        }
		void FavsTreeKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
				deleteToolStripMenuItem_Click(sender, e);
		}
    }
}