using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Kohl.Framework.Info;
using Kohl.Framework.Logging;

namespace TerminalsUpdater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void Update(object state)
        {
        	Log.Info("Starting update");
        	
        	try
        	{
                /*
	            Mutex mtx = new Mutex(false, "Terminals");
	            bool isAppRunning = true;
	
	            while (isAppRunning)
	            {
	                isAppRunning = !mtx.WaitOne(0, false);
	            }
                */
	            //wait for the process to completely end
	            //Thread.Sleep(5000);
	
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
            Thread.Sleep(5000);

            try
            {
                foreach (Process process in Process.GetProcesses().Where(x => x.ProcessName == "Terminals" && (new FileInfo(x.MainModule.FileName).Directory.FullName == Program.Args[1])))
                {
                    try
                    {
                        process.Kill();
                        Thread.Sleep(5000);
                        Log.Info("Killed '" + process.MainModule.FileName);
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal("Unable to terminate the Terminals process " + process.MainModule.FileName + " (" + process.Id + ").", ex);
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