namespace Terminals.Configuration.Files.Main.Tags
{
    /// <summary>
    ///     Informs about changes in Tags collection.
    /// </summary>
    /// <param name="args"> Not null container reporting removed and added Tags </param>
    public delegate void TagsChangedEventHandler(TagsChangedArgs args);
}