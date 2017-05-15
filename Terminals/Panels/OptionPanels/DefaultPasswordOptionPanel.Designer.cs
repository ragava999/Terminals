using System.Windows.Forms;

namespace Terminals.Panels.OptionPanels
{
    partial class DefaultPasswordOptionPanel
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
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DefaultPasswordOptionPanel));
        	this.panel1 = new System.Windows.Forms.Panel();
        	this.groupBox2 = new System.Windows.Forms.GroupBox();
        	this.lblText = new System.Windows.Forms.Label();
        	this.passwordTextBox = new System.Windows.Forms.TextBox();
        	this.lblPassword = new System.Windows.Forms.Label();
        	this.usernameTextbox = new System.Windows.Forms.TextBox();
        	this.lblUsername = new System.Windows.Forms.Label();
        	this.domainTextbox = new System.Windows.Forms.TextBox();
        	this.lblDomain = new System.Windows.Forms.Label();
        	this.panel1.SuspendLayout();
        	this.groupBox2.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// panel1
        	// 
        	this.panel1.Controls.Add(this.groupBox2);
        	this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.panel1.Location = new System.Drawing.Point(0, 0);
        	this.panel1.Name = "panel1";
        	this.panel1.Size = new System.Drawing.Size(513, 332);
        	this.panel1.TabIndex = 3;
        	// 
        	// groupBox2
        	// 
        	this.groupBox2.Controls.Add(this.lblText);
        	this.groupBox2.Controls.Add(this.passwordTextBox);
        	this.groupBox2.Controls.Add(this.lblPassword);
        	this.groupBox2.Controls.Add(this.usernameTextbox);
        	this.groupBox2.Controls.Add(this.lblUsername);
        	this.groupBox2.Controls.Add(this.domainTextbox);
        	this.groupBox2.Controls.Add(this.lblDomain);
        	this.groupBox2.Location = new System.Drawing.Point(6, 1);
        	this.groupBox2.Name = "groupBox2";
        	this.groupBox2.Size = new System.Drawing.Size(500, 326);
        	this.groupBox2.TabIndex = 0;
        	this.groupBox2.TabStop = false;
        	// 
        	// lblText
        	// 
        	this.lblText.ForeColor = System.Drawing.Color.Black;
        	this.lblText.Location = new System.Drawing.Point(16, 16);
        	this.lblText.Name = "lblText";
        	this.lblText.Size = new System.Drawing.Size(469, 49);
        	this.lblText.TabIndex = 0;
        	this.lblText.Text = resources.GetString("lblText.Text");
        	// 
        	// passwordTextBox
        	// 
        	this.passwordTextBox.Location = new System.Drawing.Point(105, 128);
        	this.passwordTextBox.Name = "passwordTextBox";
        	this.passwordTextBox.Size = new System.Drawing.Size(149, 20);
        	this.passwordTextBox.TabIndex = 25;
        	// 
        	// lblPassword
        	// 
        	this.lblPassword.AutoSize = true;
        	this.lblPassword.Location = new System.Drawing.Point(16, 131);
        	this.lblPassword.Name = "lblPassword";
        	this.lblPassword.Size = new System.Drawing.Size(56, 13);
        	this.lblPassword.TabIndex = 24;
        	this.lblPassword.Text = "Password:";
        	// 
        	// usernameTextbox
        	// 
        	this.usernameTextbox.Location = new System.Drawing.Point(105, 101);
        	this.usernameTextbox.Name = "usernameTextbox";
        	this.usernameTextbox.Size = new System.Drawing.Size(149, 20);
        	this.usernameTextbox.TabIndex = 23;
        	// 
        	// lblUsername
        	// 
        	this.lblUsername.AutoSize = true;
        	this.lblUsername.Location = new System.Drawing.Point(16, 104);
        	this.lblUsername.Name = "lblUsername";
        	this.lblUsername.Size = new System.Drawing.Size(58, 13);
        	this.lblUsername.TabIndex = 22;
        	this.lblUsername.Text = "Username:";
        	// 
        	// domainTextbox
        	// 
        	this.domainTextbox.Location = new System.Drawing.Point(105, 74);
        	this.domainTextbox.Name = "domainTextbox";
        	this.domainTextbox.Size = new System.Drawing.Size(149, 20);
        	this.domainTextbox.TabIndex = 21;
        	// 
        	// lblDomain
        	// 
        	this.lblDomain.AutoSize = true;
        	this.lblDomain.Location = new System.Drawing.Point(16, 77);
        	this.lblDomain.Name = "lblDomain";
        	this.lblDomain.Size = new System.Drawing.Size(46, 13);
        	this.lblDomain.TabIndex = 20;
        	this.lblDomain.Text = "Domain:";
        	// 
        	// DefaultPasswordOptionPanel
        	// 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.Controls.Add(this.panel1);
        	this.Name = "DefaultPasswordOptionPanel";
        	this.Size = new System.Drawing.Size(513, 332);
        	this.panel1.ResumeLayout(false);
        	this.groupBox2.ResumeLayout(false);
        	this.groupBox2.PerformLayout();
        	this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private GroupBox groupBox2;
        private TextBox passwordTextBox;
        private Label lblPassword;
        private TextBox usernameTextbox;
        private Label lblUsername;
        private TextBox domainTextbox;
        private Label lblDomain;
        private Label lblText;
    }
}
