using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Kohl.Framework.Logging;

namespace TerminalsUpdater
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // XMAS and advent
            if (XmasImminent(DateTime.Now))
            {
                pictureBox1.Image = Properties.Resources.Loading_XMAS;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }

            // Easter
            if (EasterImminent(DateTime.Now))
            {
                pictureBox1.Image = Properties.Resources.Loading_EasterBunny;
                pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                this.BackColor = System.Drawing.Color.LimeGreen;
            }
        }

        /// <summary>
        /// Get the version of the updater
        /// </summary>
        private void PerformVersionDependentUpdate()
        {
            try
            {
                switch (System.Reflection.Assembly.LoadFrom(Path.Combine(Program.Args[1], "Terminals.exe")).GetName().Version.ToString())
                {
                    case "4.9.1.0":
                        Version_4_9_1_0_to_4_9_X_X();
                        break;
                    default:
                        Log.Info("No need to perform any version dependent update.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to update perform version dependent update.", ex);
            }
        }
        
        private void Version_4_9_1_0_to_4_9_X_X()
        {
            try
            {
                // Your code here
            }
            catch (Exception ex)
            {
                Log.Error("An error occured while trying to update Terminals.", ex);
            }
        }

        private void Update(object state)
        {
            Log.Info("Starting update");

            Thread.Sleep(10000);

            try
            {
	            string source = Program.Args[0];
	            string destination = Program.Args[1];
	
	            if (Directory.Exists(source))
	            {
                    if (!Directory.Exists(destination))
	            	{
	            		Log.Error ("Unable to update Terminals, the destination directory " + destination + " doesn't exist");
	            		Application.Exit();
	            	}

                    PerformVersionDependentUpdate();

                    DirectoryInfo dir = new DirectoryInfo(source);
	
		            foreach (FileInfo file in dir.GetFiles())
		            {
		                string dest = Path.Combine(destination, file.Name);
		                //FileInfo fi = new FileInfo(dest);
		                
		                try
		                {
                            /*if (fi.CreationTime != file.CreationTime)*/

                            if (file.Name == "TerminalsUpdater.exe")
                                continue;

                            file.CopyTo(dest, true);
                            Log.Info("Replaced file " + dest);
                        }
		                catch
		                {
		                	Log.Fatal("Unable to copy file " + file.Name);
		                }
		            }
                }
	            else
	            {
	            	Log.Error ("Unable to update Terminals, the source directory " + source + " doesn't exist");
	            }

                Thread.Sleep(3000);

	            Process.Start(Path.Combine(destination, "Terminals.exe"));

	            Application.Exit();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal("Error updating Terminals ...", ex);
        	}
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                foreach (Process process in Process.GetProcesses().Where(x => x.ProcessName == "Terminals" && (new FileInfo(x.MainModule.FileName).Directory.FullName == Program.Args[1])))
                {
                    int processId = process.Id;
                    string name = process.ProcessName + " (" + process.MainWindowTitle + ") - " + process.MainModule.FileName;

                    try
                    {
                        process.Kill();
                        Log.Info("Killed '" + name + "'");
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal("Unable to terminate the Terminals process " + name + " (" + processId + ").", ex);
                        Application.Exit();
                    }
                }
            }
            catch
            {
                Log.Warn("Unable to check if Terminals is still active.");
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(Update), null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private static bool XmasImminent(DateTime isXmasOrNot)
        {
            isXmasOrNot = new DateTime(DateTime.Now.Year, isXmasOrNot.Month, isXmasOrNot.Day);

            bool xmasImminent = (new DateTime(DateTime.Now.Year, 12, 25) - isXmasOrNot).TotalDays <= 31;

            return xmasImminent;
        }

        private static DateTime EasterSunday(int year)
        {
            int day = 0;
            int month = 0;

            int g = year % 19;
            int c = year / 100;
            int h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));

            day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            return new DateTime(year, month, day);
        }

        private static bool EasterImminent(DateTime isEasterOrNot)
        {
            isEasterOrNot = new DateTime(DateTime.Now.Year, isEasterOrNot.Month, isEasterOrNot.Day);

            bool xmasImminent = (EasterSunday(DateTime.Now.Year + 1) - isEasterOrNot).TotalDays <= 40;

            return xmasImminent;
        }
    }
}