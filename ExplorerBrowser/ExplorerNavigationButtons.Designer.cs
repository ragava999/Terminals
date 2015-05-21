namespace ExplorerBrowser
{
    internal partial class ExplorerNavigationButtons
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExplorerNavigationButtons));
            this.picBackground = new System.Windows.Forms.PictureBox();
            this.picLeft = new System.Windows.Forms.PictureBox();
            this.picRight = new System.Windows.Forms.PictureBox();
            this.picDropDown = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDropDown)).BeginInit();
            this.SuspendLayout();
            // 
            // picBackground
            // 
            this.picBackground.BackColor = System.Drawing.Color.Transparent;
            this.picBackground.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picBackground.BackgroundImage")));
            this.picBackground.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picBackground.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.picBackground.Location = new System.Drawing.Point(0, 1);
            this.picBackground.Name = "picBackground";
            this.picBackground.Size = new System.Drawing.Size(72, 29);
            this.picBackground.TabIndex = 19;
            this.picBackground.TabStop = false;
            // 
            // picLeft
            // 
            this.picLeft.Location = new System.Drawing.Point(0, 0);
            this.picLeft.Name = "picLeft";
            this.picLeft.Size = new System.Drawing.Size(28, 30);
            this.picLeft.TabIndex = 20;
            this.picLeft.TabStop = false;
            this.picLeft.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picLeft_MouseClick);
            this.picLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picLeft_MouseDown);
            this.picLeft.MouseLeave += new System.EventHandler(this.picLeft_MouseLeave);
            this.picLeft.MouseHover += new System.EventHandler(this.picLeft_MouseHover);
            this.picLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picLeft_MouseUp);
            // 
            // picRight
            // 
            this.picRight.Location = new System.Drawing.Point(0, 0);
            this.picRight.Name = "picRight";
            this.picRight.Size = new System.Drawing.Size(56, 30);
            this.picRight.TabIndex = 21;
            this.picRight.TabStop = false;
            this.picRight.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picRight_MouseClick);
            this.picRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picRight_MouseDown);
            this.picRight.MouseLeave += new System.EventHandler(this.picRight_MouseLeave);
            this.picRight.MouseHover += new System.EventHandler(this.picRight_MouseHover);
            this.picRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picRight_MouseUp);
            // 
            // picDropDown
            // 
            this.picDropDown.Location = new System.Drawing.Point(0, 0);
            this.picDropDown.Name = "picDropDown";
            this.picDropDown.Size = new System.Drawing.Size(72, 30);
            this.picDropDown.TabIndex = 22;
            this.picDropDown.TabStop = false;
            this.picDropDown.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picDropDown_MouseClick);
            this.picDropDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picDropDown_MouseDown);
            this.picDropDown.MouseLeave += new System.EventHandler(this.picDropDown_MouseLeave);
            this.picDropDown.MouseHover += new System.EventHandler(this.picDropDown_MouseHover);
            this.picDropDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picDropDown_MouseUp);
            // 
            // ExplorerNavigationButtons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.picLeft);
            this.Controls.Add(this.picRight);
            this.Controls.Add(this.picDropDown);
            this.Controls.Add(this.picBackground);
            this.Name = "ExplorerNavigationButtons";
            this.Size = new System.Drawing.Size(72, 30);
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDropDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picBackground;
        private System.Windows.Forms.PictureBox picLeft;
        private System.Windows.Forms.PictureBox picRight;
        private System.Windows.Forms.PictureBox picDropDown;
    }
}
