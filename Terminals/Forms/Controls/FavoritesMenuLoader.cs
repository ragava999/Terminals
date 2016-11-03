using Terminals.Connection.Manager;

namespace Terminals.Forms.Controls
{
    using Configuration.Files.Main.Favorites;
    using Configuration.Files.Main.Settings;
    using Configuration.Files.Main.SpecialCommands;
    using Kohl.Framework.Logging;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    ///     Fills menu, tool strip menu and tool bar with favorite buttons
    /// </summary>
    public class FavoritesMenuLoader
    {
        public const string COMMAND_EXIT = "Exit";
        public const string COMMAND_DETACH = "Detach";
        public const string QUICK_CONNECT = "QuickConnect";
        public const string COMMAND_SPECIAL = "SpecialCommands";
        public const string COMMAND_RESTORESCREEN = "RestoreScreen";
        public const string COMMAND_FULLSCREEN = "FullScreen";
        public const string COMMAND_SHOWMENU = "ShowMenu";
        public const string COMMAND_OPTIONS = "Options";
        public const string COMMAND_CAPTUREMANAGER = "ScreenCaptureManager";
        public const string COMMAND_NETTOOLS = "NetworkingTools";
        public const string COMMAND_CREDENTIALMANAGER = "CredentialsManager";
        public const string COMMAND_ORGANIZEFAVORITES = "OrganizeFavorites";
        private const string COMMAND_ALPHABETICAL = "Alphabetical";

        /// <summary>
        ///     Stored in context menu Tag to identify virtual context menu groups by tag
        /// </summary>
        public const string TAG = "tag";

        /// <summary>
        ///     Stored in context menu Tag to identify favorite context menu items
        /// </summary>
        public const string FAVORITE = "favorite";

        private readonly ToolStrip favoriteToolBar;
        private readonly ToolStripMenuItem favoritesToolStripMenuItem;
        private readonly ContextMenuStrip quickContextMenu;
        private readonly ToolStripItemClickedEventHandler quickContextMenu_ItemClicked;
        private readonly EventHandler serverToolStripMenuItem_Click;
        private readonly ToolStripComboBox tscConnectTo;
        private ToolStripMenuItem alphabeticalMenu;
        private ToolStripMenuItem cpl;
        private ToolStripItem fullScreenMenuItem;
        private ToolStripMenuItem mgmt;
        private ToolStripMenuItem other;
        private ToolStripItem restoreScreenMenuItem;
        private ToolStripMenuItem special;
        private TagMenuItem unTaggedQuickMenuItem;
        private TagMenuItem untaggedToolStripMenuItem;

        public FavoritesMenuLoader(ToolStripMenuItem favoritesToolStripMenuItem,
                                   ToolStripComboBox tscConnectTo, EventHandler serverToolStripMenuItem_Click,
                                   ToolStrip favoriteToolBar,
                                   ContextMenuStrip quickContextMenu,
                                   ToolStripItemClickedEventHandler quickContextMenu_ItemClicked)
        {
            this.favoritesToolStripMenuItem = favoritesToolStripMenuItem;
            this.tscConnectTo = tscConnectTo;
            this.serverToolStripMenuItem_Click = serverToolStripMenuItem_Click;
            this.favoriteToolBar = favoriteToolBar;
            this.quickContextMenu = quickContextMenu;
            this.quickContextMenu_ItemClicked = quickContextMenu_ItemClicked;

            this.favoritesToolStripMenuItem.DropDownItems.Add("-");
            this.CreateUntaggedItem();
            this.CreateTrayMenuItems();
            this.UpdateMenuAndContextMenu();

            Settings.ConfigurationChanged += this.OnSettingsConfigurationChanged;
            DataDispatcher.Instance.TagsChanged += Instance_TagsChanged;
        }

        /// <summary>
        /// If the user has added a new tag ... Update the menu strip and the context menu (quick menu).
        /// </summary>
        /// <param name="args"></param>
        void Instance_TagsChanged(Configuration.Files.Main.Tags.TagsChangedArgs args)
        {
            this.UpdateMenuAndContextMenu();
        }

        private int AlphabeticalMenuItemIndex
        {
            get
            {
                return this.quickContextMenu.Items.IndexOf(this.alphabeticalMenu);
            }
        }

        private void OnSettingsConfigurationChanged(ConfigurationChangedEventArgs args)
        {
            this.LoadFavoritesToolbar();
        }

        /// <summary>
        ///     Simple refresh, which removes all menu items and recreates new, when necessary using lazy loading.
        /// </summary>
        private void UpdateMenuAndContextMenu()
        {
            this.FillMainMenu();
            this.FillTrayContextMenu();
        }

