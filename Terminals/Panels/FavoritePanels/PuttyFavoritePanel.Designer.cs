namespace Terminals.Panels.FavoritePanels
{
    partial class PuttyFavoritePanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        	this.grpPuttyOptions = new System.Windows.Forms.GroupBox();
        	this.grpProxyOptions = new System.Windows.Forms.GroupBox();
        	this.nudProxyPort = new System.Windows.Forms.NumericUpDown();
        	this.txtProxyHost = new System.Windows.Forms.TextBox();
        	this.lblProxyType = new System.Windows.Forms.Label();
        	this.lblProxyPort = new System.Windows.Forms.Label();
        	this.cmbProxyType = new System.Windows.Forms.ComboBox();
        	this.lblProxyHost = new System.Windows.Forms.Label();
        	this.grpPuttyOptionsSSH = new System.Windows.Forms.GroupBox();
        	this.chkPuttyCompress = new System.Windows.Forms.CheckBox();
        	this.chkPuttyShowOptions = new System.Windows.Forms.CheckBox();
        	this.btnPuttySessionImport = new System.Windows.Forms.Button();
        	this.chkPuttyVerbose = new System.Windows.Forms.CheckBox();
        	this.cmbPuttyCloseWindowOnExit = new System.Windows.Forms.ComboBox();
        	this.lblPuttyCloseWindowOnExit = new System.Windows.Forms.Label();
        	this.btnPuttySessionDelete = new System.Windows.Forms.Button();
        	this.btnPuttySessionExport = new System.Windows.Forms.Button();
        	this.cmbPuttySession = new System.Windows.Forms.ComboBox();
        	this.lblPuttySession = new System.Windows.Forms.Label();
        	this.cmbPuttyProtocol = new System.Windows.Forms.ComboBox();
        	this.lblPuttyProtocol = new System.Windows.Forms.Label();
        	this.grpPuttyOptionsTelnet = new System.Windows.Forms.GroupBox();
        	this.lblPuttyPasswordTimeout = new System.Windows.Forms.Label();
        	this.nudPuttyPasswordTimeout = new System.Windows.Forms.NumericUpDown();
        	this.chkPuttyEnableX11 = new System.Windows.Forms.CheckBox();
        	this.chkPuttyDontAddDomainToUserName = new System.Windows.Forms.CheckBox();
        	this.grpPuttyOptions.SuspendLayout();
        	this.grpProxyOptions.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.nudProxyPort)).BeginInit();
        	this.grpPuttyOptionsSSH.SuspendLayout();
        	this.grpPuttyOptionsTelnet.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.nudPuttyPasswordTimeout)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// grpPuttyOptions
        	// 
        	this.grpPuttyOptions.BackColor = System.Drawing.Color.White;
        	this.grpPuttyOptions.Controls.Add(this.chkPuttyDontAddDomainToUserName);
        	this.grpPuttyOptions.Controls.Add(this.chkPuttyEnableX11);
        	this.grpPuttyOptions.Controls.Add(this.grpProxyOptions);
        	this.grpPuttyOptions.Controls.Add(this.grpPuttyOptionsSSH);
        	this.grpPuttyOptions.Controls.Add(this.chkPuttyShowOptions);
        	this.grpPuttyOptions.Controls.Add(this.btnPuttySessionImport);
        	this.grpPuttyOptions.Controls.Add(this.chkPuttyVerbose);
        	this.grpPuttyOptions.Controls.Add(this.cmbPuttyCloseWindowOnExit);
        	this.grpPuttyOptions.Controls.Add(this.lblPuttyCloseWindowOnExit);
        	this.grpPuttyOptions.Controls.Add(this.btnPuttySessionDelete);
        	this.grpPuttyOptions.Controls.Add(this.btnPuttySessionExport);
        	this.grpPuttyOptions.Controls.Add(this.cmbPuttySession);
        	this.grpPuttyOptions.Controls.Add(this.lblPuttySession);
        	this.grpPuttyOptions.Controls.Add(this.cmbPuttyProtocol);
        	this.grpPuttyOptions.Controls.Add(this.lblPuttyProtocol);
        	this.grpPuttyOptions.Controls.Add(this.grpPuttyOptionsTelnet);
        	this.grpPuttyOptions.ForeColor = System.Drawing.Color.Black;
        	this.grpPuttyOptions.Location = new System.Drawing.Point(11, 14);
        	this.grpPuttyOptions.Name = "grpPuttyOptions";
        	this.grpPuttyOptions.Size = new System.Drawing.Size(490, 296);
        	this.grpPuttyOptions.TabIndex = 2;
        	this.grpPuttyOptions.TabStop = false;
        	this.grpPuttyOptions.Text = "Putty Options";
        	// 
        	// grpProxyOptions
        	// 
        	this.grpProxyOptions.Controls.Add(this.nudProxyPort);
        	this.grpProxyOptions.Controls.Add(this.txtProxyHost);
        	this.grpProxyOptions.Controls.Add(this.lblProxyType);
        	this.grpProxyOptions.Controls.Add(this.lblProxyPort);
        	this.grpProxyOptions.Controls.Add(this.cmbProxyType);
        	this.grpProxyOptions.Controls.Add(this.lblProxyHost);
        	this.grpProxyOptions.Location = new System.Drawing.Point(182, 170);
        	this.grpProxyOptions.Name = "grpProxyOptions";
        	this.grpProxyOptions.Size = new System.Drawing.Size(300, 120);
        	this.grpProxyOptions.TabIndex = 18;
        	this.grpProxyOptions.TabStop = false;
        	this.grpProxyOptions.Text = "Proxy Options";
        	// 
        	// nudProxyPort
        	// 
        	this.nudProxyPort.Location = new System.Drawing.Point(68, 49);
        	this.nudProxyPort.Maximum = new decimal(new int[] {
			65000,
			0,
			0,
			0});
        	this.nudProxyPort.Name = "nudProxyPort";
        	this.nudProxyPort.Size = new System.Drawing.Size(208, 20);
        	this.nudProxyPort.TabIndex = 24;
        	// 
        	// txtProxyHost
        	// 
        	this.txtProxyHost.Location = new System.Drawing.Point(68, 20);
        	this.txtProxyHost.Name = "txtProxyHost";
        	this.txtProxyHost.Size = new System.Drawing.Size(208, 20);
        	this.txtProxyHost.TabIndex = 23;
        	// 
        	// lblProxyType
        	// 
        	this.lblProxyType.AutoSize = true;
        	this.lblProxyType.Location = new System.Drawing.Point(17, 82);
        	this.lblProxyType.Name = "lblProxyType";
        	this.lblProxyType.Size = new System.Drawing.Size(34, 13);
        	this.lblProxyType.TabIndex = 22;
        	this.lblProxyType.Text = "Type:";
        	// 
        	// lblProxyPort
        	// 
        	this.lblProxyPort.AutoSize = true;
        	this.lblProxyPort.Location = new System.Drawing.Point(17, 51);
        	this.lblProxyPort.Name = "lblProxyPort";
        	this.lblProxyPort.Size = new System.Drawing.Size(29, 13);
        	this.lblProxyPort.TabIndex = 21;
        	this.lblProxyPort.Text = "Port:";
        	// 
        	// cmbProxyType
        	// 
        	this.cmbProxyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.cmbProxyType.FormattingEnabled = true;
        	this.cmbProxyType.Location = new System.Drawing.Point(68, 79);
        	this.cmbProxyType.Name = "cmbProxyType";
        	this.cmbProxyType.Size = new System.Drawing.Size(208, 21);
        	this.cmbProxyType.TabIndex = 20;
        	// 
        	// lblProxyHost
        	// 
        	this.lblProxyHost.AutoSize = true;
        	this.lblProxyHost.Location = new System.Drawing.Point(17, 23);
        	this.lblProxyHost.Name = "lblProxyHost";
        	this.lblProxyHost.Size = new System.Drawing.Size(32, 13);
        	this.lblProxyHost.TabIndex = 19;
        	this.lblProxyHost.Text = "Host:";
        	// 
        	// grpPuttyOptionsSSH
        	// 
        	this.grpPuttyOptionsSSH.Controls.Add(this.chkPuttyCompress);
        	this.grpPuttyOptionsSSH.Location = new System.Drawing.Point(6, 170);
        	this.grpPuttyOptionsSSH.Name = "grpPuttyOptionsSSH";
        	this.grpPuttyOptionsSSH.Size = new System.Drawing.Size(170, 45);
        	this.grpPuttyOptionsSSH.TabIndex = 17;
        	this.grpPuttyOptionsSSH.TabStop = false;
        	this.grpPuttyOptionsSSH.Text = "SSH Options";
        	// 
        	// chkPuttyCompress
        	// 
        	this.chkPuttyCompress.AutoSize = true;
        	this.chkPuttyCompress.Location = new System.Drawing.Point(13, 19);
        	this.chkPuttyCompress.Name = "chkPuttyCompress";
        	this.chkPuttyCompress.Size = new System.Drawing.Size(131, 17);
        	this.chkPuttyCompress.TabIndex = 14;
        	this.chkPuttyCompress.Text = "Compress SSH stream";
        	this.chkPuttyCompress.UseVisualStyleBackColor = true;
        	// 
        	// chkPuttyShowOptions
        	// 
        	this.chkPuttyShowOptions.AutoSize = true;
        	this.chkPuttyShowOptions.Location = new System.Drawing.Point(119, 147);
        	this.chkPuttyShowOptions.Name = "chkPuttyShowOptions";
        	this.chkPuttyShowOptions.Size = new System.Drawing.Size(179, 17);
        	this.chkPuttyShowOptions.TabIndex = 15;
        	this.chkPuttyShowOptions.Text = "Show PuTTY before connecting";
        	this.chkPuttyShowOptions.UseVisualStyleBackColor = true;
        	// 
        	// btnPuttySessionImport
        	// 
        	this.btnPuttySessionImport.Location = new System.Drawing.Point(314, 93);
        	this.btnPuttySessionImport.Name = "btnPuttySessionImport";
        	this.btnPuttySessionImport.Size = new System.Drawing.Size(171, 23);
        	this.btnPuttySessionImport.TabIndex = 14;
        	this.btnPuttySessionImport.Text = "Import PuTTY session";
        	this.btnPuttySessionImport.UseVisualStyleBackColor = true;
        	// 
        	// chkPuttyVerbose
        	// 
        	this.chkPuttyVerbose.AutoSize = true;
        	this.chkPuttyVerbose.Checked = true;
        	this.chkPuttyVerbose.CheckState = System.Windows.Forms.CheckState.Checked;
        	this.chkPuttyVerbose.Location = new System.Drawing.Point(119, 124);
        	this.chkPuttyVerbose.Name = "chkPuttyVerbose";
        	this.chkPuttyVerbose.Size = new System.Drawing.Size(158, 17);
        	this.chkPuttyVerbose.TabIndex = 13;
        	this.chkPuttyVerbose.Text = "Enable verbose CUI logging";
        	this.chkPuttyVerbose.UseVisualStyleBackColor = true;
        	// 
        	// cmbPuttyCloseWindowOnExit
        	// 
        	this.cmbPuttyCloseWindowOnExit.FormattingEnabled = true;
        	this.cmbPuttyCloseWindowOnExit.Location = new System.Drawing.Point(147, 95);
        	this.cmbPuttyCloseWindowOnExit.Name = "cmbPuttyCloseWindowOnExit";
        	this.cmbPuttyCloseWindowOnExit.Size = new System.Drawing.Size(162, 21);
        	this.cmbPuttyCloseWindowOnExit.TabIndex = 11;
        	// 
        	// lblPuttyCloseWindowOnExit
        	// 
        	this.lblPuttyCloseWindowOnExit.AutoSize = true;
        	this.lblPuttyCloseWindowOnExit.Location = new System.Drawing.Point(10, 98);
        	this.lblPuttyCloseWindowOnExit.Name = "lblPuttyCloseWindowOnExit";
        	this.lblPuttyCloseWindowOnExit.Size = new System.Drawing.Size(106, 13);
        	this.lblPuttyCloseWindowOnExit.TabIndex = 10;
        	this.lblPuttyCloseWindowOnExit.Text = "Close window on exit";
        	// 
        	// btnPuttySessionDelete
        	// 
        	this.btnPuttySessionDelete.Location = new System.Drawing.Point(314, 28);
        	this.btnPuttySessionDelete.Name = "btnPuttySessionDelete";
        	this.btnPuttySessionDelete.Size = new System.Drawing.Size(171, 23);
        	this.btnPuttySessionDelete.TabIndex = 9;
        	this.btnPuttySessionDelete.Text = "Delete selected PuTTY session";
        	this.btnPuttySessionDelete.UseVisualStyleBackColor = true;
        	this.btnPuttySessionDelete.Click += new System.EventHandler(this.PuttySessionDelete_Click);
        	// 
        	// btnPuttySessionExport
        	// 
        	this.btnPuttySessionExport.Location = new System.Drawing.Point(314, 60);
        	this.btnPuttySessionExport.Name = "btnPuttySessionExport";
        	this.btnPuttySessionExport.Size = new System.Drawing.Size(171, 23);
        	this.btnPuttySessionExport.TabIndex = 8;
        	this.btnPuttySessionExport.Text = "Export as PuTTY session";
        	this.btnPuttySessionExport.UseVisualStyleBackColor = true;
        	// 
        	// cmbPuttySession
        	// 
        	this.cmbPuttySession.FormattingEnabled = true;
        	this.cmbPuttySession.Location = new System.Drawing.Point(147, 62);
        	this.cmbPuttySession.Name = "cmbPuttySession";
        	this.cmbPuttySession.Size = new System.Drawing.Size(162, 21);
        	this.cmbPuttySession.TabIndex = 7;
        	// 
        	// lblPuttySession
        	// 
        	this.lblPuttySession.AutoSize = true;
        	this.lblPuttySession.Location = new System.Drawing.Point(10, 65);
        	this.lblPuttySession.Name = "lblPuttySession";
        	this.lblPuttySession.Size = new System.Drawing.Size(138, 13);
        	this.lblPuttySession.TabIndex = 6;
        	this.lblPuttySession.Text = "Use saved PuTTY Session:";
        	// 
        	// cmbPuttyProtocol
        	// 
        	this.cmbPuttyProtocol.FormattingEnabled = true;
        	this.cmbPuttyProtocol.Location = new System.Drawing.Point(147, 30);
        	this.cmbPuttyProtocol.Name = "cmbPuttyProtocol";
        	this.cmbPuttyProtocol.Size = new System.Drawing.Size(162, 21);
        	this.cmbPuttyProtocol.TabIndex = 5;
        	// 
        	// lblPuttyProtocol
        	// 
        	this.lblPuttyProtocol.AutoSize = true;
        	this.lblPuttyProtocol.Location = new System.Drawing.Point(10, 33);
        	this.lblPuttyProtocol.Name = "lblPuttyProtocol";
        	this.lblPuttyProtocol.Size = new System.Drawing.Size(49, 13);
        	this.lblPuttyProtocol.TabIndex = 4;
        	this.lblPuttyProtocol.Text = "Protocol:";
        	// 
        	// grpPuttyOptionsTelnet
        	// 
        	this.grpPuttyOptionsTelnet.Controls.Add(this.lblPuttyPasswordTimeout);
        	this.grpPuttyOptionsTelnet.Controls.Add(this.nudPuttyPasswordTimeout);
        	this.grpPuttyOptionsTelnet.Location = new System.Drawing.Point(6, 221);
        	this.grpPuttyOptionsTelnet.Name = "grpPuttyOptionsTelnet";
        	this.grpPuttyOptionsTelnet.Size = new System.Drawing.Size(170, 69);
        	this.grpPuttyOptionsTelnet.TabIndex = 3;
        	this.grpPuttyOptionsTelnet.TabStop = false;
        	this.grpPuttyOptionsTelnet.Text = "Telnet Options";
        	// 
        	// lblPuttyPasswordTimeout
        	// 
        	this.lblPuttyPasswordTimeout.AutoSize = true;
        	this.lblPuttyPasswordTimeout.Location = new System.Drawing.Point(10, 19);
        	this.lblPuttyPasswordTimeout.Name = "lblPuttyPasswordTimeout";
        	this.lblPuttyPasswordTimeout.Size = new System.Drawing.Size(97, 13);
        	this.lblPuttyPasswordTimeout.TabIndex = 1;
        	this.lblPuttyPasswordTimeout.Text = "Password Timeout:";
        	// 
        	// nudPuttyPasswordTimeout
        	// 
        	this.nudPuttyPasswordTimeout.Increment = new decimal(new int[] {
			250,
			0,
			0,
			0});
        	this.nudPuttyPasswordTimeout.Location = new System.Drawing.Point(13, 41);
        	this.nudPuttyPasswordTimeout.Maximum = new decimal(new int[] {
			14000,
			0,
			0,
			0});
        	this.nudPuttyPasswordTimeout.Minimum = new decimal(new int[] {
			250,
			0,
			0,
			0});
        	this.nudPuttyPasswordTimeout.Name = "nudPuttyPasswordTimeout";
        	this.nudPuttyPasswordTimeout.Size = new System.Drawing.Size(120, 20);
        	this.nudPuttyPasswordTimeout.TabIndex = 0;
        	this.nudPuttyPasswordTimeout.Value = new decimal(new int[] {
			7000,
			0,
			0,
			0});
        	// 
        	// chkPuttyEnableX11
        	// 
        	this.chkPuttyEnableX11.AutoSize = true;
        	this.chkPuttyEnableX11.Checked = true;
        	this.chkPuttyEnableX11.CheckState = System.Windows.Forms.CheckState.Checked;
        	this.chkPuttyEnableX11.Location = new System.Drawing.Point(19, 124);
        	this.chkPuttyEnableX11.Name = "chkPuttyEnableX11";
        	this.chkPuttyEnableX11.Size = new System.Drawing.Size(81, 17);
        	this.chkPuttyEnableX11.TabIndex = 19;
        	this.chkPuttyEnableX11.Text = "Enable X11";
        	this.chkPuttyEnableX11.UseVisualStyleBackColor = true;
        	// 
        	// chkPuttyDontAddDomainToUserName
        	// 
        	this.chkPuttyDontAddDomainToUserName.AutoSize = true;
        	this.chkPuttyDontAddDomainToUserName.Checked = true;
        	this.chkPuttyDontAddDomainToUserName.CheckState = System.Windows.Forms.CheckState.Checked;
        	this.chkPuttyDontAddDomainToUserName.Location = new System.Drawing.Point(309, 124);
        	this.chkPuttyDontAddDomainToUserName.Name = "chkPuttyDontAddDomainToUserName";
        	this.chkPuttyDontAddDomainToUserName.Size = new System.Drawing.Size(173, 17);
        	this.chkPuttyDontAddDomainToUserName.TabIndex = 20;
        	this.chkPuttyDontAddDomainToUserName.Text = "Don\'t add domain to user name";
        	this.chkPuttyDontAddDomainToUserName.UseVisualStyleBackColor = true;
        	// 
        	// PuttyFavoritePanel
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.Controls.Add(this.grpPuttyOptions);
        	this.Name = "PuttyFavoritePanel";
        	this.grpPuttyOptions.ResumeLayout(false);
        	this.grpPuttyOptions.PerformLayout();
        	this.grpProxyOptions.ResumeLayout(false);
        	this.grpProxyOptions.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.nudProxyPort)).EndInit();
        	this.grpPuttyOptionsSSH.ResumeLayout(false);
        	this.grpPuttyOptionsSSH.PerformLayout();
        	this.grpPuttyOptionsTelnet.ResumeLayout(false);
        	this.grpPuttyOptionsTelnet.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.nudPuttyPasswordTimeout)).EndInit();
        	this.ResumeLayout(false);
        }
        private System.Windows.Forms.CheckBox chkPuttyDontAddDomainToUserName;
        private System.Windows.Forms.CheckBox chkPuttyEnableX11;
        private System.Windows.Forms.NumericUpDown nudProxyPort;
        private System.Windows.Forms.TextBox txtProxyHost;
        private System.Windows.Forms.Label lblProxyType;
        private System.Windows.Forms.Label lblProxyPort;
        private System.Windows.Forms.ComboBox cmbProxyType;
        private System.Windows.Forms.Label lblProxyHost;
        private System.Windows.Forms.GroupBox grpProxyOptions;

        #endregion

        private System.Windows.Forms.GroupBox grpPuttyOptions;
        private System.Windows.Forms.GroupBox grpPuttyOptionsSSH;
        private System.Windows.Forms.CheckBox chkPuttyCompress;
        private System.Windows.Forms.CheckBox chkPuttyShowOptions;
        private System.Windows.Forms.Button btnPuttySessionImport;
        private System.Windows.Forms.CheckBox chkPuttyVerbose;
        private System.Windows.Forms.ComboBox cmbPuttyCloseWindowOnExit;
        private System.Windows.Forms.Label lblPuttyCloseWindowOnExit;
        private System.Windows.Forms.Button btnPuttySessionDelete;
        private System.Windows.Forms.Button btnPuttySessionExport;
        private System.Windows.Forms.ComboBox cmbPuttySession;
        private System.Windows.Forms.Label lblPuttySession;
        private System.Windows.Forms.ComboBox cmbPuttyProtocol;
        private System.Windows.Forms.Label lblPuttyProtocol;
        private System.Windows.Forms.GroupBox grpPuttyOptionsTelnet;
        private System.Windows.Forms.Label lblPuttyPasswordTimeout;
        private System.Windows.Forms.NumericUpDown nudPuttyPasswordTimeout;
    }
}
