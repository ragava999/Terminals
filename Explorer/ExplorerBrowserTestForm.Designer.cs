namespace Explorer
{
    partial class ExplorerBrowserTestForm
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
            this.explorer1 = new ExplorerBrowser.Explorer();
            this.SuspendLayout();
            // 
            // explorer1
            // 
            this.explorer1.AutoSize = true;
            this.explorer1.BackColor = System.Drawing.Color.Transparent;
            this.explorer1.ColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(209)))), ((int)(((byte)(234)))));
            this.explorer1.ColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(180)))), ((int)(((byte)(208)))));
            this.explorer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorer1.InitDirectory = null;
            this.explorer1.Location = new System.Drawing.Point(0, 0);
            this.explorer1.Name = "explorer1";
            this.explorer1.Size = new System.Drawing.Size(841, 874);
            this.explorer1.TabIndex = 1;
            // 
            // ExplorerBrowserTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 874);
            this.Controls.Add(this.explorer1);
            this.Name = "ExplorerBrowserTestForm";
            this.Text = "Explorer Browser Demo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ExplorerBrowser.Explorer explorer1;


    }
}

