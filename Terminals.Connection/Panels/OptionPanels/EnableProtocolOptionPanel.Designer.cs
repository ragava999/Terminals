using System.Windows.Forms;

namespace Terminals.Connection.Panels.OptionPanels
{
    partial class EnableProtocolOptionPanel
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
            this.panAutoIt = new System.Windows.Forms.Panel();
            this.grpConnections = new System.Windows.Forms.GroupBox();
            this.optSpecific = new System.Windows.Forms.RadioButton();
            this.optThis = new System.Windows.Forms.RadioButton();
            this.lblEnableThisConnectionFor = new System.Windows.Forms.Label();
            this.optAll = new System.Windows.Forms.RadioButton();
            this.pnlConnections = new System.Windows.Forms.FlowLayoutPanel();
            this.panAutoIt.SuspendLayout();
            this.grpConnections.SuspendLayout();
            this.SuspendLayout();
            // 
            // panAutoIt
            // 
            this.panAutoIt.Controls.Add(this.grpConnections);
            this.panAutoIt.Controls.Add(this.optSpecific);
            this.panAutoIt.Controls.Add(this.optThis);
            this.panAutoIt.Controls.Add(this.lblEnableThisConnectionFor);
            this.panAutoIt.Controls.Add(this.optAll);
            this.panAutoIt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panAutoIt.Location = new System.Drawing.Point(0, 0);
            this.panAutoIt.Name = "panAutoIt";
            this.panAutoIt.Size = new System.Drawing.Size(514, 332);
            this.panAutoIt.TabIndex = 25;
            // 
            // grpConnections
            // 
            this.grpConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConnections.Controls.Add(this.pnlConnections);
            this.grpConnections.Location = new System.Drawing.Point(210, 25);
            this.grpConnections.Name = "grpConnections";
            this.grpConnections.Padding = new System.Windows.Forms.Padding(10, 5, 0, 0);
            this.grpConnections.Size = new System.Drawing.Size(274, 302);
            this.grpConnections.TabIndex = 4;
            this.grpConnections.TabStop = false;
            this.grpConnections.Text = "Connections";
            // 
            // optSpecific
            // 
            this.optSpecific.AutoSize = true;
            this.optSpecific.Location = new System.Drawing.Point(32, 119);
            this.optSpecific.Name = "optSpecific";
            this.optSpecific.Size = new System.Drawing.Size(135, 17);
            this.optSpecific.TabIndex = 3;
            this.optSpecific.TabStop = true;
            this.optSpecific.Text = "for specifc connections";
            this.optSpecific.UseVisualStyleBackColor = true;
            this.optSpecific.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // optThis
            // 
            this.optThis.AutoSize = true;
            this.optThis.Location = new System.Drawing.Point(32, 78);
            this.optThis.Name = "optThis";
            this.optThis.Size = new System.Drawing.Size(121, 17);
            this.optThis.TabIndex = 2;
            this.optThis.TabStop = true;
            this.optThis.Text = "Only this connection";
            this.optThis.UseVisualStyleBackColor = true;
            this.optThis.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // lblEnableThisConnectionFor
            // 
            this.lblEnableThisConnectionFor.AutoSize = true;
            this.lblEnableThisConnectionFor.Location = new System.Drawing.Point(15, 11);
            this.lblEnableThisConnectionFor.Name = "lblEnableThisConnectionFor";
            this.lblEnableThisConnectionFor.Size = new System.Drawing.Size(142, 13);
            this.lblEnableThisConnectionFor.TabIndex = 1;
            this.lblEnableThisConnectionFor.Text = "Enable this connection for ...";
            // 
            // optAll
            // 
            this.optAll.AutoSize = true;
            this.optAll.Location = new System.Drawing.Point(32, 42);
            this.optAll.Name = "optAll";
            this.optAll.Size = new System.Drawing.Size(82, 17);
            this.optAll.TabIndex = 0;
            this.optAll.TabStop = true;
            this.optAll.Text = "All protocols";
            this.optAll.UseVisualStyleBackColor = true;
            this.optAll.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // pnlConnections
            // 
            this.pnlConnections.AutoScroll = true;
            this.pnlConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlConnections.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlConnections.Location = new System.Drawing.Point(10, 18);
            this.pnlConnections.Name = "pnlConnections";
            this.pnlConnections.Size = new System.Drawing.Size(264, 284);
            this.pnlConnections.TabIndex = 0;
            // 
            // EnableProtocolOptionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panAutoIt);
            this.Name = "EnableProtocolOptionPanel";
            this.Size = new System.Drawing.Size(514, 332);
            this.panAutoIt.ResumeLayout(false);
            this.panAutoIt.PerformLayout();
            this.grpConnections.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panAutoIt;
        private RadioButton optThis;
        private Label lblEnableThisConnectionFor;
        private RadioButton optAll;
        private GroupBox grpConnections;
        private RadioButton optSpecific;
        private FlowLayoutPanel pnlConnections;
    }
}
