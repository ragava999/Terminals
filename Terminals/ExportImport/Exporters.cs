using System.Collections.Generic;
using System.Text;
using Terminals.ExportImport.Export;
using Terminals.ExportImport.Import;

namespace Terminals.ExportImport
{
    public class Exporters : Integration<IExport>
    {
        protected override void LoadProviders()
        {
            if (this.providers == null)
            {
                this.providers = new Dictionary<string, IExport>();
                this.providers.Add(ImportTerminals.TERMINALS_FILEEXTENSION, new ExportTerminals());
                this.providers.Add(ImportRDP.FILE_EXTENSION, new ExportRdp());
                this.providers.Add(GetExtraAndroidProviderKey(), new ExportExtraLogicAndroidRd());
            }
        }

        /// <summary>
        ///     Replaces XML file extension duplicity as key in providers.
        /// </summary>
        /// <returns> </returns>
        private static string GetExtraAndroidProviderKey()
        {
            return ExportExtraLogicAndroidRd.EXTENSION + ExportExtraLogicAndroidRd.EXTENSION;
        }

        public string GetProvidersDialogFilter()
        {
            this.LoadProviders();

            StringBuilder filters = new StringBuilder();
            foreach (KeyValuePair<string, IExport> exporter in this.providers)
            {
                this.AddProviderFilter(filters, exporter.Value);
            }

            return filters.ToString();
        }

        public void Export(ExportOptions options)
        {
            IExport exporter = this.FindProvider(options.FileName);

            if (options.ProviderFilter.Contains(ExportExtraLogicAndroidRd.PROVIDER_NAME))
                exporter = this.providers[GetExtraAndroidProviderKey()];

            if (exporter != null)
                exporter.Export(options);
        }
    }
}