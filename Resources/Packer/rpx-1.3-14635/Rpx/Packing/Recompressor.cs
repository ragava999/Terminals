using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Ionic.Zip;
using Rug.Cmd.Gui;
using Rug.Cmd;

namespace Rpx.Packing
{
    /// <summary>
    /// Recompressor applys additional compression to packages but retains the structure. 
    /// This means that the package can still be loaded by System.IO.Packaging.Package but is actaully compressed. 
    /// </summary>
    internal static class Recompressor
    {
        /// <summary>
        /// Recompress a package, that is try to increase the compression on an existing package
        /// </summary>
        /// <param name="fromPath">path the original package file</param>
        /// <param name="toPath">path to the new package file</param>
        public static void RecompressPackage(string fromPath, string toPath)
        {
            RecompressPackage(fromPath, toPath, false);
        }

        /// <summary>
        /// Recompress a package, that is try to increase the compression on an existing package
        /// </summary>
        /// <param name="fromPath">path the original package file</param>
        /// <param name="toPath">path to the new package file</param>
        /// <param name="updateToConsole">should a summary be writen to the console</param>
        public static void RecompressPackage(string fromPath, string toPath, bool updateToConsole)
        {
            List<MemoryStream> streams = new List<MemoryStream>();

            long originalSize = new FileInfo(fromPath).Length; 

            try
            {
                using (ZipFile fromFile = new ZipFile(fromPath))
                {
                    using (ZipFile toFile = new ZipFile())
                    {
                        toFile.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                        
                        ConsoleProgressBar bar = new ConsoleProgressBar();
                        
                        bool showProgress = false;

                        if (updateToConsole)
                        {
                            #region Setup Progress Bar
                            if (RC.CanManipulateBuffer && (int)RC.Verbosity >= (int)ConsoleVerbosity.Normal)
                            {
                                RC.WriteLine(ConsoleThemeColor.TitleText, "");
                                showProgress = true;

                                bar.Location = new Point(0, RC.CursorTop - 1);
                                bar.Value = 0;
                                bar.Maximum = fromFile.Entries.Count;
                                bar.Width = RC.BufferWidth - 3;
                                bar.TextFormat = ConsoleProgressBarTextFormat.Percent;

                                bar.BarDimBackColor = RC.Theme[ConsoleThemeColor.AppBackground];
                                bar.BarDimForeColor = RC.Theme[ConsoleThemeColor.PanelBackground];

                                bar.BarLitBackColor = RC.Theme[ConsoleThemeColor.TitleText];
                                bar.BarLitForeColor = RC.Theme[ConsoleThemeColor.AppBackground];

                                bar.BarDimShade = ConsoleShade.Clear;
                                bar.BarLitShade = ConsoleShade.Dim;

                                bar.BackColor = RC.Theme[ConsoleThemeColor.AppBackground];
                                bar.ForeColor = RC.Theme[ConsoleThemeColor.TitleText];

                                bar.Render();
                            }
                            #endregion
                        }

                        #region Copy Entries

                        foreach (ZipEntry entry in fromFile.Entries)
                        {
                            string path = entry.FileName.Substring(0, entry.FileName.LastIndexOf("/") + 1);

                            if (path.EndsWith("/"))
                                path = path.Substring(0, path.Length - 1);

                            string fileName = entry.FileName.Substring(entry.FileName.LastIndexOf("/") + 1);

                            if (fileName.Trim().Length > 0)
                            {
                                MemoryStream stream = new MemoryStream((int)entry.UncompressedSize);

                                streams.Add(stream);

                                {
                                    entry.Extract(stream); 
                                    stream.Position = 0;
                                    ZipEntry newEntry = toFile.AddEntry(entry.FileName, stream);
                                    
                                    long size = newEntry.CompressedSize;

                                    if (updateToConsole && showProgress)
                                    {
                                        bar.Value++;
                                        bar.Render();
                                    }
                                }
                            }
                        }

                        #endregion

                        toFile.Save(toPath);
                    }
                }

                if (updateToConsole)
                {
                    #region Write the summary to the console

                    if (RC.CanManipulateBuffer && (int)RC.Verbosity >= (int)ConsoleVerbosity.Normal)
                        RC.WriteLine("");

                    if (File.Exists(toPath))
                    {
                        long compFileSize = 0;
                        
                        compFileSize = new FileInfo(toPath).Length;

                        ConsoleThemeColor resultColour = ConsoleThemeColor.Text; 

                        if (compFileSize <= (originalSize / 4) * 3)
                            resultColour = ConsoleThemeColor.TextGood;
                        else if (compFileSize < originalSize)
                            resultColour = ConsoleThemeColor.SubTextNutral;
                        else
                            resultColour = ConsoleThemeColor.TextBad;

                        CmdHelper.WriteInfoToConsole(ConsoleVerbosity.Verbose, Rpx.Strings.Recompressor_Inital, CmdHelper.GetMemStringFromBytes(originalSize, true), RC.Theme[ConsoleThemeColor.Text]);
                        CmdHelper.WriteInfoToConsole(ConsoleVerbosity.Verbose, Rpx.Strings.Recompressor_Recompressed, CmdHelper.GetMemStringFromBytes(compFileSize, true), RC.Theme[resultColour]);
                        CmdHelper.WriteInfoToConsole(ConsoleVerbosity.Verbose, Rpx.Strings.Recompressor_Compression, (100 - (((double)compFileSize / (double)originalSize) * 100.0)).ToString("N2") + "%  ", RC.Theme[ConsoleThemeColor.SubTextGood]);                        
                    }
                    else
                    {
                        CmdHelper.WriteInfoToConsole(Rpx.Strings.Recompressor_Compression, Rpx.Strings.Recompressor_Invalid, RC.Theme[ConsoleThemeColor.TextBad]);
                    }

                    #endregion 
                }
            }
            finally
            {
                foreach (MemoryStream stream in streams)
                    stream.Dispose();
            }            
        }
    }
}
