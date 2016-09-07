using System.ComponentModel;
using System.Threading;
using Kohl.Framework.Info;
using Terminals.CommandLine;

namespace Terminals.Network.Services
{
    public class SingleInstanceApplication
    {
        #region Thread safe singleton

        private SingleInstanceApplication()
        {
            this.instanceLock = new Mutex(true, this.INSTANCELOCK_NAME, out this.firstInstance);
            // we are sure, that we own the previous one, so loct the applicatin startup immediately
            this.startupLock = new Mutex(this.firstInstance, this.STARTUPLOCK_NAME);
        }

        public static SingleInstanceApplication Instance
        {
            get { return Nested.Instance; }
        }

        private static class Nested
        {
            private static SingleInstanceApplication instance;

            public static SingleInstanceApplication Instance
            {
                get
                {
                    // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
                    // designer for this class.
                    if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
                        if (instance == null)
                            instance = new SingleInstanceApplication();

                    return instance;
                }
            }
        }

        #endregion

        private readonly string INSTANCELOCK_NAME = AssemblyInfo.Title + ".SingleInstance";
        private readonly string STARTUPLOCK_NAME = AssemblyInfo.Title + ".CommandServerStartUp";
        private readonly bool firstInstance;

        /// <summary>
        ///     This is a machine wide application instances counter.
        ///     when the last application is shutdown the mutex will be released automatically
        /// </summary>
        private readonly Mutex instanceLock;

        /// <summary>
        ///     Prevent asking for window notifications, when the server is in startup or shutdown procedure
        /// </summary>
        private readonly Mutex startupLock;

        private CommandLineServer server;

        public void Start(MainForm mainForm)
        {
            if (!this.firstInstance)
                return;

            this.server = new CommandLineServer(mainForm);
            this.server.Open();
            // startupLock obtained in constructor, the server is now available to notifications
            this.startupLock.ReleaseMutex();
        }

        /// <summary>
        ///     close the server as soon as, when closing the main form,
        ///     because othewise the form can be already dead and it cant process notification requests
        /// </summary>
        public void Close()
        {
            if (!this.firstInstance)
                return;
            try
            {
                this.startupLock.WaitOne();
                this.server.Close();
            }
            finally
            {
                this.startupLock.Close();
                this.instanceLock.Close();
            }
        }

        /// <summary>
        ///     If other instance is runing, then forwards the command line to it and returns true;
        ///     otherwise returns false.
        /// </summary>
        public bool NotifyExisting(CommandLineArgs args)
        {
            if (this.firstInstance)
                return false;

            return this.ForwardCommand(args);
        }

        private bool ForwardCommand(CommandLineArgs args)
        {
            try
            {
                // wait until the main instance startup/shutdown ends
                this.startupLock.WaitOne();
                ICommandLineService client = CommandLineServer.CreateClient();
                client.ForwardCommand(args);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                this.startupLock.ReleaseMutex();
            }
        }
    }
}