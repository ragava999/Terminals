using System.Windows.Forms;

namespace Terminals.Panels.OptionPanels
{
    partial class StartShutdownOptionPanel
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
            this.grpBackgroundImage = new System.Windows.Forms.GroupBox();
            this.picColor = new System.Windows.Forms.PictureBox();
            this.cmbStyle = new System.Windows.Forms.ComboBox();
            this.lblStyle = new System.Windows.Forms.Label();
            this.picDelete = new System.Windows.Forms.PictureBox();
            this.txtImage = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblImagePath = new System.Windows.Forms.Label();
            this.picImage = new System.Windows.Forms.PictureBox();
            this.groupBoxShutdown = new System.Windows.Forms.GroupBox();
            this.chkSaveConnections = new System.Windows.Forms.CheckBox();
            this.chkShowConfirmDialog = new System.Windows.Forms.CheckBox();
            this.groupBoxStartup = new System.Windows.Forms.GroupBox();
            this.chkCheckForNewRelease = new System.Windows.Forms.CheckBox();
            this.chkNeverShowTerminalsCheckbox = new System.Windows.Forms.CheckBox();
            this.chkSingleInstance = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.grpBackgroundImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
            this.groupBoxShutdown.SuspendLayout();
            this.groupBoxStartup.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.grpBackgroundImage);
            this.panel1.Controls.Add(this.groupBoxShutdown);
            this.panel1.Controls.Add(this.groupBoxStartup);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 328);
            this.panel1.TabIndex = 1;
            // 
            // grpBackgroundImage
            // 
            this.grpBackgroundImage.Controls.Add(this.picColor);
            this.grpBackgroundImage.Controls.Add(this.cmbStyle);
            this.grpBackgroundImage.Controls.Add(this.lblStyle);
            this.grpBackgroundImage.Controls.Add(this.picDelete);
            this.grpBackgroundImage.Controls.Add(this.txtImage);
            this.grpBackgroundImage.Controls.Add(this.btnBrowse);
            this.grpBackgroundImage.Controls.Add(this.lblImagePath);
            this.grpBackgroundImage.Controls.Add(this.picImage);
            this.grpBackgroundImage.Location = new System.Drawing.Point(9, 147);
            this.grpBackgroundImage.Name = "grpBackgroundImage";
            this.grpBackgroundImage.Size = new System.Drawing.Size(500, 98);
            this.grpBackgroundImage.TabIndex = 3;
            this.grpBackgroundImage.TabStop = false;
            this.grpBackgroundImage.Text = "Background Image";
            // 
            // picColor
            // 
            this.picColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picColor.Location = new System.Drawing.Point(431, 19);
            this.picColor.Name = "picColor";
            this.picColor.Size = new System.Drawing.Size(35, 36);
            this.picColor.TabIndex = 7;
            this.picColor.TabStop = false;
            this.picColor.Click += new System.EventHandler(this.picColor_Click);
            // 
            // cmbStyle
            // 
            this.cmbStyle.FormattingEnabled = true;
            this.cmbStyle.Items.AddRange(new object[] {
            "None",
            "Tile",
            "Center",
            "Strech",
            "Zoom"});
            this.cmbStyle.Location = new System.Drawing.Point(336, 60);
            this.cmbStyle.Name = "cmbStyle";
            this.cmbStyle.Size = new System.Drawing.Size(130, 21);
            this.cmbStyle.TabIndex = 6;
            // 
            // lblStyle
            // 
            this.lblStyle.AutoSize = true;
            this.lblStyle.Location = new System.Drawing.Point(333, 28);
            this.lblStyle.Name = "lblStyle";
            this.lblStyle.Size = new System.Drawing.Size(33, 13);
            this.lblStyle.TabIndex = 5;
            this.lblStyle.Text = "Style:";
            // 
            // picDelete
            // 
            this.picDelete.BackgroundImage = global::Terminals.Properties.Resources.DeleteIcon;
            this.picDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picDelete.Location = new System.Drawing.Point(475, 61);
            this.picDelete.Name = "picDelete";
            this.picDelete.Size = new System.Drawing.Size(13, 20);
            this.picDelete.TabIndex = 4;
            this.picDelete.TabStop = false;
            this.picDelete.Click += new System.EventHandler(this.picDelete_Click);
            // 
            // txtImage
            // 
            this.txtImage.Location = new System.Drawing.Point(128, 60);
            this.txtImage.Name = "txtImage";
            this.txtImage.Size = new System.Drawing.Size(166, 20);
            this.txtImage.TabIndex = 3;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(230, 23);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(83, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblImagePath
            // 
            this.lblImagePath.AutoSize = true;
            this.lblImagePath.Location = new System.Drawing.Point(125, 28);
            this.lblImagePath.Name = "lblImagePath";
            this.lblImagePath.Size = new System.Drawing.Size(86, 13);
            this.lblImagePath.TabIndex = 1;
            this.lblImagePath.Text = "Select an image:";
            // 
            // picImage
            // 
            this.picImage.BackgroundImage = global::Terminals.Properties.Resources.Question;
            this.picImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picImage.Location = new System.Drawing.Point(17, 19);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(72, 73);
            this.picImage.TabIndex = 0;
            this.picImage.TabStop = false;
            // 
            // groupBoxShutdown
            // 
            this.groupBoxShutdown.Controls.Add(this.chkSaveConnections);
            this.groupBoxShutdown.Controls.Add(this.chkShowConfirmDialog);
            this.groupBoxShutdown.Location = new System.Drawing.Point(9, 76);
            this.groupBoxShutdown.Name = "groupBoxShutdown";
            this.groupBoxShutdown.Size = new System.Drawing.Size(500, 65);
            this.groupBoxShutdown.TabIndex = 1;
            this.groupBoxShutdown.TabStop = false;
            this.groupBoxShutdown.Text = "Shutdown";
            // 
            // chkSaveConnections
            // 
            this.chkSaveConnections.AutoSize = true;
            this.chkSaveConnections.Checked = true;
            this.chkSaveConnections.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveConnections.Location = new System.Drawing.Point(6, 43);
            this.chkSaveConnections.Name = "chkSaveConnections";
            this.chkSaveConnections.Size = new System.Drawing.Size(155, 17);
            this.chkSaveConnections.TabIndex = 5;
            this.chkSaveConnections.Text = "Save connections on close";
            this.chkSaveConnections.UseVisualStyleBackColor = true;
            this.chkSaveConnections.CheckedChanged += new System.EventHandler(this.chkSaveConnections_CheckedChanged);
            // 
            // chkShowConfirmDialog
            // 
            this.chkShowConfirmDialog.AutoSize = true;
            this.chkShowConfirmDialog.Checked = true;
            this.chkShowConfirmDialog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowConfirmDialog.Location = new System.Drawing.Point(6, 20);
            this.chkShowConfirmDialog.Name = "chkShowConfirmDialog";
            this.chkShowConfirmDialog.Size = new System.Drawing.Size(172, 17);
            this.chkShowConfirmDialog.TabIndex = 4;
            this.chkShowConfirmDialog.Text = "Show close confirmation dialog";
            this.chkShowConfirmDialog.UseVisualStyleBackColor = true;
            // 
            // groupBoxStartup
            // 
            this.groupBoxStartup.Controls.Add(this.chkCheckForNewRelease);
            this.groupBoxStartup.Controls.Add(this.chkNeverShowTerminalsCheckbox);
            this.groupBoxStartup.Controls.Add(this.chkSingleInstance);
            this.groupBoxStartup.Location = new System.Drawing.Point(6, 3);
            this.groupBoxStartup.Name = "groupBoxStartup";
            this.groupBoxStartup.Size = new System.Drawing.Size(500, 67);
            this.groupBoxStartup.TabIndex = 0;
            this.groupBoxStartup.TabStop = false;
            this.groupBoxStartup.Text = "Startup";
            // 
            // chkCheckForNewRelease
            // 
            this.chkCheckForNewRelease.AutoSize = true;
            this.chkCheckForNewRelease.Location = new System.Drawing.Point(275, 19);
            this.chkCheckForNewRelease.Name = "chkCheckForNewRelease";
            this.chkCheckForNewRelease.Size = new System.Drawing.Size(191, 17);
            this.chkCheckForNewRelease.TabIndex = 5;
            this.chkCheckForNewRelease.Text = "Check for a new release on startup";
            this.chkCheckForNewRelease.UseVisualStyleBackColor = true;
            // 
            // chkNeverShowTerminalsCheckbox
            // 
            this.chkNeverShowTerminalsCheckbox.AutoSize = true;
            this.chkNeverShowTerminalsCheckbox.Location = new System.Drawing.Point(6, 42);
            this.chkNeverShowTerminalsCheckbox.Name = "chkNeverShowTerminalsCheckbox";
            this.chkNeverShowTerminalsCheckbox.Size = new System.Drawing.Size(205, 17);
            this.chkNeverShowTerminalsCheckbox.TabIndex = 4;
            this.chkNeverShowTerminalsCheckbox.Text = "Don\'t show Terminals news on startup";
            this.chkNeverShowTerminalsCheckbox.UseVisualStyleBackColor = true;
            // 
            // chkSingleInstance
            // 
            this.chkSingleInstance.AutoSize = true;
            this.chkSingleInstance.Checked = true;
            this.chkSingleInstance.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSingleInstance.Location = new System.Drawing.Point(6, 19);
            this.chkSingleInstance.Name = "chkSingleInstance";
            this.chkSingleInstance.Size = new System.Drawing.Size(217, 17);
            this.chkSingleInstance.TabIndex = 3;
            this.chkSingleInstance.Text = "Allow a single instance of the application";
            this.chkSingleInstance.UseVisualStyleBackColor = true;
            // 
            // StartShutdownOptionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "StartShutdownOptionPanel";
            this.Size = new System.Drawing.Size(512, 328);
            this.panel1.ResumeLayout(false);
            this.grpBackgroundImage.ResumeLayout(false);
            this.grpBackgroundImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
            this.groupBoxShutdown.ResumeLayout(false);
            this.groupBoxShutdown.PerformLayout();
            this.groupBoxStartup.ResumeLayout(false);
            this.groupBoxStartup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private GroupBox groupBoxShutdown;
        private CheckBox chkSaveConnections;
        private CheckBox chkShowConfirmDialog;
        private GroupBox groupBoxStartup;
        private CheckBox chkNeverShowTerminalsCheckbox;
        private CheckBox chkSingleInstance;
        private GroupBox grpBackgroundImage;
        private PictureBox picImage;
        private Button btnBrowse;
        private Label lblImagePath;
        private TextBox txtImage;
        private PictureBox picDelete;
        private Label lblStyle;
        private ComboBox cmbStyle;
        private PictureBox picColor;
        private CheckBox chkCheckForNewRelease;
    }
}
