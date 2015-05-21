namespace Terminals.Plugins.Text.Panels.FavoritePanels
{
    partial class TextFavoritePanel
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
        	this.splitContainer1 = new System.Windows.Forms.SplitContainer();
        	this.chkShowTinyMceInEditMode = new System.Windows.Forms.CheckBox();
        	this.chkShowTinyMceInConnectionMode = new System.Windows.Forms.CheckBox();
        	((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
        	this.splitContainer1.Panel1.SuspendLayout();
        	this.splitContainer1.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// splitContainer1
        	// 
        	this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
        	this.splitContainer1.IsSplitterFixed = true;
        	this.splitContainer1.Location = new System.Drawing.Point(0, 0);
        	this.splitContainer1.Name = "splitContainer1";
        	this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
        	// 
        	// splitContainer1.Panel1
        	// 
        	this.splitContainer1.Panel1.Controls.Add(this.chkShowTinyMceInEditMode);
        	this.splitContainer1.Panel1.Controls.Add(this.chkShowTinyMceInConnectionMode);
        	this.splitContainer1.Panel1MinSize = 21;
        	this.splitContainer1.Panel2MinSize = 21;
        	this.splitContainer1.Size = new System.Drawing.Size(512, 322);
        	this.splitContainer1.SplitterDistance = 21;
        	this.splitContainer1.TabIndex = 0;
        	// 
        	// chkShowTinyMceInEditMode
        	// 
        	this.chkShowTinyMceInEditMode.AutoSize = true;
        	this.chkShowTinyMceInEditMode.Location = new System.Drawing.Point(3, 3);
        	this.chkShowTinyMceInEditMode.Name = "chkShowTinyMceInEditMode";
        	this.chkShowTinyMceInEditMode.Size = new System.Drawing.Size(157, 17);
        	this.chkShowTinyMceInEditMode.TabIndex = 1;
        	this.chkShowTinyMceInEditMode.Text = "Show TinyMce in edit mode";
        	this.chkShowTinyMceInEditMode.UseVisualStyleBackColor = true;
        	this.chkShowTinyMceInEditMode.CheckedChanged += new System.EventHandler(this.chkShowTinyMceInEditMode_CheckedChanged);
        	// 
        	// chkShowTinyMceInConnectionMode
        	// 
        	this.chkShowTinyMceInConnectionMode.AutoSize = true;
        	this.chkShowTinyMceInConnectionMode.Location = new System.Drawing.Point(222, 3);
        	this.chkShowTinyMceInConnectionMode.Name = "chkShowTinyMceInConnectionMode";
        	this.chkShowTinyMceInConnectionMode.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
        	this.chkShowTinyMceInConnectionMode.Size = new System.Drawing.Size(203, 17);
        	this.chkShowTinyMceInConnectionMode.TabIndex = 0;
        	this.chkShowTinyMceInConnectionMode.Text = "Show TinyMce in connection mode";
        	this.chkShowTinyMceInConnectionMode.UseVisualStyleBackColor = true;
        	// 
        	// TextFavoritePanel
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.Controls.Add(this.splitContainer1);
        	this.Name = "TextFavoritePanel";
        	this.splitContainer1.Panel1.ResumeLayout(false);
        	this.splitContainer1.Panel1.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
        	this.splitContainer1.ResumeLayout(false);
        	this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox chkShowTinyMceInConnectionMode;
        private System.Windows.Forms.CheckBox chkShowTinyMceInEditMode;



    }
}
