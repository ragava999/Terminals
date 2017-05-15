using System.Windows.Forms;

namespace Terminals.Panels.OptionPanels
{
    partial class MasterPasswordOptionPanel
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
        	this.groupBox3 = new System.Windows.Forms.GroupBox();
        	this.lblMasterPasswordCaption = new System.Windows.Forms.Label();
        	this.lblPasswordsMatch = new System.Windows.Forms.Label();
        	this.chkPasswordProtectTerminals = new System.Windows.Forms.CheckBox();
        	this.lblPassword = new System.Windows.Forms.Label();
        	this.PasswordTextbox = new System.Windows.Forms.TextBox();
        	this.lblConfirm = new System.Windows.Forms.Label();
        	this.ConfirmPasswordTextBox = new System.Windows.Forms.TextBox();
            this.ForgetPasswordButton = new System.Windows.Forms.Button();
        	this.panel1.SuspendLayout();
        	this.groupBox3.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// panel1
        	// 
            this.panel1.Controls.Add(this.ForgetPasswordButton);
        	this.panel1.Controls.Add(this.groupBox3);
        	this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.panel1.Location = new System.Drawing.Point(0, 0);
        	this.panel1.Name = "panel1";
        	this.panel1.Size = new System.Drawing.Size(511, 332);
        	this.panel1.TabIndex = 2;
        	// 
        	// groupBox3
        	// 
        	this.groupBox3.Controls.Add(this.lblMasterPasswordCaption);
        	this.groupBox3.Controls.Add(this.lblPasswordsMatch);
        	this.groupBox3.Controls.Add(this.chkPasswordProtectTerminals);
        	this.groupBox3.Controls.Add(this.lblPassword);
        	this.groupBox3.Controls.Add(this.PasswordTextbox);
        	this.groupBox3.Controls.Add(this.lblConfirm);
        	this.groupBox3.Controls.Add(this.ConfirmPasswordTextBox);
        	this.groupBox3.Location = new System.Drawing.Point(5, 3);
        	this.groupBox3.Name = "groupBox3";
        	this.groupBox3.Size = new System.Drawing.Size(500, 144);
        	this.groupBox3.TabIndex = 0;
        	this.groupBox3.TabStop = false;
        	// 
        	// lblMasterPasswordCaption
        	// 
        	this.lblMasterPasswordCaption.Location = new System.Drawing.Point(13, 16);
        	this.lblMasterPasswordCaption.Name = "lblMasterPasswordCaption";
        	this.lblMasterPasswordCaption.Size = new System.Drawing.Size(481, 32);
        	this.lblMasterPasswordCaption.TabIndex = 27;
        	// 
        	// lblPasswordsMatch
        	// 
        	this.lblPasswordsMatch.AutoSize = true;
        	this.lblPasswordsMatch.ForeColor = System.Drawing.SystemColors.ControlText;
        	this.lblPasswordsMatch.Location = new System.Drawing.Point(305, 112);
        	this.lblPasswordsMatch.Name = "lblPasswordsMatch";
        	this.lblPasswordsMatch.Size = new System.Drawing.Size(0, 13);
        	this.lblPasswordsMatch.TabIndex = 6;
        	// 
        	// chkPasswordProtectTerminals
        	// 
        	this.chkPasswordProtectTerminals.AutoSize = true;
        	this.chkPasswordProtectTerminals.Location = new System.Drawing.Point(13, 59);
        	this.chkPasswordProtectTerminals.Name = "chkPasswordProtectTerminals";
        	this.chkPasswordProtectTerminals.Size = new System.Drawing.Size(109, 17);
        	this.chkPasswordProtectTerminals.TabIndex = 0;
        	this.chkPasswordProtectTerminals.Text = "Password Protect";
        	this.chkPasswordProtectTerminals.UseVisualStyleBackColor = true;
        	this.chkPasswordProtectTerminals.CheckedChanged += new System.EventHandler(this.chkPasswordProtectTerminals_CheckedChanged);
        	// 
        	// lblPassword
        	// 
        	this.lblPassword.AutoSize = true;
        	this.lblPassword.Location = new System.Drawing.Point(13, 84);
        	this.lblPassword.Name = "lblPassword";
        	this.lblPassword.Size = new System.Drawing.Size(56, 13);
        	this.lblPassword.TabIndex = 3;
        	this.lblPassword.Text = "Password:";
        	// 
        	// PasswordTextbox
        	// 
        	this.PasswordTextbox.Enabled = false;
        	this.PasswordTextbox.Location = new System.Drawing.Point(113, 81);
        	this.PasswordTextbox.Name = "PasswordTextbox";
        	this.PasswordTextbox.Size = new System.Drawing.Size(176, 20);
        	this.PasswordTextbox.TabIndex = 1;
        	this.PasswordTextbox.TextChanged += new System.EventHandler(this.PasswordTextbox_TextChanged);
        	// 
        	// lblConfirm
        	// 
        	this.lblConfirm.AutoSize = true;
        	this.lblConfirm.Location = new System.Drawing.Point(13, 112);
        	this.lblConfirm.Name = "lblConfirm";
        	this.lblConfirm.Size = new System.Drawing.Size(45, 13);
        	this.lblConfirm.TabIndex = 4;
        	this.lblConfirm.Text = "Confirm:";
        	// 
        	// ConfirmPasswordTextBox
        	// 
        	this.ConfirmPasswordTextBox.Enabled = false;
        	this.ConfirmPasswordTextBox.Location = new System.Drawing.Point(113, 109);
        	this.ConfirmPasswordTextBox.Name = "ConfirmPasswordTextBox";
        	this.ConfirmPasswordTextBox.Size = new System.Drawing.Size(176, 20);
        	this.ConfirmPasswordTextBox.TabIndex = 2;
        	this.ConfirmPasswordTextBox.TextChanged += new System.EventHandler(this.ConfirmPasswordTextBox_TextChanged);
            // 
            // ForgetPasswordButton
            // 
            this.ForgetPasswordButton.Location = new System.Drawing.Point(118, 154);
            this.ForgetPasswordButton.Name = "ForgetPasswordButton";
            this.ForgetPasswordButton.Size = new System.Drawing.Size(176, 23);
            this.ForgetPasswordButton.TabIndex = 1;
            this.ForgetPasswordButton.Text = "Forget Saved Master Password";
            this.ForgetPasswordButton.UseVisualStyleBackColor = true;
            this.ForgetPasswordButton.Click += new System.EventHandler(this.ForgetPasswordButton_Click);
        	// 
        	// MasterPasswordOptionPanel
        	// 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.Controls.Add(this.panel1);
        	this.Name = "MasterPasswordOptionPanel";
        	this.Size = new System.Drawing.Size(511, 332);
        	this.panel1.ResumeLayout(false);
        	this.groupBox3.ResumeLayout(false);
        	this.groupBox3.PerformLayout();
        	this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private GroupBox groupBox3;
        private Label lblPasswordsMatch;
        private CheckBox chkPasswordProtectTerminals;
        private Label lblPassword;
        private TextBox PasswordTextbox;
        private Label lblConfirm;
        private TextBox ConfirmPasswordTextBox;
        private System.Windows.Forms.Label lblMasterPasswordCaption;
        private Button ForgetPasswordButton;
    }
}