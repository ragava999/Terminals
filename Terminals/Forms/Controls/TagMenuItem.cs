using System;
using System.Windows.Forms;

namespace Terminals.Forms.Controls
{
    /// <summary>
    ///     Custom toolstrip menu item with lazy loading
    /// </summary>
    public class TagMenuItem : ToolStripMenuItem
    {
        public TagMenuItem()
        {
            this.DropDown.Items.Add(TagTreeNode.DUMMY_NODE);
        }

        /// <summary>
        ///     Gets the value indicating, that this node contains only dummy node
        ///     and contains no favorite nodes
        /// </summary>
        public Boolean IsEmpty
        {
            get
            {
                return this.DropDown.Items.Count == 1 &&
                       String.IsNullOrEmpty(this.DropDown.Items[0].Name);
            }
        }

        public void ClearDropDownsToEmpty()
        {
            this.DropDown.Items.Clear();
            this.DropDown.Items.Add(TagTreeNode.DUMMY_NODE);
        }
    }
}