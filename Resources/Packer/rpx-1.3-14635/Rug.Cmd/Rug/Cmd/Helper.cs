namespace Rug.Cmd
{
    using System;
    using System.Xml;

    public class Helper
    {
        public static void AppendAttributeAndValue(XmlElement element, string name, string value)
        {
            if (IsNotNullOrEmpty(value))
            {
                element.Attributes.Append(element.OwnerDocument.CreateAttribute(name));
                element.Attributes[name].Value = value;
            }
        }

        public static XmlElement AppendChild(XmlElement element, string name)
        {
            if (IsNotNullOrEmpty(name) && (element != null))
            {
                XmlElement newChild = element.OwnerDocument.CreateElement(name);
                element.AppendChild(newChild);
                return newChild;
            }
            return null;
        }

        public static XmlNode FindChild(XmlNode node, string name)
        {
            return node.SelectSingleNode(name);
        }

        public static string GetAttributeValue(XmlNode node, string name, string @default)
        {
            if (node.Attributes[name] != null)
            {
                return node.Attributes[name].Value;
            }
            return @default;
        }

        public static bool HasAttribute(XmlNode node, string name)
        {
            return (node.Attributes[name] != null);
        }

        public static bool IsNotNullOrEmpty(string str)
        {
            if (str == null)
            {
                return false;
            }
            if (str.Trim().Length == 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsNullOrEmpty(string str)
        {
            return ((str == null) || (str.Trim().Length == 0));
        }

        public static string MakeNonNull(string str)
        {
            if (str == null)
            {
                return "";
            }
            return str;
        }

        public static string MakeNonNullAndEscape(string str)
        {
            if (str == null)
            {
                return "";
            }
            return str.Replace("\n", @"\n").Replace("\r", @"\r").Replace("\t", @"\t").Replace("\"", "\\\"");
        }
    }
}

