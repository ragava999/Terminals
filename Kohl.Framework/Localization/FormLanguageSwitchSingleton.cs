using System;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using System.Windows.Forms.Layout;

namespace Kohl.Framework.Localization
{
	public class FormLanguageSwitchSingleton
	{
		protected readonly static FormLanguageSwitchSingleton m_instance;

		private CultureInfo m_cultureInfo;

		public static FormLanguageSwitchSingleton Instance
		{
			get
			{
				return FormLanguageSwitchSingleton.m_instance;
			}
		}

		static FormLanguageSwitchSingleton()
		{
			FormLanguageSwitchSingleton.m_instance = new FormLanguageSwitchSingleton();
		}

		protected FormLanguageSwitchSingleton()
		{
		}

		public void ChangeCurrentThreadUICulture(CultureInfo cultureInfo)
		{
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
		}

		private void ChangeFormLanguage(Control form, bool ignoreMissingManifestResourceException, ResourceManager resources = null)
		{
			form.SuspendLayout();
			Cursor.Current = Cursors.WaitCursor;
			if (resources == null)
			{
				resources = new ResourceManager(form.GetType());
			}
			form.Text = (string)this.GetSafeValue(resources, "$this.Text", form.Text);
			this.ReloadControlCommonProperties(form, resources);
			this.RecurControls(form, resources, this.GetToolTip(form), ignoreMissingManifestResourceException);
			this.ScanNonControls(form, resources);
			form.ResumeLayout();
		}

		public void ChangeLanguage(Control control, ResourceManager resources = null, bool ignoreMissingManifestResourceException = false)
		{
			this.ChangeLanguage(control, Thread.CurrentThread.CurrentUICulture, resources, ignoreMissingManifestResourceException);
		}

		public void ChangeLanguage(Control control, CultureInfo cultureInfo, ResourceManager resources = null, bool ignoreMissingManifestResourceException = false)
		{
			this.m_cultureInfo = cultureInfo;
			this.ChangeFormLanguage(control, ignoreMissingManifestResourceException, resources);
			if (control is Form)
			{
				Form[] mdiChildren = ((Form)control).MdiChildren;
				for (int i = 0; i < (int)mdiChildren.Length; i++)
				{
					this.ChangeFormLanguage(mdiChildren[i], ignoreMissingManifestResourceException, resources);
				}
			}
		}

		private object GetSafeValue(ResourceManager resources, string name, object currentValue)
		{
			object obj = resources.GetObject(name, this.m_cultureInfo);
			if (obj == null)
			{
				return currentValue;
			}
			return obj;
		}

		private ToolTip GetToolTip(Control control)
		{
			FieldInfo[] fields = control.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < (int)fields.Length; i++)
			{
				object value = fields[i].GetValue(control);
				if (value is ToolTip)
				{
					return (ToolTip)value;
				}
			}
			return null;
		}

		private void RecurControls(Control parent, ResourceManager resources, ToolTip toolTip, bool ignoreMissingManifestResourceException)
		{
			foreach (Control control in parent.Controls)
			{
				control.SuspendLayout();
				this.ReloadControlCommonProperties(control, resources);
				this.ReloadControlSpecificProperties(control, resources);
				if (toolTip != null)
				{
					toolTip.SetToolTip(control, (string)this.GetSafeValue(resources, string.Concat(control.Name, ".ToolTip"), control.Text));
				}
				if (!(control is UserControl))
				{
					this.ReloadTextForSelectedControls(control, resources);
					this.ReloadListItems(control, resources);
					if (control is TreeView)
					{
						this.ReloadTreeViewNodes((TreeView)control, resources);
					}
					if (control.Controls.Count > 0)
					{
						this.RecurControls(control, resources, toolTip, ignoreMissingManifestResourceException);
					}
				}
				else
				{
					this.RecurUserControl((UserControl)control, ignoreMissingManifestResourceException);
				}
				control.ResumeLayout();
			}
		}

		private void RecurUserControl(UserControl userControl, bool ignoreMissingManifestResourceException)
		{
			try
			{
				ResourceManager resourceManager = new ResourceManager(userControl.GetType());
				this.RecurControls(userControl, resourceManager, this.GetToolTip(userControl), ignoreMissingManifestResourceException);
				this.ScanNonControls(userControl, resourceManager);
			}
			catch (MissingManifestResourceException missingManifestResourceException1)
			{
				MissingManifestResourceException missingManifestResourceException = missingManifestResourceException1;
				if (!ignoreMissingManifestResourceException)
				{
					FormLanguageSwitchSingleton.Log(null, missingManifestResourceException);
				}
			}
			catch (Exception exception)
			{
				FormLanguageSwitchSingleton.Log(null, exception);
			}
		}