        /// <summary>
        /// If the user plays around with the 'HideFavoritesFromQuickMenu' setting, i.e.
        /// first hiding, then showing and vice versa.
        /// </summary>
        public void ToggleShowHideFavoritesFromQuickMenu()
        {
            if (quickContextMenu != null && unTaggedQuickMenuItem != null && this.quickContextMenu.Items.Contains(unTaggedQuickMenuItem) && unTaggedQuickMenuItem.Visible == false)
                // Remove the "unTaggedQuickMenuItem"
                unTaggedQuickMenuItem.Visible = true;

            if (quickContextMenu != null && alphabeticalMenu != null && this.quickContextMenu.Items.Contains(alphabeticalMenu) && alphabeticalMenu.Visible == false)
            {
                // Remove the "-" lines
                this.quickContextMenu.Items[this.quickContextMenu.Items.IndexOf(alphabeticalMenu) - 1].Visible = true;
                this.quickContextMenu.Items[this.quickContextMenu.Items.IndexOf(alphabeticalMenu) - 2].Visible = true;
                // Remove the "alphabeticalMenu"
                alphabeticalMenu.Visible = true;
            }

            if (unTaggedQuickMenuItem == null)
            {
                if (quickContextMenu != null)
                    this.quickContextMenu.Items.Clear();

                this.CreateUntaggedItem();
                this.CreateTrayMenuItems();
                this.UpdateMenuAndContextMenu();
            }
            else
            {
                this.UpdateMenuAndContextMenu();

                // The following two items won't be removed by the above method:
                // unTaggedQuickMenuItem
                // - 
                // alphabeticalMenu
                if (Settings.HideFavoritesFromQuickMenu)
                {
                    if (this.quickContextMenu.Items.Contains(unTaggedQuickMenuItem) && unTaggedQuickMenuItem.Visible == false)
                        // Remove the "unTaggedQuickMenuItem"
                        alphabeticalMenu.Visible = false;

                    if (this.quickContextMenu.Items.Contains(alphabeticalMenu) && alphabeticalMenu.Visible == false)
                    {
                        // Remove the "-" lines
                        this.quickContextMenu.Items[this.quickContextMenu.Items.IndexOf(alphabeticalMenu) - 1].Visible = false;
                        this.quickContextMenu.Items[this.quickContextMenu.Items.IndexOf(alphabeticalMenu) - 2].Visible = false;
                        // Remove the "alphabeticalMenu"
                        alphabeticalMenu.Visible = false;
                    }
                }
            }
        }

        private void CreateUntaggedItem()
        {
            if (untaggedToolStripMenuItem == null)
            {
                this.untaggedToolStripMenuItem = CreateTagMenuItem(Settings.UNTAGGED_NODENAME);
                this.untaggedToolStripMenuItem.DropDownOpening += this.OnTagMenuDropDownOpening;
                this.favoritesToolStripMenuItem.DropDownItems.Add(this.untaggedToolStripMenuItem);
            }

            if (untaggedToolStripMenuItem.Visible == false)
                untaggedToolStripMenuItem.Visible = true;
        }

        private ToolStripItem exitMenu = null;

        /// <summary>
        ///     Creates skeleton for system tray menu items. No tags or favorites are added here.
        /// </summary>
        private void CreateTrayMenuItems()
        {
            this.AddGeneralTrayContextMenu();
            this.AddTraySpecialCommandsContextMenu();

            if (!Settings.HideFavoritesFromQuickMenu)
            {
                this.quickContextMenu.Items.Add("-");
                this.AddUntaggedQuickContextMenu();

                // here favorite Tags will be placed

                this.AddAlphabeticalContextMenu();
            }

            ToolStripItem quickConnect = this.quickContextMenu.Items.Add("Quick connect");
            quickConnect.Name = QUICK_CONNECT;

            this.quickContextMenu.Items.Add("-");
            exitMenu = this.quickContextMenu.Items.Add("Exit");
            exitMenu.Name = COMMAND_EXIT;
        }

        private void AddUntaggedQuickContextMenu()
        {
            this.unTaggedQuickMenuItem = CreateTagMenuItem(Settings.UNTAGGED_NODENAME);
            this.unTaggedQuickMenuItem.DropDownItemClicked += this.quickContextMenu_ItemClicked;
            this.unTaggedQuickMenuItem.DropDownOpening += OnTagTrayMenuItemDropDownOpening;
            this.quickContextMenu.Items.Add(this.unTaggedQuickMenuItem);
        }

