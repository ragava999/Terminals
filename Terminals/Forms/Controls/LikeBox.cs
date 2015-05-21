namespace Terminals.Forms.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    public class LikeBox : CustomComboBox
    {
        #region Private Fields (1)
        private string oldText = null;
        #endregion

        #region Public Properties (2)
        [System.ComponentModel.ReadOnly(true)]
        public new List<string> DataSource { get; set; }

        /// <summary>
        /// Shadows the base class DropDownStyle property.
        /// </summary>
        public new ComboBoxStyle DropDownStyle
        {
            get { return ComboBoxStyle.DropDown; }
            set { /* needed for designer */ }
        }
        #endregion

        #region Constructor (1)
        public LikeBox()
        {
            base.DropDownStyle = this.DropDownStyle;
            this.TextChanged += this.bnComboBox1_TextChanged;
            this.KeyDown += this.bnComboBox1_KeyDown;
            this.KeyPress += this.bnComboBox1_KeyPress;
            this.Leave += this.bnComboBox1_Leave;
            DataSource = new List<string>();
        }
        #endregion

        #region Private Methods (4)
        private void bnComboBox1_TextChanged(object sender, EventArgs e)
        {
            if (DataSource == null || DataSource.Count < 1)
                return;

            string item = this.Text;

            if (string.IsNullOrEmpty(item) && string.IsNullOrEmpty(oldText))
            {
                return;
            }

            if (item == oldText)
            {
                return;
            }

            if (string.IsNullOrEmpty(item))
                return;

            if (this.Items.Count > 0)
                this.IsDroppedDown = false;

            this.Items.Clear();

            oldText = item;

            item = item.ToLower();

            List<string> list = new List<string>();
            List<string> listComp = new List<string>();

            for (int i = 0; i < DataSource.Count; i++)
            {
                listComp.Add(DataSource[i]);

                if (DataSource[i].ToLower().Contains(item))
                    list.Add(DataSource[i]);
            }

            if (item != String.Empty)
                foreach (string str in list)
                    this.Items.Add(str);
            else
                foreach (string str in listComp)
                    this.Items.Add(str);

            if (this.Items.Count == 1)
            {
                TextChanged -= this.bnComboBox1_TextChanged;
                this.Text = (string)this.Items[0];
                TextChanged += this.bnComboBox1_TextChanged;
            }
            else if (this.Items.Count > 1)
            {
                this.Text = oldText;
                this.Focus();
                this.SelectionStart = oldText.Length;
                this.SelectionLength = 0;
                this.IsDroppedDown = true;
            }
            this.Focus();
            this.Invalidate();
        }

        private void bnComboBox1_Leave(object sender, EventArgs e)
        {
            this.IsDroppedDown = false;
        }

        private void bnComboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Hide drop down menu
            if (e.KeyChar == (char)Keys.Cancel || e.KeyChar == (char)Keys.Clear ||
                (e.KeyChar == (char)Keys.Delete && string.IsNullOrEmpty(this.Text)) ||
                e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.EraseEof || e.KeyChar == (char)Keys.Escape ||
                e.KeyChar == (char)Keys.OemClear || e.KeyChar == (char)Keys.Pause || e.KeyChar == (char)Keys.Return ||
                e.KeyChar == (char)Keys.PageUp || e.KeyChar == (char)Keys.Sleep || e.KeyChar == (char)Keys.Tab)
            {
                this.IsDroppedDown = false;
            }

            // Show the drop down menu
            if (e.KeyChar == (char)Keys.Play || e.KeyChar == (char)Keys.PageDown)
            {
                this.IsDroppedDown = true;
            }
        }

        private void bnComboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Hide drop down menu
            if (e.KeyCode == Keys.Cancel || e.KeyCode == Keys.Clear ||
                (e.KeyCode == Keys.Delete && string.IsNullOrEmpty(this.Text)) ||
                e.KeyCode == Keys.Enter || e.KeyCode == Keys.EraseEof || e.KeyCode == Keys.Escape ||
                e.KeyCode == Keys.OemClear || e.KeyCode == Keys.Pause || e.KeyCode == Keys.Return ||
                e.KeyCode == Keys.PageUp || e.KeyCode == Keys.Sleep || e.KeyCode == Keys.Tab)
            {
                this.IsDroppedDown = false;
            }

            // Show the drop down menu
            if (e.KeyCode == Keys.Play || e.KeyCode == Keys.PageDown)
            {
                this.IsDroppedDown = true;
            }

            e.Handled = false;
        }
        #endregion
    }
}
