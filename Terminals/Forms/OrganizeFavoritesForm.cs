using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Kohl.Framework.Info;
using Kohl.Framework.Lists;
using Terminals.Configuration.Files.Main;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.ExportImport;
using Terminals.ExportImport.Import;
using Terminals.Forms.Controls;
using Terminals.Network.AD;
using Terminals.Panels;

namespace Terminals.Forms
{
    public partial class OrganizeFavoritesForm : Form
    {
        private FavoriteConfigurationElement editedFavorite;
        private string editedFavoriteName = String.Empty;

        public OrganizeFavoritesForm()
        {
            this.InitializeComponent();

            this.InitializeDataGrid();
            this.ImportOpenFileDialog.Filter = Integrations.Importers.GetProvidersDialogFilter();
            this.UpdateCountLabels();
        }

        public MainForm MainForm { private get; set; }

        public bool SaveInDB { get; set; }

        private void InitializeDataGrid()
        {
            this.dataGridFavorites.AutoGenerateColumns = false;
            this.bsFavorites.DataSource = Settings.GetFavorites(SaveInDB).ToListOrderedByDefaultSorting();
            string sortingProperty = GetDefaultSortPropertyName();
            DataGridViewColumn sortedColumn = this.dataGridFavorites.FindColumnByPropertyName(sortingProperty);
            sortedColumn.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
            this.dataGridFavorites.DataSource = this.bsFavorites;
        }

        private static string GetDefaultSortPropertyName()
        {
            switch (Settings.DefaultSortProperty)
            {
                case SortProperties.ServerName:
                    return "ServerName";
                case SortProperties.Protocol:
                    return "Protocol";
                case SortProperties.ConnectionName:
                    return "Name";
                default:
                    return String.Empty;
            }
        }

        private void UpdateCountLabels()
        {
            Int32 selectedItems = this.dataGridFavorites.SelectedRows.Count;
            this.lblSelectedCount.Text = String.Format("({0} selected)", selectedItems);
            this.lblConnectionCount.Text = this.bsFavorites.Count.ToString();
        }

        private void EditFavorite(FavoriteConfigurationElement favorite)
        {
            FavoriteEditor frmNewTerminal = new FavoriteEditor(favorite);
            if (frmNewTerminal.ShowDialog() != TerminalFormDialogResult.Cancel)
            {
                this.UpdateFavoritesBindingSource();
            }
        }

        private FavoriteConfigurationElement GetSelectedFavorite()
        {
            if (this.dataGridFavorites.SelectedRows.Count > 0)
                return this.dataGridFavorites.SelectedRows[0].DataBoundItem as FavoriteConfigurationElement;
            return null;
        }

        private void dataGridFavorites_DoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // dont allow double click on column row
                this.EditFavorite();
        }

        /// <summary>
        ///     Delete key press in grid.
        /// </summary>
        private void dataGridFavorites_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
                this.CopySelectedFavorite();

