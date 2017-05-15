using System.Windows.Forms;

namespace Terminals.Panels.OptionPanels
{
    partial class RAdminOptionPanel
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
        	this.panRAdmin = new System.Windows.Forms.Panel();
        	this.grpRAdminDefaultPort = new System.Windows.Forms.GroupBox();
        	this.txtRAdminDefaultPort = new System.Windows.Forms.TextBox();
        	this.grpRAdminPath = new System.Windows.Forms.GroupBox();
        	this.txtRAdminPath = new System.Windows.Forms.TextBox();
        	this.ButtonBrowseCaptureFolder = new System.Windows.Forms.Button();
        	this.panRAdmin.SuspendLayout();
        	this.grpRAdminDefaultPort.SuspendLayout();
        	this.grpRAdminPath.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// panRAdmin
        	// 
        	this.panRAdmin.Controls.Add(this.grpRAdminDefaultPort);
        	this.panRAdmin.Controls.Add(this.grpRAdminPath);
        	this.panRAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.panRAdmin.Location = new System.Drawing.Point(0, 0);
        	this.panRAdmin.Name = "panRAdmin";
        	this.panRAdmin.Size = new System.Drawing.Size(514, 332);
        	this.panRAdmin.TabIndex = 25;
        	// 
        	// grpRAdminDefaultPort
        	// 
        	this.grpRAdminDefaultPort.Controls.Add(this.txtRAdminDefaultPort);
        	this.grpRAdminDefaultPort.Location = new System.Drawing.Point(385, 3);
        	this.grpRAdminDefaultPort.Name = "grpRAdminDefaultPort";
        	this.grpRAdminDefaultPort.Size = new System.Drawing.Size(122, 65);
        	this.grpRAdminDefaultPort.TabIndex = 5;
        	this.grpRAdminDefaultPort.TabStop = false;
        	this.grpRAdminDefaultPort.Text = "RAdmin default port";
        	// 
        	// txtRAdminDefaultPort
        	// 
        	this.txtRAdminDefaultPort.Location = new System.Drawing.Point(11, 30);
        	this.txtRAdminDefaultPort.Name = "txtRAdminDefaultPort";
        	this.txtRAdminDefaultPort.Size = new System.Drawing.Size(105, 20);
        	this.txtRAdminDefaultPort.TabIndex = 19;
        	// 
        	// grpRAdminPath
        	// 
        	this.grpRAdminPath.Controls.Add(this.ButtonBrowseCaptureFolder);
        	this.grpRAdminPath.Controls.Add(this.txtRAdminPath);
        	this.grpRAdminPath.Location = new System.Drawing.Point(8, 3);
        	this.grpRAdminPath.Name = "grpRAdminPath";
        	this.grpRAdminPath.Size = new System.Drawing.Size(371, 65);
        	this.grpRAdminPath.TabIndex = 4;
        	this.grpRAdminPath.TabStop = false;
        	this.grpRAdminPath.Text = "Path to the RAdmin executable";
        	// 
        	// txtRAdminPath
        	// 
        	this.txtRAdminPath.Location = new System.Drawing.Point(11, 30);
        	this.txtRAdminPath.Name = "txtRAdminPath";
        	this.txtRAdminPath.Size = new System.Drawing.Size(234, 20);
        	this.txtRAdminPath.TabIndex = 19;
        	// 
        	// ButtonBrowseCaptureFolder
        	// 
        	this.ButtonBrowseCaptureFolder.Location = new System.Drawing.Point(254, 28);
        	this.ButtonBrowseCaptureFolder.Name = "ButtonBrowseCaptureFolder";
        	this.ButtonBrowseCaptureFolder.Size = new System.Drawing.Size(111, 23);
        	this.ButtonBrowseCaptureFolder.TabIndex = 26;
        	this.ButtonBrowseCaptureFolder.Text = "Browse...";
        	this.ButtonBrowseCaptureFolder.UseVisualStyleBackColor = true;
        	this.ButtonBrowseCaptureFolder.Click += new System.EventHandler(this.ButtonBrowseCaptureFolderClick);
        	// 
        	// RAdminOptionPanel
        	// 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.Controls.Add(this.panRAdmin);
        	this.Name = "RAdminOptionPanel";
        	this.Size = new System.Drawing.Size(514, 332);
        	this.panRAdmin.ResumeLayout(false);
        	this.grpRAdminDefaultPort.ResumeLayout(false);
        	this.grpRAdminDefaultPort.PerformLayout();
        	this.grpRAdminPath.ResumeLayout(false);
        	this.grpRAdminPath.PerformLayout();
        	this.ResumeLayout(false);

        }

        #endregion

        private Panel panRAdmin;
        private GroupBox grpRAdminPath;
        private TextBox txtRAdminPath;
        private GroupBox grpRAdminDefaultPort;
        private TextBox txtRAdminDefaultPort;
        private System.Windows.Forms.Button ButtonBrowseCaptureFolder;
    }
}
