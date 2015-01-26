using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Kohl.Framework.Logging;
using Terminals.Panels;

namespace Terminals.Forms
{
    public partial class DiskDrivesForm : Form
    {
        private readonly FavoriteEditor _parentForm;
        private bool updatingState;

        public DiskDrivesForm(FavoriteEditor parentForm, List<string> redirectedDrives, bool redirectDevices)
        {
            this.InitializeComponent();
            this._parentForm = parentForm;
            this.RedirectDevices = redirectDevices;
            this.RedirectedDrives = redirectedDrives;
            this.LoadDevices();
        }

        public List<string> RedirectedDrives { get; set; }
        public bool RedirectDevices { get; set; }

        private void LoadDevices()
        {
            try
            {
                this.treeView1.Nodes["NodeDevices"].Checked = this.RedirectDevices;

                DriveInfo[] drives = DriveInfo.GetDrives();
                List<string> _redirectedDrives = this.RedirectedDrives;

                foreach (DriveInfo drive in drives)
                {
                    try
                    {
                        string name = drive.Name.TrimEnd("\\".ToCharArray());
                        TreeNode tn = new TreeNode(name + " (" + drive.VolumeLabel + ")") {Name = name};
                        if (_redirectedDrives != null && _redirectedDrives.Contains(name))
                            tn.Checked = true;
                        this.treeView1.Nodes["NodeDrives"].Nodes.Add(tn);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error loading a drive into the tree", e);
                    }
                }

                if (_redirectedDrives != null && _redirectedDrives.Count > 0 && _redirectedDrives[0].Equals("true"))
                    this.treeView1.Nodes["NodeDrives"].Checked = true;

                this.treeView1.ExpandAll();
            }
            catch (Exception exc)
            {
                Log.Error("Failed to load disk drive devices.", exc);
                throw;
            }
        }

        private void DiskDrivesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<string> _redirectedDrives = new List<string>();
            if (this.treeView1.Nodes["NodeDrives"].Checked)
            {
                _redirectedDrives.Add("true");
            }
            else
            {
                _redirectedDrives.AddRange(from TreeNode tn in this.treeView1.Nodes["NodeDrives"].Nodes where tn.Checked select tn.Name);
            }
            this.RedirectedDrives = _redirectedDrives;
            this.RedirectDevices = this.treeView1.Nodes["NodeDevices"].Checked;
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (this.updatingState)
                return;

            this.updatingState = true;
            if (e.Node.Nodes.Count > 0)
            {
                foreach (TreeNode childNode in e.Node.Nodes)
                    childNode.Checked = e.Node.Checked;
            }

            if (e.Node.Parent != null && !e.Node.Checked)
            {
                e.Node.Parent.Checked = e.Node.Checked;
            }
            this.updatingState = false;
        }
    }
}