        private void FillMainMenu()
        {
            this.ReFreshConnectionsComboBox();

            if (!Settings.HideFavoritesFromQuickMenu)
            {
                this.untaggedToolStripMenuItem.ClearDropDownsToEmpty();
            }

            this.ClearFavoritesToolStripmenuItems();
            this.CreateTagsToolStripMenuItems();

            this.LoadFavoritesToolbar();
        }

        private void ReFreshConnectionsComboBox()
        {
            this.tscConnectTo.Items.Clear();
            string[] connectionNames = Settings.GetFavorites(false)
                                                         .ToList()
                                                         .Select(favorite => favorite.Name).ToArray();
            this.tscConnectTo.Items.AddRange(connectionNames);
        }

        #region Menu toolstrips

        private void ClearFavoritesToolStripmenuItems()
        {
            Int32 seperatorIndex = this.favoritesToolStripMenuItem.DropDownItems.IndexOf(this.untaggedToolStripMenuItem);
            for (Int32 index = this.favoritesToolStripMenuItem.DropDownItems.Count - 1; index > seperatorIndex; index--)
            {
                ToolStripMenuItem tagMenuItem =
                    this.favoritesToolStripMenuItem.DropDownItems[index] as ToolStripMenuItem;
                this.UnregisterTagMenuItemEventHandlers(tagMenuItem);
                this.favoritesToolStripMenuItem.DropDownItems.RemoveAt(index);
            }
        }

        private void UnregisterTagMenuItemEventHandlers(ToolStripMenuItem tagMenuItem)
        {
            tagMenuItem.DropDownOpening -= this.OnTagMenuDropDownOpening;
            foreach (ToolStripMenuItem favoriteItem in tagMenuItem.DropDownItems)
            {
                favoriteItem.Click += this.serverToolStripMenuItem_Click;
            }
        }

        /// <summary>
        ///     Fills the main window "favorites" menu, after separator places all tags
        ///     and their favorites as dropdown items
        /// </summary>
        private void CreateTagsToolStripMenuItems()
        {
            string[] tags = Settings.Tags(false);

            List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();

            foreach (string tag in tags)
            {
                ToolStripMenuItem tagMenu = CreateTagMenuItem(tag);

                tagMenu.DropDownOpening += this.OnTagMenuDropDownOpening;

                items.Add(tagMenu);

                OnTagMenuDropDownOpening(tagMenu, EventArgs.Empty);

                this.favoritesToolStripMenuItem.GetCurrentParent().InvokeIfNecessary(delegate
                {
                    this.favoritesToolStripMenuItem.DropDownItems.Add(tagMenu);
                });
            }
        }

        /// <summary>
        ///     Lazy loading for favorites dropdown menu items in Tag menu item
        /// </summary>
        private void OnTagMenuDropDownOpening(object sender, EventArgs e)
        {
            TagMenuItem tagMenu = sender as TagMenuItem;

            if (tagMenu.IsEmpty)
            {
                tagMenu.DropDown.Items.Clear();

                List<FavoriteConfigurationElement> tagFavorites = Settings.GetSortedFavoritesByTag(tagMenu.Text, false);

                foreach (FavoriteConfigurationElement favorite in tagFavorites)
                {

                    ToolStripMenuItem item = this.CreateToolStripItemByFavorite(favorite);

                    tagMenu.DropDown.Items.Add(item);
                }
            }
        }

        private ToolStripMenuItem CreateToolStripItemByFavorite(FavoriteConfigurationElement favorite)
        {
            ToolStripMenuItem item = CreateFavoriteMenuItem(favorite);
            item.Click += this.serverToolStripMenuItem_Click;
            return item;
        }

        #endregion

        #region Fill tool bar by user defined items

        private void LoadFavoritesToolbar()
        {
            try
            {
                this.favoriteToolBar.SuspendLayout();
                this.favoriteToolBar.Items.Clear();
                this.CreateFavoriteButtons();
                this.favoriteToolBar.ResumeLayout();
            }
            catch (Exception exc)
            {
                Log.Error("Error loading the favorites toolbar.", exc);
            }
        }

        private void CreateFavoriteButtons()
        {
            FavoriteConfigurationElementCollection favorites = Settings.GetFavorites(false);
            foreach (string favoriteName in Settings.FavoritesToolbarButtons)
            {
                this.CreateFavoriteButton(favorites, favoriteName);
            }
        }

        private void CreateFavoriteButton(FavoriteConfigurationElementCollection favorites, string favoriteName)
        {
            FavoriteConfigurationElement favorite = favorites[favoriteName];
            if (favorite != null)
            {
                ToolStripButton favoriteBtn = this.CreateFavoriteButton(favorite);
                this.favoriteToolBar.Items.Add(favoriteBtn);
            }
        }

