using System.Linq;
using Kohl.Framework.Logging;

namespace Terminals.Updates
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
	using System.ServiceModel.Syndication;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
	using System.Xml;
    using ICSharpCode.SharpZipLib.Zip;
    using Kohl.Framework.Info;
    using Terminals.CommandLine;
    using Terminals.Configuration.Files.Main.Settings;
    using Terminals.Configuration.Serialization;

    public static class UpdateManager
    {
        /// <summary>
        ///     Check for available application updates
        /// </summary>
        public static void CheckForUpdates(CommandLineArgs commandLine)
        {
            ThreadPool.QueueUserWorkItem(PerformCheck, commandLine);
        }

        private static string GetMD5HashFromFile(string file_name)
        {
            String tmpFile = file_name + ".tmp";
            File.Copy(file_name, tmpFile, true);
            Byte[] retVal = null;

            using (FileStream file = new FileStream(tmpFile, FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                retVal = md5.ComputeHash(file);
                file.Close();
            }

            if (retVal != null)
            {
                StringBuilder s = new StringBuilder();
                foreach (Byte b in retVal)
                {
                    s.Append(b.ToString("x2").ToLower());
                }

                return s.ToString();
            }

            return null;
        }

        /// <summary>
        ///     check codeplex's rss feed to see if we have a new release available.
        /// </summary>
        private static void CheckForNewRelease(CommandLineArgs commandLineArgs)
        {
            Boolean checkForUpdate = true;
            const string releaseFile = "LastUpdateCheck.txt";
            if (File.Exists(releaseFile))
            {

                string text = null;
                using (FileStream fs = new FileStream(releaseFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite & FileShare.Delete))
                {
                    using (StreamReader stream = new StreamReader(fs))
                    {
                        text = stream.ReadToEnd().Trim();;
                    }
                }

                if (text != String.Empty)
                {
                    DateTime lastUpdate = DateTime.MinValue;
                    if (DateTime.TryParse(text, out lastUpdate))
                    {
                        //dont run the update if the file is today or later..if we have checked today or not
                        if (lastUpdate.Date >= DateTime.Now.Date)
                        {
                        	Log.Debug("No need to check for a new Terminals release.");
                            checkForUpdate = false;
                        }
                    }
                }
            }

            if (checkForUpdate)
            {
            	Log.Debug("Start to check for a new Terminals release.");
            	
            	try
            	{
	            	// https://github.com/OliverKohlDSc/Terminals/commits/master.atom
	            	// https://github.com/OliverKohlDSc/Terminals/releases.atom
	            	
	            	Log.Debug("Connecting to the internet to github.com to check if a new Terminals version is available.");
	            	
	            	XmlReader responseReader = XmlReader.Create("https://github.com/OliverKohlDSc/Terminals/releases.atom");
	            	
	            	Log.Debug("Loading syndication feed.");
	            	
					SyndicationFeed feed = SyndicationFeed.Load(responseReader);
					
					// Only the first element would be enough normally - but to be sure
					// check all of the releases
					foreach (SyndicationItem item in feed.Items)
					{
					    string title = item.Title.Text;
					    DateTimeOffset date = item.LastUpdatedTime;
					
					    Log.Debug("Release '" + title + "' has been discovered.");
					    
	                    //check the date the item was published.  
	                    //Is it after the currently executing application BuildDate? if so, then its probably a new build!
	                    if (date > AssemblyInfo.BuildDate)
	                    {
	                    	Log.Debug("Release '" + title + "' is newer than the currently installed Terminals version '" + AssemblyInfo.TitleVersion + "'.");
	                    	
	                        MainForm.ReleaseAvailable = true;
	                        MainForm.ReleaseDescription = title;
	                        string version = title.Replace("Terminals", "").Trim();
	                        // https://github.com/OliverKohlDSc/Terminals/releases/tag/4.8.0.0
	                        //Settings.UpdateSource = "https://github.com" + item.Links[0].Uri.ToString();
	                        // https://github.com/OliverKohlDSc/Terminals/releases/download/4.8.0.0/Terminals_4.8.0.0.zip
	                        Settings.UpdateSource = "https://github.com/OliverKohlDSc/Terminals/releases/download/"+version+"/Terminals_" + version + ".zip";
	                        commandLineArgs.AutomaticallyUpdate = true;
	                        break;
	                    }
					}
            	}
            	catch (Exception ex)
            	{
            		Log.Warn("Unable to check for a new Terminals version on https://github.com/OliverKohlDSc/Terminals", ex);
            	}
            	
            	File.WriteAllText(releaseFile, DateTime.Now.ToString());
            }
        }

        private static void PerformCheck(object state)
        {
        	CommandLineArgs commandLineArgs = (state as CommandLineArgs);
        	
            try
            {
                CheckForNewRelease(commandLineArgs);
            }
            catch
            {
                Log.Warn("Failed to check for a new Terminals release.");
                return;
            }

            try
            {
                if (commandLineArgs.AutomaticallyUpdate)
                {
                    {
                        String url = Settings.UpdateSource;
                        //String contents = Download(url);

                        //if (!String.IsNullOrEmpty(contents))
                        {
                            //TerminalsUpdates updates =
                            //    (TerminalsUpdates) Serialize.DeSerializeXml(contents, typeof (TerminalsUpdates));
                            //if (updates != null)
                            {
                                //String currentMD5 = GetMD5HashFromFile(AssemblyInfo.Title + ".exe");
                                //f (currentMD5 != null)
                                {
                                    //get the latest build
                                    //DataRow row = updates.Tables[0].Rows[0];
                                    //String md5 = (row["MD5"] as string);
                                    //if (!md5.Equals(currentMD5))
                                    {
                                    	String name = url.Substring(url.LastIndexOf("/")+1, url.Length - url.LastIndexOf("/")-1);
                                    	String version = name.Replace("Terminals", "").Trim().ToLower().Replace(".zip", "").Replace("_", "");
                                    	                                    	
                                        if (!Directory.Exists(AssemblyInfo.UpgradeDirectory))
                                        	Directory.CreateDirectory(AssemblyInfo.UpgradeDirectory);

                                        String finalFolder = Path.Combine(AssemblyInfo.UpgradeDirectory, version);
                                        if (!Directory.Exists(finalFolder))
                                            Directory.CreateDirectory(finalFolder);

                                        String filename = String.Format("{0}.zip", version);
                                        filename = Path.Combine(AssemblyInfo.UpgradeDirectory, filename);
                                        Boolean downloaded = true;

                                        if (!File.Exists(filename))
                                            downloaded = DownloadNewBuild(url, filename);

                                        if (downloaded && File.Exists(filename))
                                        {
                                            //ICSharpCode.SharpZipLib.Zip.FastZipEvents evts = new ICSharpCode.SharpZipLib.Zip.FastZipEvents();
                                            FastZip fz = new FastZip();
                                            fz.ExtractZip(filename, finalFolder, null);

                                            if (
                                                MessageBox.Show(
                                                    "A new build is available, would you like to install it now",
                                                    "New Build", MessageBoxButtons.OKCancel) == DialogResult.OK)
                                            {
                                                DirectoryInfo parent = FindFileInFolder(new DirectoryInfo(finalFolder), "Terminals.exe");
                                                if (parent == null)
                                                    return;

                                                finalFolder = parent.FullName;

                                                File.Copy(Settings.CONFIG_FILE_NAME,
                                                          Path.Combine(finalFolder, Settings.CONFIG_FILE_NAME), true);
                                                File.Copy(AssemblyInfo.Title + ".log4net.config",
                                                          Path.Combine(finalFolder,
                                                                       AssemblyInfo.Title + ".log4net.config"), true);

                                                String temp =
                                                    Environment.GetFolderPath(
                                                        Environment.SpecialFolder.LocalApplicationData);
                                                String updaterExe = Path.Combine(temp, "TerminalsUpdater.exe");
                                                if (
                                                    File.Exists(Path.Combine(finalFolder, "TerminalsUpdater.exe")))
                                                {
                                                    File.Copy(
                                                        Path.Combine(finalFolder, "TerminalsUpdater.exe"),
                                                        updaterExe, true);
                                                }

                                                //updaterExe = @"C:\Source\Terminals\Terminals\bin\Debug\";

                                                if (File.Exists(updaterExe))
                                                {
                                                    //String args = "\"" + finalFolder + "\" \"" + Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\"";
                                                    String args = String.Format("\"{0}\" \"{1}\"", finalFolder, AssemblyInfo.Location);
                                                    Process.Start(updaterExe, args);
                                                    Application.Exit();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Log.Error("Failed during update.", exc);
            }
        }

        private static DirectoryInfo FindFileInFolder(DirectoryInfo Path, String Filename)
        {
            if (Path.GetFiles(Filename).Length > 0)
                return Path;

            return Path.GetDirectories().Select(dir => FindFileInFolder(dir, Filename)).FirstOrDefault(found => found != null);
        }

        private static bool DownloadNewBuild(String Url, String Filename)
        {
            return Web.SaveHTTPToFile(Url, Filename);
        }

        private static string Download(String Url)
        {
            return Web.HTTPAsString(Url);
        }
    }
}