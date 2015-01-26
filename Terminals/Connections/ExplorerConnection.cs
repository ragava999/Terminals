namespace Terminals.Connections
{
    // .NET namespaces
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Kohl.Framework.Localization;
    using Kohl.Framework.Logging;
    using Kohl.Framework.Security;

    // Terminals namespaces
    using Configuration.Files.Main.Favorites;
    using Properties;

    /// <summary>
    ///     Description of ExplorerConnection.
    /// </summary>
    public class ExplorerConnection : GenericConnection
    {
        public override bool IsPortRequired
        {
            get
            {
                return false;
            }
        }

        protected override Image[] images
        {
            get { return new Image[] { Resources.Explorer }; }
        }

        protected override string RunAsUser
        {
            get
            {
                return this.Favorite.Credential.UserName;
            }
        }

        protected override string RunAsDomain
        {
            get
            {
                return this.Favorite.Credential.DomainName;
            }
        }

        protected override string RunAsPassword
        {
            get
            {
                return this.Favorite.Credential.Password;
            }
        }

        protected override string WorkingDirectory
        {
            get { return Kohl.Framework.Info.AssemblyInfo.Directory; }
        }

        protected override string ProgramPath
        {
            get { return "Kohl.Explorer.exe"; }
        }

        protected override string Arguments
        {
            get
            {
                string arguments = string.Empty;

                if (!string.IsNullOrEmpty(this.Favorite.ExplorerDirectory))
                    arguments  = this.Favorite.ExplorerStyle.ToString() + " \"" + this.Favorite.ExplorerDirectory + "\"";
                
                if (!string.IsNullOrEmpty(this.Favorite.ExplorerDirectoryDual))
                    arguments = this.Favorite.ExplorerStyle.ToString() + " \"" + this.Favorite.ExplorerDirectory + "\"" + " \"" + this.Favorite.ExplorerDirectoryDual + "\"";
                
                if (!string.IsNullOrEmpty(this.Favorite.ExplorerDirectoryTripple))
                    arguments = this.Favorite.ExplorerStyle.ToString() + " \"" + this.Favorite.ExplorerDirectory + "\"" + " \"" + this.Favorite.ExplorerDirectoryDual + "\"" + " \"" + this.Favorite.ExplorerDirectoryTripple + "\"";

                if (!string.IsNullOrEmpty(this.Favorite.ExplorerDirectoryQuad))
                    arguments = this.Favorite.ExplorerStyle.ToString() + " \"" + this.Favorite.ExplorerDirectory + "\"" + " \"" + this.Favorite.ExplorerDirectoryDual + "\"" + " \"" + this.Favorite.ExplorerDirectoryTripple + "\"" + " \"" + this.Favorite.ExplorerDirectoryQuad + "\"";

                return arguments;
            }
        }
    }
}