using System.Windows.Forms;

namespace Terminals.Plugins.AutoIt.Panels.OptionPanels
{
    partial class AutoItOptionPanel
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
            this.grpAutoItPath = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtAutoItPath = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panAutoIt.SuspendLayout();
            this.grpAutoItPath.SuspendLayout();
            this.SuspendLayout();
            // 
            // panAutoIt
            // 
            this.panAutoIt.Controls.Add(this.grpAutoItPath);
            this.panAutoIt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panAutoIt.Location = new System.Drawing.Point(0, 0);
            this.panAutoIt.Name = "panAutoIt";
            this.panAutoIt.Size = new System.Drawing.Size(514, 332);
            this.panAutoIt.TabIndex = 25;
            // 
            // grpAutoItPath
            // 
            this.grpAutoItPath.Controls.Add(this.btnBrowse);
            this.grpAutoItPath.Controls.Add(this.txtAutoItPath);
            this.grpAutoItPath.Location = new System.Drawing.Point(6, 3);
            this.grpAutoItPath.Name = "grpAutoItPath";
            this.grpAutoItPath.Size = new System.Drawing.Size(499, 65);
            this.grpAutoItPath.TabIndex = 4;
            this.grpAutoItPath.TabStop = false;
            this.grpAutoItPath.Text = "Path to the AutoIt executable";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(414, 28);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 20;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtAutoItPath
            // 
            this.txtAutoItPath.AcceptsReturn = true;
            this.txtAutoItPath.Location = new System.Drawing.Point(11, 30);
            this.txtAutoItPath.Name = "txtAutoItPath";
            this.txtAutoItPath.Size = new System.Drawing.Size(397, 20);
            this.txtAutoItPath.TabIndex = 19;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "AutoIt-Engine|AutoIt*.exe|Ausführbare Dateien|*.exe";
            this.openFileDialog1.Title = "Select the AutoIt engine";
            // 
            // AutoItOptionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panAutoIt);
            this.Name = "AutoItOptionPanel";
            this.Size = new System.Drawing.Size(514, 332);
            this.panAutoIt.ResumeLayout(false);
            this.grpAutoItPath.ResumeLayout(false);
            this.grpAutoItPath.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panAutoIt;
        private GroupBox grpAutoItPath;
        private TextBox txtAutoItPath;
        private Button btnBrowse;
        private OpenFileDialog openFileDialog1;
    }
}
