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
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Credential Store");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Security", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5,
            treeNode6});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Execute Before Connect");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Proxy");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Connections", new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Screen Capture");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionEditor));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lnkHomepage = new System.Windows.Forms.LinkLabel();
            this.OptionTitelLabel = new System.Windows.Forms.Label();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.lblBottomLine = new System.Windows.Forms.Label();
            this.OptionsTreeView = new System.Windows.Forms.TreeView();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.pnlMain.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(631, 19);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(88, 26);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(725, 19);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 26);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lnkHomepage
            // 
            this.lnkHomepage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkHomepage.AutoSize = true;
            this.lnkHomepage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkHomepage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkHomepage.Location = new System.Drawing.Point(3, 19);
            this.lnkHomepage.Name = "lnkHomepage";
            this.lnkHomepage.Size = new System.Drawing.Size(198, 13);
            this.lnkHomepage.TabIndex = 2;
            this.lnkHomepage.TabStop = true;
            this.lnkHomepage.Text = "http://oliverkohldsc.github.io/Terminals/";
            this.lnkHomepage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkHomepage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ShowHomepage);
            // 
            // OptionTitelLabel
            // 
            this.OptionTitelLabel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.OptionTitelLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OptionTitelLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.OptionTitelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OptionTitelLabel.ForeColor = System.Drawing.Color.White;
            this.OptionTitelLabel.Location = new System.Drawing.Point(208, 8);
            this.OptionTitelLabel.Name = "OptionTitelLabel";
            this.OptionTitelLabel.Size = new System.Drawing.Size(613, 27);
            this.OptionTitelLabel.TabIndex = 1;
            this.OptionTitelLabel.Text = "Option Title";
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.lblBottomLine);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(208, 35);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pnlMain.Size = new System.Drawing.Size(613, 342);
            this.pnlMain.TabIndex = 2;
            // 
            // lblBottomLine
            // 
            this.lblBottomLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblBottomLine.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblBottomLine.Location = new System.Drawing.Point(5, 340);
            this.lblBottomLine.Name = "lblBottomLine";
            this.lblBottomLine.Size = new System.Drawing.Size(608, 2);
            this.lblBottomLine.TabIndex = 0;
            // 
            // OptionsTreeView
            // 
            this.OptionsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OptionsTreeView.HotTracking = true;
            this.OptionsTreeView.Location = new System.Drawing.Point(0, 0);
            this.OptionsTreeView.Name = "OptionsTreeView";
            treeNode1.Name = "nodeStartupShutdown";
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
            treeNode6.Name = "CredentialStore";
            treeNode6.Tag = "CredentialStore";
            treeNode6.Text = "Credential Store";
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
            treeNode11.Name = "Screen Capture";
            treeNode11.Tag = "ScreenCapture";
            treeNode11.Text = "Screen Capture";
            this.OptionsTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode3,
            treeNode7,
            treeNode10,
            treeNode11});
            this.OptionsTreeView.ShowLines = false;
            this.OptionsTreeView.Size = new System.Drawing.Size(200, 369);
            this.OptionsTreeView.TabIndex = 0;
            this.OptionsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OptionsTreeView_AfterSelect);
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.lnkHomepage);
            this.pnlBottom.Controls.Add(this.btnOk);
            this.pnlBottom.Controls.Add(this.btnCancel);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(8, 377);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(813, 45);
            this.pnlBottom.TabIndex = 3;
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.OptionsTreeView);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(8, 8);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(200, 369);
            this.pnlLeft.TabIndex = 0;
            // 
            // OptionEditor
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(829, 430);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.OptionTitelLabel);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.pnlBottom);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "OptionEditor";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OptionDialog_FormClosed);
            this.pnlMain.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.pnlLeft.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.LinkLabel lnkHomepage;
        private System.Windows.Forms.Label OptionTitelLabel;
        private System.Windows.Forms.TreeView OptionsTreeView;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Label lblBottomLine;
    }
}
