using System.Windows.Forms;

namespace Terminals.Panels.OptionPanels
{
    partial class PuttyOptionPanel
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
        	this.panPutty = new System.Windows.Forms.Panel();
        	this.grpPuttyPath = new System.Windows.Forms.GroupBox();
        	this.txtPuttyPath = new System.Windows.Forms.TextBox();
        	this.ButtonBrowseCaptureFolder = new System.Windows.Forms.Button();
        	this.panPutty.SuspendLayout();
        	this.grpPuttyPath.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// panPutty
        	// 
        	this.panPutty.Controls.Add(this.grpPuttyPath);
        	this.panPutty.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.panPutty.Location = new System.Drawing.Point(0, 0);
        	this.panPutty.Name = "panPutty";
        	this.panPutty.Size = new System.Drawing.Size(514, 332);
        	this.panPutty.TabIndex = 25;
        	// 
        	// grpPuttyPath
        	// 
        	this.grpPuttyPath.Controls.Add(this.ButtonBrowseCaptureFolder);
        	this.grpPuttyPath.Controls.Add(this.txtPuttyPath);
        	this.grpPuttyPath.Location = new System.Drawing.Point(7, 3);
        	this.grpPuttyPath.Name = "grpPuttyPath";
        	this.grpPuttyPath.Size = new System.Drawing.Size(500, 65);
        	this.grpPuttyPath.TabIndex = 4;
        	this.grpPuttyPath.TabStop = false;
        	this.grpPuttyPath.Text = "Path to the PUTTY executable";
        	// 
        	// txtPuttyPath
        	// 
        	this.txtPuttyPath.Location = new System.Drawing.Point(11, 30);
        	this.txtPuttyPath.Name = "txtPuttyPath";
        	this.txtPuttyPath.Size = new System.Drawing.Size(366, 20);
        	this.txtPuttyPath.TabIndex = 19;
        	// 
        	// ButtonBrowseCaptureFolder
        	// 
        	this.ButtonBrowseCaptureFolder.Location = new System.Drawing.Point(383, 28);
        	this.ButtonBrowseCaptureFolder.Name = "ButtonBrowseCaptureFolder";
        	this.ButtonBrowseCaptureFolder.Size = new System.Drawing.Size(111, 23);
        	this.ButtonBrowseCaptureFolder.TabIndex = 26;
        	this.ButtonBrowseCaptureFolder.Text = "Browse...";
        	this.ButtonBrowseCaptureFolder.UseVisualStyleBackColor = true;
        	this.ButtonBrowseCaptureFolder.Click += new System.EventHandler(this.ButtonBrowseCaptureFolderClick);
        	// 
        	// PuttyOptionPanel
        	// 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.Controls.Add(this.panPutty);
        	this.Name = "PuttyOptionPanel";
        	this.Size = new System.Drawing.Size(514, 332);
        	this.panPutty.ResumeLayout(false);
        	this.grpPuttyPath.ResumeLayout(false);
        	this.grpPuttyPath.PerformLayout();
        	this.ResumeLayout(false);

        }

        #endregion

        private Panel panPutty;
        private GroupBox grpPuttyPath;
        private TextBox txtPuttyPath;
        private System.Windows.Forms.Button ButtonBrowseCaptureFolder;
    }
}