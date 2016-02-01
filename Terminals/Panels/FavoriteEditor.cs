using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Kohl.Framework.ExtensionMethods;
using Kohl.Framework.Info;
using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Credentials;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Manager;
using Terminals.Connection.Panels.FavoritePanels;
using Terminals.Connections;
using Terminals.Forms;

namespace Terminals.Panels
{
    public partial class FavoriteEditor : Form
    {
        private String currentToolBarFileName;
        private String oldName;

        public string Server
        {
            get
            {
                return cmbServers.Text;
            }
        }

        public Credential Credentials
        {
        	get
        	{
        		Credential credential = this.CredentialPanel.SelectedCredential;
        		
        		if (credential != null & !credential.IsSetUserName && !credential.IsSetPassword)
        			credential = null;
        		
        		return credential;
        	}
        }
        
        public bool SaveInDB { get; set; }

        private List<String> oldTags = new List<string>();

        #region Constructors

        public FavoriteEditor(String serverName, string tagNode = null)
        {
            this.InitializeComponent();
            this.Init(null, serverName);

            if (Settings.AutoSetTag)
                if (!string.IsNullOrEmpty(tagNode))
                    this.AddTagIfNotAlreadyThere(tagNode);
        }

        public FavoriteEditor(FavoriteConfigurationElement favorite)
        {
            this.InitializeComponent();
            this.Init(favorite, String.Empty);
        }
		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			if (favoriteTabPages != null)
				foreach (TabPage page in favoriteTabPages)
	                {
	                    if (page != null && page.Controls != null && page.Controls.Count > 0 && page.Controls[0] is FavoritePanel)
	                    {
	                        FavoritePanel panel = (FavoritePanel)page.Controls[0];
							panel.Dispose();
	                    }
	                }
			
			oldTags.Clear();
			oldTags = null;
			favoriteTabPages.Clear();
			this.Favorite = null;
			
			this.Dispose();

			base.OnClosing(e);
		}
				
        #endregion

        #region Properties

        private new TerminalFormDialogResult DialogResult { get; set; }
        public FavoriteConfigurationElement Favorite { get; private set; }
        private bool ShowOnToolbar { get; set; }

        #endregion

        #region Connection Extensions

        private void SetEnabledProtocols()
        {
            // Add all protocols available
            this.ProtocolComboBox.Items.AddRange(ConnectionManager.ProtocolsCamalCase);
            // Initialize the tab pages
            this.InitializeTabPages();
            // Select the rdp protocol
            this.ProtocolComboBox.SelectedIndex = ConnectionManager.ProtocolsCamalCase.Select(x => x.ToUpper()).ToList().IndexOf(typeof(RDPConnection).GetProtocolName().ToUpper());
        }

        #endregion

        private void InitializeTabPages()
        {
            TabPage[] tabPages = null;
            this.SetTabPage(tabPages);
        }

        private void SetTabPage(TabPage tabPage, bool preserve = false)
        {
            this.SetTabPage(new[] {tabPage}, preserve);
        }

        private void SetTabPage(TabPage[] tabPages, bool preserve = false)
        {
            if (tabPages == null)
            {
                this.TabControl1.TabPages.Clear();
                this.TabControl1.TabPages.Add(this.tabGeneral);
                this.TabControl1.TabPages.Add(this.tabTags);
                this.TabControl1.TabPages.Add(this.tabExecute);
            }
            else
            {
                if (!preserve)
                    foreach (TabPage p in this.TabControl1.TabPages)
                    {
                        Application.DoEvents();

                        if (p == null)
                            continue;

                        if (p != this.tabGeneral && p != this.tabTags && p != this.tabExecute)
                            this.TabControl1.TabPages.Remove(p);
                    }

                int counter = 1;

                foreach (TabPage p in tabPages)
                {
                    Application.DoEvents();

                    if (p == null)
                        continue;

                    if (!this.TabControl1.TabPages.ContainsKey(p.Name))
                        this.TabControl1.TabPages.Insert(counter, p);

                    counter++;
                }
            }
        }

        bool isEditing = false;

        private void Init(FavoriteConfigurationElement favorite, String serverName)
        {
            SetFavoritePanels(null, true);
            this.SetEnabledProtocols();
            
            this.LoadMRUs();
            this.SetOkButtonState();
            this.SSHPreferences.Keys = Settings.SSHKeys;

            if (favorite == null)
            {
                this.CredentialPanel.FillCredentials(null);
                
                FavoriteConfigurationElement defaultFav = Settings.GetDefaultFavorite();
                if (defaultFav != null)
                {
                    this.FillControls(defaultFav);
                }

                Int32 port;
                GetServerAndPort(serverName, out serverName, out port);
                this.cmbServers.Text = serverName;
                this.txtPort.Text = port.ToString();
            }
            else
            {
                isEditing = true;
                this.oldName = favorite.Name;
                this.oldTags = favorite.TagList;
                this.Text = "Edit Connection";
                this.FillControls(favorite);
            }

            this.cmbHtmlFormField1Value.Items.AddRange(MiniBrowser.FieldConstants);
            this.cmbHtmlFormField2Value.Items.AddRange(MiniBrowser.FieldConstants);
            this.cmbHtmlFormField3Value.Items.AddRange(MiniBrowser.FieldConstants);
            this.cmbHtmlFormField4Value.Items.AddRange(MiniBrowser.FieldConstants);
            this.cmbHtmlFormField5Value.Items.AddRange(MiniBrowser.FieldConstants);
            this.cmbHtmlFormField6Value.Items.AddRange(MiniBrowser.FieldConstants);

            this.cmbHttpAuthentication.Items.AddRange(Enum.GetNames(typeof (BrowserAuthentication)));
            this.cmbHttpBrowser.Items.AddRange(Enum.GetNames(typeof (BrowserType)));
            
            if (string.IsNullOrEmpty(this.cmbHttpAuthentication.Text))
            {
                this.cmbHttpAuthentication.Text = Enum.GetName(typeof (BrowserAuthentication),
                                                               BrowserAuthentication.None);
            }

            if (string.IsNullOrEmpty(this.cmbHttpBrowser.Text))
            {
                this.cmbHttpBrowser.Text = Enum.GetName(typeof (BrowserType), BrowserType.InternetExplorer);
            }
        }

