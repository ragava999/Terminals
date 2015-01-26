using Kohl.Framework.Info;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace Kohl.Framework.WinForms
{
	public class About : Form
	{
		private IContainer components;

		private Label lblAuthor;

		private Label lblVisit;

		private Button btnOK;

		private LinkLabel lnkUrl;

		private PictureBox picHomeButton;

		private Label lblDescription;

		private Label lblVersion;

		private PictureBox picTitle;

		private PictureBox picAuthor;

		public About(string description, Image titleImage, Type type)
		{
			this.InitializeComponent();
			AssemblyInfo.SetAssembly(type);
			this.lblVersion.Text = string.Concat("Version ", AssemblyInfo.Version.ToString());
			this.lblAuthor.Text = AssemblyInfo.Author;
			this.lnkUrl.Text = AssemblyInfo.Url;
			this.lblDescription.Text = description;
			this.picTitle.BackgroundImage = titleImage;
		}

		private void BtnOKClick(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(About));
			this.picAuthor = new PictureBox();
			this.picTitle = new PictureBox();
			this.lblVersion = new Label();
			this.lblDescription = new Label();
			this.picHomeButton = new PictureBox();
			this.lnkUrl = new LinkLabel();
			this.btnOK = new Button();
			this.lblVisit = new Label();
			this.lblAuthor = new Label();
			((ISupportInitialize)this.picAuthor).BeginInit();
			((ISupportInitialize)this.picTitle).BeginInit();
			((ISupportInitialize)this.picHomeButton).BeginInit();
			base.SuspendLayout();
			this.picAuthor.BackgroundImage = (Image)componentResourceManager.GetObject("picAuthor.BackgroundImage");
			this.picAuthor.BackgroundImageLayout = ImageLayout.Stretch;
			this.picAuthor.Location = new Point(12, 190);
			this.picAuthor.Name = "picAuthor";
			this.picAuthor.Size = new System.Drawing.Size(183, 162);
			this.picAuthor.TabIndex = 0;
			this.picAuthor.TabStop = false;
			this.picTitle.BackgroundImageLayout = ImageLayout.Stretch;
			this.picTitle.Location = new Point(72, 12);
			this.picTitle.Name = "picTitle";
			this.picTitle.Size = new System.Drawing.Size(384, 162);
			this.picTitle.TabIndex = 1;
			this.picTitle.TabStop = false;
			this.lblVersion.AutoSize = true;
			this.lblVersion.Font = new System.Drawing.Font("Monotype Corsiva", 14.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.lblVersion.Location = new Point(349, 123);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(123, 22);
			this.lblVersion.TabIndex = 2;
			this.lblVersion.Text = "Version X.X.X.X";
			this.lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.lblDescription.Location = new Point(218, 192);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(283, 88);
			this.lblDescription.TabIndex = 6;
			this.lblDescription.Text = "Description";
			this.picHomeButton.BackgroundImage = (Image)componentResourceManager.GetObject("picHomeButton.BackgroundImage");
			this.picHomeButton.BackgroundImageLayout = ImageLayout.Stretch;
			this.picHomeButton.Location = new Point(218, 283);
			this.picHomeButton.Name = "picHomeButton";
			this.picHomeButton.Size = new System.Drawing.Size(70, 71);
			this.picHomeButton.TabIndex = 7;
			this.picHomeButton.TabStop = false;
			this.picHomeButton.Click += new EventHandler(this.PictureBox4Click);
			this.lnkUrl.AutoSize = true;
			this.lnkUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.lnkUrl.Location = new Point(331, 318);
			this.lnkUrl.Name = "lnkUrl";
			this.lnkUrl.Size = new System.Drawing.Size(29, 13);
			this.lnkUrl.TabIndex = 8;
			this.lnkUrl.TabStop = true;
			this.lnkUrl.Text = "URL";
			this.lnkUrl.LinkClicked += new LinkLabelLinkClickedEventHandler(this.LinkLabel1LinkClicked);
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnOK.Location = new Point(451, 331);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new EventHandler(this.BtnOKClick);
			this.lblVisit.AutoSize = true;
			this.lblVisit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.lblVisit.Location = new Point(299, 318);
			this.lblVisit.Name = "lblVisit";
			this.lblVisit.Size = new System.Drawing.Size(30, 13);
			this.lblVisit.TabIndex = 10;
			this.lblVisit.Text = "visit";
			this.lblVisit.Click += new EventHandler(this.Label2Click);
			this.lblAuthor.AutoSize = true;
			this.lblAuthor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.lblAuthor.Location = new Point(299, 295);
			this.lblAuthor.Name = "lblAuthor";
			this.lblAuthor.Size = new System.Drawing.Size(59, 13);
			this.lblAuthor.TabIndex = 11;
			this.lblAuthor.Text = "AUTHOR";
			base.AcceptButton = this.btnOK;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.CancelButton = this.btnOK;
			base.ClientSize = new System.Drawing.Size(538, 364);
			base.ControlBox = false;
			base.Controls.Add(this.lblAuthor);
			base.Controls.Add(this.lblVisit);
			base.Controls.Add(this.btnOK);
			base.Controls.Add(this.lnkUrl);
			base.Controls.Add(this.picHomeButton);
			base.Controls.Add(this.lblDescription);
			base.Controls.Add(this.lblVersion);
			base.Controls.Add(this.picTitle);
			base.Controls.Add(this.picAuthor);
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "About";
			this.Text = "About";
			((ISupportInitialize)this.picAuthor).EndInit();
			((ISupportInitialize)this.picTitle).EndInit();
			((ISupportInitialize)this.picHomeButton).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void Label2Click(object sender, EventArgs e)
		{
			Process.Start(AssemblyInfo.Url);
		}

		private void LinkLabel1LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(AssemblyInfo.Url);
		}

		private void PictureBox4Click(object sender, EventArgs e)
		{
			Process.Start(AssemblyInfo.Url);
		}
	}
}