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
using System.IO.Packaging;
using System.IO;
using Rug.Cmd;

namespace Rpx.Packing.PEFile
{
    /// <summary>
    /// 
    /// </summary>
    internal static class PEFileInterrogator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="archivePath"></param>
        /// <param name="uri"></param>
        /// <param name="iconFilePath"></param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static bool TryGetPEFileInfo(string archivePath, string uri, string iconFilePath, out PEFileInfo fileInfo)
        {
            fileInfo = new PEFileInfo();

            fileInfo.Subsystem = SubsystemTypes.Unknown;

            try
            {
                Package package = PackageHelper.GetPackage(archivePath, false, FileAccess.Read);
                {
                    byte[] bytes;

                    Uri pathUri = new Uri(uri, UriKind.Relative);

                    using (Stream stream = package.GetPart(pathUri).GetStream())
                    {
                        bytes = new byte[(int)stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                    }

                    SubsystemTypes SubsystemType;

                    bool succsess = TryGetSubsystemType(bytes, out SubsystemType);

                    if (!succsess)
                        return false;                    

                    fileInfo.Subsystem = SubsystemType;

                    if (Helper.IsNotNullOrEmpty(iconFilePath))
                        succsess = TryExtractIcon(bytes, iconFilePath);

                    if (!succsess)
                    {
                        RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubTextNutral, " - " + Rpx.Strings.Reflection_NoIconFound);

                        return true;
                    }

                    if (File.Exists(iconFilePath))
                    {
                        fileInfo.IconPath = iconFilePath;

                        fileInfo.HasIcon = true;
                    }
                    else
                    {
                        RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubTextNutral, " - " + Rpx.Strings.Reflection_NoIconFound);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                RC.WriteException(0112, Rpx.Strings.Error_0112, ex); 

                return false;
            }
            finally 
            {
                PackageHelper.ReleasePackage(archivePath); 
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="SubsystemType"></param>
        /// <returns></returns>
        private static bool TryGetSubsystemType(byte[] bytes, out SubsystemTypes SubsystemType)
        {
            SubsystemType = SubsystemTypes.Unknown;

            try
            {
                PEHeader header = new PEHeader(bytes);

                SubsystemType = header.SubsystemType;

                return true;
            }
            catch 
            {
                return false;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="IconFilePath"></param>
        /// <returns></returns>
        private static bool TryExtractIcon(byte[] bytes, string IconFilePath)
        {
            try
            {
                PEHeader header = new PEHeader(bytes);
                
                return header.TryExtractIconFromExe(bytes, IconFilePath);
            }
            catch 
            {
                return false;
            }
        }
    }
}
