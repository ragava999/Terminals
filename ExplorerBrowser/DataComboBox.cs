using System;
using System.Windows.Forms;

namespace ExplorerBrowser
{
    internal class DataComboBox : ComboBox
    {
        public struct Element
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }

        public DataComboBox()
        {
            base.TextChanged += DataComboBox_TextChanged;
            text = new Element();
        }

        void DataComboBox_TextChanged(object sender, EventArgs e)
        {
            if (this.text.Text == base.Text)
                return;

            this.text = new Element() { Text = base.Text, Value = base.Text };
        }

        private Element text;

        public new Element Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                base.Text = value.Text;
            }
        }
    }
}