namespace Terminals.Forms {
    partial class QuickConnect {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (this.components != null)) {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuickConnect));
            this.ButtonConnect = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.InputTextbox = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.likeBox1 = new Terminals.Forms.Controls.LikeBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonConnect
            // 
            this.ButtonConnect.Location = new System.Drawing.Point(279, 7);
            this.ButtonConnect.Name = "ButtonConnect";
            this.ButtonConnect.Size = new System.Drawing.Size(60, 23);
            this.ButtonConnect.TabIndex = 1;
            this.ButtonConnect.Text = "&Connect";
            this.ButtonConnect.UseVisualStyleBackColor = true;
            this.ButtonConnect.Click += new System.EventHandler(this.ConnectButton_Click);
            this.ButtonConnect.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ButtonConnect_KeyUp);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(345, 7);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(60, 23);
            this.ButtonCancel.TabIndex = 2;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.CancelButton_Click);
            this.ButtonCancel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ButtonCancel_KeyUp);
            // 
            // InputTextbox
            // 
            this.InputTextbox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.InputTextbox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.InputTextbox.Location = new System.Drawing.Point(35, 9);
            this.InputTextbox.Name = "InputTextbox";
            this.InputTextbox.Size = new System.Drawing.Size(238, 20);
            this.InputTextbox.TabIndex = 0;
            this.InputTextbox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.InputTextbox_KeyUp);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Terminals.Properties.Resources.application_lightning;
            this.pictureBox1.Location = new System.Drawing.Point(5, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 24);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // likeBox1
            // 
            this.likeBox1.BackColor = System.Drawing.Color.White;
            this.likeBox1.Color1 = System.Drawing.Color.LightSkyBlue;
            this.likeBox1.Color2 = System.Drawing.Color.DodgerBlue;
            this.likeBox1.Color3 = System.Drawing.Color.DeepSkyBlue;
            this.likeBox1.Color4 = System.Drawing.Color.DeepSkyBlue;
            this.likeBox1.DrawMode = System.Windows.Forms.DrawMode.Normal;
            this.likeBox1.DropDownHeight = 200;
            this.likeBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.likeBox1.DropDownWidth = 238;
            this.likeBox1.IsDroppedDown = false;
            this.likeBox1.Location = new System.Drawing.Point(35, 6);
            this.likeBox1.MaxDropDownItems = 8;
            this.likeBox1.Name = "likeBox1";
            this.likeBox1.SelectedIndex = -1;
            this.likeBox1.SelectedItem = null;
            this.likeBox1.SelectionLength = 0;
            this.likeBox1.SelectionStart = 0;
            this.likeBox1.Size = new System.Drawing.Size(238, 21);
            this.likeBox1.Sorted = false;
            this.likeBox1.TabIndex = 4;
            this.likeBox1.Text = "likeBox1";
            // 
            // QuickConnect
            // 
            this.AcceptButton = this.ButtonConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(417, 39);
            this.Controls.Add(this.likeBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.InputTextbox);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonConnect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuickConnect";
            this.Text = "Quick connect ...";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.QuickConnect_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonConnect;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.TextBox InputTextbox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Controls.LikeBox likeBox1;
    }
}