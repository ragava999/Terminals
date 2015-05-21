using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Kohl.Framework.Info;
using Kohl.Framework.Lists;
using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Configuration.Serialization;

namespace Terminals.Configuration.Files.History
{
    public sealed class ConnectionHistory
    {
        /// <summary>
        ///     Gets the file name of stored history values
        /// </summary>
        public const string FILENAME = "History.xml";

        private readonly object _threadLock = new object();

        /// <summary>
        ///     Prevent concurent updates on History file by another program
        /// </summary>
        private readonly Mutex fileLock = new Mutex(false, AssemblyInfo.Title() + ".History");

        private readonly DataFileWatcher fileWatcher;

        private HistoryByFavorite _currentHistory;
        private bool _loadingHistory;

        #region Thread safe Singleton

        private ConnectionHistory()
        {
            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                string fullHistoryFullName = Path.Combine(AssemblyInfo.DirectoryConfigFiles, FILENAME);
                this.fileWatcher = new DataFileWatcher(fullHistoryFullName);
                this.fileWatcher.FileChanged += this.OnFileChanged;
                this.fileWatcher.StartObservation();
            }
        }

        /// <summary>
        ///     Gets the singleton instance of history provider
        /// </summary>
        public static ConnectionHistory Instance
        {
            get { return Nested.Instance; }
        }

        private void OnFileChanged(object sender, EventArgs e)
        {
            SortableList<FavoriteConfigurationElement> oldTodays = this.GetOldTodaysHistory();
            this.LoadHistory(null);
            List<FavoriteConfigurationElement> newTodays = this.MergeWithNewTodays(oldTodays);
            foreach (FavoriteConfigurationElement favorite in newTodays)
            {
                this.FireOnHistoryRecorded(favorite.Name);
            }
        }

        private List<FavoriteConfigurationElement> MergeWithNewTodays(
            SortableList<FavoriteConfigurationElement> oldTodays)
        {
            List<FavoriteConfigurationElement> newTodays = this.GetDateItems(HistoryByFavorite.TODAY);
            if (oldTodays != null)
                newTodays = DataDispatcher.GetMissingFavorites(newTodays, oldTodays);
            return newTodays;
        }

        private SortableList<FavoriteConfigurationElement> GetOldTodaysHistory()
        {
            SortableList<FavoriteConfigurationElement> oldTodays = null;
            if (this._currentHistory != null)
                oldTodays = this.GetDateItems(HistoryByFavorite.TODAY);
            return oldTodays;
        }

        private static class Nested
        {
            private static ConnectionHistory instance;

            public static ConnectionHistory Instance
            {
                get
                {
                    // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
                    // designer for this class.
                    if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
                        if (instance == null)
                            instance = new ConnectionHistory();

                    return instance;
                }
            }
        }

        #endregion

        /// <summary>
        ///     Gets or sets the private field encapsulating the lazy loading
        /// </summary>
        private HistoryByFavorite CurrentHistory
        {
            get
            {
                if (this._currentHistory == null)
                    this.LoadHistory(null);

                if (this._currentHistory == null)
                    this._currentHistory = new HistoryByFavorite();

                return this._currentHistory;
            }
        }

        public event EventHandler OnHistoryLoaded;
        public event HistoryRecorded OnHistoryRecorded;

        /// <summary>
        ///     Because filewatcher is created before the main form in GUI thread.
        ///     This lets to fire the file system watcher events in GUI thread.
        /// </summary>
        public void AssignSynchronizationObject(ISynchronizeInvoke synchronizer)
        {
            this.fileWatcher.AssignSynchronizer(synchronizer);
        }

        public SortableList<FavoriteConfigurationElement> GetDateItems(string historyDateKey)
        {
            SortableList<HistoryItem> historyGroupItems = this.HistoryGroupedByDate[historyDateKey];
            SortableList<FavoriteConfigurationElement> groupFavorites = SelectFavoritesFromHistoryItems(historyGroupItems);

            Log.Debug("Getting history items for " + historyDateKey);

            return Settings.OrderByDefaultSorting(groupFavorites);
        }

        private static SortableList<FavoriteConfigurationElement> SelectFavoritesFromHistoryItems(SortableList<HistoryItem> groupedByDate)
        {
            FavoriteConfigurationElementCollection favorites = Settings.GetFavorites(false);
            SortableList<FavoriteConfigurationElement> selection = new SortableList<FavoriteConfigurationElement>();
            foreach (HistoryItem favoriteTouch in groupedByDate)
            {
                FavoriteConfigurationElement favorite = favorites[favoriteTouch.Name];
                if (favorite != null && !selection.Contains(favorite))
                    selection.Add(favorite);
            }

            return selection;
        }

