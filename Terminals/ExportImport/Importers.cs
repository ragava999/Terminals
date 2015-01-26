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

            // work with copy because it is modified
            Dictionary<string, IImport> extraImporters = new Dictionary<string, IImport>(this.providers);

            this.AddTerminalsImporter(extraImporters, stringBuilder);

            foreach (KeyValuePair<string, IImport> importer in extraImporters)
            {
                this.AddProviderFilter(stringBuilder, importer.Value);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Forces terminals importer to be on first place
        /// </summary>
        private void AddTerminalsImporter(Dictionary<string, IImport> extraImporters, StringBuilder stringBuilder)
        {
            if (extraImporters.ContainsKey(ImportTerminals.TERMINALS_FILEEXTENSION))
            {
                IImport terminalsImporter = extraImporters[ImportTerminals.TERMINALS_FILEEXTENSION];
                this.AddProviderFilter(stringBuilder, terminalsImporter);
                extraImporters.Remove(ImportTerminals.TERMINALS_FILEEXTENSION);
            }
        }

        /// <summary>
        ///     Loads a new collection of favorites from source file.
        ///     The newly created favorites aren't imported into configuration.
        /// </summary>
        private List<FavoriteConfigurationElement> ImportFavorites(String Filename)
        {
            IImport importer = this.FindProvider(Filename);

            if (importer == null)
                return new List<FavoriteConfigurationElement>();

            return importer.ImportFavorites(Filename);
        }

        public List<FavoriteConfigurationElement> ImportFavorites(String[] files)
        {
            List<FavoriteConfigurationElement> favorites = new List<FavoriteConfigurationElement>();
            foreach (string file in files)
            {
                favorites.AddRange(ImportFavorites(file));
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
            }
        }
    }
}