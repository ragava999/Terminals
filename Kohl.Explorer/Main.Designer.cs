namespace Kohl.Explorer
{
    partial class Main
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.multiplorer1 = new ExplorerBrowser.Multiplorer();
            this.SuspendLayout();
            // 
            // multiplorer1
            // 
            this.multiplorer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.multiplorer1.ExplorerStyle = ExplorerBrowser.ControlStyle.Single;
            this.multiplorer1.Location = new System.Drawing.Point(0, 0);
            this.multiplorer1.Name = "multiplorer1";
            this.multiplorer1.Size = new System.Drawing.Size(624, 441);
            this.multiplorer1.TabIndex = 0;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.multiplorer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "Explorer";
            this.ResumeLayout(false);

        }

        #endregion

        private ExplorerBrowser.Multiplorer multiplorer1;

    }
}

