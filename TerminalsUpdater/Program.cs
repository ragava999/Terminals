using System;
using System.Windows.Forms;
using Kohl.Framework.Info;
using Kohl.Framework.Logging;

namespace TerminalsUpdater {
    static class Program {

        public static string[] Args;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
        	// Set the type to be reflected.
            AssemblyInfo.Assembly = System.Reflection.Assembly.GetAssembly(typeof(Program));
            
            Log.SetXmlConfig("Terminals");
            
            Log.Info(String.Format("-------------------------------{0} started. Version: {1}, Date: {2}-------------------------------", AssemblyInfo.Title, AssemblyInfo.Version, AssemblyInfo.BuildDate));
        	
            if (args.Length != 2)
            {
            	Log.Fatal("Unable to update Terminals, incorrect number of command line arguments for " + AssemblyInfo.TitleVersion);
            	return;
            }
            
			/*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			*/

            Args = args;

            Application.Run(new Form1());
        }
    }
}