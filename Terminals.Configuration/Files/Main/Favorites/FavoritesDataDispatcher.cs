namespace Terminals.Configuration.Files.Main.Favorites
{
    public static class FavoritesDataDispatcher
    {
        #region Thread safe singleton with lazy loading
        static FavoritesDataDispatcher()
        {
            DataDispatcher = DataDispatcher.Instance;
        }

        /// <summary>
        ///     Gets the thread safe singleton instance of the persistance layer
        /// </summary>
        public static DataDispatcher Instance
        {
            get { return DataDispatcher; }
        }
        #endregion

        private static readonly DataDispatcher DataDispatcher;
    }
}