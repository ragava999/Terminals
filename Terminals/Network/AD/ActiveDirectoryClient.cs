using System;
using System.DirectoryServices;
using System.Threading;
using Kohl.Framework.Logging;

namespace Terminals.Network.AD
{
    public class ActiveDirectoryClient
    {
        private readonly object runLock = new object();
        private Boolean cancelationPending;

        private Boolean isRunning;

        public Boolean IsRunning
        {
            get
            {
                lock (this.runLock)
                {
                    return this.isRunning;
                }
            }
            private set
            {
                lock (this.runLock)
                {
                    this.isRunning = value;
                }
            }
        }

        private Boolean CancelationPending
        {
            get
            {
                lock (this.runLock)
                {
                    return this.cancelationPending;
                }
            }
        }

        public event ListComputersDoneDelegate ListComputersDone;
        public event ComputerFoundDelegate ComputerFound;

        public void FindComputers(string domain)
        {
            // nothing is running
            if (this.IsRunning)
            {
                this.IsRunning = true;
                ThreadPool.QueueUserWorkItem(this.StartScan, domain);
            }
        }

        public void Stop()
        {
            lock (this.runLock)
            {
                if (this.isRunning)
                {
                    this.cancelationPending = true;
                }
            }
        }

        private void StartScan(object domain)
        {
            try
            {
                this.SearchComputers(domain.ToString());
                this.FireListComputersDone(true);
            }
            catch (Exception exc)
            {
                this.FireListComputersDone(false);
                Log.Error(string.Format("Network.AD.ActiveDirectoryClient.StartScan_Error", domain), exc);
            }
            finally
            {
                this.IsRunning = false;
            }
        }

        private void SearchComputers(string domain)
        {
            using (DirectoryEntry entry = new DirectoryEntry(string.Format("LDAP://{0}", domain)))
            {
                using (DirectorySearcher mySearcher = new DirectorySearcher(entry))
                {
                    mySearcher.Asynchronous = true;
                    mySearcher.Filter = ("(objectClass=computer)");
                    foreach (SearchResult result in mySearcher.FindAll())
                    {
                        if (this.CancelationPending)
                            return;
                        DirectoryEntry computer = result.GetDirectoryEntry();
                        ActiveDirectoryComputer comp = ActiveDirectoryComputer.FromDirectoryEntry(domain, computer);
                        this.FireComputerFound(comp);
                    }
                }
            }
        }

        private void FireListComputersDone(Boolean success)
        {
            if (this.ListComputersDone != null)
                this.ListComputersDone(success);
        }

        private void FireComputerFound(ActiveDirectoryComputer computer)
        {
            if (this.ComputerFound != null)
                this.ComputerFound(computer);
        }
    }
}