        private void NewTerminalForm_Load(Object sender, EventArgs e)
        {
            this.SuspendLayout();

            foreach (String tag in Settings.Tags(SaveInDB))
            {
                ListViewItem lvi = new ListViewItem(tag);
                this.AllTagsListView.Items.Add(lvi);
            }

            this.ResumeLayout(true);
        }

        private void LoadMRUs()
        {
            this.cmbServers.Items.AddRange(Settings.MRUServerNames);
            this.CredentialPanel.LoadMRUs();
            this.txtTag.AutoCompleteCustomSource.AddRange(Settings.Tags(SaveInDB));
        }

        private void SaveMRUs()
        {
            Settings.AddServerMRUItem(this.cmbServers.Text);
            this.CredentialPanel.SaveMRUs();
        }

        readonly List<TabPage> favoriteTabPages = new List<TabPage>();

        private void FillControls(FavoriteConfigurationElement favorite)
        {
            foreach (TabPage page in favoriteTabPages)
            {
                FavoritePanel panel = page.Controls.Cast<Control>().Where(control => control is FavoritePanel).Cast<FavoritePanel>().FirstOrDefault();
                
                if (panel != null)
                    panel.FillControls(favorite);
            }

            this.tabColorPreferences.FillControls(favorite);
            this.CredentialPanel.FillControls(favorite);

            if (favorite.HtmlFormFields.Length >= 1)
            {
                this.txtHtmlFormField1Key.Text = favorite.HtmlFormFields[0].Key;
                this.cmbHtmlFormField1Value.Text = favorite.HtmlFormFields[0].Value;
            }

            if (favorite.HtmlFormFields.Length >= 2)
            {
                this.txtHtmlFormField2Key.Text = favorite.HtmlFormFields[1].Key;
                this.cmbHtmlFormField2Value.Text = favorite.HtmlFormFields[1].Value;
            }

            if (favorite.HtmlFormFields.Length >= 3)
            {
                this.txtHtmlFormField3Key.Text = favorite.HtmlFormFields[2].Key;
                this.cmbHtmlFormField3Value.Text = favorite.HtmlFormFields[2].Value;
            }

            if (favorite.HtmlFormFields.Length >= 4)
            {
                this.txtHtmlFormField4Key.Text = favorite.HtmlFormFields[3].Key;
                this.cmbHtmlFormField4Value.Text = favorite.HtmlFormFields[3].Value;
            }

            if (favorite.HtmlFormFields.Length >= 5)
            {
                this.txtHtmlFormField5Key.Text = favorite.HtmlFormFields[4].Key;
                this.cmbHtmlFormField5Value.Text = favorite.HtmlFormFields[4].Value;
            }

            if (favorite.HtmlFormFields.Length >= 6)
            {
                this.txtHtmlFormField6Key.Text = favorite.HtmlFormFields[5].Key;
                this.cmbHtmlFormField6Value.Text = favorite.HtmlFormFields[5].Value;
            }

            this.cmbHttpAuthentication.Text = favorite.BrowserAuthentication.ToString();
            this.cmbHttpBrowser.Text = favorite.HttpBrowser.ToString();

            this.consolePreferences.FillControls(favorite);

            this.NewWindowCheckbox.Checked = favorite.NewWindow;

            for (int i = 0; i < this.ProtocolComboBox.Items.Count; i++)
            {
                if (this.ProtocolComboBox.Items[i].ToString().ToUpper() == favorite.Protocol.ToUpper())
                {
                    this.ProtocolComboBox.SelectedIndex = i;
                    break;
                }
            }

            this.VMRCAdminModeCheckbox.Checked = favorite.VmrcAdministratorMode;

            this.vncAutoScaleCheckbox.Checked = favorite.VncAutoScale;
            this.vncDisplayNumberInput.Value = favorite.VncDisplayNumber;
            this.VncViewOnlyCheckbox.Checked = favorite.VncViewOnly;

            this.VMRCReducedColorsCheckbox.Checked = favorite.VmrcReducedColorsMode;
            this.txtName.Text = favorite.Name;
            this.cmbServers.Text = favorite.ServerName;

            this.chkAddtoToolbar.Checked = Settings.HasToolbarButton(favorite.Name);

            this.httpUrlTextBox.Text = favorite.Url;

            this.txtPort.Text = favorite.Port.ToString();
            
            this.chkExecuteBeforeConnect.Checked = favorite.ExecuteBeforeConnect;
            this.txtCommand.Text = favorite.ExecuteBeforeConnectCommand;
            this.txtArguments.Text = favorite.ExecuteBeforeConnectArgs;
            this.txtInitialDirectory.Text = favorite.ExecuteBeforeConnectInitialDirectory;
            this.chkWaitForExit.Checked = favorite.ExecuteBeforeConnectWaitForExit;

            this.ReloadTagsListViewItems(favorite);

            if (favorite.ToolBarIcon != null && File.Exists(favorite.ToolBarIcon))
            {
            	Image image = Image.FromFile(favorite.ToolBarIcon);

                this.picCustomIcon.Image = (Image)image.Clone();

                if (image != null)
                {
                    image.Dispose();
                    image = null;
                }

                this.currentToolBarFileName = favorite.ToolBarIcon;
            }

            this.ICAClientINI.Text = favorite.IcaClientIni;
            this.ICAServerINI.Text = favorite.IcaServerIni;
            this.ICAEncryptionLevelCombobox.Text = favorite.IcaEncryptionLevel;
            this.ICAEnableEncryptionCheckbox.Checked = favorite.IcaEnableEncryption;
            this.ICAEncryptionLevelCombobox.Enabled = this.ICAEnableEncryptionCheckbox.Checked;

            this.ICAApplicationNameTextBox.Text = favorite.IcaApplicationName;

            this.NotesTextbox.Text = favorite.Notes;

            this.SSHPreferences.AuthMethod = favorite.AuthMethod;
            this.SSHPreferences.KeyTag = favorite.KeyTag;
            this.SSHPreferences.SSH1 = favorite.Ssh1;

            this.CredentialPanel.FillCredentials(favorite.XmlCredentialSetName);
        }

