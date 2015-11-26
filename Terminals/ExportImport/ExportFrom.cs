using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Kohl.Framework.Info;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.ExportImport.Export;
using Terminals.Forms.Controls;

namespace Terminals.ExportImport
{
    public partial class ExportFrom : Form
    {
        public ExportFrom()
        {
            this.InitializeComponent();

            this.favsTree.Load();
            this.saveFileDialog.Filter = Integrations.Exporters.GetProvidersDialogFilter();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.RunExport();

                MessageBox.Show("Done exporting, you can find your exported file at " + this.saveFileDialog.FileName,
                                AssemblyInfo.Title + " export");
                this.Close();
            }
        }

        private void RunExport()
        {
            List<FavoriteConfigurationElement> favorites = this.FindSelectedFavorites();
            // filter index is 1 based
            int filterSplitIndex = (this.saveFileDialog.FilterIndex - 1)*2;
            string providerFilter = this.saveFileDialog.Filter.Split('|')[filterSplitIndex];
            ExportOptions options = new ExportOptions
                                        {
                                            ProviderFilter = providerFilter,
                                            Favorites = favorites,
                                            FileName = this.saveFileDialog.FileName,
                                            IncludePasswords = this.checkBox1.Checked
                                        };
            Integrations.Exporters.Export(options);
        }

        private List<FavoriteConfigurationElement> FindSelectedFavorites()
        {
            return (from TreeNode tn in this.favsTree.Nodes from TreeNode node in tn.Nodes where node.Checked select node.Tag as FavoriteConfigurationElement).ToList();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            TreeNodeCollection tn = this.favsTree.Nodes;
            foreach (TreeNode node in tn)
            {
                node.Checked = true;
                node.ExpandAll();
                CheckNode(node);
            }
        }

        private void favsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode tn = e.Node;

            if (tn is TagTreeNode)
                this.favsTree.LoadAllFavoritesUnderTag(tn as TagTreeNode);

            CheckSubNodes(tn, tn.Checked);
        }

        private static void CheckSubNodes(TreeNode tn, Boolean check)
        {
            foreach (TreeNode node in tn.Nodes)
            {
                node.Checked = check;
            }
        }

        private static void CheckNode(TreeNode node)
        {
            TreeNodeCollection tn = node.Nodes;
            foreach (TreeNode n in tn)
            {
                n.Checked = true;
                n.ExpandAll();
                if (n.GetNodeCount(true) != 0)
                    CheckNode(n);
            }
        }

        private void ExportFrom_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.favsTree.UnregisterEvents();
        }
    }
}