		private void ReloadComboBoxItems(ComboBox comboBox, ResourceManager resources)
		{
			if (comboBox.Items.Count > 0)
			{
				int selectedIndex = comboBox.SelectedIndex;
				this.ReloadItems(comboBox.Name, comboBox.Items, comboBox.Items.Count, resources);
				if (!comboBox.Sorted)
				{
					comboBox.SelectedIndex = selectedIndex;
				}
			}
		}

		protected virtual void ReloadControlCommonProperties(Control control, ResourceManager resources)
		{
			this.SetProperty(control, "AccessibleDescription", resources);
			this.SetProperty(control, "AccessibleName", resources);
			this.SetProperty(control, "BackgroundImage", resources);
			this.SetProperty(control, "Font", resources);
			this.SetProperty(control, "ImeMode", resources);
			this.SetProperty(control, "RightToLeft", resources);
			this.SetProperty(control, "Size", resources);
			if (!(control is Form))
			{
				this.SetProperty(control, "Text", resources);
				this.SetProperty(control, "Anchor", resources);
				this.SetProperty(control, "Dock", resources);
				this.SetProperty(control, "Enabled", resources);
				this.SetProperty(control, "Location", resources);
				this.SetProperty(control, "TabIndex", resources);
				this.SetProperty(control, "Visible", resources);
			}
			if (control is ScrollableControl)
			{
				this.ReloadScrollableControlProperties((ScrollableControl)control, resources);
				if (control is Form)
				{
					this.ReloadFormProperties((Form)control, resources);
				}
			}
		}

		protected virtual void ReloadControlSpecificProperties(Control control, ResourceManager resources)
		{
			this.SetProperty(control, "ImageIndex", resources);
			this.SetProperty(control, "ToolTipText", resources);
			this.SetProperty(control, "IntegralHeight", resources);
			this.SetProperty(control, "ItemHeight", resources);
			this.SetProperty(control, "MaxDropDownItems", resources);
			this.SetProperty(control, "MaxLength", resources);
			this.SetProperty(control, "Appearance", resources);
			this.SetProperty(control, "CheckAlign", resources);
			this.SetProperty(control, "FlatStyle", resources);
			this.SetProperty(control, "ImageAlign", resources);
			this.SetProperty(control, "Indent", resources);
			this.SetProperty(control, "Multiline", resources);
			this.SetProperty(control, "BulletIndent", resources);
			this.SetProperty(control, "RightMargin", resources);
			this.SetProperty(control, "ScrollBars", resources);
			this.SetProperty(control, "WordWrap", resources);
			this.SetProperty(control, "ZoomFactor", resources);
			this.SetProperty(control, "ButtonSize", resources);
			this.SetProperty(control, "DropDownArrows", resources);
			this.SetProperty(control, "ShowToolTips", resources);
			this.SetProperty(control, "Wrappable", resources);
			this.SetProperty(control, "AutoSize", resources);
		}

		private void ReloadFormProperties(Form form, ResourceManager resources)
		{
			this.SetProperty(form, "AutoScaleBaseSize", resources);
			this.SetProperty(form, "Icon", resources);
			this.SetProperty(form, "MaximumSize", resources);
			this.SetProperty(form, "MinimumSize", resources);
		}

		private void ReloadItems(string controlName, IList list, int itemsNumber, ResourceManager resources)
		{
			string str = string.Concat(controlName, ".Items");
			object obj = resources.GetString(str, this.m_cultureInfo);
			if (obj == null)
			{
				str = string.Concat(str, ".Items");
				obj = resources.GetString(str, this.m_cultureInfo);
			}
			if (obj != null)
			{
				list.Clear();
				list.Add(obj);
				for (int i = 1; i < itemsNumber; i++)
				{
					list.Add(resources.GetString(string.Concat(str, i), this.m_cultureInfo));
				}
			}
		}

		private void ReloadListBoxItems(ListBox listBox, ResourceManager resources)
		{
			if (listBox.Items.Count > 0)
			{
				int[] numArray = new int[listBox.SelectedIndices.Count];
				listBox.SelectedIndices.CopyTo(numArray, 0);
				this.ReloadItems(listBox.Name, listBox.Items, listBox.Items.Count, resources);
				if (!listBox.Sorted)
				{
					for (int i = 0; i < (int)numArray.Length; i++)
					{
						listBox.SetSelected(numArray[i], true);
					}
				}
			}
		}

