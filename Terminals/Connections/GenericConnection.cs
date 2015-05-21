using Terminals.Connection.Manager;

namespace Terminals.Connections
{
    // .NET namespaces
    using System.Drawing;

    // Terminals namespaces
    using Connection;
    using Properties;

    public class GenericConnection : ExternalConnection
    {
        protected override Image[] images
        {
            get { return new Image[] {Resources.GenericApplication2}; }
        }

        public override bool IsPortRequired
        {
            get
            {
                return false;
            }
        }

        protected override SleepType SleepMethod
        {
            get { return SleepType.WaitForProcessMainWindowIdle; }
        }

        protected override bool UseShellExecute
        {
            get { return false; }
        }

        protected override string WorkingDirectory
        {
            get { return this.Favorite.GenericWorkingDirectory; }
        }

        protected override string ProgramPath
        {
            get { return this.Favorite.GenericProgramPath; }
        }

        protected override string Arguments
        {
            get
            {
                string arguments = this.Favorite.GenericArguments;

                return ConnectionManager.ParseValue(this.Favorite.Credential, arguments);
            }
        }

        protected override string RunAsUser
        {
            get
            {
                if (this.Favorite.GenericArguments != this.Arguments)
                    return null;

                return this.Favorite.Credential.UserName;
            }
        }

        protected override string RunAsDomain
        {
            get
            {
                if (this.Favorite.GenericArguments != this.Arguments)
                    return null;

                return this.Favorite.Credential.DomainName;
            }
        }

        protected override string RunAsPassword
        {
            get
            {
                if (this.Favorite.GenericArguments != this.Arguments)
                    return null;

                return this.Favorite.Credential.Password;
            }
        }
    }
}