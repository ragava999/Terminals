using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Terminals.Forms.Controls
{
    public class SortableUnboundGrid : DataGridView
    {
        public SortableUnboundGrid()
        {
            this.AllowUserToAddRows = false;
            this.AllowUserToOrderColumns = true;
            this.AllowUserToResizeRows = false;
            this.BackgroundColor = SystemColors.Window;
            this.BorderStyle = BorderStyle.Fixed3D;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        public static SortOrder GetNewSortDirection(DataGridViewColumn lastSortedColumn, DataGridViewColumn newColumn)
        {
            SortOrder newSortDirection = SortOrder.Ascending;
            if (lastSortedColumn != null)
            {
                if (lastSortedColumn == newColumn)
                {
                    if (newColumn.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                        newSortDirection = SortOrder.Descending;
                }
                lastSortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            return newSortDirection;
        }

        public DataGridViewColumn FindLastSortedColumn()
        {
            return this.Columns.Cast<DataGridViewColumn>().FirstOrDefault(column => column.HeaderCell.SortGlyphDirection != SortOrder.None);
        }

        public DataGridViewColumn FindColumnByPropertyName(string propertyName)
        {
            foreach (DataGridViewColumn column in this.Columns)
            {
                if (column.DataPropertyName == propertyName)
                    return column;
            }

            return this.Columns[0];
        }
    }
}