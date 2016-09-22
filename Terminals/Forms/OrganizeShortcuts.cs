using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Kohl.Framework.Drawing;
using Kohl.Framework.Info;
using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Configuration.Files.Main.SpecialCommands;

namespace Terminals.Forms
{
    public partial class OrganizeShortcuts : Form
    {
        private SpecialCommandConfigurationElementCollection shortucts = Settings.SpecialCommands;

        public OrganizeShortcuts()
        {
            this.InitializeComponent();
        }

        private void shortcutCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.executableTextBox.Text = "";
            this.argumentsTextBox.Text = "";
            this.workingFolderTextBox.Text = "";
            this.iconPicturebox.Image = null;
            this.IconImageList.Images.Clear();
            this.toolStrip1.Items.Clear();

            string name = this.shortcutCombobox.Text;
            if (name.Trim() != "" && name.Trim() != "<New...>")
            {
                SpecialCommandConfigurationElement shortcut = this.shortucts[name];
                if (shortcut != null)
                {
                    this.executableTextBox.Text = shortcut.Executable;
                    this.argumentsTextBox.Text = shortcut.Arguments;
                    this.workingFolderTextBox.Text = shortcut.WorkingFolder;
                    this.LoadIconsFromExe();
                }
            }
            else if (name.Trim() == "<New...>")
            {
                this.shortcutCombobox.SelectedIndex = -1;
                this.shortcutCombobox.Focus();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string name = this.shortcutCombobox.Text;
            if (name.Trim() != "" && name.Trim() != "<New...>")
            {
                SpecialCommandConfigurationElement shortcut = this.shortucts[name] ??
                                                              new SpecialCommandConfigurationElement();

                shortcut.Name = name.Trim();
                shortcut.Executable = this.executableTextBox.Text;
                shortcut.WorkingFolder = this.workingFolderTextBox.Text;
                shortcut.Arguments = this.argumentsTextBox.Text;
                string imageName = Path.GetFileName(shortcut.Executable) + ".ico";

                string imageFullName = Path.Combine(AssemblyInfo.DirectoryConfigFiles, imageName);
                if (this.iconPicturebox.Image != null)
                {
                    try
                    {
                        this.iconPicturebox.Image.Save(imageFullName);
                    }
                    catch (Exception exc)
                    {
                        Log.Error("Saving icon picture box failed", exc);
                        imageFullName = "";
                    }
                }
                else
                {
                    imageFullName = "";
                }
                shortcut.Thumbnail = imageFullName;
                this.shortucts.Remove(shortcut);
                this.shortucts.Add(shortcut);

                Settings.SpecialCommands = this.shortucts;
                this.shortucts = Settings.SpecialCommands;
            }
        }

        private void OrganizeShortcuts_Load(object sender, EventArgs e)
        {
            this.LoadShortcuts();
        }

        private void LoadShortcuts()
        {
            this.shortcutCombobox.Items.Clear();
            this.shortcutCombobox.Items.Add("<New...>");
            foreach (SpecialCommandConfigurationElement shortcut in this.shortucts)
            {
                this.shortcutCombobox.Items.Add(shortcut.Name);
            }
            if (this.shortcutCombobox.Items.Count > 0) this.shortcutCombobox.SelectedIndex = 0;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            string name = this.shortcutCombobox.Text;
            if (name.Trim() != "" && name.Trim() != "<New...>")
            {
                SpecialCommandConfigurationElement shortcut = this.shortucts[name];
                if (shortcut != null)
                {
                    this.shortucts.Remove(shortcut);
                    Settings.SpecialCommands = this.shortucts;
                    this.shortucts = Settings.SpecialCommands;
                }
            }
            this.LoadShortcuts();
        }

        private void LoadIconsFromExe()
        {
            try
            {
                Icon[] IconsList = IconHandler.IconsFromFile(this.executableTextBox.Text, IconSize.Small);
                if (IconsList != null && IconsList.Length > 0)
                {
                    this.iconPicturebox.Image = IconsList[0].ToBitmap();

                    this.IconImageList.Images.Clear();
                    this.toolStrip1.Items.Clear();
                    foreach (Icon TmpIcon in IconsList)
                    {
                        this.IconImageList.Images.Add(TmpIcon);
                        this.toolStrip1.Items.Add(TmpIcon.ToBitmap());
                    }
                }
            }
            catch (Exception exc)
            {
                Log.Error("LoadIconsFromExe", exc);
            }
        }

        private void executableBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Executable Files|*.exe",
                Multiselect = false,
                ShowReadOnly = true,
                Title = "Browse for executable..."
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.executableTextBox.Text = ofd.FileName;
                this.LoadIconsFromExe();
            }
        }


        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.iconPicturebox.Image = e.ClickedItem.Image;
        }
    }
}