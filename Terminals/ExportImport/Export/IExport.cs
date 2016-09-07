namespace Terminals.ExportImport.Export
{
    /// <summary>
    ///     Contract for exporter providers, which save terminals favorites into selected file
    /// </summary>
    public interface IExport : IIntegration
    {
        /// <summary>
        ///     Exports selected favorites into the specified file.
        /// </summary>
        void Export(ExportOptions options);
    }
}