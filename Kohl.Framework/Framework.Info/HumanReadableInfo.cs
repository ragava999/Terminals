using System;

namespace Kohl.Framework.Info
{
	public class HumanReadableInfo
	{
		public override string ToString()
		{
			string commandLine = AssemblyInfo.CommandLine;
			string userName = UserInfo.UserName;
			string machineName = MachineInfo.MachineName;
			string machineDomain = MachineInfo.MachineDomain;
			string directory = AssemblyInfo.Directory;
			string directoryConfigs = AssemblyInfo.DirectoryConfigFiles;
			string userNameAlias = UserInfo.UserNameAlias;
			string userSid = UserInfo.UserSid;
			string userDomain = UserInfo.UserDomain;
			string registeredOwner = MachineInfo.RegisteredOwner;
			string companyName = MachineInfo.CompanyName;
			string productName = MachineInfo.ProductName;

			string result = String.Format("Command line arguments: {0}", commandLine ?? "-");
			result = result + Environment.NewLine + String.Format("Current directory: {0}", string.IsNullOrEmpty(directory) ? "-" : directory);
			result = result + Environment.NewLine + String.Format("Config file directory: {0}", string.IsNullOrEmpty(directoryConfigs) ? "-" : directoryConfigs);
			result = result + Environment.NewLine + String.Format("Machine name: {0}", string.IsNullOrEmpty(machineName) ? "-" : machineName);
			result = result + Environment.NewLine + String.Format("Machine domain: {0}", string.IsNullOrEmpty(machineDomain) ? "-" : machineDomain);
			result = result + Environment.NewLine + String.Format("User name: {0}", string.IsNullOrEmpty(userName) ? "-" : userName);
			result = result + Environment.NewLine + String.Format("User name alias: {0}", string.IsNullOrEmpty(userNameAlias) ? "-" : userNameAlias);
			result = result + Environment.NewLine + String.Format("User SID: {0}", string.IsNullOrEmpty(userSid) ? "-" : userSid);
			result = result + Environment.NewLine + String.Format("User domain: {0}", string.IsNullOrEmpty(userDomain) ? "-" : userDomain);
			result = result + Environment.NewLine + String.Format("Registered owner: {0}", string.IsNullOrEmpty(registeredOwner) ? "-" : registeredOwner);
			result = result + Environment.NewLine + String.Format("Company name: {0}", string.IsNullOrEmpty(companyName) ? "-" : companyName);
			result = result + Environment.NewLine + String.Format("Is 64 bit OS: {0}", MachineInfo.Is64BitOperatingSystem);
			result = result + Environment.NewLine + String.Format("Is 64 bit process: {0}", AssemblyInfo.Is64BitProcess);
			result = result + Environment.NewLine + String.Format("Your Operating system: {0}", string.IsNullOrEmpty(productName) ? "-" : productName);
			result = result + Environment.NewLine + String.Format("Number of processors: {0}", MachineInfo.ProcessorCount);
			result = result + Environment.NewLine + String.Format("User interactive: {0}", (MachineInfo.IsUnixOrMac ? Console.OpenStandardInput(1) != System.IO.Stream.Null : Environment.UserInteractive));
			result = result + Environment.NewLine + String.Format((MachineInfo.IsUnixOrMac ? "Mono " : ".NET Framework ") + "version: {0}", Environment.Version);
			result = result + Environment.NewLine + String.Format("Working set: {0} MB", (MachineInfo.IsUnixOrMac ? System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 : Environment.WorkingSet) / 1024 / 1024);

			return result;
		}

        public static void ToLog()
        {
            string[] humanReadableInfo = new HumanReadableInfo().ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            foreach(string line in humanReadableInfo)
            {
                Logging.Log.Info(line);
            }
        }
	}
}
