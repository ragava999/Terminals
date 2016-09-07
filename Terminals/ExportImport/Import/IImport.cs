using System.Collections.Generic;
using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.ExportImport.Import
{
    /// <summary>
    ///     Import interface for all remote desktop providers
    /// </summary>
    public interface IImport : IIntegration
    {
        /// <summary>
        ///     Loads all favorites from the imported file
        /// </summary>
        /// <param name="Filename"> Full path and file name of the file to import from </param>
        /// <returns> Null, in incorect cases; otherwise collection with found favorites. </returns>
        List<FavoriteConfigurationElement> ImportFavorites(string Filename);
    }
}