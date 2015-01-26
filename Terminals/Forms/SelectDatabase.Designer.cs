namespace Terminals.Forms
{
    partial class SelectDatabase
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
            this.lblServerName = new System.Windows.Forms.Label();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.cmbDatabase = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDatabase = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.lblMappingQuery = new System.Windows.Forms.Label();
            this.txtMappingQuery = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblServerName
            // 
            this.lblServerName.AutoSize = true;
            this.lblServerName.Location = new System.Drawing.Point(12, 9);
            this.lblServerName.Name = "lblServerName";
            this.lblServerName.Size = new System.Drawing.Size(70, 13);
            this.lblServerName.TabIndex = 0;
            this.lblServerName.Text = "Server name:";
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(12, 50);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(53, 13);
            this.lblDatabase.TabIndex = 4;
            this.lblDatabase.Text = "Database";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(120, 6);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(267, 20);
            this.txtServerName.TabIndex = 5;
            // 
            // cmbDatabase
            // 
            this.cmbDatabase.FormattingEnabled = true;
            this.cmbDatabase.Location = new System.Drawing.Point(120, 45);
            this.cmbDatabase.Name = "cmbDatabase";
            this.cmbDatabase.Size = new System.Drawing.Size(152, 21);
            this.cmbDatabase.TabIndex = 9;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(120, 220);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(197, 220);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnDatabase
            // 
            this.btnDatabase.Location = new System.Drawing.Point(279, 45);
            this.btnDatabase.Name = "btnDatabase";
            this.btnDatabase.Size = new System.Drawing.Size(27, 23);
            this.btnDatabase.TabIndex = 12;
            this.btnDatabase.Text = "...";
            this.btnDatabase.UseVisualStyleBackColor = true;
            this.btnDatabase.Click += new System.EventHandler(this.btnDatabase_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(312, 45);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 13;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // lblMappingQuery
            // 
            this.lblMappingQuery.AutoSize = true;
            this.lblMappingQuery.Location = new System.Drawing.Point(12, 88);
            this.lblMappingQuery.Name = "lblMappingQuery";
            this.lblMappingQuery.Size = new System.Drawing.Size(82, 13);
            this.lblMappingQuery.TabIndex = 14;
            this.lblMappingQuery.Text = "Mapping Query:";
            // 
            // txtMappingQuery
            // 
            this.txtMappingQuery.Location = new System.Drawing.Point(120, 85);
            this.txtMappingQuery.Multiline = true;
            this.txtMappingQuery.Name = "txtMappingQuery";
            this.txtMappingQuery.Size = new System.Drawing.Size(267, 117);
            this.txtMappingQuery.TabIndex = 15;
            // 
            // SelectDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 255);
            this.Controls.Add(this.txtMappingQuery);
            this.Controls.Add(this.lblMappingQuery);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnDatabase);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cmbDatabase);
            this.Controls.Add(this.txtServerName);
            this.Controls.Add(this.lblDatabase);
            this.Controls.Add(this.lblServerName);
            this.Name = "SelectDatabase";
            this.Text = "Select a database";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblServerName;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.ComboBox cmbDatabase;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDatabase;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label lblMappingQuery;
        private System.Windows.Forms.TextBox txtMappingQuery;
    }
}