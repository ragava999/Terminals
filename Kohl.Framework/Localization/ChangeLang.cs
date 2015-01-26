using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Kohl.Framework.Localization
{
	public class ChangeLang : Form
	{
		private Button buttonOK;

		private Button buttonCancel;

		private ComboBox comboBox;

		private Label label;

		private CheckBox checkBox;

		private Panel panImage;

		private System.ComponentModel.Container components;

		public new Color BackColor
		{
			get
			{
				return this.BackColor;
			}
			set
			{
				base.BackColor = value;
				this.panImage.BackColor = base.BackColor;
			}
		}

		public bool ChangeCurrentThreadLanguage
		{
			get
			{
				return this.checkBox.Checked;
			}
			set
			{
				this.checkBox.Checked = value;
			}
		}

		public System.Globalization.CultureInfo CultureInfo
		{
			get
			{
				return ((CultureInfoDisplayItem)this.comboBox.SelectedItem).CultureInfo;
			}
		}

		public System.Drawing.Image Image
		{
			get
			{
				return this.panImage.BackgroundImage;
			}
			set
			{
				this.panImage.BackgroundImage = value;
			}
		}

		public string Language
		{
			get
			{
				return this.CultureInfo.ThreeLetterISOLanguageName;
			}
			set
			{
				foreach (CultureInfoDisplayItem item in this.comboBox.Items)
				{
					if (item.CultureInfo.ThreeLetterISOLanguageName != value)
					{
						continue;
					}
					this.comboBox.SelectedItem = item;
					break;
				}
			}
		}

		public ChangeLang()
		{
			int num;
			this.InitializeComponent();
			CultureInfoDisplayItem[] languages = (new LanguageCollector()).GetLanguages(LanguageCollector.LanguageNameDisplay.NativeName, out num);
			this.comboBox.Items.AddRange(languages);
			this.comboBox.SelectedIndex = num;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ChangeLang));
			this.comboBox = new ComboBox();
			this.label = new Label();
			this.buttonOK = new Button();
			this.buttonCancel = new Button();
			this.checkBox = new CheckBox();
			this.panImage = new Panel();
			base.SuspendLayout();
			this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			componentResourceManager.ApplyResources(this.comboBox, "comboBox");
			this.comboBox.Name = "comboBox";
			componentResourceManager.ApplyResources(this.label, "label");
			this.label.Name = "label";
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			componentResourceManager.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			componentResourceManager.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			componentResourceManager.ApplyResources(this.checkBox, "checkBox");
			this.checkBox.Name = "checkBox";
			componentResourceManager.ApplyResources(this.panImage, "panImage");
			this.panImage.Name = "panImage";
			base.AcceptButton = this.buttonOK;
			componentResourceManager.ApplyResources(this, "$this");
			base.CancelButton = this.buttonCancel;
			base.Controls.Add(this.panImage);
			base.Controls.Add(this.checkBox);
			base.Controls.Add(this.buttonCancel);
			base.Controls.Add(this.buttonOK);
			base.Controls.Add(this.label);
			base.Controls.Add(this.comboBox);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ChangeLang";
			base.ResumeLayout(false);
		}
	}
}