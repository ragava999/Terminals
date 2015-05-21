using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.ExportImport.Import
{
    public class ImportTerminals : IImport
    {
        public const string TERMINALS_FILEEXTENSION = ".xml";

        public static readonly string PROVIDER_NAME = "%TITLE% favorites";

        List<FavoriteConfigurationElement> IImport.ImportFavorites(string Filename)
        {
            return ImportXML(Filename, true);
        }

        public string Name
        {
            get { return PROVIDER_NAME; }
        }

        public string KnownExtension
        {
            get { return TERMINALS_FILEEXTENSION; }
        }

        /// <summary>
        ///     Loads a new collection of favorites from source file.
        ///     The newly created favorites aren't imported into configuration.
        /// </summary>
        private static List<FavoriteConfigurationElement> ImportXML(string file, bool showOnToolbar)
        {
            List<FavoriteConfigurationElement> favorites = ImportFavorites(file);
            return favorites;
        }

        private static List<FavoriteConfigurationElement> ImportFavorites(string file)
        {
            List<FavoriteConfigurationElement> favorites = new List<FavoriteConfigurationElement>();
            FavoriteConfigurationElement favorite = null;
            try
            {
                using (XmlTextReader reader = new XmlTextReader(new StreamReader(file, Encoding.Unicode)))
                {
                    while (reader.Read())
                    {
                        favorite = ReadProperty(reader, favorites, favorite);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("The XML import failed.", ex);
            }

            return favorites;
        }

        private static FavoriteConfigurationElement ReadProperty(XmlTextReader reader,
                                                                 List<FavoriteConfigurationElement> favorites,
                                                                 FavoriteConfigurationElement favorite)
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    switch (reader.Name)
                    {
                        case "favorite":
                            favorite = new FavoriteConfigurationElement();
                            favorites.Add(favorite);
                            break;
                        default:
                            if (favorite == null)
                                break;

                            PropertyInfo property = favorite.GetType().GetProperty(reader.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

                            if (property == null)
                                break;

                            // Check if the property has a setter -> if not break;
                            MethodInfo propertySetter = property.GetSetMethod(true);

                            if (propertySetter == null)
                                break;

                            try
                            {
                                if (property.GetValue(favorite, null) is Boolean)
                                {
                                    property.SetValue(favorite, Convert.ToBoolean(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is Decimal)
                                {
                                    property.SetValue(favorite, Convert.ToDecimal(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is Double)
                                {
                                    property.SetValue(favorite, Convert.ToDouble(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is Byte)
                                {
                                    property.SetValue(favorite, Convert.ToByte(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is Char)
                                {
                                    property.SetValue(favorite, Convert.ToChar(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is DateTime)
                                {
                                    property.SetValue(favorite, Convert.ToDateTime(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is Int16)
                                {
                                    property.SetValue(favorite, Convert.ToInt16(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is Int32)
                                {
                                    property.SetValue(favorite, Convert.ToInt32(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is Int64)
                                {
                                    property.SetValue(favorite, Convert.ToInt64(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is SByte)
                                {
                                    property.SetValue(favorite, Convert.ToSByte(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is Single)
                                {
                                    property.SetValue(favorite, Convert.ToSingle(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is UInt16)
                                {
                                    property.SetValue(favorite, Convert.ToUInt16(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is UInt32)
                                {
                                    property.SetValue(favorite, Convert.ToUInt32(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is UInt64)
                                {
                                    property.SetValue(favorite, Convert.ToUInt64(reader.ReadString()), null);
                                }
                                else if (property.GetValue(favorite, null) is Enum)
                                {
                                    property.SetValue(favorite, Enum.Parse(property.GetValue(favorite, null).GetType(), reader.ReadString()), null);
                                }
                                else
                                    property.SetValue(favorite, reader.ReadString(), null);
                                break;
                            }
                            catch
                            {
                                Log.Warn(string.Format("Unable to set property '{0}'.", reader.Name));
                                break;
                            }
                    }
                    break;
            }

            return favorite;
        }
    }
}