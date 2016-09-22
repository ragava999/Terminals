namespace Terminals.Connections
{
    using Properties;
    using System.Drawing;

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

        protected new virtual SleepType SleepMethod
        {
            get { return SleepType.WaitForIdleInputSleep; }
        }

        // Wait for the explorer browser at least two seconds to initialize
        protected override int Sleep
        {
            get { return 2000; }
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
                    arguments = this.Favorite.ExplorerStyle.ToString() + " \"" + this.Favorite.ExplorerDirectory + "\"";

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