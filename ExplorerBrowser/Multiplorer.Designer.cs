namespace ExplorerBrowser
{
    partial class Multiplorer
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
            this.controlStyler1 = new ExplorerBrowser.ControlStyler();
            this.SuspendLayout();
            // 
            // controlStyler1
            // 
            this.controlStyler1.ControlStyle = ExplorerBrowser.ControlStyle.Single;
            this.controlStyler1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlStyler1.Location = new System.Drawing.Point(0, 0);
            this.controlStyler1.Name = "controlStyler1";
            this.controlStyler1.Size = new System.Drawing.Size(150, 150);
            this.controlStyler1.TabIndex = 0;
            // 
            // Multiplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.controlStyler1);
            this.Name = "Multiplorer";
            this.ResumeLayout(false);

        }

        #endregion

        private ControlStyler controlStyler1;

    }
}
