using System.Windows.Forms;

namespace Terminals.Panels.OptionPanels
{
    partial class CredentialStoreOptionPanel
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
        	this.rdoUseDatabase = new System.Windows.Forms.RadioButton();
        	this.rdoUseKeePass = new System.Windows.Forms.RadioButton();
        	this.rdoUseCredentialsXml = new System.Windows.Forms.RadioButton();
        	this.grpKeePass = new System.Windows.Forms.GroupBox();
        	this.label1 = new System.Windows.Forms.Label();
        	this.lblKeePassCaption = new System.Windows.Forms.Label();
        	this.ButtonBrowseCaptureFolder = new System.Windows.Forms.Button();
        	this.lblKeePassPath = new System.Windows.Forms.Label();
        	this.txtKeePassPath = new System.Windows.Forms.TextBox();
        	this.lblKeePassPassword = new System.Windows.Forms.Label();
        	this.txtKeePassPassword = new System.Windows.Forms.TextBox();
        	this.panel1.SuspendLayout();
        	this.grpKeePass.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// panel1
        	// 
        	this.panel1.Controls.Add(this.rdoUseDatabase);
        	this.panel1.Controls.Add(this.rdoUseKeePass);
        	this.panel1.Controls.Add(this.rdoUseCredentialsXml);
        	this.panel1.Controls.Add(this.grpKeePass);
        	this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.panel1.Location = new System.Drawing.Point(0, 0);
        	this.panel1.Name = "panel1";
        	this.panel1.Size = new System.Drawing.Size(511, 332);
        	this.panel1.TabIndex = 2;
        	// 
        	// rdoUseDatabase
        	// 
        	this.rdoUseDatabase.Enabled = false;
        	this.rdoUseDatabase.Location = new System.Drawing.Point(18, 217);
        	this.rdoUseDatabase.Name = "rdoUseDatabase";
        	this.rdoUseDatabase.Size = new System.Drawing.Size(246, 24);
        	this.rdoUseDatabase.TabIndex = 16;
        	this.rdoUseDatabase.TabStop = true;
        	this.rdoUseDatabase.Text = "Use Database (In a later Terminals Version)";
        	this.rdoUseDatabase.UseVisualStyleBackColor = true;
        	this.rdoUseDatabase.CheckedChanged += new System.EventHandler(this.CredentialStoreChanged);
        	// 
        	// rdoUseKeePass
        	// 
        	this.rdoUseKeePass.Location = new System.Drawing.Point(18, 42);
        	this.rdoUseKeePass.Name = "rdoUseKeePass";
        	this.rdoUseKeePass.Size = new System.Drawing.Size(176, 24);
        	this.rdoUseKeePass.TabIndex = 15;
        	this.rdoUseKeePass.TabStop = true;
        	this.rdoUseKeePass.Text = "Use KeePass 2";
        	this.rdoUseKeePass.UseVisualStyleBackColor = true;
        	this.rdoUseKeePass.CheckedChanged += new System.EventHandler(this.CredentialStoreChanged);
        	// 
        	// rdoUseCredentialsXml
        	// 
        	this.rdoUseCredentialsXml.Location = new System.Drawing.Point(18, 12);
        	this.rdoUseCredentialsXml.Name = "rdoUseCredentialsXml";
        	this.rdoUseCredentialsXml.Size = new System.Drawing.Size(201, 24);
        	this.rdoUseCredentialsXml.TabIndex = 14;
        	this.rdoUseCredentialsXml.TabStop = true;
        	this.rdoUseCredentialsXml.Text = "Use XML Store (Credentials.xml)";
        	this.rdoUseCredentialsXml.UseVisualStyleBackColor = true;
        	this.rdoUseCredentialsXml.CheckedChanged += new System.EventHandler(this.CredentialStoreChanged);
        	// 
        	// grpKeePass
        	// 
        	this.grpKeePass.Controls.Add(this.label1);
        	this.grpKeePass.Controls.Add(this.lblKeePassCaption);
        	this.grpKeePass.Controls.Add(this.ButtonBrowseCaptureFolder);
        	this.grpKeePass.Controls.Add(this.lblKeePassPath);
        	this.grpKeePass.Controls.Add(this.txtKeePassPath);
        	this.grpKeePass.Controls.Add(this.lblKeePassPassword);
        	this.grpKeePass.Controls.Add(this.txtKeePassPassword);
        	this.grpKeePass.Location = new System.Drawing.Point(5, 65);
        	this.grpKeePass.Name = "grpKeePass";
        	this.grpKeePass.Size = new System.Drawing.Size(500, 146);
        	this.grpKeePass.TabIndex = 13;
        	this.grpKeePass.TabStop = false;
        	// 
        	// label1
        	// 
        	this.label1.ForeColor = System.Drawing.Color.Blue;
        	this.label1.Location = new System.Drawing.Point(13, 104);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(481, 32);
        	this.label1.TabIndex = 27;
        	this.label1.Text = "Hint: The domain will be extracted from either the UserName field in KeePass (if " +
	"it has the form of DOMAIN\\USERNAME) or from an advanced field called Domain.";
        	// 
        	// lblKeePassCaption
        	// 
        	this.lblKeePassCaption.Location = new System.Drawing.Point(13, 16);
        	this.lblKeePassCaption.Name = "lblKeePassCaption";
        	this.lblKeePassCaption.Size = new System.Drawing.Size(481, 32);
        	this.lblKeePassCaption.TabIndex = 26;
        	this.lblKeePassCaption.Text = "Tired of using the credentials.xml file as Terminals\' default credential store? U" +
	"se your own private KeePass 2 database including all its encryption benefits.";
        	// 
        	// ButtonBrowseCaptureFolder
        	// 
        	this.ButtonBrowseCaptureFolder.Location = new System.Drawing.Point(304, 49);
        	this.ButtonBrowseCaptureFolder.Name = "ButtonBrowseCaptureFolder";
        	this.ButtonBrowseCaptureFolder.Size = new System.Drawing.Size(111, 23);
        	this.ButtonBrowseCaptureFolder.TabIndex = 25;
        	this.ButtonBrowseCaptureFolder.Text = "Browse...";
        	this.ButtonBrowseCaptureFolder.UseVisualStyleBackColor = true;
        	this.ButtonBrowseCaptureFolder.Click += new System.EventHandler(this.ButtonBrowse_Click);
        	// 
        	// lblKeePassPath
        	// 
        	this.lblKeePassPath.AutoSize = true;
        	this.lblKeePassPath.Location = new System.Drawing.Point(13, 54);
        	this.lblKeePassPath.Name = "lblKeePassPath";
        	this.lblKeePassPath.Size = new System.Drawing.Size(63, 13);
        	this.lblKeePassPath.TabIndex = 3;
        	this.lblKeePassPath.Text = "KDBX path:";
        	// 
        	// txtKeePassPath
        	// 
        	this.txtKeePassPath.Location = new System.Drawing.Point(113, 51);
        	this.txtKeePassPath.Name = "txtKeePassPath";
        	this.txtKeePassPath.Size = new System.Drawing.Size(176, 20);
        	this.txtKeePassPath.TabIndex = 1;
        	// 
        	// lblKeePassPassword
        	// 
        	this.lblKeePassPassword.AutoSize = true;
        	this.lblKeePassPassword.Location = new System.Drawing.Point(13, 82);
        	this.lblKeePassPassword.Name = "lblKeePassPassword";
        	this.lblKeePassPassword.Size = new System.Drawing.Size(56, 13);
        	this.lblKeePassPassword.TabIndex = 4;
        	this.lblKeePassPassword.Text = "Password:";
        	// 
        	// txtKeePassPassword
        	// 
        	this.txtKeePassPassword.Location = new System.Drawing.Point(113, 79);
        	this.txtKeePassPassword.Name = "txtKeePassPassword";
        	this.txtKeePassPassword.Size = new System.Drawing.Size(176, 20);
        	this.txtKeePassPassword.TabIndex = 2;
        	// 
        	// CredentialStoreOptionPanel
        	// 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.Controls.Add(this.panel1);
        	this.Name = "CredentialStoreOptionPanel";
        	this.Size = new System.Drawing.Size(511, 332);
        	this.panel1.ResumeLayout(false);
        	this.grpKeePass.ResumeLayout(false);
        	this.grpKeePass.PerformLayout();
        	this.ResumeLayout(false);

        }

        #endregion

        private Label lblKeePassPath;
        private TextBox txtKeePassPath;
        private Label lblKeePassPassword;
        private TextBox txtKeePassPassword;
        private Panel panel1;
        private GroupBox grpKeePass;
        private System.Windows.Forms.Button ButtonBrowseCaptureFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblKeePassCaption;
        private System.Windows.Forms.RadioButton rdoUseDatabase;
        private System.Windows.Forms.RadioButton rdoUseKeePass;
        private System.Windows.Forms.RadioButton rdoUseCredentialsXml;
    }
}