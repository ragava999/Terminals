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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.IO.Packaging;
using System.Xml;
using Rug.Cmd;

namespace Rpx.Packing
{
    internal class PackingProcess
    {
        #region Private Members
        
        private long m_TotalInitialSize = 0;
        private long m_CompressedSize = 0;

        #endregion

        #region Public Members

        public bool Recompress = true;
        public string OutputFile;
        
        public bool Protected = false;
        public string Password = null;
        public string SaltValue = null;

        public readonly List<string> Files = new List<string>();

        #endregion

        #region Properties 

        /// <summary>
        /// 
        /// </summary>
        public long TotalInitialSize
        {
            get
            {
                return m_TotalInitialSize;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long CompressedSize
        {
            get
            {
                return m_CompressedSize;
            }
        }

        #endregion 

        #region Constructors 

        public PackingProcess() { }

        public PackingProcess(XmlNode node)
        {
            // Recompress = true;
            // OutputFile;

            // Protected = false;
            // Password = null;
            // SaltValue = null;

            // List<string> Files = new List<string>();
        }

        #endregion

        #region Create Package

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string CreatePackage()
        {
            if (Helper.IsNullOrEmpty(SaltValue))
                SaltValue = Guid.NewGuid().ToString();

            string archivePath = CreateTempPackage();

            long packageSize = new FileInfo(archivePath).Length;

            if (Recompress)
                archivePath = RecompressPackage(archivePath);

            m_CompressedSize = new FileInfo(archivePath).Length;

            ConsoleThemeColor resultColour = ConsoleThemeColor.Text;

            if (m_CompressedSize <= (m_TotalInitialSize / 4) * 3)
                resultColour = ConsoleThemeColor.TextGood;
            else if (m_CompressedSize < m_TotalInitialSize)
                resultColour = ConsoleThemeColor.SubTextNutral;
            else
                resultColour = ConsoleThemeColor.TextBad;

            RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.TitleText, "\n" + Rpx.Strings.Package_SummaryTitle);
            CmdHelper.WriteInfoToConsole(ConsoleVerbosity.Normal, Rpx.Strings.Package_FilesTotalSize, CmdHelper.GetMemStringFromBytes(m_TotalInitialSize, true), RC.Theme[ConsoleThemeColor.Text]);
            CmdHelper.WriteInfoToConsole(ConsoleVerbosity.Normal, Rpx.Strings.Package_CompressedSize, CmdHelper.GetMemStringFromBytes(m_CompressedSize, true), RC.Theme[resultColour]);
            CmdHelper.WriteInfoToConsole(ConsoleVerbosity.Normal, Rpx.Strings.Package_Compression, (100 - (((double)m_CompressedSize / (double)m_TotalInitialSize) * 100.0)).ToString("N2") + "%  ", RC.Theme[resultColour]);                        

            OutputFile = archivePath;

            return archivePath;
        } 

        #endregion

        #region Create Temp Package

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string CreateTempPackage()
        {
            RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.TitleText, "\n" + Rpx.Strings.Compiler_PackingAsm);

            string path = Application.UserAppDataPath + @"\" + Guid.NewGuid() + ".zip";

            FileInfo fileInfo = new FileInfo(path);

            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create(); 

            Package package = PackageHelper.GetPackage(path, true, FileAccess.ReadWrite);

            m_TotalInitialSize = 0;

            bool writeFullPaths = RC.ShouldWrite(ConsoleVerbosity.Debug) || RC.IsBuildMode; 

            foreach (string asm in Files)
            {
                FileInfo info = new FileInfo(asm);

                long size = info.Length;

                m_TotalInitialSize += size;
                string sizeString = CmdHelper.GetMemStringFromBytes(size, true);

                if (writeFullPaths)
                    RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.Text, " " + asm.PadRight((RC.BufferWidth - 6) - sizeString.Length, '.') + ".." + sizeString);
                else
                {
                    string asmStr = asm;

                    if (asmStr.Length > (RC.BufferWidth - 6) - sizeString.Length)
                    {
                        while (asmStr.Length > (RC.BufferWidth - 9) - sizeString.Length && asmStr.IndexOf('\\', 1) > 0)
                        {
                            asmStr = asmStr.Substring(asmStr.IndexOf('\\', 1));
                        }

                        asmStr = ">> " + asmStr; 
                    }

                    //asmStr = ">> " + asmStr.Substring(asmStr.Length - ((RC.BufferWidth - 9) - sizeString.Length));

                    RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.Text, " " + asmStr.PadRight((RC.BufferWidth - 6) - sizeString.Length, '.') + ".." + sizeString);
                }

                PackageHelper.AddFileToPackage(package, PackageHelper.MakeUriSafe(info.Name), asm);
            }

            PackageHelper.ReleasePackage(path);                      

            return path;
        }

        #endregion 

        #region Recompress Package

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string RecompressPackage(string path)
        {
            string path2 = Application.UserAppDataPath + @"\" + Guid.NewGuid() + ".zip";

            RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.TitleText, "\n" + Rpx.Strings.Compiler_Recompressor);

            Recompressor.RecompressPackage(path, path2, true);

            File.Delete(path);

            return path2;
        } 

        #endregion 

        #region Finalise Package

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archivePath"></param>
        public void FinalisePackage(string archivePath)
        {
            if (Protected)
            {
                RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.TitleText, "\n" + Rpx.Strings.Protection_SummaryTitle);

                if (Helper.IsNotNullOrEmpty(Password))
                    Helpers.EncryptHelper.EncryptFile(archivePath, Password, SaltValue, 2);
                else
                    Helpers.DisguiseHelper.DisguiseFile(archivePath);
            }
        }

        #endregion
    }
}