        private void ReloadTagsListViewItems(FavoriteConfigurationElement favorite)
        {
            List<string> tagsArray = favorite.TagList;
            this.lvConnectionTags.Items.Clear();
            foreach (String tag in tagsArray)
            {
                this.lvConnectionTags.Items.Add(tag, tag, -1);
            }
        }

        private Boolean FillFavorite(Boolean defaultFav)
        {
            try
            {
                string name = (String.IsNullOrEmpty(this.txtName.Text) ? this.cmbServers.Text : this.txtName.Text);

                if (this.Favorite == null)
                    this.Favorite = new FavoriteConfigurationElement();

                // we create a new one -> check if the name has already been used
                // if yes append the name with a guid.
                // check too if user has decided to rename the favorite
                if (string.IsNullOrEmpty(this.oldName) || name != this.oldName)
                    name = FavoriteConfigurationElement.CreateNewFavoriteNameIfAlreadyExists(name, SaveInDB);

                this.consolePreferences.FillFavorite(this.Favorite);

                // Check if the port is a mandatory field (this depends on the connection):
                Type type = ConnectionManager.GetConnectionTypes().FirstOrDefault(t => t.IsEqual(this.ProtocolComboBox.SelectedItem.ToString()));

                if (type != null)
                    using (Connection.Connection connection = (Connection.Connection)Activator.CreateInstance(type))
                        if (connection.IsPortRequired)
                            if (!this.IsPortValid())
                                return false;

                foreach (TabPage page in favoriteTabPages)
                {
                    if (page != null && page.Controls != null && page.Controls.Count > 0 && page.Controls[0] is FavoritePanel)
                    {
                        FavoritePanel panel = (FavoritePanel)page.Controls[0];
                        panel.FillFavorite(this.Favorite);
                    }
                }

                this.tabColorPreferences.FillFavorite(this.Favorite);
                this.CredentialPanel.FillFavorite(this.Favorite);

                this.Favorite.HtmlFormFields = new[]
                                                   {
                                                       new HtmlFormField
                                                           {
                                                               Key = this.txtHtmlFormField1Key.Text,
                                                               Value = this.cmbHtmlFormField1Value.Text
                                                           },
                                                       new HtmlFormField
                                                           {
                                                               Key = this.txtHtmlFormField2Key.Text,
                                                               Value = this.cmbHtmlFormField2Value.Text
                                                           },
                                                       new HtmlFormField
                                                           {
                                                               Key = this.txtHtmlFormField3Key.Text,
                                                               Value = this.cmbHtmlFormField3Value.Text
                                                           },
                                                       new HtmlFormField
                                                           {
                                                               Key = this.txtHtmlFormField4Key.Text,
                                                               Value = this.cmbHtmlFormField4Value.Text
                                                           },
                                                       new HtmlFormField
                                                           {
                                                               Key = this.txtHtmlFormField5Key.Text,
                                                               Value = this.cmbHtmlFormField5Value.Text
                                                           },
                                                       new HtmlFormField
                                                           {
                                                               Key = this.txtHtmlFormField6Key.Text,
                                                               Value = this.cmbHtmlFormField6Value.Text
                                                           }
                                                   };

                this.Favorite.BrowserAuthentication =
                    (BrowserAuthentication)
                    Enum.Parse(typeof (BrowserAuthentication), this.cmbHttpAuthentication.Text, true);
                this.Favorite.HttpBrowser =
                    (BrowserType) Enum.Parse(typeof (BrowserType), this.cmbHttpBrowser.Text, true);

                this.Favorite.Name = name;

                this.Favorite.VmrcAdministratorMode = this.VMRCAdminModeCheckbox.Checked;
                this.Favorite.VmrcReducedColorsMode = this.VMRCReducedColorsCheckbox.Checked;

                this.Favorite.VncAutoScale = this.vncAutoScaleCheckbox.Checked;
                this.Favorite.VncDisplayNumber = (Int32) this.vncDisplayNumberInput.Value;
                this.Favorite.VncViewOnly = this.VncViewOnlyCheckbox.Checked;

                this.Favorite.NewWindow = this.NewWindowCheckbox.Checked;

                this.Favorite.Protocol = this.ProtocolComboBox.SelectedItem.ToString();
                this.Favorite.Port = Int32.Parse(this.txtPort.Text);

                if (!defaultFav)
                    this.Favorite.ServerName = this.cmbServers.Text;

                CredentialSet set = this.CredentialPanel.SelectedCredentialSet;
                this.Favorite.XmlCredentialSetName = (set == null ? String.Empty : set.Name);

                
                this.ShowOnToolbar = this.chkAddtoToolbar.Checked;

                this.Favorite.ExecuteBeforeConnect = this.chkExecuteBeforeConnect.Checked;
                this.Favorite.ExecuteBeforeConnectCommand = this.txtCommand.Text;
                this.Favorite.ExecuteBeforeConnectArgs = this.txtArguments.Text;
                this.Favorite.ExecuteBeforeConnectInitialDirectory = this.txtInitialDirectory.Text;
                this.Favorite.ExecuteBeforeConnectWaitForExit = this.chkWaitForExit.Checked;
                this.Favorite.ToolBarIcon = this.currentToolBarFileName;

                this.Favorite.IcaApplicationName = this.ICAApplicationNameTextBox.Text;
                
                this.Favorite.Url = this.httpUrlTextBox.Text;

                this.Favorite.IcaClientIni = this.ICAClientINI.Text;
                this.Favorite.IcaServerIni = this.ICAServerINI.Text;
                this.Favorite.IcaEncryptionLevel = this.ICAEncryptionLevelCombobox.Text;
                this.Favorite.IcaEnableEncryption = this.ICAEnableEncryptionCheckbox.Checked;
                this.Favorite.IcaApplicationName = this.ICAApplicationNameTextBox.Text;

                this.Favorite.Notes = this.NotesTextbox.Text;

                this.Favorite.KeyTag = this.SSHPreferences.KeyTag;
                this.Favorite.Ssh1 = this.SSHPreferences.SSH1;
                this.Favorite.AuthMethod = this.SSHPreferences.AuthMethod;

                List<String> updatedTags = this.UpdateFavoriteTags();

                // Write our settings to the database instead of writing it to a file if true
                Favorite.IsDatabaseFavorite = SaveInDB;

                if (defaultFav)
                    this.SaveDefaultFavorite();
                else
                    this.CommitChangesToSettings(updatedTags);

                return true;
            }
            catch (Exception ex)
            {
                Log.Info("Filling the favorites has failed.", ex);
                this.ShowErrorMessageBox(ex.Message);
                return false;
            }
        }

