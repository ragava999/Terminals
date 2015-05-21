namespace Terminals.Forms.Controls
{
    partial class FontControl
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
            this.label5 = new System.Windows.Forms.Label();
            this.TextColorTextBox = new System.Windows.Forms.TextBox();
            this.FontButton = new System.Windows.Forms.Button();
            this.TextColorButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.FontTextbox = new System.Windows.Forms.TextBox();
            this.BackColorTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.BackcolorButton = new System.Windows.Forms.Button();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 55;
            this.label5.Text = "Text:";
            // 
            // TextColorTextBox
            // 
            this.TextColorTextBox.Location = new System.Drawing.Point(84, 57);
            this.TextColorTextBox.Name = "TextColorTextBox";
            this.TextColorTextBox.ReadOnly = true;
            this.TextColorTextBox.Size = new System.Drawing.Size(160, 20);
            this.TextColorTextBox.TabIndex = 54;
            this.TextColorTextBox.Text = "FFFFFFFF (White)";
            // 
            // FontButton
            // 
            this.FontButton.Location = new System.Drawing.Point(250, 1);
            this.FontButton.Name = "FontButton";
            this.FontButton.Size = new System.Drawing.Size(31, 23);
            this.FontButton.TabIndex = 47;
            this.FontButton.Text = "...";
            this.FontButton.UseVisualStyleBackColor = true;
            this.FontButton.Click += new System.EventHandler(this.FontButton_Click);
            // 
            // TextColorButton
            // 
            this.TextColorButton.Location = new System.Drawing.Point(250, 55);
            this.TextColorButton.Name = "TextColorButton";
            this.TextColorButton.Size = new System.Drawing.Size(31, 23);
            this.TextColorButton.TabIndex = 53;
            this.TextColorButton.Text = "...";
            this.TextColorButton.UseVisualStyleBackColor = true;
            this.TextColorButton.Click += new System.EventHandler(this.TextColorButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 52;
            this.label4.Text = "Back:";
            // 
            // FontTextbox
            // 
            this.FontTextbox.Location = new System.Drawing.Point(84, 3);
            this.FontTextbox.Name = "FontTextbox";
            this.FontTextbox.ReadOnly = true;
            this.FontTextbox.Size = new System.Drawing.Size(160, 20);
            this.FontTextbox.TabIndex = 48;
            this.FontTextbox.Text = "[Font: Name=Courier New, Size=10, Units=3, GdiCharSet=0, GdiVerticalFont=False, S" +
    "tyle=0]";
            // 
            // BackColorTextBox
            // 
            this.BackColorTextBox.Location = new System.Drawing.Point(84, 30);
            this.BackColorTextBox.Name = "BackColorTextBox";
            this.BackColorTextBox.ReadOnly = true;
            this.BackColorTextBox.Size = new System.Drawing.Size(160, 20);
            this.BackColorTextBox.TabIndex = 51;
            this.BackColorTextBox.Text = "FF000000 (Black)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 49;
            this.label3.Text = "Font:";
            // 
            // BackcolorButton
            // 
            this.BackcolorButton.Location = new System.Drawing.Point(250, 28);
            this.BackcolorButton.Name = "BackcolorButton";
            this.BackcolorButton.Size = new System.Drawing.Size(31, 23);
            this.BackcolorButton.TabIndex = 50;
            this.BackcolorButton.Text = "...";
            this.BackcolorButton.UseVisualStyleBackColor = true;
            this.BackcolorButton.Click += new System.EventHandler(this.BackcolorButton_Click);
            // 
            // fontDialog1
            // 
            this.fontDialog1.Color = System.Drawing.SystemColors.ControlText;
            // 
            // FontControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TextColorTextBox);
            this.Controls.Add(this.FontButton);
            this.Controls.Add(this.TextColorButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.FontTextbox);
            this.Controls.Add(this.BackColorTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BackcolorButton);
            this.Name = "FontControl";
            this.Size = new System.Drawing.Size(284, 82);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Button BackcolorButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox BackColorTextBox;
        private System.Windows.Forms.TextBox FontTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button TextColorButton;
        private System.Windows.Forms.Button FontButton;
        private System.Windows.Forms.TextBox TextColorTextBox;
        private System.Windows.Forms.Label label5;

        #endregion
    }
}
