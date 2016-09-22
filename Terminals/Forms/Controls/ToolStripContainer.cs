using System;
using System.Drawing;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Configuration.Files.Main.ToolTip;

namespace Terminals.Forms.Controls
{
    /// <summary>
    ///     Extens Windows Forms ToolStripContainer to provide load and save its layout
    /// </summary>
    public class ToolStripContainer : System.Windows.Forms.ToolStripContainer
    {
        public ToolStrip toolbarStd { private get; set; }
        public ToolStripMenuItem standardToolbarToolStripMenuItem { private get; set; }
        public ToolStrip favoriteToolBar { private get; set; }
        public ToolStripMenuItem toolStripMenuItemShowHideFavoriteToolbar { private get; set; }
        public ToolStrip SpecialCommandsToolStrip { private get; set; }
        public ToolStripMenuItem shortcutsToolStripMenuItem { private get; set; }
        public MenuStrip menuStrip { private get; set; }

        public void AssignToolStripsLocationChangedEventHandler()
        {
            this.toolbarStd.EndDrag += this.OnToolStripLocationChanged;
            this.favoriteToolBar.EndDrag += this.OnToolStripLocationChanged;
            this.menuStrip.EndDrag += this.OnToolStripLocationChanged;
        }

        private void OnToolStripLocationChanged(object sender, EventArgs e)
        {
            this.SaveLayout();
        }

        public void SaveLayout()
        {
            if (!Settings.ToolbarsLocked)
            {
                ToolStripSettingElementCollection newSettings = new ToolStripSettingElementCollection();
                this.SaveToolStripPanel(this.TopToolStripPanel, "Top", newSettings);
                this.SaveToolStripPanel(this.LeftToolStripPanel, "Left", newSettings);
                this.SaveToolStripPanel(this.RightToolStripPanel, "Right", newSettings);
                this.SaveToolStripPanel(this.BottomToolStripPanel, "Bottom", newSettings);
                Settings.ToolbarSettings = newSettings;
            }
        }

        private void SaveToolStripPanel(ToolStripPanel panel, string position,
                                        ToolStripSettingElementCollection newSettings)
        {
            int rowIndex = 0;

            foreach (ToolStripPanelRow row in panel.Rows)
            {
                this.SaveToolStripRow(row, newSettings, position, rowIndex);
                rowIndex++;
            }
        }

        private void SaveToolStripRow(ToolStripPanelRow row, ToolStripSettingElementCollection newSettings,
                                      string position, int rowIndex)
        {
            foreach (ToolStrip strip in row.Controls)
            {
                newSettings.Add(new ToolStripSettingElement
                {
                    Dock = position,
                    Row = rowIndex,
                    Left = strip.Left,
                    Top = strip.Top,
                    Name = strip.Name,
                    Visible = strip.Visible
                });
            }
        }

        public void LoadToolStripsState()
        {
            ToolStripSettingElementCollection newSettings = Settings.ToolbarSettings;
            if (newSettings != null && newSettings.Count > 0)
            {
                this.SuspendLayout();
                this.ClearAllPanels();
                this.ReJoinAllPanels(newSettings);

                // paranoic, because the previous join can reset the position
                // dont assign if already there. Because it can reorder the toolbars
                // http://www.visualbasicask.com/visual-basic-language/toolstrips-controls-becoming-desorganized.shtml
                this.AplyAllPanelPositions(newSettings);
                this.ResumeLayout(true);

                this.ChangeLockState();
            }
        }

        private void ClearAllPanels()
        {
            this.RightToolStripPanel.Controls.Clear();
            this.LeftToolStripPanel.Controls.Clear();
            this.TopToolStripPanel.Controls.Clear();
            this.BottomToolStripPanel.Controls.Clear();
        }

        private void AplyAllPanelPositions(ToolStripSettingElementCollection newSettings)
        {
            foreach (ToolStripSettingElement setting in newSettings)
            {
                ToolStrip strip = this.FindToolStripForSetting(setting);

                if (strip == null)
                    continue;

                strip.GripStyle = ToolStripGripStyle.Visible;
                ApplyLastPosition(setting, strip);
            }
        }

