namespace Terminals.Forms.Controls
{
    using System;
    using System.Windows.Forms;

    using Terminals.Configuration.Files.Main.Favorites;

    public partial class ConsolePreferences : UserControl
    {
        public ConsolePreferences()
        {
            this.InitializeComponent();
        }

        public void FillControls(FavoriteConfigurationElement favorite)
        {
            this.fontControl1.BackColor = favorite.ConsoleBackColor;
            this.fontControl1.Font = favorite.ConsoleFont;
            this.CursorColorTextBox.Text = favorite.ConsoleCursorColor;
            this.fontControl1.ForeColor = favorite.ConsoleTextColor;
            this.ColumnsTextBox.Text = favorite.ConsoleCols.ToString();
            this.RowsTextBox.Text = favorite.ConsoleRows.ToString();
        }

        public void FillFavorite(FavoriteConfigurationElement favorite)
        {
            favorite.ConsoleBackColor = this.fontControl1.BackColor;
            favorite.ConsoleFont = this.fontControl1.Font;
            favorite.ConsoleCursorColor = this.CursorColorTextBox.Text;
            favorite.ConsoleTextColor = this.fontControl1.ForeColor;
            favorite.ConsoleCols = Convert.ToInt32(this.ColumnsTextBox.Text);
            favorite.ConsoleRows = Convert.ToInt32(this.RowsTextBox.Text);
        }

        private void CursorColorButton_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = FavoriteConfigurationElement.TranslateColor(this.CursorColorTextBox.Text);
            DialogResult result = this.colorDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.CursorColorTextBox.Text = FavoriteConfigurationElement.GetDisplayColor(this.colorDialog1.Color);
            }
        }
    }
}