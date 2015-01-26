using System;
using System.Windows.Forms;
using Kohl.Framework.Converters;
using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.Forms.Controls
{
    public partial class FontControl : UserControl
    {
        public FontControl()
        {
            InitializeComponent();
        }
        
        public new string ForeColor
        {
            get { return this.TextColorTextBox.Text; }
            set { this.TextColorTextBox.Text = value; }
        }

        public new string BackColor
        {
            get { return this.BackColorTextBox.Text; }
            set { this.BackColorTextBox.Text = value; }
        }

        public new string Font
        {
            get { return this.FontTextbox.Text; }
            set { this.FontTextbox.Text = value; }
        }
        
        private void FontButton_Click(object sender, EventArgs e)
        {
            this.fontDialog1.Font = FontParser.FromString(this.FontTextbox.Text);
            DialogResult result = this.fontDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.FontTextbox.Text = FontParser.ToString(this.fontDialog1.Font);
            }
        }

        private void BackcolorButton_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = FavoriteConfigurationElement.TranslateColor(this.BackColorTextBox.Text);
            DialogResult result = this.colorDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.BackColorTextBox.Text = FavoriteConfigurationElement.GetDisplayColor(this.colorDialog1.Color);
            }
        }

        private void TextColorButton_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = FavoriteConfigurationElement.TranslateColor(this.TextColorTextBox.Text);
            DialogResult result = this.colorDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.TextColorTextBox.Text = FavoriteConfigurationElement.GetDisplayColor(this.colorDialog1.Color);
            }
        }
    }
}