            if (e.KeyCode == Keys.Delete)
                this.DeleteSelectedFavorites();
        }

        private void dataGridFavorites_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // the only editable cell should be name
            this.editedFavoriteName = this.dataGridFavorites.CurrentCell.Value.ToString();
            this.editedFavorite = this.dataGridFavorites.SelectedRows[0].DataBoundItem as FavoriteConfigurationElement;
        }

        /// <summary>
        ///     Rename favorite directly in a cell has to be confirmed into the Settings
        /// </summary>
        private void dataGridFavorites_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (String.IsNullOrEmpty(this.editedFavorite.Name)) // cancel or nothing changed
                this.editedFavorite.Name = this.editedFavoriteName;
            if (this.editedFavorite.Name.Equals(this.editedFavoriteName, StringComparison.CurrentCultureIgnoreCase))
                return;

            FavoriteConfigurationElement copy = this.editedFavorite.Clone() as FavoriteConfigurationElement;
            this.editedFavorite.Name = this.editedFavoriteName;
            FavoriteConfigurationElement oldFavorite = Settings.GetOneFavorite(copy.Name, SaveInDB);
            if (oldFavorite != null)
            {
                string message =
                    String.Format("A connection named \"{0}\" already exists\r\nDo you want to overwrite it?", copy.Name);
                if (
                    MessageBox.Show(this, message, AssemblyInfo.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    DialogResult.Yes)
                {
                    this.ReplaceFavoriteInBindingSource(copy);
                }
            }
            else
            {
                this.ReplaceFavoriteInBindingSource(copy);
            }
        }

        private void ReplaceFavoriteInBindingSource(FavoriteConfigurationElement copy)
        {
            Settings.EditFavorite(this.editedFavoriteName, copy);
            this.UpdateFavoritesBindingSource();
        }

        private void OrganizeFavoritesForm_Shown(object sender, EventArgs e)
        {
            if (this.dataGridFavorites.RowCount > 0)
                this.dataGridFavorites.Rows[0].Selected = true;
        }

        /// <summary>
        ///     Opens file dialog to import favorites and imports them from selected files.
        /// </summary>
        public void CallImport()
        {
            if (this.ImportOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                String[] filenames = this.ImportOpenFileDialog.FileNames;
                this.Focus();
                this.Refresh();
                this.Cursor = Cursors.WaitCursor;

                List<FavoriteConfigurationElement> favoritesToImport = Integrations.Importers.ImportFavorites(filenames, this.ImportOpenFileDialog.FilterIndex);
                this.ImportFavoritesWithManagerImport(favoritesToImport);
            }
        }

        private void ImportFavoritesWithManagerImport(List<FavoriteConfigurationElement> favoritesToImport)
        {
            ImportWithDialogs managedImport = new ImportWithDialogs(this);
            bool imported = managedImport.Import(favoritesToImport);
            if (imported)
                this.UpdateFavoritesBindingSource();
        }

        /// <summary>
        ///     Replace the favorites in binding source doesnt affect performance
        /// </summary>
        private void UpdateFavoritesBindingSource()
        {
            SortableList<FavoriteConfigurationElement> data = Settings.GetFavorites(false).ToListOrderedByDefaultSorting();

            DataGridViewColumn lastSortedColumn = this.dataGridFavorites.FindLastSortedColumn();
            if (lastSortedColumn != null) // keep last ordered column
            {
                SortOrder backupGliph = lastSortedColumn.HeaderCell.SortGlyphDirection;
                this.bsFavorites.DataSource = data.SortByProperty(lastSortedColumn.DataPropertyName,
                                                                  lastSortedColumn.HeaderCell.SortGlyphDirection);

                lastSortedColumn.HeaderCell.SortGlyphDirection = backupGliph;
            }
            else
            {
                this.bsFavorites.DataSource = data;
            }

            this.UpdateCountLabels();
        }

        /// <summary>
        ///     Sort columns
        /// </summary>
        private void dataGridFavorites_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn lastSortedColumn = this.dataGridFavorites.FindLastSortedColumn();
            DataGridViewColumn column = this.dataGridFavorites.Columns[e.ColumnIndex];

            SortOrder newSortDirection = SortableUnboundGrid.GetNewSortDirection(lastSortedColumn, column);
            SortableList<FavoriteConfigurationElement> data =
                this.bsFavorites.DataSource as SortableList<FavoriteConfigurationElement>;
            this.bsFavorites.DataSource = data.SortByProperty(column.DataPropertyName, newSortDirection);
            column.HeaderCell.SortGlyphDirection = newSortDirection;
        }

        private void dataGridFavorites_SelectionChanged(object sender, EventArgs e)
        {
            this.UpdateCountLabels();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FavoriteEditor frmNewTerminal = new FavoriteEditor(string.Empty))
            {
                if (frmNewTerminal.ShowDialog() != TerminalFormDialogResult.Cancel)
                {
                    this.UpdateFavoritesBindingSource();
                }
            }
        }

        private void editConnectinoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.EditFavorite();
        }

        private void EditFavorite()
        {
            FavoriteConfigurationElement favorite = this.GetSelectedFavorite();
            if (favorite != null)
                this.EditFavorite(favorite);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DeleteSelectedFavorites();
        }

        private void DeleteSelectedFavorites()
        {
            // prevent delete whole favorite when editing its name directly in grid cell
            if (this.dataGridFavorites.IsCurrentCellInEditMode)
                return;

            this.Cursor = Cursors.WaitCursor;
            List<FavoriteConfigurationElement> selectedFavorites = this.GetSelectedFavorites();
            Settings.DeleteFavorites(selectedFavorites);
            this.UpdateFavoritesBindingSource();
            this.Cursor = Cursors.Default;
        }

        private List<FavoriteConfigurationElement> GetSelectedFavorites()
        {
            return (from DataGridViewRow selectedRow in this.dataGridFavorites.SelectedRows select selectedRow.DataBoundItem as FavoriteConfigurationElement).ToList();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.dataGridFavorites.IsCurrentCellInEditMode)
                this.CopySelectedFavorite();
        }

        private void CopySelectedFavorite()
        {
            FavoriteConfigurationElement favorite = this.GetSelectedFavorite();
            if (favorite != null)
            {
            	string input = "New Connection Name";
                if (InputBox.Show(ref input) == DialogResult.OK && !string.IsNullOrEmpty(input))
                {
                    this.CopySelectedFavorite(favorite, input);
                }
            }
        }

        private void CopySelectedFavorite(FavoriteConfigurationElement favorite, string newName)
        {
            FavoriteConfigurationElement newFav = favorite.Clone() as FavoriteConfigurationElement;
            if (newFav != null)
            {
                newFav.Name = newName;
                Settings.AddFavorite(newFav);
                if (Settings.HasToolbarButton(favorite.Name))
                    Settings.AddFavoriteButton(newFav.Name);
                this.UpdateFavoritesBindingSource();
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridFavorites.SelectedRows.Count > 0)
            {
                this.dataGridFavorites.CurrentCell = this.dataGridFavorites.SelectedRows[0].Cells["colName"];
                this.dataGridFavorites.BeginEdit(true);
            }
        }

        private void scanActiveDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportFromAD activeDirectoryForm = new ImportFromAD();
            activeDirectoryForm.ShowDialog();
            this.UpdateFavoritesBindingSource();
        }

        private void scanRegistryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<FavoriteConfigurationElement> favoritesToImport = ImportRdpRegistry.Import();
            this.ImportFavoritesWithManagerImport(favoritesToImport);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void importFromFileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.CallImport();
        }

        private void exportToAFileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ExportFrom exportFrom = new ExportFrom();
            exportFrom.Show();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.MainForm != null)
            {
                FavoriteConfigurationElement favorite = this.GetSelectedFavorite();
                if (favorite != null)
                {
                    this.MainForm.Connect(favorite.Name, favorite.ConnectToConsole, favorite.NewWindow, favorite.IsDatabaseFavorite, waitForEnd: false);
                }
            }
        }
    }
}