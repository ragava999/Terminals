namespace Terminals.Network.Servers
{
    partial class TerminalServerManager
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
        	this.components = new System.ComponentModel.Container();
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TerminalServerManager));
        	this.ConnectButton = new System.Windows.Forms.Button();
        	this.label1 = new System.Windows.Forms.Label();
        	this.splitContainer1 = new System.Windows.Forms.SplitContainer();
        	this.ServerNameComboBox = new System.Windows.Forms.ComboBox();
        	this.splitContainer2 = new System.Windows.Forms.SplitContainer();
        	this.splitContainer3 = new System.Windows.Forms.SplitContainer();
        	this.dataGridView1 = new System.Windows.Forms.DataGridView();
        	this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.sendMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.logoffSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        	this.rebootServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.shutdownServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
        	this.dataGridView2 = new System.Windows.Forms.DataGridView();
        	this.progress = new System.Windows.Forms.PictureBox();
        	((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
        	this.splitContainer1.Panel1.SuspendLayout();
        	this.splitContainer1.Panel2.SuspendLayout();
        	this.splitContainer1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
        	this.splitContainer2.Panel1.SuspendLayout();
        	this.splitContainer2.Panel2.SuspendLayout();
        	this.splitContainer2.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
        	this.splitContainer3.Panel1.SuspendLayout();
        	this.splitContainer3.Panel2.SuspendLayout();
        	this.splitContainer3.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
        	this.contextMenuStrip1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.progress)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// ConnectButton
        	// 
        	this.ConnectButton.Location = new System.Drawing.Point(458, 8);
        	this.ConnectButton.Name = "ConnectButton";
        	this.ConnectButton.Size = new System.Drawing.Size(94, 23);
        	this.ConnectButton.TabIndex = 1;
        	this.ConnectButton.Text = "Connect...";
        	this.ConnectButton.UseVisualStyleBackColor = true;
        	this.ConnectButton.Click += new System.EventHandler(this.button1_Click);
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(3, 13);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(84, 13);
        	this.label1.TabIndex = 2;
        	this.label1.Text = "Terminal Server:";
        	// 
        	// splitContainer1
        	// 
        	this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.splitContainer1.Location = new System.Drawing.Point(0, 0);
        	this.splitContainer1.Name = "splitContainer1";
        	this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
        	// 
        	// splitContainer1.Panel1
        	// 
        	this.splitContainer1.Panel1.Controls.Add(this.ServerNameComboBox);
        	this.splitContainer1.Panel1.Controls.Add(this.label1);
        	this.splitContainer1.Panel1.Controls.Add(this.ConnectButton);
        	// 
        	// splitContainer1.Panel2
        	// 
        	this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
        	this.splitContainer1.Size = new System.Drawing.Size(702, 426);
        	this.splitContainer1.SplitterDistance = 46;
        	this.splitContainer1.TabIndex = 3;
        	// 
        	// ServerNameComboBox
        	// 
        	this.ServerNameComboBox.FormattingEnabled = true;
        	this.ServerNameComboBox.Location = new System.Drawing.Point(119, 10);
        	this.ServerNameComboBox.Name = "ServerNameComboBox";
        	this.ServerNameComboBox.Size = new System.Drawing.Size(333, 21);
        	this.ServerNameComboBox.TabIndex = 0;
        	this.ServerNameComboBox.Text = "localhost";
        	this.ServerNameComboBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ServerNameComboBox_KeyUp);
        	// 
        	// splitContainer2
        	// 
        	this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.splitContainer2.Location = new System.Drawing.Point(0, 0);
        	this.splitContainer2.Name = "splitContainer2";
        	this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
        	// 
        	// splitContainer2.Panel1
        	// 
        	this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
        	// 
        	// splitContainer2.Panel2
        	// 
        	this.splitContainer2.Panel2.Controls.Add(this.dataGridView2);
        	this.splitContainer2.Size = new System.Drawing.Size(702, 376);
        	this.splitContainer2.SplitterDistance = 219;
        	this.splitContainer2.TabIndex = 0;
        	// 
        	// splitContainer3
        	// 
        	this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.splitContainer3.Location = new System.Drawing.Point(0, 0);
        	this.splitContainer3.Name = "splitContainer3";
        	// 
        	// splitContainer3.Panel1
        	// 
        	this.splitContainer3.Panel1.Controls.Add(this.dataGridView1);
        	// 
        	// splitContainer3.Panel2
        	// 
        	this.splitContainer3.Panel2.Controls.Add(this.propertyGrid1);
        	this.splitContainer3.Size = new System.Drawing.Size(702, 219);
        	this.splitContainer3.SplitterDistance = 380;
        	this.splitContainer3.TabIndex = 0;
        	// 
        	// dataGridView1
        	// 
        	this.dataGridView1.AllowUserToAddRows = false;
        	this.dataGridView1.AllowUserToDeleteRows = false;
        	this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
        	this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
        	this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        	this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
        	this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.dataGridView1.Location = new System.Drawing.Point(0, 0);
        	this.dataGridView1.Name = "dataGridView1";
        	this.dataGridView1.ReadOnly = true;
        	this.dataGridView1.RowHeadersVisible = false;
        	this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
        	this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        	this.dataGridView1.ShowCellErrors = false;
        	this.dataGridView1.ShowCellToolTips = false;
        	this.dataGridView1.ShowEditingIcon = false;
        	this.dataGridView1.ShowRowErrors = false;
        	this.dataGridView1.Size = new System.Drawing.Size(380, 219);
        	this.dataGridView1.TabIndex = 15;
        	this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
        	// 
        	// contextMenuStrip1
        	// 
        	this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.sendMessageToolStripMenuItem,
			this.logoffSessionToolStripMenuItem,
			this.toolStripSeparator1,
			this.rebootServerToolStripMenuItem,
			this.shutdownServerToolStripMenuItem});
        	this.contextMenuStrip1.Name = "contextMenuStrip1";
        	this.contextMenuStrip1.Size = new System.Drawing.Size(164, 98);
        	// 
        	// sendMessageToolStripMenuItem
        	// 
        	this.sendMessageToolStripMenuItem.Name = "sendMessageToolStripMenuItem";
        	this.sendMessageToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
        	this.sendMessageToolStripMenuItem.Text = "Send Message";
        	this.sendMessageToolStripMenuItem.Click += new System.EventHandler(this.sendMessageToolStripMenuItem_Click);
        	// 
        	// logoffSessionToolStripMenuItem
        	// 
        	this.logoffSessionToolStripMenuItem.Name = "logoffSessionToolStripMenuItem";
        	this.logoffSessionToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
        	this.logoffSessionToolStripMenuItem.Text = "Logoff Session";
        	this.logoffSessionToolStripMenuItem.Click += new System.EventHandler(this.logoffSessionToolStripMenuItem_Click);
        	// 
        	// toolStripSeparator1
        	// 
        	this.toolStripSeparator1.Name = "toolStripSeparator1";
        	this.toolStripSeparator1.Size = new System.Drawing.Size(160, 6);
        	// 
        	// rebootServerToolStripMenuItem
        	// 
        	this.rebootServerToolStripMenuItem.Name = "rebootServerToolStripMenuItem";
        	this.rebootServerToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
        	this.rebootServerToolStripMenuItem.Text = "Reboot Server";
        	this.rebootServerToolStripMenuItem.Click += new System.EventHandler(this.rebootServerToolStripMenuItem_Click);
        	// 
        	// shutdownServerToolStripMenuItem
        	// 
        	this.shutdownServerToolStripMenuItem.Name = "shutdownServerToolStripMenuItem";
        	this.shutdownServerToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
        	this.shutdownServerToolStripMenuItem.Text = "Shutdown Server";
        	this.shutdownServerToolStripMenuItem.Click += new System.EventHandler(this.shutdownServerToolStripMenuItem_Click);
        	// 
        	// propertyGrid1
        	// 
        	this.propertyGrid1.CanShowVisualStyleGlyphs = false;
        	this.propertyGrid1.CommandsVisibleIfAvailable = false;
        	this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.propertyGrid1.HelpVisible = false;
        	this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
        	this.propertyGrid1.Name = "propertyGrid1";
        	this.propertyGrid1.Size = new System.Drawing.Size(318, 219);
        	this.propertyGrid1.TabIndex = 16;
        	this.propertyGrid1.ToolbarVisible = false;
        	this.propertyGrid1.Click += new System.EventHandler(this.PropertyGrid1Click);
        	// 
        	// dataGridView2
        	// 
        	this.dataGridView2.AllowUserToAddRows = false;
        	this.dataGridView2.AllowUserToDeleteRows = false;
        	this.dataGridView2.AllowUserToOrderColumns = true;
        	this.dataGridView2.AllowUserToResizeRows = false;
        	this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        	this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.dataGridView2.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
        	this.dataGridView2.Location = new System.Drawing.Point(0, 0);
        	this.dataGridView2.MultiSelect = false;
        	this.dataGridView2.Name = "dataGridView2";
        	this.dataGridView2.ReadOnly = true;
        	this.dataGridView2.RowHeadersVisible = false;
        	this.dataGridView2.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
        	this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        	this.dataGridView2.ShowCellErrors = false;
        	this.dataGridView2.ShowCellToolTips = false;
        	this.dataGridView2.ShowEditingIcon = false;
        	this.dataGridView2.ShowRowErrors = false;
        	this.dataGridView2.Size = new System.Drawing.Size(702, 153);
        	this.dataGridView2.StandardTab = true;
        	this.dataGridView2.TabIndex = 11;
        	// 
        	// progress
        	// 
        	this.progress.BackColor = System.Drawing.Color.Transparent;
        	this.progress.Image = ((System.Drawing.Image)(resources.GetObject("progress.Image")));
        	this.progress.Location = new System.Drawing.Point(87, 8);
        	this.progress.Name = "progress";
        	this.progress.Size = new System.Drawing.Size(297, 297);
        	this.progress.TabIndex = 19;
        	this.progress.TabStop = false;
        	// 
        	// TerminalServerManager
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.Controls.Add(this.progress);
        	this.Controls.Add(this.splitContainer1);
        	this.Name = "TerminalServerManager";
        	this.Size = new System.Drawing.Size(702, 426);
        	this.Load += new System.EventHandler(this.TerminalServerManager_Load);
        	this.splitContainer1.Panel1.ResumeLayout(false);
        	this.splitContainer1.Panel1.PerformLayout();
        	this.splitContainer1.Panel2.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
        	this.splitContainer1.ResumeLayout(false);
        	this.splitContainer2.Panel1.ResumeLayout(false);
        	this.splitContainer2.Panel2.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
        	this.splitContainer2.ResumeLayout(false);
        	this.splitContainer3.Panel1.ResumeLayout(false);
        	this.splitContainer3.Panel2.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
        	this.splitContainer3.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
        	this.contextMenuStrip1.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.progress)).EndInit();
        	this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sendMessageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoffSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem rebootServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shutdownServerToolStripMenuItem;
        private System.Windows.Forms.ComboBox ServerNameComboBox;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.PictureBox progress;
    }
}