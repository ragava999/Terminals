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
    }
}