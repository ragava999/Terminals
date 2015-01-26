using System;
using Kohl.Framework.Localization;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Connection.Manager;
using Terminals.Connection.Panels.FavoritePanels;
using Terminals.Connections;

namespace Terminals.Panels.FavoritePanels
{
    public partial class ExplorerFavoritePanel : FavoritePanel
    {
        public override string[] ProtocolName
        {
            get
            {
                return new[] { typeof(ExplorerConnection).GetProtocolName() };
            }
        }
        
        public ExplorerFavoritePanel()
        {
            this.InitializeComponent();
            Localization.SetLanguage(this);

            // Load default values
            string[] strings = new ExplorerBrowser.Explorer().KnownFolders;
        	this.cmbDirectory.Items.AddRange(strings);
        	this.cmbDirectory.Sorted = true;
        	this.cmbDirectoryDual.Items.AddRange(strings);
        	this.cmbDirectory.Sorted = true;
			this.cmbExplorerStyle.Items.AddRange(Enum.GetNames(typeof(ExplorerBrowser.ControlStyle)));
			
			this.cmbExplorerStyle.SelectedIndexChanged += delegate
			{
				this.lblDirectoryDual.Visible = this.cmbDirectoryDual.Visible = (this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.DualVertical.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.DualHorizontal.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.TrippleBottom.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.TrippleHorizontal.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.TrippleLeft.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.TrippleRight.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.TrippleTop.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.TrippleVertical.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.Quad.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.QuadHorizontal.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.QuadVertical.ToString().ToLower());
				this.lblDirectoryTripple.Visible = this.cmbDirectoryTripple.Visible = (this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.TrippleBottom.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.TrippleHorizontal.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.TrippleLeft.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.TrippleRight.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.TrippleTop.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.TrippleVertical.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.Quad.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.QuadHorizontal.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.QuadVertical.ToString().ToLower());
				this.lblDirectoryQuad.Visible = this.cmbDirectoryQuad.Visible = (this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.Quad.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.QuadHorizontal.ToString().ToLower() || this.cmbExplorerStyle.Text.ToLower() == ExplorerBrowser.ControlStyle.QuadVertical.ToString().ToLower());
			};
			
			this.cmbExplorerStyle.Sorted = true;
			this.cmbExplorerStyle.Text = ExplorerBrowser.ControlStyle.Single.ToString();
        }

        public override void FillControls(FavoriteConfigurationElement favorite)
        {
            this.cmbExplorerStyle.Text = favorite.ExplorerStyle.ToString();
            this.cmbDirectory.Text = favorite.ExplorerDirectory;
            this.cmbDirectoryDual.Text = favorite.ExplorerDirectoryDual;
            this.cmbDirectoryTripple.Text = favorite.ExplorerDirectoryTripple;
            this.cmbDirectoryQuad.Text = favorite.ExplorerDirectoryQuad;
        }

        public override void FillFavorite(FavoriteConfigurationElement favorite)
        {
            favorite.ExplorerStyle = this.cmbExplorerStyle.Text;
            favorite.ExplorerDirectory = this.cmbDirectory.Text;
            favorite.ExplorerDirectoryDual = this.cmbDirectoryDual.Text;
            favorite.ExplorerDirectoryTripple =this.cmbDirectoryTripple.Text;
            favorite.ExplorerDirectoryQuad = this.cmbDirectoryQuad.Text;
        }
    }
}