		protected virtual void ReloadListItems(Control control, ResourceManager resources)
		{
			if (control is ComboBox)
			{
				this.ReloadComboBoxItems((ComboBox)control, resources);
				return;
			}
			if (control is ListBox)
			{
				this.ReloadListBoxItems((ListBox)control, resources);
				return;
			}
			if (control is DomainUpDown)
			{
				this.ReloadUpDownItems((DomainUpDown)control, resources);
			}
		}

		private void ReloadScrollableControlProperties(ScrollableControl control, ResourceManager resources)
		{
			this.SetProperty(control, "AutoScroll", resources);
			this.SetProperty(control, "AutoScrollMargin", resources);
			this.SetProperty(control, "AutoScrollMinSize", resources);
		}

		protected virtual void ReloadTextForSelectedControls(Control control, ResourceManager resources)
		{
			if (control is AxHost || control is ButtonBase || control is GroupBox || control is Label || control is ScrollableControl || control is StatusBar || control is TabControl || control is ToolBar)
			{
				control.Text = (string)this.GetSafeValue(resources, string.Concat(control.Name, ".Text"), control.Text);
			}
		}

		private void ReloadTreeViewNodes(TreeView treeView, ResourceManager resources)
		{
			if (treeView.Nodes.Count > 0)
			{
				string str = string.Concat(treeView.Name, ".Nodes");
				TreeNode[] obj = new TreeNode[treeView.Nodes.Count];
				obj[0] = (TreeNode)resources.GetObject(str, this.m_cultureInfo);
				if (obj[0] == null)
				{
					str = string.Concat(str, ".Nodes");
					if (resources.GetObject(str, this.m_cultureInfo) == null)
					{
						return;
					}
					obj[0] = (TreeNode)resources.GetObject(str, this.m_cultureInfo);
				}
				for (int i = 1; i < treeView.Nodes.Count; i++)
				{
					obj[i] = (TreeNode)resources.GetObject(string.Concat(str, i.ToString()), this.m_cultureInfo);
				}
				treeView.Nodes.Clear();
				treeView.Nodes.AddRange(obj);
			}
		}

		private void ReloadUpDownItems(DomainUpDown domainUpDown, ResourceManager resources)
		{
			if (domainUpDown.Items.Count > 0)
			{
				int selectedIndex = domainUpDown.SelectedIndex;
				this.ReloadItems(domainUpDown.Name, domainUpDown.Items, domainUpDown.Items.Count, resources);
				if (!domainUpDown.Sorted)
				{
					domainUpDown.SelectedIndex = selectedIndex;
				}
			}
		}

