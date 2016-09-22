using Kohl.Framework.Lists;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Terminals.Configuration.Files.History;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Forms.Controls
{
    public partial class HistoryTreeView : TreeView
    {
        /// <summary>
        ///     Gets the time stamp of last full refresh. This allowes to reload the tree nodes
        ///     when Terminals longer than one day
        /// </summary>
        private DateTime lastFullRefresh = DateTime.Now;

        public HistoryTreeView()
        {
            this.InitializeComponent();

            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                // init groups before loading the history to prevent to run the callback earlier
                this.InitializeTimeLineTreeNodes();

                // consider remove next line to perform full lazy loading without loading the history data
                // directly after application start
                ConnectionHistory.Instance.LoadHistoryAsync();

                // dont apply OnHistoryLoaded event handler to do the lazy loading
                ConnectionHistory.Instance.OnHistoryRecorded += this.OnHistoryRecorded;
            }
        }

        private void InitializeTimeLineTreeNodes()
        {
            this.SuspendLayout();

            // keep chronological order
            this.AddNewHistoryGroupNode(HistoryByFavorite.TODAY, "HISTORY.png");
            this.AddNewHistoryGroupNode(HistoryByFavorite.YESTERDAY, "HISTORY2.png");
            this.AddNewHistoryGroupNode(HistoryByFavorite.WEEK, "HISTORY3.png");
            this.AddNewHistoryGroupNode(HistoryByFavorite.TWOWEEKS, "HISTORY4.png");
            this.AddNewHistoryGroupNode(HistoryByFavorite.MONTH, "HISTORY5.png");
            this.AddNewHistoryGroupNode(HistoryByFavorite.OVERONEMONTH, "HISTORY6.png");
            this.AddNewHistoryGroupNode(HistoryByFavorite.HALFYEAR, "HISTORY7.png");
            this.AddNewHistoryGroupNode(HistoryByFavorite.YEAR, "HISTORY8.png");

            this.ResumeLayout();
        }

        private void AddNewHistoryGroupNode(string name, string imageKey)
        {
            TagTreeNode groupNode = new TagTreeNode(name, imageKey);
            this.Nodes.Add(groupNode);
        }

        /// <summary>
        ///     add new history item in todays list and/or perform full refresh,
        ///     if day has changed since last refresh
        /// </summary>
        private void OnHistoryRecorded(ConnectionHistory sender, HistoryRecordedEventArgs args)
        {
            if (this.IsDayGone())
                this.RefreshAllExpanded();
            else
                this.InsertRecordedNode(args);
        }

        private bool IsDayGone()
        {
            return this.lastFullRefresh.Date < DateTime.Now.Date;
        }

        private void RefreshAllExpanded()
        {
            IEnumerable<TagTreeNode> expandedNodes =
                this.Nodes.Cast<TagTreeNode>().Where(groupNode => !groupNode.NotLoadedYet);
            foreach (TagTreeNode groupNode in expandedNodes)
            {
                RefreshGroupNodes(groupNode);
            }

            this.lastFullRefresh = DateTime.Now;
        }

        private void InsertRecordedNode(HistoryRecordedEventArgs args)
        {
            TagTreeNode todayGroup = this.Nodes[HistoryByFavorite.TODAY] as TagTreeNode;
            if (todayGroup.NotLoadedYet)
                return;

            if (!todayGroup.ContainsFavoriteNode(args.ConnectionName))
                InsertRecordedNode(todayGroup, args);
        }

        private static void InsertRecordedNode(TagTreeNode todayGroup, HistoryRecordedEventArgs args)
        {
            FavoriteConfigurationElement favorite = Settings.GetOneFavorite(args.ConnectionName, false);

            // shouldn't happen, because the favorite was actualy processed
            if (favorite != null)
            {
                int insertIndex = TreeListLoader.FindFavoriteNodeInsertIndex(todayGroup.Nodes, favorite);
                FavoriteTreeNode favoriteNode = new FavoriteTreeNode(favorite);
                todayGroup.Nodes.Insert(insertIndex, favoriteNode);
            }
        }

        private void OnTreeViewExpand(object sender, TreeViewEventArgs e)
        {
            TagTreeNode groupNode = e.Node as TagTreeNode;
            this.ExpandDateGroupNode(groupNode);
        }

        private void ExpandDateGroupNode(TagTreeNode groupNode)
        {
            this.Cursor = Cursors.WaitCursor;
            if (groupNode.NotLoadedYet)
            {
                RefreshGroupNodes(groupNode);
            }
            this.Cursor = Cursors.Default;
        }

        private static void RefreshGroupNodes(TagTreeNode groupNode)
        {
            groupNode.Nodes.Clear();
            SortableList<FavoriteConfigurationElement> groupFavorites =
                ConnectionHistory.Instance.GetDateItems(groupNode.Name);
            CreateGroupNodes(groupNode, groupFavorites);
        }

        private static void CreateGroupNodes(TagTreeNode groupNode,
                                             SortableList<FavoriteConfigurationElement> groupFavorites)
        {
            foreach (FavoriteConfigurationElement favorite in groupFavorites)
            {
                FavoriteTreeNode favoriteNode = new FavoriteTreeNode(favorite);
                groupNode.Nodes.Add(favoriteNode);
            }
        }
    }
}