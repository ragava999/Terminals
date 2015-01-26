namespace Terminals.Panels.FavoritePanels
{
    partial class GenericFavoritePanel
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.lblGenericExecutablePath = new System.Windows.Forms.Label();
            this.lblGenericArguments = new System.Windows.Forms.Label();
            this.lblGenericWorkingDirectory = new System.Windows.Forms.Label();
            this.txtGenericWorkingDirectory = new System.Windows.Forms.TextBox();
            this.txtGenericArguments = new System.Windows.Forms.TextBox();
            this.txtGenericExecutablePath = new System.Windows.Forms.TextBox();
            this.btnGetIconFromExecutable = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblGenericExecutablePath
            // 
            this.lblGenericExecutablePath.AutoSize = true;
            this.lblGenericExecutablePath.Location = new System.Drawing.Point(16, 21);
            this.lblGenericExecutablePath.Name = "lblGenericExecutablePath";
            this.lblGenericExecutablePath.Size = new System.Drawing.Size(88, 13);
            this.lblGenericExecutablePath.TabIndex = 6;
            this.lblGenericExecutablePath.Text = "&Executable Path:";
            // 
            // lblGenericArguments
            // 
            this.lblGenericArguments.AutoSize = true;
            this.lblGenericArguments.Location = new System.Drawing.Point(16, 48);
            this.lblGenericArguments.Name = "lblGenericArguments";
            this.lblGenericArguments.Size = new System.Drawing.Size(60, 13);
            this.lblGenericArguments.TabIndex = 8;
            this.lblGenericArguments.Text = "&Arguments:";
            // 
            // lblGenericWorkingDirectory
            // 
            this.lblGenericWorkingDirectory.AutoSize = true;
            this.lblGenericWorkingDirectory.Location = new System.Drawing.Point(16, 78);
            this.lblGenericWorkingDirectory.Name = "lblGenericWorkingDirectory";
            this.lblGenericWorkingDirectory.Size = new System.Drawing.Size(95, 13);
            this.lblGenericWorkingDirectory.TabIndex = 10;
            this.lblGenericWorkingDirectory.Text = "&Working Directory:";
            // 
            // txtGenericWorkingDirectory
            // 
            this.txtGenericWorkingDirectory.Location = new System.Drawing.Point(115, 75);
            this.txtGenericWorkingDirectory.Name = "txtGenericWorkingDirectory";
            this.txtGenericWorkingDirectory.Size = new System.Drawing.Size(334, 20);
            this.txtGenericWorkingDirectory.TabIndex = 11;
            // 
            // txtGenericArguments
            // 
            this.txtGenericArguments.Location = new System.Drawing.Point(115, 45);
            this.txtGenericArguments.Name = "txtGenericArguments";
            this.txtGenericArguments.Size = new System.Drawing.Size(334, 20);
            this.txtGenericArguments.TabIndex = 9;
            // 
            // txtGenericExecutablePath
            // 
            this.txtGenericExecutablePath.Location = new System.Drawing.Point(115, 18);
            this.txtGenericExecutablePath.Name = "txtGenericExecutablePath";
            this.txtGenericExecutablePath.Size = new System.Drawing.Size(334, 20);
            this.txtGenericExecutablePath.TabIndex = 7;
            // 
            // btnGetIconFromExecutable
            // 
            this.btnGetIconFromExecutable.Location = new System.Drawing.Point(293, 110);
            this.btnGetIconFromExecutable.Name = "btnGetIconFromExecutable";
            this.btnGetIconFromExecutable.Size = new System.Drawing.Size(156, 23);
            this.btnGetIconFromExecutable.TabIndex = 12;
            this.btnGetIconFromExecutable.Text = "Get Icon from Executable";
            this.btnGetIconFromExecutable.UseVisualStyleBackColor = true;
            this.btnGetIconFromExecutable.Click += new System.EventHandler(this.btnGetIconFromExecutable_Click);
            // 
            // GenericFavoritePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnGetIconFromExecutable);
            this.Controls.Add(this.lblGenericExecutablePath);
            this.Controls.Add(this.lblGenericArguments);
            this.Controls.Add(this.lblGenericWorkingDirectory);
            this.Controls.Add(this.txtGenericWorkingDirectory);
            this.Controls.Add(this.txtGenericArguments);
            this.Controls.Add(this.txtGenericExecutablePath);
            this.Name = "GenericFavoritePanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblGenericExecutablePath;
        private System.Windows.Forms.Label lblGenericArguments;
        private System.Windows.Forms.Label lblGenericWorkingDirectory;
        private System.Windows.Forms.TextBox txtGenericWorkingDirectory;
        private System.Windows.Forms.TextBox txtGenericArguments;
        private System.Windows.Forms.TextBox txtGenericExecutablePath;
        private System.Windows.Forms.Button btnGetIconFromExecutable;

    }
}
