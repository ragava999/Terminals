namespace Terminals.Plugins.AutoIt.Panels.FavoritePanels
{
    partial class AutoItFavoritePanel
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
            this.edit1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // edit1
            // 
            this.edit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edit1.Location = new System.Drawing.Point(0, 0);
            this.edit1.Name = "edit1";
            this.edit1.Size = new System.Drawing.Size(512, 322);
            this.edit1.TabIndex = 0;
            this.edit1.Text = "";
            // 
            // AutoItFavoritePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.edit1);
            this.Name = "AutoItFavoritePanel";
            this.ResumeLayout(false);

        }





        #endregion

        private System.Windows.Forms.RichTextBox edit1;
    }
}
