namespace ExplorerBrowser
{
    internal partial class ExporerNavigationComboBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExporerNavigationComboBox));
            this.picExporerNavigationComboBox = new System.Windows.Forms.PictureBox();
            this.picExporerNavigationComboBoxButton = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picExporerNavigationComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExporerNavigationComboBoxButton)).BeginInit();
            this.SuspendLayout();
            // 
            // picExporerNavigationComboBox
            // 
            this.picExporerNavigationComboBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picExporerNavigationComboBox.BackgroundImage")));
            this.picExporerNavigationComboBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picExporerNavigationComboBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.picExporerNavigationComboBox.Location = new System.Drawing.Point(0, 0);
            this.picExporerNavigationComboBox.Name = "picExporerNavigationComboBox";
            this.picExporerNavigationComboBox.Size = new System.Drawing.Size(5, 31);
            this.picExporerNavigationComboBox.TabIndex = 16;
            this.picExporerNavigationComboBox.TabStop = false;
            // 
            // picExporerNavigationComboBoxButton
            // 
            this.picExporerNavigationComboBoxButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picExporerNavigationComboBoxButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.picExporerNavigationComboBoxButton.Location = new System.Drawing.Point(541, 0);
            this.picExporerNavigationComboBoxButton.Name = "picExporerNavigationComboBoxButton";
            this.picExporerNavigationComboBoxButton.Size = new System.Drawing.Size(29, 31);
            this.picExporerNavigationComboBoxButton.TabIndex = 18;
            this.picExporerNavigationComboBoxButton.TabStop = false;
            this.picExporerNavigationComboBoxButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picTextBoxEnd_MouseClick);
            this.picExporerNavigationComboBoxButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picTextBoxEnd_MouseDown);
            this.picExporerNavigationComboBoxButton.MouseLeave += new System.EventHandler(this.picTextBoxEnd_MouseLeave);
            this.picExporerNavigationComboBoxButton.MouseHover += new System.EventHandler(this.picTextBoxEnd_MouseHover);
            this.picExporerNavigationComboBoxButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picTextBoxEnd_MouseUp);
            // 
            // ExporerNavigationComboBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.picExporerNavigationComboBox);
            this.Controls.Add(this.picExporerNavigationComboBoxButton);
            this.Name = "ExporerNavigationComboBox";
            this.Size = new System.Drawing.Size(570, 31);
            ((System.ComponentModel.ISupportInitialize)(this.picExporerNavigationComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExporerNavigationComboBoxButton)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picExporerNavigationComboBox;
        private System.Windows.Forms.PictureBox picExporerNavigationComboBoxButton;
    }
}
