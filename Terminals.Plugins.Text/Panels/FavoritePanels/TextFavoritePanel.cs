namespace Terminals.Plugins.Text.Panels.FavoritePanels
{
    // .NET namespaces
    using System;
    using System.Linq;
    using System.Windows.Forms;

    // Terminals namespaces
    using Configuration.Files.Main.Favorites;
    using Connection; 
    using Terminals.Connection.Manager;
    using Terminals.Connection.Panels.FavoritePanels;

    public partial class TextFavoritePanel : FavoritePanel
    {
        #region Private Fields (2)
        private readonly Kohl.TinyMce.TinyMce tinyMce = null;
        private TextConnection textConnection = null;
        #endregion 

        #region Public Properties (1)
        public override string DefaultProtocolName
        {
            get
            {
                return typeof(TextConnection).GetProtocolName();
            }
        }
        #endregion

        #region Constructors (1)
        public TextFavoritePanel()
        {
            InitializeComponent();

            if (tinyMce == null)
            {
                textConnection = new TextConnection();
                tinyMce = textConnection.CreateInstance();
                tinyMce.Dock = DockStyle.Fill;
                tinyMce.BackColor = System.Drawing.Color.Red;
                splitContainer1.Panel2.Controls.Add(tinyMce);
            }
        }
        #endregion

        #region Public Methods (3)
        public override void HandleSelectionChanged(string protocol, bool isEditing)
        {
            if (DefaultProtocolName != protocol.ToUpper() && !ProtocolName.Contains(protocol.ToUpper()))
                return;

            // if we are either dealing with a new or an existing connection and have more than one
            // FavoritePanel per connection defined -> hide this option, because the user won't be
            // able to use it.
            if (DefaultProtocolName == protocol.ToUpper())
            	chkShowTinyMceInConnectionMode.Visible = true;
			else
				chkShowTinyMceInConnectionMode.Checked = false;
            
            // we are dealing with a new connection
            if (!isEditing)
            {
                chkShowTinyMceInEditMode.Checked = tinyMce.IsTextEditable = true;
                tinyMce.Render();
            }
        }

        public override void FillControls(FavoriteConfigurationElement favorite)
        {
            tinyMce.Text = favorite.TinyMceText();
            chkShowTinyMceInConnectionMode.Checked = favorite.ShowTinyMceInConnectionMode();
            tinyMce.IsTextEditable = chkShowTinyMceInEditMode.Checked = favorite.ShowTinyMceInEditMode();
            tinyMce.Render();
        }

        public override void FillFavorite(FavoriteConfigurationElement favorite)
        {
        	tinyMce.Save();
        	string text = tinyMce.Text;
        	text = ImageTools.ParseHtmlLinks(text);
            favorite.TinyMceText(text);
            favorite.ShowTinyMceInConnectionMode(chkShowTinyMceInConnectionMode.Checked);
            favorite.ShowTinyMceInEditMode(chkShowTinyMceInEditMode.Checked);
        }
        #endregion

        #region Protected Methods (1)
        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
			tinyMce.Dispose();
			
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Private Methods (1)
        private void chkShowTinyMceInEditMode_CheckedChanged(object sender, EventArgs e)
        {
            // Avoid unnecessary IO -> only render if the value has changed.
            if (tinyMce.IsTextEditable != chkShowTinyMceInEditMode.Checked)
            {
                tinyMce.IsTextEditable = chkShowTinyMceInEditMode.Checked;
                tinyMce.Render();
            }
        }
        #endregion
    }
}