namespace Terminals.Connection
{
    // .NET namespaces
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    // Terminals and framework namespaces
    using Configuration.Files.Main.Settings;
    using TabControl;
    using Terminals.Configuration.Files.Main.Favorites;
    using Terminals.Connection.Manager;

    public abstract class Connection : ConnectionBase
    {
        #region Private Fields (1)
        private readonly List<ConnectionImage> list = new List<ConnectionImage>();
        #endregion

        #region Public Properties (4)
        public virtual ushort Port
        {
            get { return 0; }
        }

        public virtual bool IsPortRequired
        {
            get
            {
                return true;
            }
        }

        public ConnectionImage[] Images
        {
            get
            {
                if (this.list.Count < 1)
                    if (this.images != null)
                        foreach (Image img in this.images)
                        {
                            this.list.Add(new ConnectionImage(img) { Name = Guid.NewGuid().ToString() });
                        }

                return this.list.ToArray();
            }
        }

        public ConnectionImage Image
        {
            get
            {
                if (this.Images == null || this.Images.Length < 1 || this.image < 0 || this.image >= this.Images.Length)
                    return null;

                return this.Images[this.image];
            }
        }
        #endregion

        #region Protected Properties (2)
        protected virtual int image
        {
            get { return 0; }
        }

        protected abstract Image[] images { get; }
        #endregion

        #region Proteced Methods (3)
        /// <summary>
        ///     Used to embed connections that don't derive from <see cref="ExternalConnection" />.
        /// </summary>
        /// <param name="ctrl"> The control to be embedded in the tab page. </param>
        protected void Embed(Control ctrl)
        {
            ctrl.InvokeIfNecessary(delegate
            {
                ctrl.Dock = DockStyle.Fill;
                this.Controls.Add(ctrl);
            });

            if (this.TerminalTabPage != null)
                TerminalTabPage.InvokeIfNecessary(delegate { this.TerminalTabPage.Dock = DockStyle.Fill; ctrl.Parent = this.TerminalTabPage; });
        }

        protected string GetDesktopShare()
        {
            string desktopShare = this.Favorite.DesktopShare;

            if (string.IsNullOrEmpty(desktopShare))
                desktopShare = Settings.DefaultDesktopShare;

            if (string.IsNullOrEmpty(desktopShare))
                return null;

            desktopShare = desktopShare.ToUpper().Replace("%SERVER%", this.Favorite.ServerName).Replace("%USER%", this.Favorite.Credential.UserName);

            return desktopShare;
        }

        /// <summary>
        /// Saves the favorites.
        /// </summary>
        protected void SaveFavorites()
        {
            if (this.Favorite == null)
                return;

            Settings.EditFavorite(this.Favorite.Name, this.Favorite);
            Settings.SaveAndFinishDelayedUpdate();
        }
        #endregion
    }
}