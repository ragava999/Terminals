using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using Kohl.Framework.Drawing;
using Kohl.Framework.Logging;

namespace Terminals.Wizard.MMC
{
    public class MMCFile
    {
        private readonly FileInfo mmcFileInfo;
        public string Name;
        public bool Parsed = false;
        public Icon SmallIcon;
        private string rawContents;

        public MMCFile(FileInfo MMCFile)
        {
            this.mmcFileInfo = MMCFile;
            this.Parse();
        }

        public MMCFile(string MMCFile)
        {
            if (File.Exists(MMCFile))
            {
                this.mmcFileInfo = new FileInfo(MMCFile);
                this.Parse();
            }
        }

        private void Parse()
        {
            try
            {
                using (FileStream fs = new FileStream(this.mmcFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite & FileShare.Delete))
                {
                    using (StreamReader stream = new StreamReader(fs))
                    {
                        this.rawContents = stream.ReadToEnd();
                    }
                }

                if (this.rawContents != null && this.rawContents.Trim() != "" && this.rawContents.StartsWith("<?xml"))
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(this.rawContents);
                    XmlNode node = xDoc.SelectSingleNode("/MMC_ConsoleFile/StringTables/StringTable/Strings");
                    foreach (XmlNode cNode in node.ChildNodes)
                    {
                        string name = cNode.InnerText;
                        if (name != "Favorites" && name != "Console Root")
                        {
                            this.Name = name;
                            this.Parsed = true;
                            break;
                        }
                    }


                    //System.Xml.XmlNode binarynode = xDoc.SelectSingleNode("/MMC_ConsoleFile/BinaryStorage");
                    //foreach (System.Xml.XmlNode child in binarynode.ChildNodes)
                    //{
                    //    string childname = child.Attributes["Name"].Value;
                    //    if (childname.ToLower().Contains("small"))
                    //    {
                    //        string image = child.InnerText;
                    //        byte[] buff = System.Convert.FromBase64String(child.InnerText.Trim());
                    //        System.IO.MemoryStream stm = new System.IO.MemoryStream(buff);
                    //        if (stm.Position > 0 && stm.CanSeek) stm.Seek(0, System.IO.SeekOrigin.Begin);
                    //        System.IO.File.WriteAllBytes(@"C:\Users\Administrator\Desktop\foo.ico", buff);
                    //        System.Drawing.Icon ico = new System.Drawing.Icon(stm);

                    //    }
                    //}


                    XmlNode visual = xDoc.SelectSingleNode("/MMC_ConsoleFile/VisualAttributes/Icon");
                    if (visual != null)
                    {
                        string iconFile = visual.Attributes["File"].Value;
                        int index = Convert.ToInt32(visual.Attributes["Index"].Value);
                        Icon[] icons = IconHandler.IconsFromFile(iconFile, IconSize.Small);
                        if (icons != null && icons.Length > 0)
                        {
                            this.SmallIcon = icons.Length > index ? icons[index] : icons[0];
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Log.Error("Error parsing MMC File", exc);
            }
        }
    }
}