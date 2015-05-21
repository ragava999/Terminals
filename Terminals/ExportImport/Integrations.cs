namespace Terminals.ExportImport
{
    public static class Integrations
    {
        private static readonly Importers importers = new Importers();
        private static readonly Exporters exporters = new Exporters();

        public static Importers Importers
        {
            get { return importers; }
        }

        public static Exporters Exporters
        {
            get { return exporters; }
        }
    }
}