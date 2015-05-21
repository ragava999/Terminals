
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Connection.Manager;
using Terminals.Connection.Panels.FavoritePanels;
using Terminals.Connections;
using Kohl.Framework.Drawing;
using System.Drawing;
using System.Windows.Forms;

namespace Terminals.Panels.FavoritePanels
{
    public partial class GenericFavoritePanel : FavoritePanel
    {
        public override string[] ProtocolName
        {
            get
            {
                return new[] { typeof(GenericConnection).GetProtocolName() };
            }
        }

        public GenericFavoritePanel()
        {
            this.InitializeComponent();
        }

        public override void FillControls(FavoriteConfigurationElement favorite)
        {
            this.txtGenericArguments.Text = favorite.GenericArguments;
            this.txtGenericExecutablePath.Text = favorite.GenericProgramPath;
            this.txtGenericWorkingDirectory.Text = favorite.GenericWorkingDirectory;

            //SetIcon();
        }

        public override void FillFavorite(FavoriteConfigurationElement favorite)
        {
            favorite.GenericArguments = this.txtGenericArguments.Text;
            favorite.GenericProgramPath = this.txtGenericExecutablePath.Text;
            favorite.GenericWorkingDirectory = this.txtGenericWorkingDirectory.Text;

            //SetIcon();
        }

        private void SetIcon(bool overwrite = false)
        {
            if (string.IsNullOrEmpty(this.txtGenericExecutablePath.Text))
                return;

            Icon icon = IconHandler.IconFromFile(this.txtGenericExecutablePath.Text, IconSize.Large, 0);

            if (icon == null)
                return;

            Bitmap bitmap = icon.ToBitmap();

            if (bitmap == null)
                return;

            if (this.ParentForm == null || this.ParentForm.GetType() != typeof(FavoriteEditor))
                return;

            ((FavoriteEditor)this.ParentForm).SetToolBarIcon(bitmap, System.IO.Path.GetFileNameWithoutExtension(this.txtGenericExecutablePath.Text), overwrite);

            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }

            if (icon != null)
            {
                icon.Dispose();
                icon = null;
            }
        }

        private void btnGetIconFromExecutable_Click(object sender, System.EventArgs e)
        {
            SetIcon(true);
        }
    }
}