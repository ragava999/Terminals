using System;
using System.Windows.Forms;

using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Panels.OptionPanels;

namespace Terminals.Panels.OptionPanels
{
    public partial class InterfaceOptionPanel : IOptionPanel
    {
        public InterfaceOptionPanel()
        {
            this.InitializeComponent();
            
        }

        public override void LoadSettings()
        {
            this.chkEnableGroupsMenu.Checked = Settings.EnableGroupsMenu;
            this.chkMinimizeToTrayCheckbox.Checked = Settings.MinimizeToTray;
            this.chkShowUserNameInTitle.Checked = Settings.ShowUserNameInTitle;
            this.chkShowInformationToolTips.Checked = Settings.ShowInformationToolTips;
            this.chkShowFullInfo.Checked = Settings.ShowFullInformationToolTips;
            this.chkSortTabPagesByCaption.Checked = Settings.SortTabPagesByCaption;

            this.chkInvertTabPageOrder.Checked = Settings.InvertTabPageOrder;

            if (Settings.Office2007BlueFeel)
                this.RenderBlueRadio.Checked = true;
            else if (Settings.Office2007BlackFeel)
                this.RenderBlackRadio.Checked = true;
            else
                this.RenderNormalRadio.Checked = true;
        }

        public override void SaveSettings()
        {
            Settings.EnableGroupsMenu = this.chkEnableGroupsMenu.Checked;
            Settings.MinimizeToTray = this.chkMinimizeToTrayCheckbox.Checked;
            Settings.ShowUserNameInTitle = this.chkShowUserNameInTitle.Checked;
            Settings.ShowInformationToolTips = this.chkShowInformationToolTips.Checked;
            Settings.ShowFullInformationToolTips = this.chkShowFullInfo.Checked;
            Settings.SortTabPagesByCaption = this.chkSortTabPagesByCaption.Checked;

            Settings.InvertTabPageOrder = this.chkInvertTabPageOrder.Checked;

            Settings.Office2007BlackFeel = false;
            Settings.Office2007BlueFeel = false;

            if (this.RenderBlueRadio.Checked)
                Settings.Office2007BlueFeel = true;
            else if (this.RenderBlackRadio.Checked)
                Settings.Office2007BlackFeel = true;
        }

        public new IHostingForm IHostingForm { get; set; }

        private void chkShowInformationToolTips_CheckedChanged(object sender, EventArgs e)
        {
            this.chkShowFullInfo.Enabled = this.chkShowInformationToolTips.Checked;
            if (!this.chkShowInformationToolTips.Checked)
            {
                this.chkShowFullInfo.Checked = false;
            }
        }
    }
}