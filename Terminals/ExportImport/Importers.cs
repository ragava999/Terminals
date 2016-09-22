using System;
using System.Collections.Generic;
using System.Text;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.ExportImport.Import;

namespace Terminals.ExportImport
{
    public class Importers : Integration<IImport>
    {
        public string GetProvidersDialogFilter()
        {
            this.LoadProviders();


            StringBuilder stringBuilder = new StringBuilder();

            foreach (KeyValuePair<string, IImport> importer in this.providers)
            {
                this.AddProviderFilter(stringBuilder, importer.Value);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Loads a new collection of favorites from source file.
        ///     The newly created favorites aren't imported into configuration.
        /// </summary>
        private List<FavoriteConfigurationElement> ImportFavorites(String Filename, int index)
        {
            IImport importer = null;

            // if no index has been selected -> choose our default i.e. the TerminalsImporter
            if (index < 1)
                index = 1;

            try
            {
                importer = this.FindProvider(Filename, index);
            }
            catch (Exception ex)
            {
                Kohl.Framework.Logging.Log.Error("Unable to find favorites importer.", ex);
            }

            if (importer == null)
                return new List<FavoriteConfigurationElement>();

            return importer.ImportFavorites(Filename);
        }

        public List<FavoriteConfigurationElement> ImportFavorites(String[] files, int index)
        {
            List<FavoriteConfigurationElement> favorites = new List<FavoriteConfigurationElement>();
            foreach (string file in files)
            {
                favorites.AddRange(ImportFavorites(file, index));
            }
            return favorites;
        }

        protected override void LoadProviders()
        {
            if (this.providers == null)
            {
                this.providers = new Dictionary<string, IImport>();
                this.providers.Add(ImportTerminals.TERMINALS_FILEEXTENSION, new ImportTerminals());
                this.providers.Add(ImportRDP.FILE_EXTENSION, new ImportRDP());
                this.providers.Add(ImportvRD.FILE_EXTENSION, new ImportvRD());
                this.providers.Add(ImportMuRD.FILE_EXTENSION, new ImportMuRD());
                this.providers.Add("CodePlex_Terminals_" + ImportCodePlexTerminals.FILE_EXTENSION, new ImportCodePlexTerminals());
                this.providers.Add("Remote Desktop Manager", new ImportRemoteDesktopManager());
            }
        }
    }
}