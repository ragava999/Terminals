namespace Terminals.Wizard
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using Kohl.Framework.Drawing;
    using Terminals.Configuration;
    using Terminals.Configuration.Files.Main.SpecialCommands;
    using Terminals.Properties;
    using Terminals.Wizard.MMC;

    public static class SpecialCommandsWizard
    {
        private static DirectoryInfo SystemRoot
        {
            get { return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.System)); }
        }

        public static SpecialCommandConfigurationElementCollection LoadSpecialCommands()
        {
            SpecialCommandConfigurationElementCollection cmdList = new SpecialCommandConfigurationElementCollection();

            AddCmdCommand(cmdList);
            AddRegEditCommand(cmdList);
            AddMmcCommands(cmdList);
            AddControlPanelApplets(cmdList);

            return cmdList;
        }

        private static void AddControlPanelApplets(SpecialCommandConfigurationElementCollection cmdList)
        {
            foreach (FileInfo file in SystemRoot.GetFiles("*.cpl"))
            {
                SpecialCommandConfigurationElement elm1 = new SpecialCommandConfigurationElement(file.Name);

                Icon[] fileIcons = IconHandler.IconsFromFile(file.FullName, IconSize.Small);

                if (fileIcons != null && fileIcons.Length > 0)
                {
                    elm1.Thumbnail = fileIcons[0].ToBitmap().ImageToBase64(System.Drawing.Imaging.ImageFormat.Png);
                    elm1.Name = file.Name;
                    elm1.Executable = @"%systemroot%\system32\" + file.Name;
                    cmdList.Add(elm1);
                }
            }
        }

        private static void AddMmcCommands(SpecialCommandConfigurationElementCollection cmdList)
        {
            Icon[] IconsList = IconHandler.IconsFromFile(Path.Combine(SystemRoot.FullName, "mmc.exe"),
                                                         IconSize.Small);
            Random rnd = new Random();

            foreach (FileInfo file in SystemRoot.GetFiles("*.msc"))
            {
                MMCFile fileMMC = new MMCFile(file);

                SpecialCommandConfigurationElement elm1 = new SpecialCommandConfigurationElement(file.Name);

                if (IconsList != null && IconsList.Length > 0)
                {
                    if (fileMMC.SmallIcon != null)
                    {
                        elm1.Thumbnail = fileMMC.SmallIcon.ToBitmap().ImageToBase64(System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else
                    {
                        elm1.Thumbnail = IconsList[rnd.Next(IconsList.Length - 1)].ToBitmap().ImageToBase64(System.Drawing.Imaging.ImageFormat.Png);
                    }

                    elm1.Name = fileMMC.Parsed ? fileMMC.Name : file.Name.Replace(file.Extension, "");
                }

                elm1.Executable = @"%systemroot%\system32\" + file.Name;
                cmdList.Add(elm1);
            }
        }

        private static void AddRegEditCommand(SpecialCommandConfigurationElementCollection cmdList)
        {
            string regEditFile = Path.Combine(SystemRoot.FullName, "regedt32.exe");
            Icon[] regeditIcons = IconHandler.IconsFromFile(regEditFile, IconSize.Small);
            SpecialCommandConfigurationElement regEditElm = new SpecialCommandConfigurationElement("Registry Editor");

            if (regeditIcons != null && regeditIcons.Length > 0)
            {
                regEditElm.Thumbnail = regeditIcons[0].ToBitmap().ImageToBase64(System.Drawing.Imaging.ImageFormat.Png);
            }

            regEditElm.Executable = regEditFile;
            cmdList.Add(regEditElm);
        }

        private static void AddCmdCommand(SpecialCommandConfigurationElementCollection cmdList)
        {
            cmdList.Add(new SpecialCommandConfigurationElement("Command Shell")
                                                         {
                                                             Executable = @"%systemroot%\system32\cmd.exe"
                                                         });
        }
    }
}