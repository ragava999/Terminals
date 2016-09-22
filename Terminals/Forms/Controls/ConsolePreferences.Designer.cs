namespace Terminals.Forms.Controls
{
    partial class ConsolePreferences
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
            this.label6 = new System.Windows.Forms.Label();
            this.CursorColorTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CursorColorButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ColumnsTextBox = new System.Windows.Forms.TextBox();
            this.RowsTextBox = new System.Windows.Forms.TextBox();
            this.fontControl1 = new Terminals.Forms.Controls.FontControl();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 152);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 49;
            this.label6.Text = "Cursor:";
            // 
            // CursorColorTextBox
            // 
            this.CursorColorTextBox.Location = new System.Drawing.Point(88, 149);
            this.CursorColorTextBox.Name = "CursorColorTextBox";
            this.CursorColorTextBox.ReadOnly = true;
            this.CursorColorTextBox.Size = new System.Drawing.Size(160, 20);
            this.CursorColorTextBox.TabIndex = 48;
            this.CursorColorTextBox.Text = "FFFF0000 (Red)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "Rows:";
            // 
            // CursorColorButton
            // 
            this.CursorColorButton.Location = new System.Drawing.Point(254, 147);
            this.CursorColorButton.Name = "CursorColorButton";
            this.CursorColorButton.Size = new System.Drawing.Size(31, 23);
            this.CursorColorButton.TabIndex = 47;
            this.CursorColorButton.Text = "...";
            this.CursorColorButton.UseVisualStyleBackColor = true;
            this.CursorColorButton.Click += new System.EventHandler(this.CursorColorButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "Columns:";
            // 
            // ColumnsTextBox
            // 
            this.ColumnsTextBox.Location = new System.Drawing.Point(88, 33);
            this.ColumnsTextBox.Name = "ColumnsTextBox";
            this.ColumnsTextBox.Size = new System.Drawing.Size(38, 20);
            this.ColumnsTextBox.TabIndex = 37;
            this.ColumnsTextBox.Text = "110";
            // 
            // RowsTextBox
            // 
            this.RowsTextBox.Location = new System.Drawing.Point(88, 4);
            this.RowsTextBox.Name = "RowsTextBox";
            this.RowsTextBox.Size = new System.Drawing.Size(38, 20);
            this.RowsTextBox.TabIndex = 36;
            this.RowsTextBox.Text = "38";
            // 
            // fontControl1
            // 
            this.fontControl1.AutoSize = true;
            this.fontControl1.Location = new System.Drawing.Point(-1, 62);
            this.fontControl1.Name = "fontControl1";
            this.fontControl1.Size = new System.Drawing.Size(288, 82);
            this.fontControl1.TabIndex = 50;
            // 
            // ConsolePreferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.fontControl1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.CursorColorTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CursorColorButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ColumnsTextBox);
            this.Controls.Add(this.RowsTextBox);
            this.Name = "ConsolePreferences";
            this.Size = new System.Drawing.Size(294, 178);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private Terminals.Forms.Controls.FontControl fontControl1;

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox CursorColorTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button CursorColorButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ColumnsTextBox;
        private System.Windows.Forms.TextBox RowsTextBox;
    }
}
