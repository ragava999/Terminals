namespace Terminals.Forms.Credentials
{
    partial class ManageCredentialForm
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
        	this.components = new System.ComponentModel.Container();
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageCredentialForm));
        	this.CancelButton_cred = new System.Windows.Forms.Button();
        	this.SaveButton_cred = new System.Windows.Forms.Button();
        	this.lblName = new System.Windows.Forms.Label();
        	this.txtName = new System.Windows.Forms.TextBox();
        	this.txtPassword = new System.Windows.Forms.TextBox();
        	this.lblPassword = new System.Windows.Forms.Label();
        	this.txtUserName = new System.Windows.Forms.TextBox();
        	this.lblUserName = new System.Windows.Forms.Label();
        	this.txtDomain = new System.Windows.Forms.TextBox();
        	this.lblDomain = new System.Windows.Forms.Label();
        	this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        	this.SuspendLayout();
        	// 
        	// CancelButton_cred
        	// 
        	this.CancelButton_cred.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        	this.CancelButton_cred.Location = new System.Drawing.Point(199, 116);
        	this.CancelButton_cred.Name = "CancelButton_cred";
        	this.CancelButton_cred.Size = new System.Drawing.Size(75, 23);
        	this.CancelButton_cred.TabIndex = 5;
        	this.CancelButton_cred.Text = "Cancel";
        	this.CancelButton_cred.UseVisualStyleBackColor = true;
        	// 
        	// SaveButton_cred
        	// 
        	this.SaveButton_cred.Location = new System.Drawing.Point(118, 116);
        	this.SaveButton_cred.Name = "SaveButton_cred";
        	this.SaveButton_cred.Size = new System.Drawing.Size(75, 23);
        	this.SaveButton_cred.TabIndex = 4;
        	this.SaveButton_cred.Text = "Save";
        	this.SaveButton_cred.UseVisualStyleBackColor = true;
        	this.SaveButton_cred.Click += new System.EventHandler(this.SaveButton_Click);
        	// 
        	// lblName
        	// 
        	this.lblName.AutoSize = true;
        	this.lblName.Location = new System.Drawing.Point(7, 15);
        	this.lblName.Name = "lblName";
        	this.lblName.Size = new System.Drawing.Size(38, 13);
        	this.lblName.TabIndex = 2;
        	this.lblName.Text = "Name:";
        	// 
        	// txtName
        	// 
        	this.txtName.Location = new System.Drawing.Point(70, 12);
        	this.txtName.Name = "txtName";
        	this.txtName.Size = new System.Drawing.Size(204, 20);
        	this.txtName.TabIndex = 0;
        	this.toolTip1.SetToolTip(this.txtName, "");
        	// 
        	// txtPassword
        	// 
        	this.txtPassword.Location = new System.Drawing.Point(70, 64);
        	this.txtPassword.Name = "txtPassword";
        	this.txtPassword.Size = new System.Drawing.Size(204, 20);
        	this.txtPassword.TabIndex = 2;
        	this.toolTip1.SetToolTip(this.txtPassword, "");
        	// 
        	// lblPassword
        	// 
        	this.lblPassword.AutoSize = true;
        	this.lblPassword.Location = new System.Drawing.Point(7, 67);
        	this.lblPassword.Name = "lblPassword";
        	this.lblPassword.Size = new System.Drawing.Size(56, 13);
        	this.lblPassword.TabIndex = 7;
        	this.lblPassword.Text = "Password:";
        	this.lblPassword.DoubleClick += new System.EventHandler(this.LblPasswordDoubleClick);
        	// 
        	// txtUserName
        	// 
        	this.txtUserName.Location = new System.Drawing.Point(70, 38);
        	this.txtUserName.Name = "txtUserName";
        	this.txtUserName.Size = new System.Drawing.Size(204, 20);
        	this.txtUserName.TabIndex = 1;
        	this.toolTip1.SetToolTip(this.txtUserName, "");
        	// 
        	// lblUserName
        	// 
        	this.lblUserName.AutoSize = true;
        	this.lblUserName.Location = new System.Drawing.Point(7, 41);
        	this.lblUserName.Name = "lblUserName";
        	this.lblUserName.Size = new System.Drawing.Size(63, 13);
        	this.lblUserName.TabIndex = 9;
        	this.lblUserName.Text = "User Name:";
        	// 
        	// txtDomain
        	// 
        	this.txtDomain.Location = new System.Drawing.Point(70, 90);
        	this.txtDomain.Name = "txtDomain";
        	this.txtDomain.Size = new System.Drawing.Size(204, 20);
        	this.txtDomain.TabIndex = 3;
        	this.toolTip1.SetToolTip(this.txtDomain, "");
        	// 
        	// lblDomain
        	// 
        	this.lblDomain.AutoSize = true;
        	this.lblDomain.Location = new System.Drawing.Point(7, 93);
        	this.lblDomain.Name = "lblDomain";
        	this.lblDomain.Size = new System.Drawing.Size(46, 13);
        	this.lblDomain.TabIndex = 11;
        	this.lblDomain.Text = "Domain:";
        	// 
        	// ManageCredentialForm
        	// 
        	this.AcceptButton = this.SaveButton_cred;
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.CancelButton = this.CancelButton_cred;
        	this.ClientSize = new System.Drawing.Size(284, 145);
        	this.Controls.Add(this.txtDomain);
        	this.Controls.Add(this.lblDomain);
        	this.Controls.Add(this.txtUserName);
        	this.Controls.Add(this.lblUserName);
        	this.Controls.Add(this.txtPassword);
        	this.Controls.Add(this.lblPassword);
        	this.Controls.Add(this.txtName);
        	this.Controls.Add(this.lblName);
        	this.Controls.Add(this.SaveButton_cred);
        	this.Controls.Add(this.CancelButton_cred);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        	this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        	this.MaximizeBox = false;
        	this.MinimizeBox = false;
        	this.Name = "ManageCredentialForm";
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button CancelButton_cred;
        private System.Windows.Forms.Button SaveButton_cred;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Label lblDomain;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}