		protected virtual void ScanNonControls(Control containerControl, ResourceManager resources)
		{
			FieldInfo[] fields = containerControl.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < (int)fields.Length; i++)
			{
				object value = fields[i].GetValue(containerControl);
				string name = fields[i].Name;
				if (value is DataGridViewColumn)
				{
					DataGridViewColumn safeValue = (DataGridViewColumn)value;
					safeValue.Visible = (bool)this.GetSafeValue(resources, string.Concat(name, ".Visible"), safeValue.Visible);
					safeValue.HeaderText = (string)this.GetSafeValue(resources, string.Concat(name, ".HeaderText"), safeValue.HeaderText);
					safeValue.Tag = (string)this.GetSafeValue(resources, string.Concat(name, ".Tag"), safeValue.Tag);
					safeValue.ToolTipText = (string)this.GetSafeValue(resources, string.Concat(name, ".ToolTipText"), safeValue.ToolTipText);
				}
				if (value is MenuItem)
				{
					MenuItem menuItem = (MenuItem)value;
					menuItem.Enabled = (bool)this.GetSafeValue(resources, string.Concat(name, ".Enabled"), menuItem.Enabled);
					menuItem.Shortcut = (Shortcut)this.GetSafeValue(resources, string.Concat(name, ".Shortcut"), menuItem.Shortcut);
					menuItem.ShowShortcut = (bool)this.GetSafeValue(resources, string.Concat(name, ".ShowShortcut"), menuItem.ShowShortcut);
					menuItem.Text = (string)this.GetSafeValue(resources, string.Concat(name, ".Text"), menuItem.Text);
					menuItem.Visible = (bool)this.GetSafeValue(resources, string.Concat(name, ".Visible"), menuItem.Visible);
				}
				if (value is ToolStripMenuItem)
				{
					ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)value;
					toolStripMenuItem.Enabled = (bool)this.GetSafeValue(resources, string.Concat(name, ".Enabled"), toolStripMenuItem.Enabled);
					toolStripMenuItem.ToolTipText = (string)this.GetSafeValue(resources, string.Concat(name, ".ToolTipText"), toolStripMenuItem.ToolTipText);
					toolStripMenuItem.Image = (Image)this.GetSafeValue(resources, string.Concat(name, ".Image"), toolStripMenuItem.Image);
					toolStripMenuItem.Text = (string)this.GetSafeValue(resources, string.Concat(name, ".Text"), toolStripMenuItem.Text);
					toolStripMenuItem.Tag = (string)this.GetSafeValue(resources, string.Concat(name, ".Tag"), toolStripMenuItem.Tag);
				}
				if (value is ToolStripItem)
				{
					ToolStripItem toolStripItem = (ToolStripItem)value;
					toolStripItem.Enabled = (bool)this.GetSafeValue(resources, string.Concat(name, ".Enabled"), toolStripItem.Enabled);
					toolStripItem.ToolTipText = (string)this.GetSafeValue(resources, string.Concat(name, ".ToolTipText"), toolStripItem.ToolTipText);
					toolStripItem.Image = (Image)this.GetSafeValue(resources, string.Concat(name, ".Image"), toolStripItem.Image);
					toolStripItem.Text = (string)this.GetSafeValue(resources, string.Concat(name, ".Text"), toolStripItem.Text);
					toolStripItem.Tag = (string)this.GetSafeValue(resources, string.Concat(name, ".Tag"), toolStripItem.Tag);
				}
				if (value is StatusBarPanel)
				{
					StatusBarPanel statusBarPanel = (StatusBarPanel)value;
					statusBarPanel.Alignment = (HorizontalAlignment)this.GetSafeValue(resources, string.Concat(name, ".Alignment"), statusBarPanel.Alignment);
					statusBarPanel.Icon = (Icon)this.GetSafeValue(resources, string.Concat(name, ".Icon"), statusBarPanel.Icon);
					statusBarPanel.MinWidth = (int)this.GetSafeValue(resources, string.Concat(name, ".MinWidth"), statusBarPanel.MinWidth);
					statusBarPanel.Text = (string)this.GetSafeValue(resources, string.Concat(name, ".Text"), statusBarPanel.Text);
					statusBarPanel.ToolTipText = (string)this.GetSafeValue(resources, string.Concat(name, ".ToolTipText"), statusBarPanel.ToolTipText);
					statusBarPanel.Width = (int)this.GetSafeValue(resources, string.Concat(name, ".Width"), statusBarPanel.Width);
				}
				if (value is ColumnHeader)
				{
					ColumnHeader columnHeader = (ColumnHeader)value;
					columnHeader.Text = (string)this.GetSafeValue(resources, string.Concat(name, ".Text"), columnHeader.Text);
					columnHeader.TextAlign = (HorizontalAlignment)this.GetSafeValue(resources, string.Concat(name, ".TextAlign"), columnHeader.TextAlign);
					columnHeader.Width = (int)this.GetSafeValue(resources, string.Concat(name, ".Width"), columnHeader.Width);
				}
				if (value is ToolBarButton)
				{
					ToolBarButton toolBarButton = (ToolBarButton)value;
					toolBarButton.Enabled = (bool)this.GetSafeValue(resources, string.Concat(name, ".Enabled"), toolBarButton.Enabled);
					toolBarButton.ImageIndex = (int)this.GetSafeValue(resources, string.Concat(name, ".ImageIndex"), toolBarButton.ImageIndex);
					toolBarButton.Text = (string)this.GetSafeValue(resources, string.Concat(name, ".Text"), toolBarButton.Text);
					toolBarButton.ToolTipText = (string)this.GetSafeValue(resources, string.Concat(name, ".ToolTipText"), toolBarButton.ToolTipText);
					toolBarButton.Visible = (bool)this.GetSafeValue(resources, string.Concat(name, ".Visible"), toolBarButton.Visible);
				}
			}
		}

		private void SetProperty(Control control, string propertyName, ResourceManager resources)
		{
			try
			{
				PropertyInfo property = control.GetType().GetProperty(propertyName);
				if (property != null)
				{
					string name = control.Name;
					if (control is Form)
					{
						name = "$this";
					}
					object obj = resources.GetObject(string.Concat(name, ".", propertyName), this.m_cultureInfo);
					if (obj != null)
					{
						property.SetValue(control, Convert.ChangeType(obj, property.PropertyType), null);
					}
				}
			}
			catch
			{
			}
		}

		public static event FormLanguageSwitchSingleton.Logger Log;

		public delegate void Logger(string message, Exception ex);
	}
}