using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Drawing;

namespace Rpx
{
    internal class Helper
    {
        #region String Helpers

        public static string MakeNonNullAndEscape(string str)
        {
            if (str == null)
                return "";

            return str.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\"", "\\\"");
        }

        public static string MakeNonNull(string str)
        {
            if (str == null)
                return "";

            return str;
        }

        public static bool IsNullOrEmpty(string str)
        {
            if (str == null)
                return true;

            if (str.Trim().Length == 0)
                return true;

            return false;
        }

        public static bool IsNotNullOrEmpty(string str)
        {
            if (str == null)
                return false;

            if (str.Trim().Length == 0)
                return false;

            return true;
        }
        #endregion
    }
}
