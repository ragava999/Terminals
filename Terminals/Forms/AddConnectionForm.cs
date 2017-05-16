using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Forms
{
    public partial class AddConnectionForm : Form
    {
        public AddConnectionForm(bool saveInDB)
        {
            this.InitializeComponent();

            var favorites = Settings.GetFavorites(saveInDB);
            foreach (FavoriteConfigurationElement favorite in favorites)
            {
                var checkBox = new CheckBox()
                {
                    Text                    = favorite.Name,
                    UseVisualStyleBackColor = true,
                    AutoSize                = true
                };
                checkBox.CheckedChanged += SetButtonOkState;

                pnlControls.Controls.Add(checkBox);
            }
        }

        IEnumerable<CheckBox> SelectedCheckBoxes
        {
            get { return pnlControls.Controls.OfType<CheckBox>().Where(x => x.Checked); }
        }

        public string[] Connections { get; private set; }

        private void SetButtonOkState(object sender = null, EventArgs e = null)
        {
            this.btnOk.Enabled = SelectedCheckBoxes.Any();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Connections = SelectedCheckBoxes.Select(x => x.Text).ToArray();
        }
    }
}