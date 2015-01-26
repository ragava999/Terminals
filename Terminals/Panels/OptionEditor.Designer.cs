using Terminals.Panels.OptionPanels;

namespace Terminals.Panels
{
    partial class OptionEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Startup & Shutdown");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Favorites");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Interface", new System.Windows.Forms.TreeNode[] {
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Master Password");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Default Password");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Security", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Execute Before Connect");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Proxy");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Connections", new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9});
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Screen Capture");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionEditor));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lnkHomepage = new System.Windows.Forms.LinkLabel();
            this.OptionTitelLabel = new System.Windows.Forms.Label();
            this.tabCtrlOptionPanels = new System.Windows.Forms.TabControl();
            this.tabPageStartupShutdown = new System.Windows.Forms.TabPage();
            this.tabPageInterface = new System.Windows.Forms.TabPage();
            this.tabPageFavorites = new System.Windows.Forms.TabPage();
            this.tabPageMasterPwd = new System.Windows.Forms.TabPage();
            this.tabPageDefaultPwd = new System.Windows.Forms.TabPage();
            this.tabPageConnections = new System.Windows.Forms.TabPage();
            this.tabPageBeforeConnect = new System.Windows.Forms.TabPage();
            this.tabPageProxy = new System.Windows.Forms.TabPage();
            this.tabPageScreenCapture = new System.Windows.Forms.TabPage();
            this.OptionsTreeView = new System.Windows.Forms.TreeView();
            this.panelStartupShutdown = new Terminals.Panels.OptionPanels.StartShutdownOptionPanel();
            this.panelInterface = new Terminals.Panels.OptionPanels.InterfaceOptionPanel();
            this.panelFavorites = new Terminals.Panels.OptionPanels.FavoritesOptionPanel();
            this.panelMasterPassword = new Terminals.Panels.OptionPanels.MasterPasswordOptionPanel();
            this.panelDefaultPassword = new Terminals.Panels.OptionPanels.DefaultPasswordOptionPanel();
            this.panelConnections = new Terminals.Panels.OptionPanels.ConnectionsOptionPanel();
            this.panelExecuteBeforeConnect = new Terminals.Panels.OptionPanels.ConnectCommandOptionPanel();
            this.panelProxy = new Terminals.Panels.OptionPanels.ProxyOptionPanel();
            this.panelScreenCapture = new Terminals.Panels.OptionPanels.CaptureOptionPanel();
            this.tabCtrlOptionPanels.SuspendLayout();
            this.tabPageStartupShutdown.SuspendLayout();
            this.tabPageInterface.SuspendLayout();
            this.tabPageFavorites.SuspendLayout();
            this.tabPageMasterPwd.SuspendLayout();
            this.tabPageDefaultPwd.SuspendLayout();
            this.tabPageConnections.SuspendLayout();
            this.tabPageBeforeConnect.SuspendLayout();
            this.tabPageProxy.SuspendLayout();
            this.tabPageScreenCapture.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(697, 392);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(88, 26);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(791, 392);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 26);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lnkHomepage
            // 
            this.lnkHomepage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkHomepage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkHomepage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkHomepage.Location = new System.Drawing.Point(5, 398);
            this.lnkHomepage.Name = "lnkHomepage";
            this.lnkHomepage.Size = new System.Drawing.Size(150, 20);
            this.lnkHomepage.TabIndex = 4;
            this.lnkHomepage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkHomepage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // OptionTitelLabel
            // 
            this.OptionTitelLabel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.OptionTitelLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OptionTitelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OptionTitelLabel.ForeColor = System.Drawing.Color.White;
            this.OptionTitelLabel.Location = new System.Drawing.Point(281, 12);
            this.OptionTitelLabel.Name = "OptionTitelLabel";
            this.OptionTitelLabel.Size = new System.Drawing.Size(509, 27);
            this.OptionTitelLabel.TabIndex = 11;
            this.OptionTitelLabel.Text = "Option Title";
            // 
            // tabCtrlOptionPanels
            // 
            this.tabCtrlOptionPanels.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.tabCtrlOptionPanels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCtrlOptionPanels.Controls.Add(this.tabPageStartupShutdown);
            this.tabCtrlOptionPanels.Controls.Add(this.tabPageInterface);
            this.tabCtrlOptionPanels.Controls.Add(this.tabPageFavorites);
            this.tabCtrlOptionPanels.Controls.Add(this.tabPageMasterPwd);
            this.tabCtrlOptionPanels.Controls.Add(this.tabPageDefaultPwd);
            this.tabCtrlOptionPanels.Controls.Add(this.tabPageConnections);
            this.tabCtrlOptionPanels.Controls.Add(this.tabPageBeforeConnect);
            this.tabCtrlOptionPanels.Controls.Add(this.tabPageProxy);
            this.tabCtrlOptionPanels.Controls.Add(this.tabPageScreenCapture);
            this.tabCtrlOptionPanels.ItemSize = new System.Drawing.Size(20, 20);
            this.tabCtrlOptionPanels.Location = new System.Drawing.Point(268, 12);
            this.tabCtrlOptionPanels.Multiline = true;
            this.tabCtrlOptionPanels.Name = "tabCtrlOptionPanels";
            this.tabCtrlOptionPanels.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabCtrlOptionPanels.SelectedIndex = 0;
            this.tabCtrlOptionPanels.Size = new System.Drawing.Size(618, 365);
            this.tabCtrlOptionPanels.TabIndex = 10;
            // 
            // tabPageStartupShutdown
            // 
            this.tabPageStartupShutdown.AutoScroll = true;
            this.tabPageStartupShutdown.Controls.Add(this.panelStartupShutdown);
            this.tabPageStartupShutdown.Location = new System.Drawing.Point(4, 4);
            this.tabPageStartupShutdown.Name = "tabPageStartupShutdown";
            this.tabPageStartupShutdown.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageStartupShutdown.Size = new System.Drawing.Size(570, 357);
            this.tabPageStartupShutdown.TabIndex = 0;
            this.tabPageStartupShutdown.Text = "Startup";
            this.tabPageStartupShutdown.UseVisualStyleBackColor = true;
            // 
            // tabPageInterface
            // 
            this.tabPageInterface.AutoScroll = true;
            this.tabPageInterface.Controls.Add(this.panelInterface);
            this.tabPageInterface.Location = new System.Drawing.Point(4, 4);
            this.tabPageInterface.Name = "tabPageInterface";
            this.tabPageInterface.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInterface.Size = new System.Drawing.Size(570, 357);
            this.tabPageInterface.TabIndex = 1;
            this.tabPageInterface.Text = "Interface";
            this.tabPageInterface.UseVisualStyleBackColor = true;
            // 
            // tabPageFavorites
            // 
            this.tabPageFavorites.AutoScroll = true;
            this.tabPageFavorites.Controls.Add(this.panelFavorites);
            this.tabPageFavorites.Location = new System.Drawing.Point(4, 4);
            this.tabPageFavorites.Name = "tabPageFavorites";
            this.tabPageFavorites.Size = new System.Drawing.Size(570, 357);
            this.tabPageFavorites.TabIndex = 10;
            this.tabPageFavorites.Text = "Favorites";
            this.tabPageFavorites.UseVisualStyleBackColor = true;
            // 
            // tabPageMasterPwd
            // 
            this.tabPageMasterPwd.AutoScroll = true;
            this.tabPageMasterPwd.Controls.Add(this.panelMasterPassword);
            this.tabPageMasterPwd.Location = new System.Drawing.Point(4, 4);
            this.tabPageMasterPwd.Name = "tabPageMasterPwd";
            this.tabPageMasterPwd.Size = new System.Drawing.Size(570, 357);
            this.tabPageMasterPwd.TabIndex = 2;
            this.tabPageMasterPwd.Text = "Master Pwd";
            this.tabPageMasterPwd.UseVisualStyleBackColor = true;
            // 
            // tabPageDefaultPwd
            // 
            this.tabPageDefaultPwd.AutoScroll = true;
            this.tabPageDefaultPwd.Controls.Add(this.panelDefaultPassword);
            this.tabPageDefaultPwd.Location = new System.Drawing.Point(4, 4);
            this.tabPageDefaultPwd.Name = "tabPageDefaultPwd";
            this.tabPageDefaultPwd.Size = new System.Drawing.Size(570, 357);
            this.tabPageDefaultPwd.TabIndex = 4;
            this.tabPageDefaultPwd.Text = "Default Pwd";
            this.tabPageDefaultPwd.UseVisualStyleBackColor = true;
            // 
            // tabPageConnections
            // 
            this.tabPageConnections.AutoScroll = true;
            this.tabPageConnections.Controls.Add(this.panelConnections);
            this.tabPageConnections.Location = new System.Drawing.Point(4, 4);
            this.tabPageConnections.Name = "tabPageConnections";
            this.tabPageConnections.Size = new System.Drawing.Size(570, 357);
            this.tabPageConnections.TabIndex = 3;
            this.tabPageConnections.Text = "Connections";
            this.tabPageConnections.UseVisualStyleBackColor = true;
            // 
            // tabPageBeforeConnect
            // 
            this.tabPageBeforeConnect.AutoScroll = true;
            this.tabPageBeforeConnect.Controls.Add(this.panelExecuteBeforeConnect);
            this.tabPageBeforeConnect.Location = new System.Drawing.Point(4, 4);
            this.tabPageBeforeConnect.Name = "tabPageBeforeConnect";
            this.tabPageBeforeConnect.Size = new System.Drawing.Size(570, 357);
            this.tabPageBeforeConnect.TabIndex = 6;
            this.tabPageBeforeConnect.Text = "Before Connect";
            this.tabPageBeforeConnect.UseVisualStyleBackColor = true;
            // 
            // tabPageProxy
            // 
            this.tabPageProxy.AutoScroll = true;
            this.tabPageProxy.Controls.Add(this.panelProxy);
            this.tabPageProxy.Location = new System.Drawing.Point(4, 4);
            this.tabPageProxy.Name = "tabPageProxy";
            this.tabPageProxy.Size = new System.Drawing.Size(570, 357);
            this.tabPageProxy.TabIndex = 7;
            this.tabPageProxy.Text = "Proxy";
            this.tabPageProxy.UseVisualStyleBackColor = true;
            // 
            // tabPageScreenCapture
            // 
            this.tabPageScreenCapture.AutoScroll = true;
            this.tabPageScreenCapture.Controls.Add(this.panelScreenCapture);
            this.tabPageScreenCapture.Location = new System.Drawing.Point(4, 4);
            this.tabPageScreenCapture.Name = "tabPageScreenCapture";
            this.tabPageScreenCapture.Size = new System.Drawing.Size(570, 357);
            this.tabPageScreenCapture.TabIndex = 8;
            this.tabPageScreenCapture.Text = "Capture";
            this.tabPageScreenCapture.UseVisualStyleBackColor = true;
            // 
            // OptionsTreeView
            // 
            this.OptionsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.OptionsTreeView.HotTracking = true;
            this.OptionsTreeView.Location = new System.Drawing.Point(5, 12);
            this.OptionsTreeView.Name = "OptionsTreeView";
            treeNode1.Name = "Startup & Shutdown";
            treeNode1.Tag = "StartupShutdown";
            treeNode1.Text = "Startup & Shutdown";
            treeNode2.Name = "Favorites";
            treeNode2.Tag = "Favorites";
            treeNode2.Text = "Favorites";
            treeNode3.Name = "Interface";
            treeNode3.Tag = "Interface";
            treeNode3.Text = "Interface";
            treeNode4.Name = "Master Password";
            treeNode4.Tag = "MasterPassword";
            treeNode4.Text = "Master Password";
            treeNode5.Name = "Default Password";
            treeNode5.Tag = "DefaultPassword";
            treeNode5.Text = "Default Password";
            treeNode7.Name = "Master Password";
            treeNode7.Tag = "MasterPassword";
            treeNode7.Text = "Security";
            treeNode8.Name = "Execute Before Connect";
            treeNode8.Tag = "ExecuteBeforeConnect";
            treeNode8.Text = "Execute Before Connect";
            treeNode9.Name = "Proxy";
            treeNode9.Tag = "Proxy";
            treeNode9.Text = "Proxy";
            treeNode10.Name = "Connections";
            treeNode10.Tag = "Connections";
            treeNode10.Text = "Connections";
            treeNode12.Name = "Screen Capture";
            treeNode12.Tag = "ScreenCapture";
            treeNode12.Text = "Screen Capture";
            this.OptionsTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode3,
            treeNode7,
            treeNode10,
            treeNode12});
            this.OptionsTreeView.ShowLines = false;
            this.OptionsTreeView.Size = new System.Drawing.Size(261, 365);
            this.OptionsTreeView.TabIndex = 9;
            this.OptionsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OptionsTreeView_AfterSelect);
            // 
            // panelStartupShutdown
            // 
            this.panelStartupShutdown.IHostingForm = null;
            this.panelStartupShutdown.Location = new System.Drawing.Point(6, 26);
            this.panelStartupShutdown.Name = "panelStartupShutdown";
            this.panelStartupShutdown.Size = new System.Drawing.Size(512, 328);
            this.panelStartupShutdown.TabIndex = 0;
            // 
            // panelInterface
            // 
            this.panelInterface.IHostingForm = null;
            this.panelInterface.Location = new System.Drawing.Point(8, 27);
            this.panelInterface.Name = "panelInterface";
            this.panelInterface.Size = new System.Drawing.Size(519, 325);
            this.panelInterface.TabIndex = 0;
            // 
            // panelFavorites
            // 
            this.panelFavorites.IHostingForm = null;
            this.panelFavorites.Location = new System.Drawing.Point(4, 26);
            this.panelFavorites.Name = "panelFavorites";
            this.panelFavorites.Size = new System.Drawing.Size(519, 328);
            this.panelFavorites.TabIndex = 0;
            // 
            // panelMasterPassword
            // 
            this.panelMasterPassword.IHostingForm = null;
            this.panelMasterPassword.Location = new System.Drawing.Point(8, 27);
            this.panelMasterPassword.Name = "panelMasterPassword";
            this.panelMasterPassword.Size = new System.Drawing.Size(511, 327);
            this.panelMasterPassword.TabIndex = 0;
            // 
            // panelDefaultPassword
            // 
            this.panelDefaultPassword.IHostingForm = null;
            this.panelDefaultPassword.Location = new System.Drawing.Point(4, 29);
            this.panelDefaultPassword.Name = "panelDefaultPassword";
            this.panelDefaultPassword.Size = new System.Drawing.Size(513, 325);
            this.panelDefaultPassword.TabIndex = 0;
            // 
            // panelConnections
            // 
            this.panelConnections.IHostingForm = null;
            this.panelConnections.Location = new System.Drawing.Point(4, 22);
            this.panelConnections.Name = "panelConnections";
            this.panelConnections.Size = new System.Drawing.Size(514, 332);
            this.panelConnections.TabIndex = 0;
            // 
            // panelExecuteBeforeConnect
            // 
            this.panelExecuteBeforeConnect.IHostingForm = null;
            this.panelExecuteBeforeConnect.Location = new System.Drawing.Point(3, 26);
            this.panelExecuteBeforeConnect.Name = "panelExecuteBeforeConnect";
            this.panelExecuteBeforeConnect.Size = new System.Drawing.Size(513, 327);
            this.panelExecuteBeforeConnect.TabIndex = 0;
            // 
            // panelProxy
            // 
            this.panelProxy.IHostingForm = null;
            this.panelProxy.Location = new System.Drawing.Point(3, 22);
            this.panelProxy.Name = "panelProxy";
            this.panelProxy.Size = new System.Drawing.Size(514, 332);
            this.panelProxy.TabIndex = 0;
            // 
            // panelScreenCapture
            // 
            this.panelScreenCapture.IHostingForm = null;
            this.panelScreenCapture.Location = new System.Drawing.Point(4, 24);
            this.panelScreenCapture.Name = "panelScreenCapture";
            this.panelScreenCapture.Size = new System.Drawing.Size(513, 330);
            this.panelScreenCapture.TabIndex = 0;
            // 
            // OptionEditor
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(891, 430);
            this.Controls.Add(this.OptionTitelLabel);
            this.Controls.Add(this.tabCtrlOptionPanels);
            this.Controls.Add(this.OptionsTreeView);
            this.Controls.Add(this.lnkHomepage);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionEditor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OptionDialog_FormClosed);
            this.tabCtrlOptionPanels.ResumeLayout(false);
            this.tabPageStartupShutdown.ResumeLayout(false);
            this.tabPageInterface.ResumeLayout(false);
            this.tabPageFavorites.ResumeLayout(false);
            this.tabPageMasterPwd.ResumeLayout(false);
            this.tabPageDefaultPwd.ResumeLayout(false);
            this.tabPageConnections.ResumeLayout(false);
            this.tabPageBeforeConnect.ResumeLayout(false);
            this.tabPageProxy.ResumeLayout(false);
            this.tabPageScreenCapture.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.LinkLabel lnkHomepage;
        private System.Windows.Forms.Label OptionTitelLabel;
        private System.Windows.Forms.TabControl tabCtrlOptionPanels;
        private System.Windows.Forms.TabPage tabPageStartupShutdown;
        private StartShutdownOptionPanel panelStartupShutdown;
        private System.Windows.Forms.TabPage tabPageInterface;
        private InterfaceOptionPanel panelInterface;
        private System.Windows.Forms.TabPage tabPageFavorites;
        private FavoritesOptionPanel panelFavorites;
        private System.Windows.Forms.TabPage tabPageMasterPwd;
        private MasterPasswordOptionPanel panelMasterPassword;
        private System.Windows.Forms.TabPage tabPageDefaultPwd;
        private DefaultPasswordOptionPanel panelDefaultPassword;
        private System.Windows.Forms.TabPage tabPageConnections;
        private ConnectionsOptionPanel panelConnections;
        private System.Windows.Forms.TabPage tabPageBeforeConnect;
        private ConnectCommandOptionPanel panelExecuteBeforeConnect;
        private System.Windows.Forms.TabPage tabPageProxy;
        private ProxyOptionPanel panelProxy;
        private System.Windows.Forms.TabPage tabPageScreenCapture;
        private CaptureOptionPanel panelScreenCapture;
        private System.Windows.Forms.TreeView OptionsTreeView;
    }
}