        private ToolStripButton CreateFavoriteButton(FavoriteConfigurationElement favorite)
        {
            Image buttonImage = ConnectionImageHandler.GetFavoriteIcon(favorite);
            ToolStripButton favoriteBtn = new ToolStripButton(favorite.Name, buttonImage,
                                                              this.serverToolStripMenuItem_Click)
            {
                ToolTipText = favorite.GetToolTipText(),
                Tag = favorite,
                Overflow = ToolStripItemOverflow.AsNeeded
            };
            return favoriteBtn;
        }

        #endregion

        #region System tray context menu

        private void FillTrayContextMenu()
        {
            if (!Settings.HideFavoritesFromQuickMenu)
            {
                this.alphabeticalMenu.DropDownItems.Clear();
                this.unTaggedQuickMenuItem.ClearDropDownsToEmpty();
            }

            this.ClearTrayFavoritesMenu();
            this.AddTagTrayMenuItems();
        }

        private void ClearTrayFavoritesMenu()
        {
            int startIndex = this.quickContextMenu.Items.IndexOf(this.unTaggedQuickMenuItem) + 1;
            while (startIndex < this.AlphabeticalMenuItemIndex)
            {
                // unregister event handler to release the menu item
                ToolStripMenuItem tagItem = this.quickContextMenu.Items[startIndex] as ToolStripMenuItem;
                tagItem.DropDownItemClicked -= this.quickContextMenu_ItemClicked;
                tagItem.DropDownOpening -= OnTagTrayMenuItemDropDownOpening;
                this.quickContextMenu.Items.RemoveAt(startIndex);
            }
        }

        private void AddTagTrayMenuItems()
        {
            if (!Settings.HideFavoritesFromQuickMenu)
            {
                foreach (string tag in Settings.Tags(false))
                {
                    ToolStripMenuItem tagMenuItem = CreateTagMenuItem(tag);
                    tagMenuItem.DropDownItemClicked +=
                        this.quickContextMenu_ItemClicked;
                    tagMenuItem.DropDownOpening += OnTagTrayMenuItemDropDownOpening;
                    this.quickContextMenu.Items.Insert(this.AlphabeticalMenuItemIndex, tagMenuItem);
                }
            }
        }

        private static void OnTagTrayMenuItemDropDownOpening(object sender, EventArgs e)
        {
            TagMenuItem tagMenu = sender as TagMenuItem;
            if (tagMenu.IsEmpty)
            {
                tagMenu.DropDown.Items.Clear();
                List<FavoriteConfigurationElement> tagFavorites = Settings.GetSortedFavoritesByTag(tagMenu.Text, false);
                foreach (FavoriteConfigurationElement favorite in tagFavorites)
                {
                    ToolStripMenuItem item = CreateFavoriteMenuItem(favorite);
                    tagMenu.DropDown.Items.Add(item);
                }
            }
        }

        private void AddTraySpecialCommandsContextMenu()
        {
            this.AddCommandMenuItems();

            foreach (SpecialCommandConfigurationElement command in Settings.SpecialCommands)
            {
                this.AddSpecialMenuItem(command);
            }
        }

        private void AddSpecialMenuItem(SpecialCommandConfigurationElement commad)
        {
            ToolStripItem specialItem = this.CreateSpecialItem(commad);
            specialItem.Image = ConnectionImageHandler.LoadImage(commad.Thumbnail, Resources.server_administrator_icon);
            specialItem.ImageTransparentColor = Color.Magenta;
            specialItem.Tag = commad;
            specialItem.Click += specialItem_Click;
        }

        private ToolStripItem CreateSpecialItem(SpecialCommandConfigurationElement commad)
        {
            string commandExeName = commad.Executable.ToLower();

            if (commandExeName.EndsWith("cpl"))
                return this.cpl.DropDown.Items.Add(commad.Name);
            if (commandExeName.EndsWith("msc"))
                return this.mgmt.DropDown.Items.Add(commad.Name);

            return this.other.DropDown.Items.Add(commad.Name);
        }

        private void AddCommandMenuItems()
        {
            this.special = new ToolStripMenuItem("Special Commands", Resources.computer_link);
            this.mgmt = new ToolStripMenuItem("Management", Resources.CompMgmt);
            this.cpl = new ToolStripMenuItem("Control Panel", Resources.ControlPanel);
            this.other = new ToolStripMenuItem("Other");

            this.quickContextMenu.Items.Add(this.special);
            this.special.DropDown.Items.Add(this.mgmt);
            this.special.DropDown.Items.Add(this.cpl);
            this.special.DropDown.Items.Add(this.other);
        }

