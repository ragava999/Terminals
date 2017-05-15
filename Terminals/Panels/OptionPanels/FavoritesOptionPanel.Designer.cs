using System.Windows.Forms;

namespace Terminals.Panels.OptionPanels
{
    partial class FavoritesOptionPanel
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        	this.panel1 = new System.Windows.Forms.Panel();
        	this.groupBox1 = new System.Windows.Forms.GroupBox();
        	this.ImageWidth = new System.Windows.Forms.NumericUpDown();
        	this.ImageHeight = new System.Windows.Forms.NumericUpDown();
        	this.lblImageWidth = new System.Windows.Forms.Label();
        	this.lblImageHeight = new System.Windows.Forms.Label();
        	this.FavoritesFont = new Terminals.Forms.Controls.FontControl();
        	this.FavSortGroupBox = new System.Windows.Forms.GroupBox();
        	this.NoneRadioButton = new System.Windows.Forms.RadioButton();
        	this.ProtocolRadionButton = new System.Windows.Forms.RadioButton();
        	this.ConnectionNameRadioButton = new System.Windows.Forms.RadioButton();
        	this.ServerNameRadio = new System.Windows.Forms.RadioButton();
        	this.groupBox11 = new System.Windows.Forms.GroupBox();
        	this.chkAutoSetTag = new System.Windows.Forms.CheckBox();
        	this.chkHideFavoritesFromQuickMenu = new System.Windows.Forms.CheckBox();
        	this.chkAutoCaseTags = new System.Windows.Forms.CheckBox();
        	this.chkAutoExapandTagsPanel = new System.Windows.Forms.CheckBox();
        	this.chkEnableFavoritesPanel = new System.Windows.Forms.CheckBox();
        	this.panel1.SuspendLayout();
        	this.groupBox1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.ImageWidth)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.ImageHeight)).BeginInit();
        	this.FavSortGroupBox.SuspendLayout();
        	this.groupBox11.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// panel1
        	// 
        	this.panel1.Controls.Add(this.groupBox1);
        	this.panel1.Controls.Add(this.FavSortGroupBox);
        	this.panel1.Controls.Add(this.groupBox11);
        	this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.panel1.Location = new System.Drawing.Point(0, 0);
        	this.panel1.Name = "panel1";
        	this.panel1.Size = new System.Drawing.Size(519, 335);
        	this.panel1.TabIndex = 2;
        	// 
        	// groupBox1
        	// 
        	this.groupBox1.Controls.Add(this.ImageWidth);
        	this.groupBox1.Controls.Add(this.ImageHeight);
        	this.groupBox1.Controls.Add(this.lblImageWidth);
        	this.groupBox1.Controls.Add(this.lblImageHeight);
        	this.groupBox1.Controls.Add(this.FavoritesFont);
        	this.groupBox1.Location = new System.Drawing.Point(9, 225);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(500, 99);
        	this.groupBox1.TabIndex = 27;
        	this.groupBox1.TabStop = false;
        	this.groupBox1.Text = "Favorites Size";
        	// 
        	// ImageWidth
        	// 
            this.ImageWidth.Location = new System.Drawing.Point(414, 44);
        	this.ImageWidth.Name = "ImageWidth";
        	this.ImageWidth.Size = new System.Drawing.Size(77, 20);
        	this.ImageWidth.TabIndex = 4;
        	// 
        	// ImageHeight
        	// 
            this.ImageHeight.Location = new System.Drawing.Point(414, 17);
        	this.ImageHeight.Name = "ImageHeight";
        	this.ImageHeight.Size = new System.Drawing.Size(77, 20);
        	this.ImageHeight.TabIndex = 3;
        	// 
        	// lblImageWidth
        	// 
        	this.lblImageWidth.AutoSize = true;
            this.lblImageWidth.Location = new System.Drawing.Point(338, 46);
        	this.lblImageWidth.Name = "lblImageWidth";
        	this.lblImageWidth.Size = new System.Drawing.Size(67, 13);
        	this.lblImageWidth.TabIndex = 2;
        	this.lblImageWidth.Text = "Image width:";
        	// 
        	// lblImageHeight
        	// 
        	this.lblImageHeight.AutoSize = true;
            this.lblImageHeight.Location = new System.Drawing.Point(338, 19);
        	this.lblImageHeight.Name = "lblImageHeight";
        	this.lblImageHeight.Size = new System.Drawing.Size(71, 13);
        	this.lblImageHeight.TabIndex = 1;
        	this.lblImageHeight.Text = "Image height:";
        	// 
        	// FavoritesFont
        	// 
        	this.FavoritesFont.AutoSize = true;
        	this.FavoritesFont.Location = new System.Drawing.Point(7, 14);
        	this.FavoritesFont.Name = "FavoritesFont";
            this.FavoritesFont.Size = new System.Drawing.Size(328, 83);
        	this.FavoritesFont.TabIndex = 0;
        	// 
        	// FavSortGroupBox
        	// 
        	this.FavSortGroupBox.Controls.Add(this.NoneRadioButton);
        	this.FavSortGroupBox.Controls.Add(this.ProtocolRadionButton);
        	this.FavSortGroupBox.Controls.Add(this.ConnectionNameRadioButton);
        	this.FavSortGroupBox.Controls.Add(this.ServerNameRadio);
        	this.FavSortGroupBox.Location = new System.Drawing.Point(9, 151);
        	this.FavSortGroupBox.Name = "FavSortGroupBox";
        	this.FavSortGroupBox.Size = new System.Drawing.Size(500, 68);
        	this.FavSortGroupBox.TabIndex = 26;
        	this.FavSortGroupBox.TabStop = false;
        	this.FavSortGroupBox.Text = "Favorites Sort";
        	// 
        	// NoneRadioButton
        	// 
        	this.NoneRadioButton.AutoSize = true;
        	this.NoneRadioButton.Location = new System.Drawing.Point(313, 41);
        	this.NoneRadioButton.Name = "NoneRadioButton";
        	this.NoneRadioButton.Size = new System.Drawing.Size(51, 17);
        	this.NoneRadioButton.TabIndex = 3;
        	this.NoneRadioButton.TabStop = true;
        	this.NoneRadioButton.Text = "None";
        	this.NoneRadioButton.UseVisualStyleBackColor = true;
        	// 
        	// ProtocolRadionButton
        	// 
        	this.ProtocolRadionButton.AutoSize = true;
        	this.ProtocolRadionButton.Location = new System.Drawing.Point(313, 18);
        	this.ProtocolRadionButton.Name = "ProtocolRadionButton";
        	this.ProtocolRadionButton.Size = new System.Drawing.Size(64, 17);
        	this.ProtocolRadionButton.TabIndex = 2;
        	this.ProtocolRadionButton.TabStop = true;
        	this.ProtocolRadionButton.Text = "Protocol";
        	this.ProtocolRadionButton.UseVisualStyleBackColor = true;
        	// 
        	// ConnectionNameRadioButton
        	// 
        	this.ConnectionNameRadioButton.AutoSize = true;
        	this.ConnectionNameRadioButton.Location = new System.Drawing.Point(7, 41);
        	this.ConnectionNameRadioButton.Name = "ConnectionNameRadioButton";
        	this.ConnectionNameRadioButton.Size = new System.Drawing.Size(110, 17);
        	this.ConnectionNameRadioButton.TabIndex = 1;
        	this.ConnectionNameRadioButton.TabStop = true;
        	this.ConnectionNameRadioButton.Text = "Connection Name";
        	this.ConnectionNameRadioButton.UseVisualStyleBackColor = true;
        	// 
        	// ServerNameRadio
        	// 
        	this.ServerNameRadio.AutoSize = true;
        	this.ServerNameRadio.Location = new System.Drawing.Point(7, 18);
        	this.ServerNameRadio.Name = "ServerNameRadio";
        	this.ServerNameRadio.Size = new System.Drawing.Size(87, 17);
        	this.ServerNameRadio.TabIndex = 0;
        	this.ServerNameRadio.TabStop = true;
        	this.ServerNameRadio.Text = "Server Name";
        	this.ServerNameRadio.UseVisualStyleBackColor = true;
        	// 
        	// groupBox11
        	// 
        	this.groupBox11.Controls.Add(this.chkAutoSetTag);
        	this.groupBox11.Controls.Add(this.chkHideFavoritesFromQuickMenu);
        	this.groupBox11.Controls.Add(this.chkAutoCaseTags);
        	this.groupBox11.Controls.Add(this.chkAutoExapandTagsPanel);
        	this.groupBox11.Controls.Add(this.chkEnableFavoritesPanel);
        	this.groupBox11.Location = new System.Drawing.Point(9, 1);
        	this.groupBox11.Name = "groupBox11";
        	this.groupBox11.Size = new System.Drawing.Size(500, 148);
        	this.groupBox11.TabIndex = 0;
        	this.groupBox11.TabStop = false;
        	// 
        	// chkAutoSetTag
        	// 
        	this.chkAutoSetTag.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
        	this.chkAutoSetTag.Location = new System.Drawing.Point(7, 88);
        	this.chkAutoSetTag.Name = "chkAutoSetTag";
        	this.chkAutoSetTag.Size = new System.Drawing.Size(416, 25);
        	this.chkAutoSetTag.TabIndex = 28;
        	this.chkAutoSetTag.Text = "Use the currently selected tag node in the TreeView for new connections.";
        	this.chkAutoSetTag.TextAlign = System.Drawing.ContentAlignment.TopLeft;
        	this.chkAutoSetTag.UseVisualStyleBackColor = true;
        	// 
        	// chkHideFavoritesFromQuickMenu
        	// 
        	this.chkHideFavoritesFromQuickMenu.AutoSize = true;
        	this.chkHideFavoritesFromQuickMenu.Location = new System.Drawing.Point(7, 115);
        	this.chkHideFavoritesFromQuickMenu.Name = "chkHideFavoritesFromQuickMenu";
        	this.chkHideFavoritesFromQuickMenu.Size = new System.Drawing.Size(181, 17);
        	this.chkHideFavoritesFromQuickMenu.TabIndex = 29;
        	this.chkHideFavoritesFromQuickMenu.Text = "Hide favorites from context menu";
        	this.chkHideFavoritesFromQuickMenu.UseVisualStyleBackColor = true;
        	// 
        	// chkAutoCaseTags
        	// 
        	this.chkAutoCaseTags.AutoSize = true;
        	this.chkAutoCaseTags.Location = new System.Drawing.Point(7, 65);
        	this.chkAutoCaseTags.Name = "chkAutoCaseTags";
        	this.chkAutoCaseTags.Size = new System.Drawing.Size(143, 17);
        	this.chkAutoCaseTags.TabIndex = 23;
        	this.chkAutoCaseTags.Text = "Auto-Case Favorite Tags";
        	this.chkAutoCaseTags.UseVisualStyleBackColor = true;
        	// 
        	// chkAutoExapandTagsPanel
        	// 
        	this.chkAutoExapandTagsPanel.AutoSize = true;
        	this.chkAutoExapandTagsPanel.Location = new System.Drawing.Point(7, 41);
        	this.chkAutoExapandTagsPanel.Name = "chkAutoExapandTagsPanel";
        	this.chkAutoExapandTagsPanel.Size = new System.Drawing.Size(264, 17);
        	this.chkAutoExapandTagsPanel.TabIndex = 27;
        	this.chkAutoExapandTagsPanel.Text = "Auto Expand (show) Favorites Panel, when hidden";
        	this.chkAutoExapandTagsPanel.UseVisualStyleBackColor = true;
        	// 
        	// chkEnableFavoritesPanel
        	// 
        	this.chkEnableFavoritesPanel.AutoSize = true;
        	this.chkEnableFavoritesPanel.Checked = true;
        	this.chkEnableFavoritesPanel.CheckState = System.Windows.Forms.CheckState.Checked;
        	this.chkEnableFavoritesPanel.Location = new System.Drawing.Point(7, 18);
        	this.chkEnableFavoritesPanel.Name = "chkEnableFavoritesPanel";
        	this.chkEnableFavoritesPanel.Size = new System.Drawing.Size(135, 17);
        	this.chkEnableFavoritesPanel.TabIndex = 24;
        	this.chkEnableFavoritesPanel.Text = "Enable Favorites Panel";
        	this.chkEnableFavoritesPanel.UseVisualStyleBackColor = true;
        	// 
        	// FavoritesOptionPanel
        	// 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.Controls.Add(this.panel1);
        	this.Name = "FavoritesOptionPanel";
        	this.Size = new System.Drawing.Size(519, 335);
        	this.panel1.ResumeLayout(false);
        	this.groupBox1.ResumeLayout(false);
        	this.groupBox1.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.ImageWidth)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.ImageHeight)).EndInit();
        	this.FavSortGroupBox.ResumeLayout(false);
        	this.FavSortGroupBox.PerformLayout();
        	this.groupBox11.ResumeLayout(false);
        	this.groupBox11.PerformLayout();
        	this.ResumeLayout(false);

        }
        private System.Windows.Forms.Label lblImageHeight;
        private System.Windows.Forms.Label lblImageWidth;
        private System.Windows.Forms.NumericUpDown ImageHeight;
        private System.Windows.Forms.NumericUpDown ImageWidth;

        #endregion

        private Panel panel1;
        private GroupBox FavSortGroupBox;
        private RadioButton NoneRadioButton;
        private RadioButton ProtocolRadionButton;
        private RadioButton ConnectionNameRadioButton;
        private RadioButton ServerNameRadio;
        private GroupBox groupBox11;
        private CheckBox chkAutoCaseTags;
        private CheckBox chkAutoExapandTagsPanel;
        private CheckBox chkEnableFavoritesPanel;
        private GroupBox groupBox1;
        private Forms.Controls.FontControl FavoritesFont;
        private CheckBox chkAutoSetTag;
        private CheckBox chkHideFavoritesFromQuickMenu;
    }
}