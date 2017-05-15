using System.Windows.Forms;

namespace Terminals.Panels.OptionPanels
{
    partial class ConnectionsOptionPanel
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
            this.grpPortScanner = new System.Windows.Forms.GroupBox();
            this.lblPortScannerTimeout = new System.Windows.Forms.Label();
            this.PortscanTimeoutTextBox = new System.Windows.Forms.TextBox();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.grpDesktopShare = new System.Windows.Forms.GroupBox();
            this.lblDefaultDesktopShare = new System.Windows.Forms.Label();
            this.txtDefaultDesktopShare = new System.Windows.Forms.TextBox();
            this.groupBoxConnections = new System.Windows.Forms.GroupBox();
            this.chkLetTerminalsAutomaticallyManageRevertFromFullScreenMode = new System.Windows.Forms.CheckBox();
            this.chkAskReconnectRdp = new System.Windows.Forms.CheckBox();
            this.validateServerNamesCheckbox = new System.Windows.Forms.CheckBox();
            this.warnDisconnectCheckBox = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.grpPortScanner.SuspendLayout();
            this.grpDesktopShare.SuspendLayout();
            this.groupBoxConnections.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.grpPortScanner);
            this.panel1.Controls.Add(this.grpDesktopShare);
            this.panel1.Controls.Add(this.groupBoxConnections);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(514, 332);
            this.panel1.TabIndex = 25;
            // 
            // grpPortScanner
            // 
            this.grpPortScanner.Controls.Add(this.lblPortScannerTimeout);
            this.grpPortScanner.Controls.Add(this.PortscanTimeoutTextBox);
            this.grpPortScanner.Controls.Add(this.lblSeconds);
            this.grpPortScanner.Location = new System.Drawing.Point(8, 219);
            this.grpPortScanner.Name = "grpPortScanner";
            this.grpPortScanner.Size = new System.Drawing.Size(500, 56);
            this.grpPortScanner.TabIndex = 23;
            this.grpPortScanner.TabStop = false;
            this.grpPortScanner.Text = "Port scanner";
            // 
            // lblPortScannerTimeout
            // 
            this.lblPortScannerTimeout.AutoSize = true;
            this.lblPortScannerTimeout.Location = new System.Drawing.Point(5, 27);
            this.lblPortScannerTimeout.Name = "lblPortScannerTimeout";
            this.lblPortScannerTimeout.Size = new System.Drawing.Size(113, 13);
            this.lblPortScannerTimeout.TabIndex = 20;
            this.lblPortScannerTimeout.Text = "Port Scanner Timeout:";
            // 
            // PortscanTimeoutTextBox
            // 
            this.PortscanTimeoutTextBox.Location = new System.Drawing.Point(189, 24);
            this.PortscanTimeoutTextBox.Name = "PortscanTimeoutTextBox";
            this.PortscanTimeoutTextBox.Size = new System.Drawing.Size(121, 20);
            this.PortscanTimeoutTextBox.TabIndex = 22;
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Location = new System.Drawing.Point(316, 27);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(49, 13);
            this.lblSeconds.TabIndex = 21;
            this.lblSeconds.Text = "Seconds";
            // 
            // grpDesktopShare
            // 
            this.grpDesktopShare.Controls.Add(this.lblDefaultDesktopShare);
            this.grpDesktopShare.Controls.Add(this.txtDefaultDesktopShare);
            this.grpDesktopShare.Location = new System.Drawing.Point(7, 140);
            this.grpDesktopShare.Name = "grpDesktopShare";
            this.grpDesktopShare.Size = new System.Drawing.Size(500, 73);
            this.grpDesktopShare.TabIndex = 20;
            this.grpDesktopShare.TabStop = false;
            this.grpDesktopShare.Text = "RDP and ICA Desktop Share";
            // 
            // lblDefaultDesktopShare
            // 
            this.lblDefaultDesktopShare.AutoSize = true;
            this.lblDefaultDesktopShare.Location = new System.Drawing.Point(6, 24);
            this.lblDefaultDesktopShare.Name = "lblDefaultDesktopShare";
            this.lblDefaultDesktopShare.Size = new System.Drawing.Size(455, 13);
            this.lblDefaultDesktopShare.TabIndex = 17;
            this.lblDefaultDesktopShare.Text = "Default Desktop Share (Use %SERVER% and %USER%) used for drag && drop copy operat" +
    "ions:";
            // 
            // txtDefaultDesktopShare
            // 
            this.txtDefaultDesktopShare.Location = new System.Drawing.Point(6, 40);
            this.txtDefaultDesktopShare.Name = "txtDefaultDesktopShare";
            this.txtDefaultDesktopShare.Size = new System.Drawing.Size(360, 20);
            this.txtDefaultDesktopShare.TabIndex = 18;
            // 
            // groupBoxConnections
            // 
            this.groupBoxConnections.Controls.Add(this.chkAskReconnectRdp);
            this.groupBoxConnections.Controls.Add(this.validateServerNamesCheckbox);
            this.groupBoxConnections.Controls.Add(this.warnDisconnectCheckBox);
            this.groupBoxConnections.Controls.Add(this.chkLetTerminalsAutomaticallyManageRevertFromFullScreenMode);
            this.groupBoxConnections.Location = new System.Drawing.Point(7, 3);
            this.groupBoxConnections.Name = "groupBoxConnections";
            this.groupBoxConnections.Size = new System.Drawing.Size(500, 131);
            this.groupBoxConnections.TabIndex = 4;
            this.groupBoxConnections.TabStop = false;
            // 
            // chkLetTerminalsAutomaticallyManageRevertFromFullScreenMode
            // 
            this.chkLetTerminalsAutomaticallyManageRevertFromFullScreenMode.Checked = true;
            this.chkLetTerminalsAutomaticallyManageRevertFromFullScreenMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLetTerminalsAutomaticallyManageRevertFromFullScreenMode.Location = new System.Drawing.Point(6, 89);
            this.chkLetTerminalsAutomaticallyManageRevertFromFullScreenMode.Name = "chkLetTerminalsAutomaticallyManageRevertFromFullScreenMode";
            this.chkLetTerminalsAutomaticallyManageRevertFromFullScreenMode.Size = new System.Drawing.Size(410, 36);
            this.chkLetTerminalsAutomaticallyManageRevertFromFullScreenMode.TabIndex = 21;
            this.chkLetTerminalsAutomaticallyManageRevertFromFullScreenMode.Text = "Automatically change from full screen mode (Alt + F11) back to the normal display" +
    " state after closing the last connection";
            this.chkLetTerminalsAutomaticallyManageRevertFromFullScreenMode.UseVisualStyleBackColor = true;
            // 
            // chkAskReconnectRdp
            // 
            this.chkAskReconnectRdp.AutoSize = true;
            this.chkAskReconnectRdp.Checked = true;
            this.chkAskReconnectRdp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAskReconnectRdp.Location = new System.Drawing.Point(6, 66);
            this.chkAskReconnectRdp.Name = "chkAskReconnectRdp";
            this.chkAskReconnectRdp.Size = new System.Drawing.Size(390, 17);
            this.chkAskReconnectRdp.TabIndex = 20;
            this.chkAskReconnectRdp.Text = "Ask to reconnect when connection is lost due shutdown or reboot (RDP only)";
            this.chkAskReconnectRdp.UseVisualStyleBackColor = true;
            // 
            // validateServerNamesCheckbox
            // 
            this.validateServerNamesCheckbox.AutoSize = true;
            this.validateServerNamesCheckbox.Checked = true;
            this.validateServerNamesCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.validateServerNamesCheckbox.Location = new System.Drawing.Point(6, 20);
            this.validateServerNamesCheckbox.Name = "validateServerNamesCheckbox";
            this.validateServerNamesCheckbox.Size = new System.Drawing.Size(134, 17);
            this.validateServerNamesCheckbox.TabIndex = 18;
            this.validateServerNamesCheckbox.Text = "Validate Server Names";
            this.validateServerNamesCheckbox.UseVisualStyleBackColor = true;
            // 
            // warnDisconnectCheckBox
            // 
            this.warnDisconnectCheckBox.AutoSize = true;
            this.warnDisconnectCheckBox.Checked = true;
            this.warnDisconnectCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.warnDisconnectCheckBox.Location = new System.Drawing.Point(6, 43);
            this.warnDisconnectCheckBox.Name = "warnDisconnectCheckBox";
            this.warnDisconnectCheckBox.Size = new System.Drawing.Size(272, 17);
            this.warnDisconnectCheckBox.TabIndex = 19;
            this.warnDisconnectCheckBox.Text = "Show confirm dialog on close or warn on disconnect";
            this.warnDisconnectCheckBox.UseVisualStyleBackColor = true;
            // 
            // ConnectionsOptionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "ConnectionsOptionPanel";
            this.Size = new System.Drawing.Size(514, 332);
            this.panel1.ResumeLayout(false);
            this.grpPortScanner.ResumeLayout(false);
            this.grpPortScanner.PerformLayout();
            this.grpDesktopShare.ResumeLayout(false);
            this.grpDesktopShare.PerformLayout();
            this.groupBoxConnections.ResumeLayout(false);
            this.groupBoxConnections.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.GroupBox grpPortScanner;

        #endregion

        private Panel panel1;
        private GroupBox grpDesktopShare;
        private TextBox PortscanTimeoutTextBox;
        private Label lblDefaultDesktopShare;
        private TextBox txtDefaultDesktopShare;
        private Label lblSeconds;
        private Label lblPortScannerTimeout;
        private GroupBox groupBoxConnections;
        private CheckBox validateServerNamesCheckbox;
        private CheckBox warnDisconnectCheckBox;
        private CheckBox chkAskReconnectRdp;
        private CheckBox chkLetTerminalsAutomaticallyManageRevertFromFullScreenMode;
    }
}