        private static void specialItem_Click(object sender, EventArgs e)
        {
            ToolStripItem specialItem = sender as ToolStripItem;
            SpecialCommandConfigurationElement elm = specialItem.Tag as SpecialCommandConfigurationElement;
            elm.Launch();
        }

        private static TagMenuItem CreateTagMenuItem(string tag)
        {
            TagMenuItem tagMenuItem = new TagMenuItem { Tag = TAG, Text = tag };
            return tagMenuItem;
        }

        private static ToolStripMenuItem CreateFavoriteMenuItem(FavoriteConfigurationElement favorite)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(favorite.Name)
            {
                Tag = FAVORITE,
                Image = ConnectionImageHandler.GetFavoriteIcon(favorite)
            };
            return item;
        }

        private void AddAlphabeticalContextMenu()
        {
            this.alphabeticalMenu = new ToolStripMenuItem("Alphabetical") { Name = COMMAND_ALPHABETICAL };

            this.alphabeticalMenu.DropDownItemClicked += this.quickContextMenu_ItemClicked;
            this.alphabeticalMenu.DropDownOpening += this.OnAlphabeticalMenuDropDownOpening;
            this.alphabeticalMenu.Image = Resources.atoz;
            this.quickContextMenu.Items.Add(this.alphabeticalMenu);
        }

        private void OnAlphabeticalMenuDropDownOpening(object sender, EventArgs e)
        {
            if (!this.alphabeticalMenu.HasDropDownItems)
            {
                List<FavoriteConfigurationElement> favorites = Settings.GetFavorites(false)
                                                                                 .ToList()
                                                                                 .SortByProperty("Name",
                                                                                                 SortOrder.Ascending);

                this.CreateAlphabeticalFavoriteMenuItems(favorites);
                Boolean alphaMenuVisible = this.alphabeticalMenu.DropDownItems.Count > 0;
                this.alphabeticalMenu.Visible = alphaMenuVisible;
            }
        }

        private void CreateAlphabeticalFavoriteMenuItems(List<FavoriteConfigurationElement> favorites)
        {
            foreach (FavoriteConfigurationElement favorite in favorites)
            {
                ToolStripMenuItem sortedItem = CreateFavoriteMenuItem(favorite);
                this.alphabeticalMenu.DropDownItems.Add(sortedItem);
            }
        }

        private void AddGeneralTrayContextMenu()
        {
            this.restoreScreenMenuItem = this.CreateGeneralTrayContextMenuItem("Restore Screen",
                                                                               COMMAND_RESTORESCREEN, Resources.arrow_in);
            this.fullScreenMenuItem = this.CreateGeneralTrayContextMenuItem("Full Screen",
                                                                            COMMAND_FULLSCREEN, Resources.arrow_out);

            this.CreateGeneralTrayContextMenuItem("Detach", COMMAND_DETACH, Resources.unlink);

            this.quickContextMenu.Items.Add("-");
            ToolStripItem showMenu = this.quickContextMenu.Items.Add("Show Menu");
            showMenu.Name = COMMAND_SHOWMENU;
            this.quickContextMenu.Items.Add("-");
            this.CreateGeneralTrayContextMenuItem("Capture Manager", COMMAND_CAPTUREMANAGER,
                                                  Resources.screen_capture_box);
            this.CreateGeneralTrayContextMenuItem("Networking Tools", COMMAND_NETTOOLS,
                                                  Resources.computer_link);
            this.quickContextMenu.Items.Add("-");
            this.CreateGeneralTrayContextMenuItem(
                "Credential Manager", COMMAND_CREDENTIALMANAGER,
                Resources.computer_security);
            this.CreateGeneralTrayContextMenuItem("Organize Favorites", COMMAND_ORGANIZEFAVORITES,
                                                  Resources.star);
            this.CreateGeneralTrayContextMenuItem("Options", COMMAND_OPTIONS, Resources.options);
            this.quickContextMenu.Items.Add("-");
        }

        public void UpdateSwitchFullScreenMenuItemsVisibility(Boolean fullScreen)
        {
            this.restoreScreenMenuItem.Visible = fullScreen;
            this.fullScreenMenuItem.Visible = !fullScreen;
        }

        private ToolStripItem CreateGeneralTrayContextMenuItem(string menuText, string commnadName, Image icon)
        {
            ToolStripItem menuItem = this.quickContextMenu.Items.Add(menuText, icon);
            menuItem.Name = commnadName;
            return menuItem;
        }

        #endregion
    }
}