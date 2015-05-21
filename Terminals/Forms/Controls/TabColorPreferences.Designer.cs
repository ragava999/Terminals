namespace Terminals.Forms.Controls
{
    partial class TabColorPreferences
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
        	this.colorDialog1 = new System.Windows.Forms.ColorDialog();
        	this.fontDialog1 = new System.Windows.Forms.FontDialog();
        	this.txtActive = new System.Windows.Forms.TextBox();
        	this.btnActive = new System.Windows.Forms.Button();
        	this.lblActive = new System.Windows.Forms.Label();
        	this.SuspendLayout();
        	// 
        	// txtActive
        	// 
        	this.txtActive.Location = new System.Drawing.Point(102, 3);
        	this.txtActive.Name = "txtActive";
        	this.txtActive.ReadOnly = true;
        	this.txtActive.Size = new System.Drawing.Size(313, 20);
        	this.txtActive.TabIndex = 47;
        	this.txtActive.Text = "FFFFFFFF (White)";
        	// 
        	// btnActive
        	// 
        	this.btnActive.Location = new System.Drawing.Point(428, 0);
        	this.btnActive.Name = "btnActive";
        	this.btnActive.Size = new System.Drawing.Size(30, 23);
        	this.btnActive.TabIndex = 46;
        	this.btnActive.Text = "...";
        	this.btnActive.UseVisualStyleBackColor = true;
        	this.btnActive.Click += new System.EventHandler(this.BtnActiveClick);
        	// 
        	// lblActive
        	// 
        	this.lblActive.AutoSize = true;
        	this.lblActive.Location = new System.Drawing.Point(3, 6);
        	this.lblActive.Name = "lblActive";
        	this.lblActive.Size = new System.Drawing.Size(55, 13);
        	this.lblActive.TabIndex = 48;
        	this.lblActive.Text = "Tab color:";
        	// 
        	// TabColorPreferences
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.Controls.Add(this.txtActive);
        	this.Controls.Add(this.btnActive);
        	this.Controls.Add(this.lblActive);
        	this.Name = "TabColorPreferences";
        	this.Size = new System.Drawing.Size(461, 26);
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private System.Windows.Forms.Button btnActive;
        private System.Windows.Forms.TextBox txtActive;

        #endregion

        private System.Windows.Forms.Label lblActive;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.FontDialog fontDialog1;
    }
}