        private void SaveDefaultFavorite()
        {
            this.Favorite.Name = String.Empty;
            this.Favorite.ServerName = String.Empty;
            this.Favorite.DomainName = String.Empty;
            this.Favorite.UserName = String.Empty;
            this.Favorite.Password = String.Empty;
            this.Favorite.Notes = String.Empty;
            this.Favorite.EnableSecuritySettings = false;
            this.Favorite.SecurityWorkingFolder = String.Empty;
            this.Favorite.SecurityStartProgram = String.Empty;
            this.Favorite.SecurityFullScreen = false;
            this.Favorite.Url = String.Empty;
            Settings.SaveDefaultFavorite(this.Favorite);
        }

        private void CommitChangesToSettings(List<string> updatedTags)
        {
            Settings.StartDelayedUpdate();
            if (String.IsNullOrEmpty(this.oldName))
            {
                Settings.AddFavorite(this.Favorite);
                if (this.ShowOnToolbar)
                    Settings.AddFavoriteButton(this.Favorite.Name);
            }
            else
            {
                Settings.EditFavorite(this.oldName, this.Favorite);
                Settings.EditFavoriteButton(this.oldName, this.Favorite.Name, this.ShowOnToolbar);
                this.UpdateTags(updatedTags, SaveInDB);
            }
            Settings.SaveAndFinishDelayedUpdate();
        }

        private void ShowErrorMessageBox(string message)
        {
            MessageBox.Show(this, message, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        ///     Call this after favorite was saved, otherwise the removed, otherwise not used tags wouldnt be removed
        /// </summary>
        private void UpdateTags(List<string> updatedTags, bool isDatabaseFavorite)
        {
            List<String> tagsToRemove = this.oldTags.GetMissingSourcesInTarget(updatedTags);
            List<String> tagsToAdd = updatedTags.GetMissingSourcesInTarget(this.oldTags);
            Settings.AddTags(tagsToAdd, isDatabaseFavorite);
            Settings.DeleteTags(tagsToRemove, SaveInDB);
        }

        /// <summary>
        ///     Confirms changes into the favorite tags and returns collection of newly assigned tags.
        /// </summary>
        private List<String> UpdateFavoriteTags()
        {
            List<String> updatedTags = new List<String>();
            foreach (ListViewItem listViewItem in this.lvConnectionTags.Items)
                updatedTags.Add(listViewItem.Text);

            this.Favorite.Tags = String.Join(",", updatedTags.ToArray());
            return updatedTags;
        }

        private bool IsPortValid()
        {
            Int32 result;
            if (Int32.TryParse(this.txtPort.Text, out result) && result >= 0 && result < 65536)
                return true;

            this.ShowErrorMessageBox("Port must be a number between 0 and 65535");
            this.txtPort.Focus();
            return false;
        }

        private void SetOkButtonState()
        {
            this.btnSave.Enabled = this.txtName.Text != String.Empty;
        }

        private static void GetServerAndPort(String Connection, out String Server, out Int32 Port)
        {
            Server = Connection;
            Port = ConnectionManager.GetPort(typeof (RDPConnection).GetProtocolName());
            if (Connection != null && Connection.Trim() != String.Empty && Connection.Contains(":"))
            {
                String server = Connection.Substring(0, Connection.IndexOf(":"));
                String rawPort = Connection.Substring(Connection.IndexOf(":") + 1);
                Int32 port = ConnectionManager.GetPort(typeof (RDPConnection).GetProtocolName());
                if (rawPort != null && rawPort.Trim() != String.Empty)
                {
                    rawPort = rawPort.Trim();
                    Int32.TryParse(rawPort, out port);
                }

                Server = server;
                Port = port;
            }
        }

        private void AddTag()
        {
            if (!String.IsNullOrEmpty(this.txtTag.Text))
            {
                this.AddTagIfNotAlreadyThere(this.txtTag.Text);
            }
        }

        private void AddTagsToFavorite()
        {
            foreach (ListViewItem lv in this.AllTagsListView.SelectedItems)
            {
                this.AddTagIfNotAlreadyThere(lv.Text);
            }
        }

        private void AddTagIfNotAlreadyThere(String selectedTag)
        {
            ListViewItem[] items = this.lvConnectionTags.Items.Find(selectedTag, false);
            if (items.Length == 0)
            {
                this.lvConnectionTags.Items.Add(selectedTag);
            }
        }

        private void DeleteTag()
        {
            foreach (ListViewItem item in this.lvConnectionTags.SelectedItems)
            {
                this.lvConnectionTags.Items.Remove(item);
            }
        }

        /// <summary>
        ///     Overload ShowDialog and return custom result.
        /// </summary>
        /// <returns> Returns custom dialogresult. </returns>
        public new TerminalFormDialogResult ShowDialog()
        {
            try
            {
                base.ShowDialog();
            }
            catch (Exception ex)
            {
                Kohl.Framework.Logging.Log.Fatal("Unalbe to show dialog", ex);
            }

            return this.DialogResult;
        }

        /// <summary>
        ///     Save favorite and close form. If the form isnt valid the form control is focused.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.SaveMRUs();
            if (this.FillFavorite(false))
            {
                this.DialogResult = TerminalFormDialogResult.SaveAndClose;
                this.Close();
            }
        }

        /// <summary>
        ///     Save favorite, close form and immediatly connect to the favorite.
        /// </summary>
        private void saveConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveMRUs();

            if (this.FillFavorite(false))
                this.DialogResult = TerminalFormDialogResult.SaveAndConnect;

            this.Close();
        }

