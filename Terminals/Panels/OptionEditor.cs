using Kohl.Framework.Logging;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Manager;
using Terminals.Connection.Native;
using Terminals.Connection.Panels.OptionPanels;

namespace Terminals.Panels
{
    public partial class OptionEditor : Form
    {
        private IOptionPanel currentPanel;
        OptionPanels.FavoritesOptionPanel panelFavorites;

        public OptionEditor(IHostingForm parent)
        {
            this.InitializeComponent();
            SetupPanels(parent);

            Settings.ConfigurationChanged += LoadSettings;
            this.LoadSettings();
            this.UpdateLookAndFeel();
        }

        private void UpdateLookAndFeel()
        {
            if (!Kohl.Framework.Info.MachineInfo.IsUnixOrMac)
                // Update the old treeview theme to the new theme
                WindowsApi.SetWindowTheme(this.OptionsTreeView.Handle, "Explorer", null);

            this.OptionsTreeView.SelectedNode = this.OptionsTreeView.Nodes[0];
            this.OptionsTreeView.Select();
        }

        public bool FavoritesTreeViewOptionsChanged
        {
            get { return this.panelFavorites.Changed; }
        }

        private void OptionsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                this.currentPanel?.Hide();
                this.SelectNewPanel();
                this.currentPanel.Show();
                this.OptionTitelLabel.Text = this.OptionsTreeView.SelectedNode.Text.Replace("&", "&&");
                UpdateTreeNodeState(e);
            }
            catch (Exception ex)
            {
                Log.Info(ex);
            }
        }

        private static void UpdateTreeNodeState(TreeViewEventArgs e)
        {
            if (e.Node.GetNodeCount(true) > 0)
            {
                switch (e.Action)
                {
                    case TreeViewAction.ByKeyboard :
                    case TreeViewAction.ByMouse    : e.Node.Toggle(); break;
                }
            }
        }

        private void SelectNewPanel()
        {
            string panelName = this.OptionsTreeView.SelectedNode.Tag.ToString();
            Debug.WriteLine("Selected panel: " + panelName);
            var uc           = FindOptionPanels();
            currentPanel     = uc.FirstOrDefault(panel => GetPanelName(panel) == panelName);
        }

        string GetPanelName(IOptionPanel panel)
        {
            if (panel is OptionPanel)
                return ((OptionPanel)panel).Name;

            if (panel.Name.StartsWith("panel"))
                return panel.Name.Substring(5);

            return panel.Name;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.StartDelayedUpdate();
                this.SaveAllPanels();
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                MessageBox.Show(String.Format("Error saving settings.\r\n{0}", exception.Message));
            }
            finally
            {
                Settings.SaveAndFinishDelayedUpdate();
            }
        }

        private void SaveAllPanels()
        {
            foreach (IOptionPanel optionPanel in this.FindOptionPanels())
            {
                optionPanel.SaveSettings();
            }
        }

        private void LoadSettings(ConfigurationChangedEventArgs args = null)
        {
            foreach (IOptionPanel optionPanel in this.FindOptionPanels())
            {
                optionPanel.LoadSettings();
            }
        }

        private IOptionPanel[] FindOptionPanels()
        {
            return pnlMain.Controls.OfType<IOptionPanel>().ToArray();
        }

        private void OptionDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.ConfigurationChanged -= LoadSettings;
        }

        private void ShowHomepage(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.lnkHomepage.Text);
        }

        void SetupPanels(IHostingForm parent)
        {
            var panelStartupShutdown      = SetupPanel<OptionPanels.StartShutdownOptionPanel  >(parent, "panelStartupShutdown");
            var panelInterface            = SetupPanel<OptionPanels.InterfaceOptionPanel      >(parent, "panelInterface");
            panelFavorites                = SetupPanel<OptionPanels.FavoritesOptionPanel      >(parent, "panelFavorites");
            var panelMasterPassword       = SetupPanel<OptionPanels.MasterPasswordOptionPanel >(parent, "panelMasterPassword");
            var panelDefaultPassword      = SetupPanel<OptionPanels.DefaultPasswordOptionPanel>(parent, "panelDefaultPassword");
            var panelConnections          = SetupPanel<OptionPanels.ConnectionsOptionPanel    >(parent, "panelConnections");
            var panelExecuteBeforeConnect = SetupPanel<OptionPanels.ConnectCommandOptionPanel >(parent, "panelExecuteBeforeConnect");
            var panelProxy                = SetupPanel<OptionPanels.ProxyOptionPanel          >(parent, "panelProxy");
            var panelScreenCapture        = SetupPanel<OptionPanels.CaptureOptionPanel        >(parent, "panelScreenCapture");
            var panelCredentialStore      = SetupPanel<OptionPanels.CredentialStoreOptionPanel>(parent, "panelCredentialStore");

            // Dynamically added option panels.
            var panels = ConnectionManager.GetOptionDialogTypes()
                                          .Select(x => SetupPanel(x, parent))
                                          .OfType<OptionPanel>()
                                          .Select(y => new TreeNode { Text = y.Text, Tag = y.Name });

            OptionsTreeView.Nodes["Connections"].Nodes.AddRange(panels.ToArray());
        }

        T SetupPanel<T>(IHostingForm parent, string name) where T : IOptionPanel, new()
        {
            return (T)SetupPanel(typeof(T), parent, name);
        }

        IOptionPanel SetupPanel(Type type, IHostingForm parent, string name = null)
        {
            IOptionPanel panel = null;
            try
            {
                panel = (IOptionPanel)Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                Log.Debug("ERROR creating option panel! Type name: " + type.Name, ex);
                return null;
            }

            panel.IHostingForm = parent;
            panel.Dock         = DockStyle.Fill;
            panel.Name         = name;
            panel.Hide();

            pnlMain.Controls.Add(panel);
            return panel;
        }
    }
}