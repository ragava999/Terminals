using System;
using System.Linq;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Forms
{
    internal partial class QuickConnect : Form
    {
        private const bool uselightBox = true;

        public QuickConnect()
        {
            this.InitializeComponent();
            this.LoadFavorites();


#if (!LIGHTBOX_FOR_SEARCH)
            {
                this.InputTextbox.Focus();
                this.likeBox1.Visible = false;
            }
#else
            {
                this.likeBox1.Focus();
                this.InputTextbox.Visible = false;
            }
#endif
        }
        public bool SaveInDB { get; set; }

        private void LoadFavorites()
        {
            var favorites = Settings.GetFavorites(SaveInDB).ToList();
            var favoriteNames = (from f in favorites select f.Name).ToArray();

#if (!LIGHTBOX_FOR_SEARCH)
            {
                this.InputTextbox.AutoCompleteCustomSource = new AutoCompleteStringCollection();
                this.InputTextbox.AutoCompleteCustomSource.AddRange(favoriteNames);
            }
#else
            {
                this.likeBox1.DataSource.AddRange(favoriteNames);
                this.likeBox1.Text = null;
            }
#endif
        }

        public string ConnectionName
        {
            get
            {
#if (LIGHTBOX_FOR_SEARCH)
                {
                    return this.likeBox1.Text;
                }
#else
	            {
	                return this.InputTextbox.Text;
	            }
#endif
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void IsExitKey(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }

        }
        private void QuickConnect_KeyUp(object sender, KeyEventArgs e)
        {
            this.IsExitKey(e);
        }

        private void InputTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            this.IsExitKey(e);
        }

        private void ButtonConnect_KeyUp(object sender, KeyEventArgs e)
        {
            this.IsExitKey(e);
        }

        private void ButtonCancel_KeyUp(object sender, KeyEventArgs e)
        {
            this.IsExitKey(e);
        }
    }
}