        /// <summary>
        ///     Save favorite and clear form for a new favorite.
        /// </summary>
        private void saveNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveMRUs();
            if (this.FillFavorite(false))
            {
                this.Favorite = null;
                this.oldName = String.Empty;
                this.Init(null, String.Empty);
                this.cmbServers.Focus();
            }
        }

        /// <summary>
        ///     Save favorite and copy the current favorite settings, except favorite and connection name.
        /// </summary>
        private void saveCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveMRUs();
            if (this.FillFavorite(false))
            {
                this.txtName.Text = FavoriteConfigurationElement.CreateNewFavoriteNameForCopy(this.Favorite.Name, SaveInDB);
                this.Favorite = null;
                this.oldName = String.Empty;
                this.cmbServers.Text = String.Empty;
                this.cmbServers.Focus();
            }
        }

        private void control_TextChanged(object sender, EventArgs e)
        {
            this.SetOkButtonState();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            this.SetOkButtonState();
        }

        private void NewTerminalForm_Shown(object sender, EventArgs e)
        {
            this.cmbServers.Focus();
        }

        private void cmbServers_TextChanged(object sender, EventArgs e)
        {
            this.SetOkButtonState();
        }

        private void cmbServers_Leave(object sender, EventArgs e)
        {
            if (this.txtName.Text == String.Empty)
            {
                if (this.cmbServers.Text.Contains(":"))
                {
                    String server = String.Empty;
                    int port;
                    GetServerAndPort(this.cmbServers.Text, out server, out port);
                    this.cmbServers.Text = server;
                    this.txtPort.Text = port.ToString();
                    this.cmbServers.Text = server;
                }

                this.txtName.Text = this.cmbServers.Text;
            }
        }

        private void btnAddNewTag_Click(object sender, EventArgs e)
        {
            this.AddTag();
        }

        private void btnRemoveTag_Click(object sender, EventArgs e)
        {
            this.DeleteTag();
        }

        private void lvConnectionTags_DoubleClick(object sender, EventArgs e)
        {
            this.DeleteTag();
        }

        private void btnSaveDefault_Click(object sender, EventArgs e)
        {
            this.contextMenuStripDefaults.Show(this.btnSaveDefault, 0, this.btnSaveDefault.Height);
        }

        private void saveCurrentSettingsAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.FillFavorite(true);
        }

        private void removeSavedDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.RemoveDefaultFavorite();
        }

        private void SetFavoritePanels(TabPage[] tabPagesToLoad, bool init)
        {
            TabPage[] tabPages = null;
            this.SetTabPage(tabPages);

            List<Type> favoritePanelTypes = ConnectionManager.GetFavoritePanels();

            // Load all favorite tab pages, set the name to match the protocol name
            // Either an existing favorite tab page or a existing panel or initialize one.
            foreach (Type type in favoritePanelTypes)
            {
                FavoritePanel panel = null;
                TabPage tabPage = null;

                foreach (TabPage page in favoriteTabPages)
                {
                    Application.DoEvents();
                    if (page != null && page.Controls != null && page.Controls.Count > 0 && page.Controls[0] is FavoritePanel)
                    {
                        if (((FavoritePanel)page.Controls[0]).Name == type.Name)
                        {
                            panel = (FavoritePanel)page.Controls[0];
                            tabPage = page;
                        }
                    }
                }

                if (panel == null)
                {
                    try
                    {
                        Application.DoEvents();
                        panel = (FavoritePanel)Activator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        //string message = ex.InnerException.Message;
                        Log.Debug("ERROR ceating favorite panel! Type name: " + type.Name, ex);
                        continue;
                    }
                }

                if (tabPage == null)
                    tabPage = AddTabPage(panel, panel.Name);

                tabPage.Name = type.GetProtocolName();

                // This prevents the CredentialPanel and maybe some other types that have no protocol names from being added.
                if (!favoriteTabPages.Contains(tabPage) && panel.ProtocolName != null && panel.ProtocolName.Length >= 1 && (!string.IsNullOrEmpty(panel.ProtocolName[0])))
                    favoriteTabPages.Add(tabPage);
            }

            List<TabPage> loadTabPages = new List<TabPage>();

            TabPage defaultTabPage = GetTabPageByProtocolName(this.ProtocolComboBox.Text.ToUpper());

            // Fall back - backward compatbility
            if (defaultTabPage == null)
                if (tabPagesToLoad == null)
                    return;
                else
                    loadTabPages.AddRange(tabPagesToLoad);
            // new system
            else
                loadTabPages.Add(defaultTabPage);

            foreach (TabPage page in favoriteTabPages)
            {
                FavoritePanel panel = (FavoritePanel)page.Controls[0];

                panel.ParentForm = this;
                panel.HandleSelectionChanged(this.ProtocolComboBox.Text, isEditing);

                foreach (string protocolName in panel.ProtocolName)
                    if (this.ProtocolComboBox.Text.ToUpper() == protocolName.ToUpper())
                    {
                        if (!loadTabPages.Contains (page))
                            loadTabPages.Add(page);
                    }
            }
            
            this.SetTabPage(loadTabPages.ToArray(), true);
        }

        private TabPage GetTabPageByProtocolName(string protocolName)
        {
            return favoriteTabPages.FirstOrDefault(currentPanel => currentPanel.Controls.Count >= 1 && currentPanel.Controls[0] is FavoritePanel && ((FavoritePanel)currentPanel.Controls[0]).GetType().IsEqual(protocolName));
        }

        private FavoritePanel GetFavoritePanelByProtocolName(string protocolName)
        {
            TabPage p = GetTabPageByProtocolName(protocolName);

            if (p == null)
                return null;

            return ((FavoritePanel)p.Controls[0]);
        }

        private TabPage AddTabPage(FavoritePanel panel, string protocolName)
        {
            if (panel == null)
                return null;

            string name = "tab" + protocolName;
            TabPage tabPage = favoriteTabPages.FirstOrDefault(t => t.Name == name);

            if (tabPage == null)
            {
                tabPage = new TabPage(panel.Text) { Name = name };

                //if (!favoriteTabPages.Contains(tabPage))
                //    favoriteTabPages.Add(tabPage);

                tabPage.Controls.Add(panel);
                panel.Dock = DockStyle.Fill;
                panel.AutoSize = true;
            }
            
            return tabPage;
        }

        private void ProtocolComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.cmbServers.Enabled = true;
            this.txtPort.Enabled = true;

            this.vncAutoScaleCheckbox.Enabled = false;
            this.vncDisplayNumberInput.Enabled = false;
            this.VncViewOnlyCheckbox.Enabled = false;

            this.VMRCReducedColorsCheckbox.Enabled = false;
            this.VMRCAdminModeCheckbox.Enabled = false;
            this.RASGroupBox.Enabled = false;

            this.ICAClientINI.Enabled = false;
            this.ICAServerINI.Enabled = false;
            this.ICAEncryptionLevelCombobox.Enabled = false;
            this.ICAEnableEncryptionCheckbox.Enabled = false;
            this.ICAApplicationNameTextBox.Enabled = false;

            this.httpUrlTextBox.Enabled = false;
            this.txtPort.Enabled = true;

            List<TabPage> loadTabPages = new List<TabPage>();

            if (typeof (VMRCConnection).IsEqual(this.ProtocolComboBox.Text))
            {
                loadTabPages.Add(this.tabVMRC);
                this.VMRCReducedColorsCheckbox.Enabled = true;
                this.VMRCAdminModeCheckbox.Enabled = true;
            }
            else if (typeof (RASConnection).IsEqual(this.ProtocolComboBox.Text))
            {
                loadTabPages.Add(this.tabRAS);
                this.cmbServers.Items.Clear();
                this.RASGroupBox.Enabled = true;
                this.txtPort.Enabled = false;
                this.RASDetailsListBox.Items.Clear();
            }
            else if (typeof (VNCConnection).IsEqual(this.ProtocolComboBox.Text))
            {
                //vnc settings
                loadTabPages.Add(this.tabVNC);
                this.vncAutoScaleCheckbox.Enabled = true;
                this.vncDisplayNumberInput.Enabled = true;
                this.VncViewOnlyCheckbox.Enabled = true;
            }
            else if (typeof (TerminalConnection).IsEqual(this.ProtocolComboBox.Text))
            {
                loadTabPages.Add(this.tabConsole);
            }
            else if (typeof (SshConnection).IsEqual(this.ProtocolComboBox.Text))
            {
                loadTabPages.AddRange(new[] { this.tabSSH, this.tabConsole });
            }
            else if (typeof (ICAConnection).IsEqual(this.ProtocolComboBox.Text))
            {
                loadTabPages.Add(this.tabICA);
                this.ICAClientINI.Enabled = true;
                this.ICAServerINI.Enabled = true;
                this.ICAEncryptionLevelCombobox.Enabled = false;
                this.ICAEnableEncryptionCheckbox.Enabled = true;
                this.ICAApplicationNameTextBox.Enabled = true;
            }
            else if (typeof (HTTPConnection).IsEqual(this.ProtocolComboBox.Text))
            {
                loadTabPages.Add(this.tabHTTP);
                this.tabHTTP.Text = typeof (HTTPConnection).GetProtocolName();
                if (string.IsNullOrEmpty(this.httpUrlTextBox.Text) || this.httpUrlTextBox.Text == "https://")
                {
                    this.httpUrlTextBox.Text = "http://";
                }

                if (this.httpUrlTextBox.Text.Contains("http://"))
                {
                    this.httpUrlTextBox.Text = this.httpUrlTextBox.Text.Replace("https://", "http://");
                }

                this.cmbServers.Enabled = false;
                this.httpUrlTextBox.Enabled = true;
            }
            else if (typeof (HTTPSConnection).IsEqual(this.ProtocolComboBox.Text))
            {
                loadTabPages.Add(this.tabHTTP);
                this.tabHTTP.Text = typeof (HTTPSConnection).GetProtocolName();
                if (string.IsNullOrEmpty(this.httpUrlTextBox.Text) || this.httpUrlTextBox.Text == "http://")
                {
                    this.httpUrlTextBox.Text = "https://";
                }

                if (this.httpUrlTextBox.Text.Contains("http://"))
                {
                    this.httpUrlTextBox.Text = this.httpUrlTextBox.Text.Replace("http://", "https://");
                }

                this.cmbServers.Enabled = false;
                this.httpUrlTextBox.Enabled = true;
            }
            else if (typeof (GenericConnection).IsEqual(this.ProtocolComboBox.Text))
            {
                this.cmbServers.Enabled = false;
                this.httpUrlTextBox.Enabled = true;
            }
            else if (typeof (ExplorerConnection).IsEqual(this.ProtocolComboBox.Text))
            {
                this.cmbServers.Enabled = false;
                this.httpUrlTextBox.Enabled = true;
            }

            int defaultPort = ConnectionManager.GetPort(this.ProtocolComboBox.Text);

            if (defaultPort <= 0)
                this.txtPort.Enabled = false;

            this.txtPort.Text = defaultPort.ToString();

            SetFavoritePanels(loadTabPages.ToArray(), false);
        }

        public string SetToolBarIcon(Image image, string fileNameWithoutExtension, bool overwrite = false)
        {
            if (image == null)
                return string.Empty;

            string fileName = System.IO.Path.GetTempPath() + fileNameWithoutExtension + ".png";
            
            // Delete a temporary file if it already exists
            if (File.Exists(fileName))
                File.Delete(fileName);

            image.Save(fileName);
            
            string returnValue = SetToolBarIcon(fileName, ref image, overwrite);

            if (image != null)
            {
                if (string.IsNullOrEmpty(this.currentToolBarFileName) || overwrite)
                {
                    this.picCustomIcon.Image = (Image)image.Clone();
                    this.currentToolBarFileName = returnValue;

                    if (this.Favorite != null)
                        this.Favorite.ToolBarIcon = this.currentToolBarFileName;
                }

                image.Dispose();
                image = null;
            }

            // Clean up - delete the file
            if (File.Exists(fileName))
                File.Delete(fileName);

            return returnValue;
        }

        public string SetToolBarIcon(string filename, ref Image image, bool overwrite = false)
        {
            try
            {
                image = Image.FromFile(filename);

                if (image != null)
                {
                    String newFile = Path.Combine(Kohl.Framework.Info.AssemblyInfo.DirectoryConfigFiles, Path.GetFileName(filename));

                    if (newFile != filename && (!File.Exists(newFile) || overwrite))
                        File.Copy(filename, newFile);

                    return newFile;
                }
            }
            catch (Exception ex)
            {
                Log.Info("Set image failed.", ex);
            }

            image = null;
            return string.Empty;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
                                                {
                                                    CheckFileExists = true,
                                                    InitialDirectory = Kohl.Framework.Info.AssemblyInfo.Directory,
                                                    Filter = "All files (*.*)|*.*|TIFF Files (*.tiff)|*.tiff|Bitmap files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif|ICON Files (*.ico)|*.ico|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg",
                                                    Multiselect = false,
                                                    Title = "Select a custom connection image."
                                                };
            
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Image image = null;
                this.currentToolBarFileName = SetToolBarIcon(openFileDialog.FileName, ref image);

                if (image != null)
                {
                    this.picCustomIcon.Image = (Image)image.Clone();

                    if (image != null)
                    {
                        image.Dispose();
                        image = null;
                    }
                }
            }
        }

        private void AllTagsAddButton_Click(object sender, EventArgs e)
        {
            this.AddTagsToFavorite();
        }

        private void ICAEnableEncryptionCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            this.ICAEncryptionLevelCombobox.Enabled = this.ICAEnableEncryptionCheckbox.Checked;
        }

        private void ChangeHTTPTab(bool https)
        {
            if (https)
            {
                this.ProtocolComboBox.SelectedIndexChanged -= this.ProtocolComboBox_SelectedIndexChanged;
                this.ProtocolComboBox.Text = "HTTPS";
                this.ProtocolComboBox.SelectedIndexChanged += this.ProtocolComboBox_SelectedIndexChanged;
                this.tabHTTP.Text = typeof (HTTPSConnection).GetProtocolName();

                if (this.txtPort.Text == "80")
                {
                    this.txtPort.Text = "443";
                }
                return;
            }

            this.ProtocolComboBox.SelectedIndexChanged -= this.ProtocolComboBox_SelectedIndexChanged;
            this.ProtocolComboBox.Text = "HTTP";
            this.ProtocolComboBox.SelectedIndexChanged += this.ProtocolComboBox_SelectedIndexChanged;
            this.tabHTTP.Text = typeof (HTTPConnection).GetProtocolName();

            if (this.txtPort.Text == "443")
            {
                this.txtPort.Text = "80";
            }
        }

        private void httpUrlTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.ProtocolComboBox.Text == "HTTP" | this.ProtocolComboBox.Text == "HTTPS")
            {
                string url = this.httpUrlTextBox.Text;

                try
                {
                    if (this.ProtocolComboBox.Text == "HTTP" && url.StartsWith("https://"))
                    {
                        this.ChangeHTTPTab(true);
                    }

                    if (this.ProtocolComboBox.Text == "HTTPS" && url.StartsWith("http://"))
                    {
                        this.ChangeHTTPTab(false);
                    }

                    if (url == "http://" || url == "https://")
                        return;

                    Uri uri = new Uri(url);
                    this.cmbServers.Text = uri.Host;

                    // If the port has been recognized as 443 and the protocol HTTP has been selected
                    // than auto-correct the protocol to HTTPS.
                    if (url.StartsWith("http://") && uri.Port == 443)
                    {
                        this.ChangeHTTPTab(true);
                    }
                    // or vice versa
                    if (url.StartsWith("https://") && uri.Port == 80)
                    {
                        this.ChangeHTTPTab(false);
                    }

                    this.txtPort.Text = uri.Port.ToString();
                }
                catch (Exception ex)
                {
                    Log.Error(this.ProtocolComboBox.Text + " url parse failed.", ex);
                }
            }
        }

        private void httpUrlTextBox_Leave(object sender, EventArgs e)
        {
            if (this.ProtocolComboBox.Text == "HTTP" | this.ProtocolComboBox.Text == "HTTPS")
            {
                string url = this.httpUrlTextBox.Text;

                if (url == "http://" || url == "https://")
                    return;

                try
                {
                    Uri uri = new Uri(url);
                    this.cmbServers.Text = uri.Host;

                    // Replace redundant specifications for HTTP
                    if (url.StartsWith("http://") && uri.Port == 80)
                    {
                        this.httpUrlTextBox.Text = this.httpUrlTextBox.Text.Replace(":80", "");
                    }
                    // Replace redundant specifications for HTTPS
                    if (url.StartsWith("https://") && uri.Port == 443)
                    {
                        this.httpUrlTextBox.Text = this.httpUrlTextBox.Text.Replace(":443", "");
                    }

                    // If the port has been recognized as 443 and the protocol HTTP has been selected
                    // than auto-correct the protocol to HTTPS.
                    if (url.StartsWith("http://") && uri.Port == 443)
                    {
                        this.ChangeHTTPTab(true);
                        this.httpUrlTextBox.Text = this.httpUrlTextBox.Text.Replace(":443", "")
                                                       .Replace("http://", "https://");
                    }
                    // or vice versa
                    if (url.StartsWith("https://") && uri.Port == 80)
                    {
                        this.ChangeHTTPTab(false);
                        this.httpUrlTextBox.Text = this.httpUrlTextBox.Text.Replace(":80", "")
                                                       .Replace("https://", "http://");
                    }

                    this.txtPort.Text = uri.Port.ToString();
                }
                catch (Exception ex)
                {
                    Log.Error(this.ProtocolComboBox.Text + " url parse failed.", ex);
                }
            }
        }

        private void AllTagsListView_DoubleClick(object sender, EventArgs e)
        {
            this.AddTagsToFavorite();
        }

        private void ServerINIBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog {DefaultExt = "*.ini", CheckFileExists = true};
            DialogResult result = d.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.ICAServerINI.Text = d.FileName;
            }
        }

        private void ClientINIBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog {DefaultExt = "*.ini", CheckFileExists = true};

            DialogResult result = d.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.ICAClientINI.Text = d.FileName;
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            this.SetOkButtonState();
        }

        private void cmbHttpAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If the user has selected forms authentication
            if (this.cmbHttpAuthentication.Text ==
                Enum.GetName(typeof (BrowserAuthentication), BrowserAuthentication.Forms))
            {
                // If the user hasn't made any input
                if (string.IsNullOrEmpty(this.cmbHtmlFormField1Value.Text) &&
                    string.IsNullOrEmpty(this.cmbHtmlFormField2Value.Text) &&
                    string.IsNullOrEmpty(this.cmbHtmlFormField3Value.Text) &&
                    string.IsNullOrEmpty(this.txtHtmlFormField1Key.Text) &&
                    string.IsNullOrEmpty(this.txtHtmlFormField2Key.Text) &&
                    string.IsNullOrEmpty(this.txtHtmlFormField3Key.Text))
                {
                    this.cmbHtmlFormField1Value.Text = ConnectionManager.ParsingConstants.UserName;
                    this.cmbHtmlFormField2Value.Text = ConnectionManager.ParsingConstants.Password;
                    this.cmbHtmlFormField3Value.Text = ConnectionManager.ParsingConstants.Click;
                }
            }
            else
            {
                // If the user has accidently changed the authentication -> revert to the default.
                if (this.cmbHtmlFormField1Value.Text == ConnectionManager.ParsingConstants.UserName && this.cmbHtmlFormField2Value.Text == ConnectionManager.ParsingConstants.Password &&
                    this.cmbHtmlFormField3Value.Text == ConnectionManager.ParsingConstants.Click &&
                    string.IsNullOrEmpty(this.txtHtmlFormField1Key.Text) &&
                    string.IsNullOrEmpty(this.txtHtmlFormField2Key.Text) &&
                    string.IsNullOrEmpty(this.txtHtmlFormField3Key.Text))
                {
                    this.cmbHtmlFormField1Value.Text =
                        this.cmbHtmlFormField2Value.Text = this.cmbHtmlFormField3Value.Text = string.Empty;
                }
            }
        }

        private void picCustomIcon_MouseHover(object sender, EventArgs e)
        {
            picCustomIconDelete.Visible = true;
        }

        private void picCustomIconDelete_MouseHover(object sender, EventArgs e)
        {
            picCustomIconDelete.Visible = true;
        }

        private void picCustomIconDelete_Click(object sender, EventArgs e)
        {
            this.picCustomIcon.Image = (this.Favorite == null) ? Terminals.Properties.Resources.World : ConnectionManager.GetConnection(this.Favorite.Protocol).Image.Image;

            if (this.Favorite != null)
                this.Favorite.ToolBarIcon = null;

            this.currentToolBarFileName = null;
        }

        #region PuttyConnection

        #endregion
    }
}