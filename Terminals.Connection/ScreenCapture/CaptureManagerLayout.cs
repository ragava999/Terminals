using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Kohl.Framework.Logging;
using Kohl.Framework.WinForms;
using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Connection.ScreenCapture
{
    public partial class CaptureManagerLayout : UserControl
    {
        private readonly TreeNode root =
            new TreeNode("Capture Root Folder");

        public CaptureManagerLayout()
        {
            this.InitializeComponent();

            this.viewComboBox.Items.AddRange(new object[]
                                                 {
                                                     "Large Icons",
                                                     "Small Icons",
                                                     "Title",
                                                     "List"
                                                 });
        }

        private void CaptureManagerLayout_Load(object sender, EventArgs e)
        {
            this.LoadRoot();
            this.viewComboBox.SelectedIndex = 1;
            this.trackBarZoom.Value = 45;

            treeViewFolders.AfterSelect += treeView1_AfterSelect;
        }

        public void RefreshView()
        {
            if (this.treeViewFolders.SelectedNode != null)
            {
                DirectoryInfo folder = (this.treeViewFolders.SelectedNode.Tag as DirectoryInfo);
                this.LoadFolder(folder.FullName, this.treeViewFolders.SelectedNode);
            }
        }

        private void LoadRoot()
        {
            string rootPath = Settings.CaptureRoot.NormalizePath();

            this.root.Tag = new DirectoryInfo(rootPath);
            AssignImageIndexes(this.root);

            this.treeViewFolders.Nodes.Add(this.root);
            this.treeViewFolders.SelectedNode = this.root;
            this.root.Expand();
        }

        private static void AssignImageIndexes(TreeNode treeNodeToConfigure)
        {
            treeNodeToConfigure.ImageIndex = 0;
            treeNodeToConfigure.SelectedImageIndex = 1;
        }

        private readonly object locker = new object();

        Thread pictureUpdater = null;

        private void LoadFolder(string Path, TreeNode Parent)
        {
            Parent.Nodes.Clear();

            if (!Directory.Exists(Path))
                return;

            DirectoryInfo[] directories = new DirectoryInfo(Path).GetDirectories();

            foreach (DirectoryInfo folder in directories)
            {
                AddNewDirectoryTreeNode(Parent, folder);
            }

            if (pictureUpdater != null && pictureUpdater.IsAlive)
                pictureUpdater.Abort();

            pictureUpdater = new System.Threading.Thread((System.Threading.ThreadStart)delegate
            {
                lock (locker)
                {
                    this.listViewFiles.InvokeIfNecessary(() => this.imageList.Images.Clear());
                    this.listViewFiles.InvokeIfNecessary(() => this.listViewFiles.SuspendLayout());
                    this.listViewFiles.InvokeIfNecessary(() => this.listViewFiles.Items.Clear());

                    FileInfo[] fileInfos = new DirectoryInfo(Path).GetFiles("*.png");

                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        Capture cap = new Capture(fileInfo.FullName);
                        this.AddNewCaptureListViewItem(cap);
                        cap.Image.Dispose();
                    }

                    this.listViewFiles.InvokeIfNecessary(() => this.listViewFiles.ResumeLayout(false));
                }
            });

            pictureUpdater.Start();
        }

        private static void AddNewDirectoryTreeNode(TreeNode Parent, DirectoryInfo folder)
        {
            TreeNode child = new TreeNode(folder.Name);
            AssignImageIndexes(child);
            child.Tag = folder;
            Parent.Nodes.Add(child);
        }

        private void AddNewCaptureListViewItem(Capture cap)
        {
            ListViewItem item = new ListViewItem();
            item.Tag = cap;
            item.Text = cap.Name;
            item.ToolTipText = cap.FilePath;
            int index = 0;
            this.listViewFiles.InvokeIfNecessary(() => {
                if (!this.imageList.Images.ContainsKey(cap.Name))
                    this.imageList.Images.Add(cap.Name, cap.Image);
                
                index = this.imageList.Images.IndexOfKey(cap.Name);
            });
            item.ImageIndex = index;
            this.listViewFiles.InvokeIfNecessary(() => this.listViewFiles.Items.Add(item));
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.pictureBox1.Image = null;
            this.pictureCommentsTextBox.Text = string.Empty;
            this.saveButton.Enabled = false;
            this.deleteButton.Enabled = false;

            DirectoryInfo dir = (e.Node.Tag as DirectoryInfo);
            this.LoadFolder(dir.FullName, e.Node);
        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeViewFolders.SelectedNode != null)
            {
                DirectoryInfo dir = (this.treeViewFolders.SelectedNode.Tag as DirectoryInfo);
                
                string input = "New folder name";
                if (InputBox.Show(ref input) == DialogResult.OK)
                {
                    string rootFolder = dir.FullName;
                    string fullNewName = Path.Combine(rootFolder, input);
                    if (!Directory.Exists(fullNewName))
                    {
                        DirectoryInfo info = Directory.CreateDirectory(fullNewName);
                        TreeNode node = new TreeNode(input);
                        node.Tag = info;
                        this.treeViewFolders.SelectedNode.Nodes.Add(node);
                        this.treeViewFolders.SelectedNode.Expand();
                    }
                }
            }
        }

        private void deleteFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeViewFolders.SelectedNode != null && this.treeViewFolders.SelectedNode != this.root)
            {
                DirectoryInfo dir = (this.treeViewFolders.SelectedNode.Tag as DirectoryInfo);
                if (Directory.Exists(dir.FullName))
                {
                    FileInfo[] files = dir.GetFiles();
                    DirectoryInfo[] dirs = dir.GetDirectories();
                    string msg = string.Format("{0}\r\n\r\n", "Are you sure you want to delete this file?");
                    if (files.Length > 0)
                    {
                        msg += string.Format("The folder \"{0}\" contains {1} directories.",
                                             this.treeViewFolders.SelectedNode.Text, files.Length);
                    }

                    if (dirs.Length > 0)
                    {
                        msg += string.Format("The folder \"{0}\" contains {1} files.",
                            this.treeViewFolders.SelectedNode.Text, dirs.Length);
                    }

                    DialogResult result = MessageBox.Show(msg, "Delete Folder?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.OK)
                    {
                        string rootFolder = dir.FullName;
                        Directory.Delete(rootFolder, true);
                        this.treeViewFolders.SelectedNode.Remove();
                    }
                }
                else
                {
                    this.treeViewFolders.SelectedNode.Remove();
                }
            }
        }

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            this.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            ListViewItem NewNode;

            if (e.Data.GetDataPresent("System.Windows.Forms.ListViewItem", false))
            {
                Point pt = ((TreeView) sender).PointToClient(new Point(e.X, e.Y));
                TreeNode DestinationNode = ((TreeView) sender).GetNodeAt(pt);
                NewNode = (ListViewItem) e.Data.GetData("System.Windows.Forms.ListViewItem");
                Capture c = (NewNode.Tag as Capture);

                if (DestinationNode != null)
                {
                    DirectoryInfo destInfo = (DestinationNode.Tag as DirectoryInfo);
                    string dest = Path.Combine(destInfo.FullName, Path.GetFileName(c.FilePath));
                    c.Move(dest);

                    this.treeView1_AfterSelect(null, new TreeViewEventArgs(this.treeViewFolders.SelectedNode));
                }
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listViewFiles.SelectedItems != null && this.listViewFiles.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in this.listViewFiles.SelectedItems)
                {
                    Capture cap = (lvi.Tag as Capture);
                    cap.Show();
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pictureBox1.Image = null;
            this.pictureCommentsTextBox.Text = string.Empty;
            this.saveButton.Enabled = false;
            this.deleteButton.Enabled = false;
            if (this.listViewFiles.SelectedItems != null && this.listViewFiles.SelectedItems.Count > 0)
            {
                Capture cap = (this.listViewFiles.SelectedItems[0].Tag as Capture);
                this.pictureBox1.Image = cap.Image;
                this.pictureCommentsTextBox.Text = cap.Comments;
                this.saveButton.Enabled = true;
                this.deleteButton.Enabled = true;
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this.listViewFiles.SelectedItems != null &&
                this.listViewFiles.SelectedItems.Count > 0)
            {
                this.listViewFiles.ContextMenuStrip.Show();
            }
        }

        /// <summary>
        ///     Delete selected listview item on delete key press.
        /// </summary>
        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                this.deleteFileToolStripMenuItem_Click(null, null);
            }
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this.treeViewFolders.SelectedNode != null)
            {
                TreeNode node = this.treeViewFolders.HitTest(e.Location).Node;

                if (node == null || treeViewFolders.SelectedNode == node)
                    return;

                this.treeViewFolders.SelectedNode = node;
            }
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            Point pos = this.treeViewFolders.PointToClient(new Point(e.X, e.Y));
            TreeViewHitTestInfo hit = this.treeViewFolders.HitTest(pos);

            if (hit.Node != null)
            {
                hit.Node.Expand();
                this.treeViewFolders.SelectedNode = hit.Node;
                e.Effect = DragDropEffects.Move;
            }
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            this.deleteFolderToolStripMenuItem_Click(null, null);
        }

        private void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listViewFiles.SelectedItems != null && this.listViewFiles.SelectedItems.Count > 0)
            {
                int cnt = this.listViewFiles.SelectedItems.Count;
                string msg = string.Empty;
                string cpt = string.Empty;

                if (this.listViewFiles.SelectedItems.Count == 1)
                {
                    msg = "Are you sure you want to delete this file?";
                    cpt = "Delete Item";
                }
                else
                {
                    msg =
                        string.Format(
                            "Are you sure you want to delete these {0} files?",
                            cnt);
                    cpt =
                    	"Delete Multiple Items";
                }

                if (MessageBox.Show(msg, cpt, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    foreach (ListViewItem lvi in this.listViewFiles.SelectedItems)
                    {
                        Capture cap = (lvi.Tag as Capture);
                        cap.Delete();
                        this.listViewFiles.Items.Remove(lvi);
                    }
                }
            }
        }

        private void copyImageToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listViewFiles.SelectedItems != null && this.listViewFiles.SelectedItems.Count > 0)
            {
                Capture cap = (this.listViewFiles.SelectedItems[0].Tag as Capture);
                Clipboard.SetImage(cap.Image);
            }
        }

        private void copyImagePathToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listViewFiles.SelectedItems != null && this.listViewFiles.SelectedItems.Count > 0)
            {
                Capture cap = (this.listViewFiles.SelectedItems[0].Tag as Capture);
                Clipboard.SetText(cap.FilePath);
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            this.deleteFileToolStripMenuItem_Click(null, null);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (this.listViewFiles.SelectedItems != null && this.listViewFiles.SelectedItems.Count > 0)
            {
                Capture cap = (this.listViewFiles.SelectedItems[0].Tag as Capture);
                cap.Comments = this.pictureCommentsTextBox.Text;
                cap.Save();
            }
        }

        private void View_SelectedIndexChanged(object sender, EventArgs e)
        {
        	if (this.viewComboBox.Text == this.viewComboBox.Items[0].ToString())
            {
                this.imageList.ImageSize = new Size(150, 150);
                this.listViewFiles.View = View.LargeIcon;
            }
            else if (this.viewComboBox.Text == this.viewComboBox.Items[1].ToString())
            {
                this.imageList.ImageSize = new Size(50, 50);
                this.listViewFiles.View = View.LargeIcon;
            }
            else if (this.viewComboBox.Text == this.viewComboBox.Items[2].ToString())
            {
                this.imageList.ImageSize = new Size(150, 150);
                this.listViewFiles.View = View.Tile;
            }
            else
            {
                this.imageList.ImageSize = new Size(150, 150);
                this.listViewFiles.View = View.List;
            }

            this.RefreshView();
        }

        private void RefreshScreen()
        {
            this.imageList.ImageSize = new Size(this.trackBarZoom.Value, this.trackBarZoom.Value);
            this.listViewFiles.View = View.LargeIcon;
            this.RefreshView();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.RefreshScreen();
        }
    }
}