using Kohl.Framework.Info;

namespace Terminals.Forms
{
    partial class ExpirationDialog
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.components != null)
                {
                    this.components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpirationDialog));
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblInfo2 = new System.Windows.Forms.Label();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.lblThankYou = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.BackColor = System.Drawing.Color.White;
            this.lblInfo.ForeColor = System.Drawing.Color.Black;
            this.lblInfo.Location = new System.Drawing.Point(7, 9);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(309, 40);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "Your license has expired. Please close your connections.\r\nThe program will end in" +
    "";
            // 
            // lblInfo2
            // 
            this.lblInfo2.BackColor = System.Drawing.Color.White;
            this.lblInfo2.ForeColor = System.Drawing.Color.Black;
            this.lblInfo2.Location = new System.Drawing.Point(7, 113);
            this.lblInfo2.Name = "lblInfo2";
            this.lblInfo2.Size = new System.Drawing.Size(309, 40);
            this.lblInfo2.TabIndex = 1;
            this.lblInfo2.Text = "seconds.";
            this.lblInfo2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblSeconds
            // 
            this.lblSeconds.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSeconds.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.lblSeconds.Location = new System.Drawing.Point(7, 49);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(309, 54);
            this.lblSeconds.TabIndex = 2;
            this.lblSeconds.Text = "60";
            this.lblSeconds.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progress
            // 
            this.progress.BackColor = System.Drawing.Color.White;
            this.progress.Location = new System.Drawing.Point(7, 168);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(309, 23);
            this.progress.TabIndex = 3;
            // 
            // lblThankYou
            // 
            this.lblThankYou.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblThankYou.Location = new System.Drawing.Point(9, 207);
            this.lblThankYou.Name = "lblThankYou";
            this.lblThankYou.Size = new System.Drawing.Size(307, 40);
            this.lblThankYou.TabIndex = 4;
            this.lblThankYou.Text = "Thank you for using " + AssemblyInfo.Title + ".";
            this.lblThankYou.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ExpirationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(321, 256);
            this.Controls.Add(this.lblThankYou);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.lblSeconds);
            this.Controls.Add(this.lblInfo2);
            this.Controls.Add(this.lblInfo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(365, 0);
            this.Name = "ExpirationDialog";
            this.Text = "Licsense expired";
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label lblSeconds;
        private System.Windows.Forms.Label lblThankYou;
        private System.Windows.Forms.Label lblInfo2;
        private System.Windows.Forms.Label lblInfo;
    }
}