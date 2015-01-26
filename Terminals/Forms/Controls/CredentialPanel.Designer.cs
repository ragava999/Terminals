namespace Terminals.Forms.Controls
{
    partial class CredentialPanel
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.label15 = new System.Windows.Forms.Label();
            this.CredentialDropdown = new System.Windows.Forms.ComboBox();
            this.CredentialManagerPicturebox = new System.Windows.Forms.PictureBox();
            this.CredentialsPanel = new System.Windows.Forms.Panel();
            this.cmbUsers = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbDomains = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.chkSavePassword = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.CredentialManagerPicturebox)).BeginInit();
            this.CredentialsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(1, 6);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 13);
            this.label15.TabIndex = 22;
            this.label15.Text = "Credential:";
            // 
            // CredentialDropdown
            // 
            this.CredentialDropdown.DisplayMember = "Name";
            this.CredentialDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CredentialDropdown.FormattingEnabled = true;
            this.CredentialDropdown.Location = new System.Drawing.Point(100, 3);
            this.CredentialDropdown.Name = "CredentialDropdown";
            this.CredentialDropdown.Size = new System.Drawing.Size(334, 21);
            this.CredentialDropdown.TabIndex = 23;
            this.CredentialDropdown.SelectedIndexChanged += new System.EventHandler(this.CredentialDropdown_SelectedIndexChanged);
            // 
            // CredentialManagerPicturebox
            // 
            this.CredentialManagerPicturebox.Image = global::Terminals.Properties.Resources.computer_security;
            this.CredentialManagerPicturebox.Location = new System.Drawing.Point(444, 3);
            this.CredentialManagerPicturebox.Name = "CredentialManagerPicturebox";
            this.CredentialManagerPicturebox.Size = new System.Drawing.Size(16, 16);
            this.CredentialManagerPicturebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CredentialManagerPicturebox.TabIndex = 25;
            this.CredentialManagerPicturebox.TabStop = false;
            this.CredentialManagerPicturebox.Click += new System.EventHandler(this.CredentialManagerPicturebox_Click);
            // 
            // CredentialsPanel
            // 
            this.CredentialsPanel.Controls.Add(this.cmbUsers);
            this.CredentialsPanel.Controls.Add(this.label1);
            this.CredentialsPanel.Controls.Add(this.cmbDomains);
            this.CredentialsPanel.Controls.Add(this.label3);
            this.CredentialsPanel.Controls.Add(this.label4);
            this.CredentialsPanel.Controls.Add(this.txtPassword);
            this.CredentialsPanel.Controls.Add(this.chkSavePassword);
            this.CredentialsPanel.Location = new System.Drawing.Point(0, 30);
            this.CredentialsPanel.Name = "CredentialsPanel";
            this.CredentialsPanel.Size = new System.Drawing.Size(460, 106);
            this.CredentialsPanel.TabIndex = 24;
            // 
            // cmbUsers
            // 
            this.cmbUsers.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbUsers.Location = new System.Drawing.Point(100, 30);
            this.cmbUsers.Name = "cmbUsers";
            this.cmbUsers.Size = new System.Drawing.Size(356, 21);
            this.cmbUsers.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Domain:";
            // 
            // cmbDomains
            // 
            this.cmbDomains.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbDomains.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbDomains.Location = new System.Drawing.Point(100, 3);
            this.cmbDomains.Name = "cmbDomains";
            this.cmbDomains.Size = new System.Drawing.Size(356, 21);
            this.cmbDomains.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "&User name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "&Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(100, 60);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(356, 20);
            this.txtPassword.TabIndex = 5;
            // 
            // chkSavePassword
            // 
            this.chkSavePassword.AutoSize = true;
            this.chkSavePassword.Location = new System.Drawing.Point(100, 87);
            this.chkSavePassword.Name = "chkSavePassword";
            this.chkSavePassword.Size = new System.Drawing.Size(99, 17);
            this.chkSavePassword.TabIndex = 6;
            this.chkSavePassword.Text = "S&ave password";
            this.chkSavePassword.UseVisualStyleBackColor = true;
            // 
            // CredentialPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.label15);
            this.Controls.Add(this.CredentialDropdown);
            this.Controls.Add(this.CredentialManagerPicturebox);
            this.Controls.Add(this.CredentialsPanel);
            this.Name = "CredentialPanel";
            this.Size = new System.Drawing.Size(463, 139);
            ((System.ComponentModel.ISupportInitialize)(this.CredentialManagerPicturebox)).EndInit();
            this.CredentialsPanel.ResumeLayout(false);
            this.CredentialsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox CredentialDropdown;
        private System.Windows.Forms.PictureBox CredentialManagerPicturebox;
        private System.Windows.Forms.Panel CredentialsPanel;
        private System.Windows.Forms.ComboBox cmbUsers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbDomains;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkSavePassword;
    }
}
