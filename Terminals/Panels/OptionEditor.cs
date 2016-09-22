using Kohl.Framework.Logging;
using System;
using System.Collections.Generic;
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

        public OptionEditor(IHostingForm parent)
        {
            this.ApplySystemFont();

            this.InitializeComponent();

            // Dynamically added option panels.
            List<Type> types = ConnectionManager.GetOptionDialogTypes();

            foreach (Type type in types)
            {
                OptionPanel panel = null;
                try
                {
                    panel = (OptionPanel)Activator.CreateInstance(type);
                }
                catch (Exception ex)
                {
                    Log.Debug("ERROR ceating option panel! Type name: " + type.Name, ex);
                    continue;
                }

                panel.Location = new System.Drawing.Point(6, 26);
                panel.Tag = panel.Name;
                panel.Size = new System.Drawing.Size(512, 328);
                panel.TabIndex = 0;

                TreeNode treeNode11 = new TreeNode(panel.Text);
                treeNode11.Text = panel.Text;
                treeNode11.Tag = panel.Tag;
                this.OptionsTreeView.Nodes["Connections"].Nodes.Add(treeNode11);

                TabPage page = new TabPage(panel.Text);
                page.AutoScroll = true;
                page.Controls.Add(panel);
                page.Padding = new System.Windows.Forms.Padding(3);
                page.Dock = DockStyle.Fill;
                page.AutoSize = true;
                page.TabIndex = 0;
                page.Text = panel.Text;
                page.UseVisualStyleBackColor = true;

                this.tabCtrlOptionPanels.Controls.Add(page);
            }

            this.MovePanelsFromTabsIntoControls();
            Settings.ConfigurationChanged += this.SettingsConfigFileReloaded;
            this.LoadSettings();

            this.SetFormSize();

            IOptionPanel[] optionPanels = this.FindOptionPanels().ToArray<IOptionPanel>();

            foreach (IOptionPanel optionPanel in optionPanels)
            {
                optionPanel.IHostingForm = parent;
            }

            this.UpdateLookAndFeel();


        }

        private void SettingsConfigFileReloaded(ConfigurationChangedEventArgs args)
        {
            this.LoadSettings();
        }

        private void UpdateLookAndFeel()
        {
            if (!Kohl.Framework.Info.MachineInfo.IsUnixOrMac)
                // Update the old treeview theme to the new theme
                WindowsApi.SetWindowTheme(this.OptionsTreeView.Handle, "Explorer", null);

            this.currentPanel = this.panelStartupShutdown;
            this.OptionsTreeView.SelectedNode = this.OptionsTreeView.Nodes[0];
            this.OptionsTreeView.Select();
            this.OptionTitelLabel.BackColor = Color.FromArgb(17, 0, 252);

            this.DrawBottomLine();
        }

        /// <summary>
        ///     Set default font type by Windows theme to use for all controls on form
        /// </summary>
        private void ApplySystemFont()
        {
            this.Font = SystemFonts.IconTitleFont;
            this.AutoScaleMode = AutoScaleMode.Dpi;
        }

        private void SetFormSize()
        {
            // The option title label is the anchor for the form's width
            Int32 formWidth = this.OptionTitelLabel.Location.X + this.OptionTitelLabel.Width + 15;
            this.Width = formWidth;
        }

        /// <summary>
        ///     Hide tabpage control, only used in design time
        /// </summary>
        private void MovePanelsFromTabsIntoControls()
        {
            this.tabCtrlOptionPanels.Hide();
            this.CollectOptionPanelControls();
        }

        /// <summary>
        ///     Get all the panel control from the tabpages 
        ///     and add them to the form controls collection and hide the controls
        /// </summary>
        private void CollectOptionPanelControls()
        {
            foreach (TabPage tp in this.tabCtrlOptionPanels.TabPages)
            {
                foreach (Control ctrl in tp.Controls)
                {
                    if (ctrl is IOptionPanel)
                    {
                        ctrl.Hide();
                        this.Controls.Add(ctrl);
                    }
                }
            }
        }

        public bool FavoritesTreeViewOptionsChanged
        {
            get
            {
                return this.panelFavorites.Changed;
            }
        }

        private void DrawBottomLine()
        {
            Label lbl = new Label { AutoSize = false, BorderStyle = BorderStyle.Fixed3D };
            lbl.SetBounds(
                this.OptionTitelLabel.Left,
                this.OptionsTreeView.Top + this.OptionsTreeView.Height - 1,
                this.OptionTitelLabel.Width,
                2);
            this.Controls.Add(lbl);
            lbl.Show();
        }

        private void OptionsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (currentPanel != null)
                    this.currentPanel.Hide();
                this.SelectNewPanel();
                this.UpdatePanelPosition();
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
                    case TreeViewAction.ByKeyboard:
                    case TreeViewAction.ByMouse:
                        if (e.Node.IsExpanded)
                            e.Node.Collapse();
                        else
                            e.Node.Expand();
                        break;
                }
            }
        }

        private void SelectNewPanel()
        {
            string panelName = this.OptionsTreeView.SelectedNode.Tag.ToString();
            Debug.WriteLine("Selected panel: " + panelName);
            List<IOptionPanel> uc = this.FindOptionPanels();
            this.currentPanel = uc.FirstOrDefault(panel => ((panel is OptionPanel) ? ((OptionPanel)panel).Name : panel.Name.StartsWith("panel") ? panel.Name.Substring(5, panel.Name.Length - 5) : panel.Name) == panelName);
        }

        private void UpdatePanelPosition()
        {
            if (currentPanel == null)
                return;

            Int32 x = this.OptionTitelLabel.Left;
            Int32 y = this.OptionTitelLabel.Top + this.OptionTitelLabel.Height + 3;
            this.currentPanel.Location = new Point(x, y);
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

        private void LoadSettings()
        {
            foreach (IOptionPanel optionPanel in this.FindOptionPanels())
            {
                optionPanel.LoadSettings();
            }
        }

        private List<IOptionPanel> FindOptionPanels()
        {
            return this.Controls
                       .Cast<Control>()
                       .Where(control => control is IOptionPanel)
                       .Cast<IOptionPanel>().ToList<IOptionPanel>();
        }

        private void OptionDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.ConfigurationChanged -= this.SettingsConfigFileReloaded;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.lnkHomepage.Text);
        }
    }
}