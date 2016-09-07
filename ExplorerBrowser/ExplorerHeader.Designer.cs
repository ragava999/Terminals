namespace ExplorerBrowser
{
    internal partial class ExplorerHeader
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.picDeleteHistory = new System.Windows.Forms.PictureBox();
            this.explorerNavigationButtons1 = new ExplorerBrowser.ExplorerNavigationButtons();
            this.pathEdit = new ExplorerBrowser.ExporerNavigationComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.picDeleteHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(61, 4);
            this.contextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenu_ItemClicked);
            // 
            // picDeleteHistory
            // 
            this.picDeleteHistory.BackColor = System.Drawing.Color.Transparent;
            this.picDeleteHistory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picDeleteHistory.Dock = System.Windows.Forms.DockStyle.Right;
            this.picDeleteHistory.Location = new System.Drawing.Point(417, 0);
            this.picDeleteHistory.MaximumSize = new System.Drawing.Size(27, 32);
            this.picDeleteHistory.MinimumSize = new System.Drawing.Size(27, 32);
            this.picDeleteHistory.Name = "picDeleteHistory";
            this.picDeleteHistory.Size = new System.Drawing.Size(27, 32);
            this.picDeleteHistory.TabIndex = 19;
            this.picDeleteHistory.TabStop = false;
            this.picDeleteHistory.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            this.picDeleteHistory.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.picDeleteHistory.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            this.picDeleteHistory.MouseHover += new System.EventHandler(this.pictureBox1_MouseHover);
            this.picDeleteHistory.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // explorerNavigationButtons1
            // 
            this.explorerNavigationButtons1.BackColor = System.Drawing.Color.Transparent;
            this.explorerNavigationButtons1.Dock = System.Windows.Forms.DockStyle.Left;
            this.explorerNavigationButtons1.HasLeftHistroy = false;
            this.explorerNavigationButtons1.HasRightHistroy = false;
            this.explorerNavigationButtons1.Location = new System.Drawing.Point(0, 0);
            this.explorerNavigationButtons1.MaximumSize = new System.Drawing.Size(72, 30);
            this.explorerNavigationButtons1.MinimumSize = new System.Drawing.Size(72, 30);
            this.explorerNavigationButtons1.Name = "explorerNavigationButtons1";
            this.explorerNavigationButtons1.Size = new System.Drawing.Size(72, 30);
            this.explorerNavigationButtons1.TabIndex = 18;
            // 
            // pathEdit
            // 
            this.pathEdit.BackColor = System.Drawing.Color.Transparent;
            this.pathEdit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pathEdit.Location = new System.Drawing.Point(0, 32);
            this.pathEdit.Name = "pathEdit";
            this.pathEdit.Size = new System.Drawing.Size(444, 30);
            this.pathEdit.TabIndex = 17;
            // 
            // ExplorerHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picDeleteHistory);
            this.Controls.Add(this.explorerNavigationButtons1);
            this.Controls.Add(this.pathEdit);
            this.Name = "ExplorerHeader";
            this.Size = new System.Drawing.Size(444, 62);
            this.Resize += new System.EventHandler(this.ExplorerHeader_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picDeleteHistory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ExporerNavigationComboBox pathEdit;
        private ExplorerNavigationButtons explorerNavigationButtons1;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.PictureBox picDeleteHistory;
    }
}
