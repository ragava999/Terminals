using System.Collections.Generic;
using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.ExportImport.Export
{
    /// <summary>
    ///     Export parameters container
    /// </summary>
    public class ExportOptions
    {
        public string ProviderFilter { get; set; }

        /// <summary>
        ///     Full path and name of the destination file including extension.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     Not null collection of favorites to export.
        /// </summary>
        public List<FavoriteConfigurationElement> Favorites { get; set; }

        /// <summary>
        ///     if set to <c>true</c> includes paswords in not encrypted form into the destination file.
        /// </summary>
        public bool IncludePasswords { get; set; }
    }
}