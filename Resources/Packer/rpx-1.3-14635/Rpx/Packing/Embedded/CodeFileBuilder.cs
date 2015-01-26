/* 
 * RPX 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 * 
 * Copyright (C) 2008 Phill Tew. All rights reserved.
 * 
 */

using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rpx.Packing.Embedded
{
    /// <summary>
    /// 
    /// </summary>
    internal class CodeFileBuilder
    {
        #region Pivate Members
        
        private XmlDocument m_Template;
        
        #endregion

        /// <summary>
        /// Construct a code file builder from a template XML document
        /// </summary>
        /// <param name="template">code template xml</param>
        public CodeFileBuilder(XmlDocument template)
        {
            m_Template = template; 
        }
        
        #region Build Code File

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacements"></param>
        /// <param name="objectReplacements"></param>
        /// <param name="defines"></param>
        /// <returns></returns>
        public string BuildCodeFile(Dictionary<string, string> replacements, Dictionary<string, List<Dictionary<string, string>>> objectReplacements, List<string> defines)
        {
            XmlNode root = m_Template.DocumentElement;

            string final = BuildCodeSection(root, defines, objectReplacements);

            foreach (KeyValuePair<string, string> replacement in replacements)
            {
                final = final.Replace(replacement.Key, replacement.Value);
            }

            return final;
        } 

        #endregion

        #region Build Code Section

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="defines"></param>
        /// <param name="objectReplacements"></param>
        /// <returns></returns>
        private string BuildCodeSection(XmlNode root, List<string> defines, Dictionary<string, List<Dictionary<string, string>>> objectReplacements)
        {
            StringBuilder sb = new StringBuilder();

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.CDATA)
                {
                    sb.AppendLine(node.InnerText.TrimEnd(' ', '\n', '\r', '\t'));
                }
                else if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                else if (node.Name == "code")
                {
                    if (node.Attributes["defines"] != null)
                    {
                        string[] defineParts = node.Attributes["defines"].Value.Split(',');

                        bool failed = false;

                        foreach (string part in defineParts)
                        {
                            string def = part.Trim();

                            if (Helper.IsNullOrEmpty(def))
                                continue;

                            if (def.StartsWith("!"))
                            {
                                def = def.Substring(1);

                                if (defines.Contains(def))
                                {
                                    failed = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (!defines.Contains(def))
                                {
                                    failed = true;
                                    break;
                                }
                            }
                        }

                        if (failed)
                            continue;
                    }

                    if (node.Attributes["foreach"] != null)
                    {
                        string objectCollectionName = node.Attributes["foreach"].Value;

                        if (objectReplacements.ContainsKey(objectCollectionName))
                        {
                            bool UseElseAfterFirst = node.Attributes["else-after-first"] != null && bool.Parse(node.Attributes["else-after-first"].Value);
                            bool first = true;

                            foreach (Dictionary<string, string> @object in objectReplacements[objectCollectionName])
                            {
                                string final = BuildCodeSection(node, defines, objectReplacements).TrimEnd(' ', '\n', '\r', '\t');

                                foreach (KeyValuePair<string, string> replacement in @object)
                                {
                                    final = final.Replace(replacement.Key, replacement.Value);
                                }
                                if (UseElseAfterFirst && !first)
                                    sb.AppendLine("else " + final);
                                else
                                    sb.AppendLine(final);

                                first = false;
                            }
                        }
                    }
                    else
                    {
                        sb.AppendLine(BuildCodeSection(node, defines, objectReplacements).TrimEnd(' ', '\n', '\r', '\t'));
                    }
                }
            }

            return sb.ToString();
        } 

        #endregion
    }
}
