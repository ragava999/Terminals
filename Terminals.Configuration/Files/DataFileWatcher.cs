using System;
using System.ComponentModel;
using System.IO;
using Kohl.Framework.Logging;

namespace Terminals.Configuration.Files
{
    /// <summary>
    ///     Detects data or configuration file changes done 
    ///     by another application or Terminals instance and reports them.
    ///     Raises events in GUI thread, so no Invoke is required.
    /// </summary>
    public class DataFileWatcher
    {
        private FileSystemWatcher fileWatcher;

        private string fullfileName;

        public DataFileWatcher(string fullFileName)
        {
            this.InitializeFileWatcher();
			this.FullFileName = fullFileName;
        }

        public string FullFileName
        {
            get { return this.fullfileName; }
            set
            {
                this.fullfileName = value;
                this.SetFileToWatch(value);
            }
        }

        public event EventHandler FileChanged;

        /// <summary>
        ///     Because filewatcher is created before the main form,
        ///     the synchronization object has to be assigned later.
        ///     This lets to fire the file system watcher events in GUI thread.
        /// </summary>
        public void AssignSynchronizer(ISynchronizeInvoke synchronizer)
        {
            this.fileWatcher.SynchronizingObject = synchronizer;
        }

        private void InitializeFileWatcher()
        {
            if (this.fileWatcher != null)
                return;

            this.fileWatcher = new FileSystemWatcher();

			if (this.FullFileName != null)
            	this.SetFileToWatch(this.FullFileName);
			
            this.fileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName |
                                            NotifyFilters.CreationTime | NotifyFilters.Size;
            this.fileWatcher.Changed += this.ConfigFileChanged;
        }

        private void SetFileToWatch(string fullFileName)
        {
            this.fileWatcher.Path = Path.GetDirectoryName(fullFileName);
            this.fileWatcher.Filter = Path.GetFileName(fullFileName);
        }

        private void ConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            Log.Debug(this.FullFileName + " file change by another application (or Terminals instance) detected!");
            if (this.FileChanged != null && this.fileWatcher.SynchronizingObject != null)
                this.FileChanged(this.FullFileName, new EventArgs());
        }

        public void StopObservation()
        {
            this.fileWatcher.EnableRaisingEvents = false;
        }

        public void StartObservation()
        {
            this.fileWatcher.EnableRaisingEvents = true;
        }
    }
}