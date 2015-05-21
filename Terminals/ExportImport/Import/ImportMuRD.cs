using System.Collections.Generic;
using System.IO;

using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.ExportImport.Import
{
    public class ImportMuRD : IImport
    {
        public const string FILE_EXTENSION = ".mrc";

        List<FavoriteConfigurationElement> IImport.ImportFavorites(string Filename)
        {
            List<FavoriteConfigurationElement> coll = new List<FavoriteConfigurationElement>();
            if (File.Exists(Filename))
            {
                FavoriteConfigurationElement fav = null;
                string[] lines = File.ReadAllLines(Filename);
                foreach (string line in lines)
                {
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        if (fav == null)
                        {
                            fav = new FavoriteConfigurationElement {Name = line.Substring(1, line.Length - 2)};
                        }
                        else
                        {
                            coll.Add(fav);
                            fav = new FavoriteConfigurationElement();
                        }
                    }
                    else
                    {
                        if (line == "")
                        {
                            coll.Add(fav);
                            fav = new FavoriteConfigurationElement();
                        }
                        else
                        {
                            string propertyName = line.Substring(0, line.IndexOf("="));
                            string pValue = line.Substring(line.IndexOf("=") + 1);
                            switch (propertyName)
                            {
                                case "ServerName":
                                    fav.ServerName = pValue;
                                    break;
                                case "Port":
                                    int p = 3389;
                                    int.TryParse(pValue, out p);
                                    fav.Port = p;
                                    break;
                                case "Username":
                                    fav.UserName = pValue;
                                    break;
                                case "Domain":
                                    fav.DomainName = pValue;
                                    break;
                                case "Comment":
                                    break;
                                case "Password":
                                    break;
                                case "DesktopWidth":
                                    fav.DesktopSize = DesktopSize.AutoScale;
                                    break;
                                case "DesktopHeight":
                                    fav.DesktopSize = DesktopSize.AutoScale;
                                    break;
                                case "ColorDepth":
                                    switch (pValue)
                                    {
                                        case "8":
                                            fav.Colors = Colors.Bits8;
                                            break;
                                        case "16":
                                            fav.Colors = Colors.Bit16;
                                            break;
                                        case "24":
                                            fav.Colors = Colors.Bits24;
                                            break;
                                        case "32":
                                            fav.Colors = Colors.Bits32;
                                            break;
                                        default:
                                            fav.Colors = Colors.Bit16;
                                            break;
                                    }

                                    break;
                                case "UseSmartSizing":
                                    if (pValue == "1") fav.DesktopSize = DesktopSize.AutoScale;
                                    break;
                                case "FullScreenMonitor":
                                    if (pValue == "1") fav.DesktopSize = DesktopSize.FullScreen;
                                    break;
                                case "ConsoleConnection":
                                    fav.ConnectToConsole = false;
                                    if (pValue == "1") fav.ConnectToConsole = true;
                                    break;
                                case "UseCompression":
                                    break;
                                case "BitmapPersistence":
                                    break;
                                case "DisableWallpaper":
                                    fav.DisableWallPaper = false;
                                    if (pValue == "1") fav.DisableWallPaper = true;
                                    break;
                                case "DisableFullWindowDrag":
                                    break;
                                case "DisableMenuAnimations":
                                    break;
                                case "DisableTheming":
                                    break;
                                case "DisableCursorShadow":
                                    break;
                                case "DisableCursorSettings":
                                    break;
                                case "RedirectSmartCards":
                                    fav.RedirectSmartCards = false;
                                    if (pValue == "1") fav.RedirectSmartCards = true;
                                    break;
                                case "RedirectPorts":
                                    fav.RedirectPorts = false;
                                    if (pValue == "1") fav.RedirectPorts = true;
                                    break;
                                case "RedirectPrinters":
                                    fav.RedirectPrinters = false;
                                    if (pValue == "1") fav.RedirectPrinters = true;
                                    break;
                                case "EnableWindowsKey":
                                    break;
                                case "KeyboardHookMode":
                                    break;
                                case "AudioRedirectionMode":
                                    fav.Sounds = ImportRDP.ConvertToSounds(pValue);
                                    break;
                                case "AlternateShell":
                                    break;
                                case "ShellWorkingDir":
                                    break;
                                case "AuthenticationLevel":
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            return coll;
        }

        public string Name
        {
            get { return "Multiple Remote Desktops Manager"; }
        }

        public string KnownExtension
        {
            get { return FILE_EXTENSION; }
        }
    }
}