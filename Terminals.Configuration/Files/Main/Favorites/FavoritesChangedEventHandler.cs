namespace Terminals.Configuration.Files.Main.Favorites
{
    /// <summary>
    ///     Informs about changes in favorites collection.
    /// </summary>
    /// <param name="args"> Not null container, reporting Added, removed, and updated favorites </param>
    public delegate void FavoritesChangedEventHandler(FavoritesChangedEventArgs args);
}