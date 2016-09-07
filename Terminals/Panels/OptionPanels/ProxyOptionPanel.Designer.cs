using System.Windows.Forms;

namespace Terminals.Panels.OptionPanels
{
    partial class ProxyOptionPanel
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
        	this.panCredentials = new System.Windows.Forms.Panel();
        	this.rdoCustomCredentials = new System.Windows.Forms.RadioButton();
        	this.rdoNoAuth = new System.Windows.Forms.RadioButton();
        	this.rdoDefaultCredentials = new System.Windows.Forms.RadioButton();
        	this.rdoDontUseProxy = new System.Windows.Forms.RadioButton();
        	this.lblPort = new System.Windows.Forms.Label();
        	this.txtProxyPort = new System.Windows.Forms.TextBox();
        	this.txtProxyAddress = new System.Windows.Forms.TextBox();
        	this.lblAddress = new System.Windows.Forms.Label();
        	this.rdoAutoProxy = new System.Windows.Forms.RadioButton();
        	this.rdoProxy = new System.Windows.Forms.RadioButton();
        	this.ProxyCredentials = new Terminals.Forms.Controls.CredentialPanel();
        	this.panel1.SuspendLayout();
        	this.panCredentials.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// panel1
        	// 
        	this.panel1.Controls.Add(this.panCredentials);
        	this.panel1.Controls.Add(this.rdoDontUseProxy);
        	this.panel1.Controls.Add(this.lblPort);
        	this.panel1.Controls.Add(this.txtProxyPort);
        	this.panel1.Controls.Add(this.txtProxyAddress);
        	this.panel1.Controls.Add(this.lblAddress);
        	this.panel1.Controls.Add(this.rdoAutoProxy);
        	this.panel1.Controls.Add(this.rdoProxy);
        	this.panel1.Controls.Add(this.ProxyCredentials);
        	this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.panel1.Location = new System.Drawing.Point(0, 0);
        	this.panel1.Name = "panel1";
        	this.panel1.Size = new System.Drawing.Size(514, 332);
        	this.panel1.TabIndex = 27;
        	// 
        	// panCredentials
        	// 
        	this.panCredentials.Controls.Add(this.rdoCustomCredentials);
        	this.panCredentials.Controls.Add(this.rdoNoAuth);
        	this.panCredentials.Controls.Add(this.rdoDefaultCredentials);
        	this.panCredentials.Location = new System.Drawing.Point(278, 25);
        	this.panCredentials.Name = "panCredentials";
        	this.panCredentials.Size = new System.Drawing.Size(204, 124);
        	this.panCredentials.TabIndex = 19;
        	// 
        	// rdoCustomCredentials
        	// 
        	this.rdoCustomCredentials.Location = new System.Drawing.Point(14, 80);
        	this.rdoCustomCredentials.Name = "rdoCustomCredentials";
        	this.rdoCustomCredentials.Size = new System.Drawing.Size(187, 24);
        	this.rdoCustomCredentials.TabIndex = 2;
        	this.rdoCustomCredentials.TabStop = true;
        	this.rdoCustomCredentials.Text = "Use custom credentials";
        	this.rdoCustomCredentials.UseVisualStyleBackColor = true;
        	this.rdoCustomCredentials.CheckedChanged += new System.EventHandler(this.CredentialsCheckedChanged);
        	// 
        	// rdoNoAuth
        	// 
        	this.rdoNoAuth.Location = new System.Drawing.Point(14, 50);
        	this.rdoNoAuth.Name = "rdoNoAuth";
        	this.rdoNoAuth.Size = new System.Drawing.Size(187, 24);
        	this.rdoNoAuth.TabIndex = 1;
        	this.rdoNoAuth.TabStop = true;
        	this.rdoNoAuth.Text = "Authentication not needed";
        	this.rdoNoAuth.UseVisualStyleBackColor = true;
        	this.rdoNoAuth.CheckedChanged += new System.EventHandler(this.CredentialsCheckedChanged);
        	// 
        	// rdoDefaultCredentials
        	// 
        	this.rdoDefaultCredentials.Location = new System.Drawing.Point(14, 20);
        	this.rdoDefaultCredentials.Name = "rdoDefaultCredentials";
        	this.rdoDefaultCredentials.Size = new System.Drawing.Size(187, 24);
        	this.rdoDefaultCredentials.TabIndex = 0;
        	this.rdoDefaultCredentials.TabStop = true;
        	this.rdoDefaultCredentials.Text = "Use default credentials";
        	this.rdoDefaultCredentials.UseVisualStyleBackColor = true;
        	this.rdoDefaultCredentials.CheckedChanged += new System.EventHandler(this.CredentialsCheckedChanged);
        	// 
        	// rdoDontUseProxy
        	// 
        	this.rdoDontUseProxy.AutoSize = true;
        	this.rdoDontUseProxy.Location = new System.Drawing.Point(26, 48);
        	this.rdoDontUseProxy.Name = "rdoDontUseProxy";
        	this.rdoDontUseProxy.Size = new System.Drawing.Size(98, 17);
        	this.rdoDontUseProxy.TabIndex = 14;
        	this.rdoDontUseProxy.TabStop = true;
        	this.rdoDontUseProxy.Text = "Don\'t use proxy";
        	this.rdoDontUseProxy.UseVisualStyleBackColor = true;
        	this.rdoDontUseProxy.CheckedChanged += new System.EventHandler(this.ProxyCheckedChanged);
        	// 
        	// lblPort
        	// 
        	this.lblPort.AutoSize = true;
        	this.lblPort.Location = new System.Drawing.Point(26, 132);
        	this.lblPort.Name = "lblPort";
        	this.lblPort.Size = new System.Drawing.Size(29, 13);
        	this.lblPort.TabIndex = 18;
        	this.lblPort.Text = "Port:";
        	// 
        	// txtProxyPort
        	// 
        	this.txtProxyPort.Location = new System.Drawing.Point(80, 129);
        	this.txtProxyPort.Name = "txtProxyPort";
        	this.txtProxyPort.Size = new System.Drawing.Size(44, 20);
        	this.txtProxyPort.TabIndex = 17;
        	// 
        	// txtProxyAddress
        	// 
        	this.txtProxyAddress.Location = new System.Drawing.Point(80, 100);
        	this.txtProxyAddress.Name = "txtProxyAddress";
        	this.txtProxyAddress.Size = new System.Drawing.Size(192, 20);
        	this.txtProxyAddress.TabIndex = 16;
        	// 
        	// lblAddress
        	// 
        	this.lblAddress.AutoSize = true;
        	this.lblAddress.Location = new System.Drawing.Point(26, 103);
        	this.lblAddress.Name = "lblAddress";
        	this.lblAddress.Size = new System.Drawing.Size(48, 13);
        	this.lblAddress.TabIndex = 15;
        	this.lblAddress.Text = "Address:";
        	// 
        	// rdoAutoProxy
        	// 
        	this.rdoAutoProxy.AutoSize = true;
        	this.rdoAutoProxy.Location = new System.Drawing.Point(26, 25);
        	this.rdoAutoProxy.Name = "rdoAutoProxy";
        	this.rdoAutoProxy.Size = new System.Drawing.Size(148, 17);
        	this.rdoAutoProxy.TabIndex = 12;
        	this.rdoAutoProxy.TabStop = true;
        	this.rdoAutoProxy.Text = "Automatically detect proxy";
        	this.rdoAutoProxy.UseVisualStyleBackColor = true;
        	this.rdoAutoProxy.CheckedChanged += new System.EventHandler(this.ProxyCheckedChanged);
        	// 
        	// rdoProxy
        	// 
        	this.rdoProxy.AutoSize = true;
        	this.rdoProxy.Location = new System.Drawing.Point(26, 71);
        	this.rdoProxy.Name = "rdoProxy";
        	this.rdoProxy.Size = new System.Drawing.Size(169, 17);
        	this.rdoProxy.TabIndex = 13;
        	this.rdoProxy.TabStop = true;
        	this.rdoProxy.Text = "Use the following proxy server:";
        	this.rdoProxy.UseVisualStyleBackColor = true;
        	this.rdoProxy.CheckedChanged += new System.EventHandler(this.ProxyCheckedChanged);
        	// 
        	// ProxyCredentials
        	// 
        	this.ProxyCredentials.AutoSize = true;
        	this.ProxyCredentials.BackColor = System.Drawing.Color.White;
        	this.ProxyCredentials.DontLoadMe = false;
        	this.ProxyCredentials.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.ProxyCredentials.Location = new System.Drawing.Point(26, 172);
        	this.ProxyCredentials.Name = "ProxyCredentials";
        	this.ProxyCredentials.Size = new System.Drawing.Size(463, 139);
        	this.ProxyCredentials.TabIndex = 20;
        	// 
        	// ProxyOptionPanel
        	// 
        	this.Controls.Add(this.panel1);
        	this.Name = "ProxyOptionPanel";
        	this.Size = new System.Drawing.Size(514, 332);
        	this.panel1.ResumeLayout(false);
        	this.panel1.PerformLayout();
        	this.panCredentials.ResumeLayout(false);
        	this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label lblPort;
        private TextBox txtProxyPort;
        private TextBox txtProxyAddress;
        private Label lblAddress;
        private RadioButton rdoAutoProxy;
        private RadioButton rdoProxy;
        private System.Windows.Forms.RadioButton rdoDontUseProxy;
        private System.Windows.Forms.Panel panCredentials;
        private System.Windows.Forms.RadioButton rdoCustomCredentials;
        private System.Windows.Forms.RadioButton rdoNoAuth;
        private System.Windows.Forms.RadioButton rdoDefaultCredentials;
        private Terminals.Forms.Controls.CredentialPanel ProxyCredentials;
    }
}