        private void ReJoinAllPanels(ToolStripSettingElementCollection newSettings)
        {
            foreach (ToolStripSettingElement setting in newSettings)
            {
                ToolStrip strip = this.FindToolStripForSetting(setting);
                ToolStripMenuItem menuItem = this.FindMenuForSetting(setting);

                if (menuItem != null)
                {
                    menuItem.Checked = setting.Visible;
                }

                this.RestoreStripLayout(setting, strip);
            }
        }

        private void RestoreStripLayout(ToolStripSettingElement setting, ToolStrip strip)
        {
            if (strip != null)
            {
                strip.Visible = setting.Visible;
                this.JoinPanelOnLastPosition(strip, setting);
            }
        }

        private ToolStripMenuItem FindMenuForSetting(ToolStripSettingElement setting)
        {
            if (setting.Name == this.toolbarStd.Name)
                return this.standardToolbarToolStripMenuItem;

            if (setting.Name == this.favoriteToolBar.Name)
                return this.toolStripMenuItemShowHideFavoriteToolbar;

            if (setting.Name == this.SpecialCommandsToolStrip.Name)
                return this.shortcutsToolStripMenuItem;

            return null;
        }

        private ToolStrip FindToolStripForSetting(ToolStripSettingElement setting)
        {
            if (setting.Name == this.toolbarStd.Name)
                return this.toolbarStd;

            if (setting.Name == this.favoriteToolBar.Name)
                return this.favoriteToolBar;

            if (setting.Name == this.SpecialCommandsToolStrip.Name)
                return this.SpecialCommandsToolStrip;

            if (setting.Name == this.menuStrip.Name)
                return this.menuStrip;

            return null;
        }

        private void JoinPanelOnLastPosition(ToolStrip strip, ToolStripSettingElement setting)
        {
            ToolStripPanel toolStripPanel = this.GetToolStripPanelToJoin(setting);
            if (!toolStripPanel.Controls.Contains(strip))
            {
                Point lastPosition = new Point(setting.Left, setting.Top);
                toolStripPanel.Join(strip, lastPosition);
            }
            else // set position only when comming from fullscreen
            {
                ApplyLastPosition(setting, strip);
            }
        }

        private static void ApplyLastPosition(ToolStripSettingElement setting, ToolStrip strip)
        {
            strip.Left = setting.Left;
            strip.Top = setting.Top;
        }

        private ToolStripPanel GetToolStripPanelToJoin(ToolStripSettingElement setting)
        {
            switch (setting.Dock)
            {
                case "Left":
                    return this.LeftToolStripPanel;
                case "Right":
                    return this.RightToolStripPanel;
                case "Bottom":
                    return this.BottomToolStripPanel;
                default: // defensive position
                    return this.TopToolStripPanel;
            }
        }

        private static void ChangeToolStripLock(ToolStrip strip)
        {
            strip.GripStyle = Settings.ToolbarsLocked ? ToolStripGripStyle.Hidden : ToolStripGripStyle.Visible;
        }

        /// <summary>
        ///     Locks or unlocks the toolstrip panesl
        /// </summary>
        public void ChangeLockState()
        {
            ChangeToolStripPanelLockState(this.TopToolStripPanel);
            ChangeToolStripPanelLockState(this.RightToolStripPanel);
            ChangeToolStripPanelLockState(this.LeftToolStripPanel);
            ChangeToolStripPanelLockState(this.BottomToolStripPanel);
        }

        private static void ChangeToolStripPanelLockState(ToolStripPanel toolStripPanel)
        {
            foreach (ToolStripPanelRow row in toolStripPanel.Rows)
            {
                foreach (ToolStrip toolStrip in row.Controls)
                {
                    if (toolStrip != null)
                    {
                        ChangeToolStripLock(toolStrip);
                    }
                }
            }
        }
    }
}