        private SerializableDictionary<string, SortableList<HistoryItem>> historyGroupedByDate = null;

        private SerializableDictionary<string, SortableList<HistoryItem>> HistoryGroupedByDate
        {
            get
            {
                if (historyGroupedByDate == null)
                    historyGroupedByDate = this.CurrentHistory.GroupByDate();

                Log.Debug("Getting grouped history items.");

                return historyGroupedByDate;
            }
        }

        /// <summary>
        ///     Load or re-load history from HistoryLocation
        /// </summary>
        private void LoadHistory(object threadState)
        {
            if (this._loadingHistory)
                return;

            lock (this._threadLock)
            {
                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    this._loadingHistory = true;
                    this.TryLoadHistory();
                }
                catch (Exception exc)
                {
                    Log.Error("Error Loading History", exc);
                    this.TryToRecoverHistoryFile();
                }
                finally
                {
                    this._loadingHistory = false;
                    Log.Info(string.Format("Load History Duration: {0} ms", sw.ElapsedMilliseconds));
                }
            }

            if (this._currentHistory != null && this.OnHistoryLoaded != null)
                this.OnHistoryLoaded(this, new EventArgs());
        }

        private void TryLoadHistory()
        {
            if (!File.Exists(Path.Combine(AssemblyInfo.DirectoryConfigFiles, FILENAME)))
                this.SaveHistory(); //the file doesnt exist. Lets save it out for the first time
            else
                this.LoadFile();
        }

        private void LoadFile()
        {
            this.fileLock.WaitOne();

            Log.Debug("Deserializing history file.");

            this._currentHistory = Serialize.DeserializeXmlFromDisk(Path.Combine(AssemblyInfo.DirectoryConfigFiles, FILENAME), typeof(HistoryByFavorite)) as HistoryByFavorite;
            
            this.fileLock.ReleaseMutex();
        }

        private void TryToRecoverHistoryFile()
        {
            try
            {
                this.fileLock.WaitOne();
                this.fileWatcher.StopObservation();

                if (File.Exists(Path.Combine(AssemblyInfo.DirectoryConfigFiles, FILENAME)))
                    File.Delete(Path.Combine(AssemblyInfo.DirectoryConfigFiles, FILENAME));

                this._currentHistory = new HistoryByFavorite();
            }
            catch (Exception ex1)
            {
                Log.Error("Try to recover History file failed.", ex1);
            }
            finally
            {
                this.fileLock.ReleaseMutex();
                this.fileWatcher.StartObservation();
            }
        }

        private void SaveHistory()
        {
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                this.fileLock.WaitOne();
                this.fileWatcher.StopObservation();
                Serialize.SerializeXmlToDisk(this.CurrentHistory, Path.Combine(AssemblyInfo.DirectoryConfigFiles, FILENAME));

                sw.Stop();
                Log.Info(string.Format("History saved. Duration:{0} ms", sw.ElapsedMilliseconds));
            }
            catch (Exception exc)
            {
                Log.Error("Error Saving History", exc);
            }
            finally
            {
                this.fileLock.ReleaseMutex();
                this.fileWatcher.StartObservation();
            }
        }

        public void RecordHistoryItem(string favoriteName)
        {
            if (this._currentHistory == null)
                return;

            List<HistoryItem> favoriteHistoryList = this.GetFavoriteHistoryList(favoriteName);
            favoriteHistoryList.Add(new HistoryItem(favoriteName));
            this.SaveHistory();
            this.FireOnHistoryRecorded(favoriteName);
        }

        private void FireOnHistoryRecorded(string favoriteName)
        {
            HistoryRecordedEventArgs args = new HistoryRecordedEventArgs {ConnectionName = favoriteName};
            if (this.OnHistoryRecorded != null)
            {
                this.OnHistoryRecorded(this, args);
            }
        }

        private List<HistoryItem> GetFavoriteHistoryList(string favoriteName)
        {
            if (!this._currentHistory.ContainsKey(favoriteName))
                this._currentHistory.Add(favoriteName, new List<HistoryItem>());

            return this._currentHistory[favoriteName];
        }

        /// <summary>
        ///     Capture the OnHistoryLoaded Event
        /// </summary>
        public void LoadHistoryAsync()
        {
            ThreadPool.QueueUserWorkItem(this.LoadHistory, null);
        }
    }
}