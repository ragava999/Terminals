namespace Terminals.Connection
{
    using Configuration.Files.Main.Settings;
    using Configuration.Files.Main.Favorites;
    using Kohl.Framework.Logging;
    using Kohl.Framework.Info;
    using Kohl.PInvoke;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    public abstract class Connection : ConnectionBase
    {
        #region Private Fields (1)
        private readonly List<ConnectionImage> list = new List<ConnectionImage>();
        #endregion

        #region Public Properties (5)
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

        public virtual bool SupportsDragAndDropFileCopy
        {
            get
            {
                return false;
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

        #region Private Methods - Drag & Drop File Copy Support (2)
        protected void CopyDragAndDropFilesToServer(object sender, DragEventArgs e)
        {
            if (!SupportsDragAndDropFileCopy)
            {
                Log.Warn("There's no support for drag&drop file copy for this Terminals protocol.");
                return;
            }

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string desktopShare = this.GetDesktopShare();

            if (String.IsNullOrEmpty(desktopShare))
            {
                MessageBox.Show(this, "A desktop share was not defined for this connection.\nPlease define a share in the connection properties window (under the Local Resources tab).", AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                CopyFiles(files, desktopShare);
            }
        }

        private void CopyFiles(string[] sourceFilesToCopy, string destinationShare)
        {
            NativeMethods.NETRESOURCE nr = new NativeMethods.NETRESOURCE();
            nr.dwType = NativeMethods.ResourceType.RESOURCETYPE_DISK;
            nr.lpLocalName = null;
            nr.lpRemoteName = destinationShare;
            nr.lpProvider = null;

            int retCode = NativeMethods.WNetAddConnection2(nr, Favorite.Password, Favorite.Credential.IsSetUserNameAndDomainName ? Favorite.Credential.UserNameWithDomain : Favorite.Credential.UserName, 0);

            if (retCode != 0)
            {
                string errorMessage = "Terminals was unable to copy your file" + (sourceFilesToCopy.Length > 1 ? "s" : "") + " to the server.";
                Log.Error(errorMessage, new Exception("WNetAddConnection2 error return code: " + retCode));
                MessageBox.Show(errorMessage, "File copy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string result = "Your files have been copied to:" + Environment.NewLine;

            foreach (string sourceFileToCopy in sourceFilesToCopy)
            {
                try
                {
                    string destination = Path.Combine(destinationShare, Path.GetFileName(sourceFileToCopy));
                    File.Copy(sourceFileToCopy, destination, true);
                    result += Environment.NewLine + destination;
                }
                catch (Exception ex)
                {
                    Log.Error("There's been a problem copying your drag&drop file '" + sourceFileToCopy + "' to the server", ex);
                }
            }

            Log.Info(result);

            MessageBox.Show(result, "Copy result", MessageBoxButtons.OK, MessageBoxIcon.Information);

            NativeMethods.WNetCancelConnection2(destinationShare, 0, true);
        }
        #endregion
    }
}