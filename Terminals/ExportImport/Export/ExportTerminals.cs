using System;
using System.Reflection;
using System.Text;
using System.Xml;

using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.ExportImport.Import;

namespace Terminals.ExportImport.Export
{
    /// <summary>
    ///     This is the Terminals native exporter, which exports favorites into its own xml file
    /// </summary>
    public class ExportTerminals : IExport
    {
        public void Export(ExportOptions options)
        {
            try
            {
                using (XmlTextWriter w = new XmlTextWriter(options.FileName, Encoding.Unicode))
                {
                    w.WriteStartDocument();
                    w.WriteStartElement("favorites");
                    foreach (FavoriteConfigurationElement favorite in options.Favorites)
                    {
                        WriteFavorite(w, options.IncludePasswords, favorite);
                    }
                    w.WriteEndElement();
                    w.WriteEndDocument();
                    w.Flush();
                    w.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Export  to XML failed.", ex);
            }
        }

        string IIntegration.Name
        {
            get { return ImportTerminals.PROVIDER_NAME; }
        }

        string IIntegration.KnownExtension
        {
            get { return ImportTerminals.TERMINALS_FILEEXTENSION; }
        }

        private static void WriteFavorite(XmlTextWriter w, bool includePassword, FavoriteConfigurationElement favorite)
        {
            if (favorite == null)
                return;

            if (w.WriteState == WriteState.Closed || w.WriteState == WriteState.Error)
                return;

            w.WriteStartElement("favorite");

            PropertyInfo[] infos = favorite.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty);

            foreach (PropertyInfo info in infos)
            {
                if (info.Name == "UserName" || info.Name == "Password" || info.Name == "DomainName" || info.Name == "EncryptedPassword" ||
                    info.Name == "TsgwUsername" || info.Name == "TsgwPassword" || info.Name == "TsgwDomain" || info.Name == "TsgwEncryptedPassword")
                    if (!includePassword)
                        continue;
                
                if (info.Name == "HtmlFormFields")
                {
                    w.WriteElementString("HtmlFormFieldsString", favorite.GetType().GetProperty("HtmlFormFieldsString", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(favorite, null).ToString());
                    continue;
                }

                // Has already been exported as favorite.Tag
                // favorite.TagList is only for a human readable
                // list of strings.
                // The same applies to "RedirectedDrives" which is handled by
                // "redirectDrives"
                if (info.Name == "TagList" || info.Name == "RedirectedDrives")
                {
                    continue;
                }

                object value = info.GetValue(favorite, null);

                // if a value is null than the tostring wouldn't be callable. ->
                // therefore set it to string.Empty.
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                    value = string.Empty;

                w.WriteElementString(info.Name, value.ToString());
            }
            
            w.WriteEndElement();
        }
    }
}