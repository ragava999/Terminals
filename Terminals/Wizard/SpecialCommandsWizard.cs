namespace Terminals.Wizard
{
    using Configuration.Files.Main.SpecialCommands;
    using Kohl.Framework.Drawing;
    using MMC;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    public static class SpecialCommandsWizard
    {
        private static DirectoryInfo SystemRoot
        {
            get
            {
                if (!Kohl.Framework.Info.MachineInfo.IsUnixOrMac)
                    return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.System));

                return null;
            }
        }

        public static SpecialCommandConfigurationElementCollection LoadSpecialCommands()
        {
            SpecialCommandConfigurationElementCollection cmdList = new SpecialCommandConfigurationElementCollection();

            AddCmdCommand(cmdList);
            AddRegEditCommand(cmdList);
            AddMmcCommands(cmdList);
            AddControlPanelApplets(cmdList);
            AddUsefulTools(cmdList);

            SpecialCommandConfigurationElementCollection collection = new SpecialCommandConfigurationElementCollection();

            // Sort the result by the display name
            foreach (SpecialCommandConfigurationElement element in (from SpecialCommandConfigurationElement element in cmdList
                                                              orderby element.Name
                                                              select element))
            {
                collection.Add(element);
            }

            return collection;
        }

        private static void AddControlPanelApplets(SpecialCommandConfigurationElementCollection cmdList)
        {
            if (SystemRoot == null)
                return;

            foreach (FileInfo file in SystemRoot.GetFiles("*.cpl"))
            {
                SpecialCommandConfigurationElement elm1 = new SpecialCommandConfigurationElement(file.Name);

                Icon[] fileIcons = IconHandler.IconsFromFile(file.FullName, IconSize.Small);

                if (fileIcons != null && fileIcons.Length > 0)
                {
                    elm1.Thumbnail = fileIcons[0].ToBitmap().ImageToBase64(System.Drawing.Imaging.ImageFormat.Png);
                    
                    switch (file.Name.ToLowerInvariant())
                    {
                        case "access.cpl":
                            elm1.Name = "Accessibility Controls";
                            break;
                        case "hdwwiz.cpl":
                            elm1.Name = "Add Hardware Wizard";
                            break;
                        case "appwiz.cpl":
                            elm1.Name = "Add/Remove Programs";
                            break;
                        case "cjtpl.cpl":
                            elm1.Name = "REINER SCT cyberJack Device Manager";
                            break;
                        case "bthprops.cpl":
                            elm1.Name = "Mobile Bluetooth Control Panel";
                            break;
                        case "collab.cpl":
                            elm1.Name = "People Near Me";
                            break;
                        case "desk.cpl":
                            elm1.Name = "Display Properties (Screen Resolution & Orientation)";
                            break;
                        case "directx.cpl":
                            elm1.Name = "Direct X Control Panel";
                            break;
                        case "diskmgmt.msc":
                            elm1.Name = "Disk Management";
                            break;
                        case "findfast.cpl":
                            elm1.Name = "Findfast";
                            break;
                        case "firewall.cpl":
                            elm1.Name = "Windows Firewall";
                            break;
                        case "flashplayercplapp.cpl":
                            elm1.Name = "Adobe Flash Player Control Panel";
                            break;
                        case "fsmgmt.msc":
                            elm1.Name = "Shared Folders";
                            break;
                        case "idtnc64.cpl":
                            elm1.Name = "Sound and Volume Settings";
                            break;
                        case "igfxcpl.cpl":
                            elm1.Name = "Intel Graphics and Media Control Panel";
                            break;
                        case "inetcpl.cpl":
                            elm1.Name = "Internet Options";
                            break;
                        case "infocardcpl.cpl":
                            elm1.Name = "Windows CardSpace";
                            break;
                        case "intl.cpl":
                            elm1.Name = "Regional and Language Settings";
                            break;
                        case "irprops.cpl":
                            elm1.Name = "Microsoft Windows Infrared Properties Control Panel";
                            break;
                        case "joy.cpl":
                            elm1.Name = "Game Controllers";
                            break;
                        case "main.cpl":
                            elm1.Name = "Mouse Properties";
                            break;
                        case "mmsys.cpl":
                            elm1.Name = "Sounds and Audio";
                            break;
                        case "ncpa.cpl":
                            elm1.Name = "Network Connections";
                            break;
                        case "netsetup.cpl":
                            elm1.Name = "Network Setup Wizard";
                            break;
                        case "nvtuicpl.cpl":
                            elm1.Name = "Nview Desktop Manager";
                            break;
                        case "password.cpl":
                            elm1.Name = "Password Properties";
                            break;
                        case "powercfg.cpl":
                            elm1.Name = "Power Configurations";
                            break;
                        case "quicktime.cpl":
                            elm1.Name = "Quick Time";
                            break;
                        case "sapi.cpl":
                            elm1.Name = "Speech and Text to Speech Settings";
                            break;
                        case "sticpl.cpl":
                            elm1.Name = "Scanners and Cameras";
                            break;
                        case "sysdm.cpl":
                            elm1.Name = "System Properties";
                            break;
                        case "tabletpc.cpl":
                            elm1.Name = "Pen and Touch Settings";
                            break;
                        case "telephon.cpl":
                            elm1.Name = "Phone and Modem Options";
                            break;
                        case "timedate.cpl":
                            elm1.Name = "Date and Time Properties";
                            break;
                        case "wuaucpl.cpl":
                            elm1.Name = "Automatic Updates";
                            break;
                        case "wscui.cpl":
                            elm1.Name = "Security Center";
                            break;
                        default:
                            elm1.Name = file.Name;
                            Kohl.Framework.Logging.Log.Warn("The display name for the control panel applet hasn't been found using file name '" + file.Name + "'.");
                            break;
                    }
                    
                    elm1.Executable = @"%systemroot%\system32\" + file.Name;
                    cmdList.Add(elm1);
                }
            }
        }

        private static void AddMmcCommands(SpecialCommandConfigurationElementCollection cmdList)
        {
            if (SystemRoot == null)
                return;

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
              
                    if (!fileMMC.Parsed)
                        switch (file.Name.ToLowerInvariant())
                        {
                            case "azman.msc":
                                elm1.Name = "Authorization Manager";
                                break;
                            case "certmgr.msc":
                                elm1.Name = "Certificate Manager";
                                break;
                            case "ciadv.msc":
                                elm1.Name = "Indexing Service";
                                break;
                            case "comexp.msc":
                                elm1.Name = "Component Services";
                                break;
                            case "compmgmt.msc":
                                elm1.Name = "Computer Management";
                                break;
                            case "devmgmt.msc":
                                elm1.Name = "Device Manager";
                                break;
                            case "diskmgmt.msc":
                                elm1.Name = "Disk Management";
                                break;
                            case "dfrg.msc":
                                elm1.Name = "Disk Defragmentation";
                                break;
                            case "eventvwr.msc":
                                elm1.Name = "Event Viewer";
                                break;
                            case "fsmgmt.msc":
                                elm1.Name = "Shared Folders";
                                break;
                            case "gpedit.msc":
                                elm1.Name = "Group Policy Editor";
                                break;
                            case "iis.msc":
                                elm1.Name = "Internet Information Services Manager";
                                break;
                            case "iis6.msc":
                                elm1.Name = "Internet Information Services 6.0 Manager";
                                break;
                            case "lusrmgr.msc":
                                elm1.Name = "Local Users and Groups";
                                break;
                            case "napclcfg.msc":
                                elm1.Name = "NAP Client Configuration";
                                break;
                            case "nfsmgmt.msc":
                                elm1.Name = "Microsoft Services for NFS (Network File System)";
                                break;
                            case "ntmsmgr.msc":
                                elm1.Name = "Removable Storage";
                                break;
                            case "ntmsoprq.msc":
                                elm1.Name = "Removable Storage Operator Requests";
                                break;
                            case "perfmon.msc":
                                elm1.Name = "Performance Monitor";
                                break;
                            case "printmanagement.msc":
                                elm1.Name = "Print Management";
                                break;
                            case "rsop.msc":
                                elm1.Name = "Resultant Set of Policy";
                                break;
                            case "secpol.msc":
                                elm1.Name = "Local Security Settings";
                                break;
                            case "scanmanagement.msc":
                                elm1.Name = "Scan Management";
                                break;
                            case "services.msc":
                                elm1.Name = "Services";
                                break;
                            case "taskschd.msc":
                                elm1.Name = "Task Scheduler";
                                break;
                            case "tpm.msc":
                                elm1.Name = "Trusted Platform Module Mangement";
                                break;
                            case "wf.msc":
                                elm1.Name = "Windows Firewall with Advanced Security";
                                break;
                            case "wmimgmt.msc":
                                elm1.Name = "Windows Management Infrastructure";
                                break;
                            default:
                                elm1.Name = file.Name;
                                Kohl.Framework.Logging.Log.Warn("The display name for the management console plugin hasn't been found using file name '" + file.Name + "'.");
                                break;
                        }
                }

                elm1.Executable = @"%systemroot%\system32\" + file.Name;
                cmdList.Add(elm1);
            }
        }

        private static void AddRegEditCommand(SpecialCommandConfigurationElementCollection cmdList)
        {
            if (SystemRoot == null)
                return;

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
            if (!Kohl.Framework.Info.MachineInfo.IsUnixOrMac)
            {
                cmdList.Add(new SpecialCommandConfigurationElement("Command Shell")
                {
                    Executable = @"%systemroot%\system32\cmd.exe"
                });
                cmdList.Add(new SpecialCommandConfigurationElement("PowerShell")
                {
                    Executable = @"%systemroot%\system32\WindowsPowerShell\v1.0\powershell.exe"
                });
                cmdList.Add(new SpecialCommandConfigurationElement("PowerShell ISE")
                {
                    Executable = @"%systemroot%\system32\WindowsPowerShell\v1.0\powershell_ise.exe"
                });
            }

            if (Kohl.Framework.Info.MachineInfo.IsMac)
            {
                cmdList.Add(new SpecialCommandConfigurationElement("Command Shell")
                {
                    Executable = @"Terminal.app"
                });
            }
        }

        private static void AddUsefulTools(SpecialCommandConfigurationElementCollection cmdList)
        {
            AddUsefulTool(cmdList, "calc", "Calculator");
            AddUsefulTool(cmdList, "charmap", "Character Map");
            AddUsefulTool(cmdList, "cleanmgr", "Disk Cleanup Utility");
            AddUsefulTool(cmdList, "colorcpl", "Color Management");
            AddUsefulTool(cmdList, "computerdefaults", "Program Access and Computer Defaults");
            AddUsefulTool(cmdList, "control color", "Display Properties (Color & Appearance)");
            AddUsefulTool(cmdList, "control desktop", "Display Properties (Themes, Desktop, Screensaver)");
            AddUsefulTool(cmdList, "control", "Add New Programs", "appwiz.cpl,,1");
            AddUsefulTool(cmdList, "control", "Add/Remove Windows Components", "appwiz.cpl,,2");
            AddUsefulTool(cmdList, "control", "Administrative Tools", "admintools");
            AddUsefulTool(cmdList, "control", "Automatic Update", "wuaucpl.cpl");
            AddUsefulTool(cmdList, "control", "Control Panel");
            AddUsefulTool(cmdList, "control", "Folder Options", "folders");
            AddUsefulTool(cmdList, "control", "Fonts list", "fonts");
            AddUsefulTool(cmdList, "control", "Keyboard Properties", "keyboard");
            AddUsefulTool(cmdList, "control", "Printers and Faxes", "printers");
            AddUsefulTool(cmdList, "control", "Set Program Access & Defaults", "appwiz.cpl,,3");
            AddUsefulTool(cmdList, "control", "User Accounts (Autologon)", "userpasswords2");
            AddUsefulTool(cmdList, "credwiz", "Credential (passwords) Backup and Restore Wizard");
            AddUsefulTool(cmdList, "cttune", "Clear Type (tune or turn off)");
            AddUsefulTool(cmdList, "dccw", "Display Color Calibration");
            AddUsefulTool(cmdList, "DevicePairingWizard", "Device Pairing Wizard");
            AddUsefulTool(cmdList, "dfrgui", "Disk Defragmenter");
            AddUsefulTool(cmdList, "dialer", "Phone Dialer");
            AddUsefulTool(cmdList, "diskpart", "Disk Partition Manager");
            AddUsefulTool(cmdList, "displayswitch", "Switch display");
            AddUsefulTool(cmdList, "dpiscaling", "Display DPI / Text size");
            AddUsefulTool(cmdList, "dvdplay", "DVD Player");
            AddUsefulTool(cmdList, "dxdiag", "Direct X Troubleshooter");
            AddUsefulTool(cmdList, "eudcedit", "Private Character Editor");
            AddUsefulTool(cmdList, "explorer", "Windows Explorer");
            AddUsefulTool(cmdList, "fontview", "Font preview", "arial.ttf");
            AddUsefulTool(cmdList, "fsquirt", "Bluetooth Transfer Wizard");
            AddUsefulTool(cmdList, "gettingstarted", "OOB Getting Started");
            AddUsefulTool(cmdList, "hdwwiz", "Add Hardware Wizard");
            AddUsefulTool(cmdList, "iscsicpl", "iSCSI Initiator configuration");
            AddUsefulTool(cmdList, "lpksetup", "Language Pack Installer");
            AddUsefulTool(cmdList, "magnify", "Windows Magnifier");
            AddUsefulTool(cmdList, "mdsched", "Windows Memory Diagnostic Scheduler");
            AddUsefulTool(cmdList, "migwiz\\migwiz", "Files and Settings Transfer Tool");
            AddUsefulTool(cmdList, "mmc", "Microsoft Management Console");
            AddUsefulTool(cmdList, "mobsync", "Syncronization Tool (Offline files)");
            AddUsefulTool(cmdList, "mrt", "Microsoft Malicious Software Removal Tool");
            AddUsefulTool(cmdList, "msconfig", "System Configuration Utility");
            AddUsefulTool(cmdList, "msdt", "Microsoft Support Diagnostic Tool");
            AddUsefulTool(cmdList, "msinfo32", "System Information");
            AddUsefulTool(cmdList, "msra", "Remote Assistance");
            AddUsefulTool(cmdList, "mstsc", "Remote Desktop");
            AddUsefulTool(cmdList, "netplwiz", "Advanced User Accounts Control Panel");
            AddUsefulTool(cmdList, "netproj", "Connect to Network Projector");
            AddUsefulTool(cmdList, "notepad", "NotePad");
            AddUsefulTool(cmdList, "odbcad32", "ODBC Data Source Administrator");
            AddUsefulTool(cmdList, "optionalfeatures", "Windows Features");
            AddUsefulTool(cmdList, "osk", "On Screen Keyboard");
            AddUsefulTool(cmdList, "PresentationSettings", "Presentation Settings");
            AddUsefulTool(cmdList, "printbrmui", "Printer Migration (backup/restore)");
            AddUsefulTool(cmdList, "printui", "Change Printing Settings");
            AddUsefulTool(cmdList, "psr", "Problem Steps Recorder");
            AddUsefulTool(cmdList, "recdisc", "System Repair - Create a System Repair Disc");
            AddUsefulTool(cmdList, "regedit", "Registry Editor");
            AddUsefulTool(cmdList, "rekeywiz", "Encrypting File System Wizard (EFS)");
            AddUsefulTool(cmdList, "resmon", "Resource Monitor");
            AddUsefulTool(cmdList, "rstrui", "System Restore");
            AddUsefulTool(cmdList, "rundll32", "Edit Environment Variables", "sysdm.cpl,EditEnvironmentVariables");
            AddUsefulTool(cmdList, "rundll32", "User Profiles - Edit/Change type", "sysdm.cpl,EditUserProfiles");
            AddUsefulTool(cmdList, "sdclt", "Backup and Restore Utility");
            AddUsefulTool(cmdList, "shrpubw", "Shared Folder Wizard");
            AddUsefulTool(cmdList, "sigverif", "File Signature Verification Tool (Device drivers)");
            AddUsefulTool(cmdList, "slui", "Windows Activation/Licensing");
            AddUsefulTool(cmdList, "sndvol", "Sound Volume");
            AddUsefulTool(cmdList, "snippingtool", "Screenshot Snipping Tool");
            AddUsefulTool(cmdList, "soundrecorder", "Sound Recorder");
            AddUsefulTool(cmdList, "syskey", "Windows System Security Tool (Encrypt the SAM database)");
            AddUsefulTool(cmdList, "SystemPropertiesAdvanced", "System Properties - Advanced");
            AddUsefulTool(cmdList, "SystemPropertiesDataExecutionPrevention", "Data Execution Prevention");
            AddUsefulTool(cmdList, "SystemPropertiesHardware", "System Properties - Hardware");
            AddUsefulTool(cmdList, "SystemPropertiesPerformance", "System Properties - Performance");
            AddUsefulTool(cmdList, "tabcal", "Digitizer Calibration Tool (Tablets/Touch screens)");
            AddUsefulTool(cmdList, "taskmgr", "Task Manager");
            AddUsefulTool(cmdList, "tpmInit", "Trusted Platform Module Initialization Wizard");
            AddUsefulTool(cmdList, "UserAccountControlSettings", "User Account Control (UAC) Settings");
            AddUsefulTool(cmdList, "utilman", "Ease of Access Center");
            AddUsefulTool(cmdList, "verifier", "Driver Verifier Utility");
            AddUsefulTool(cmdList, "wiaacmgr", "Windows Image Acquisition");
            AddUsefulTool(cmdList, "winver", "Windows Version (About Windows)");
            AddUsefulTool(cmdList, "write", "WordPad");
            AddUsefulTool(cmdList, "wuapp", "Windows Update");
            AddUsefulTool(cmdList, "wusa", "Windows Update Standalone Installer");
        }

        private static void AddUsefulTool(SpecialCommandConfigurationElementCollection cmdList, string name, string displayName, string options = "")
        {
            name = Environment.SystemDirectory + "\\" + name + ".exe";

            if (File.Exists(name))
            {
                Icon[] icons = null;

                try
                {
                    icons = IconHandler.IconsFromFile(name, IconSize.Small);
                }
                catch
                {
                    Kohl.Framework.Logging.Log.Debug("Unable to retrieve Icon from " + name);
                }

                var command = new SpecialCommandConfigurationElement(displayName)
                {
                    Executable = name
                };

                if (!string.IsNullOrWhiteSpace(options))
                    command.Arguments = options;

                if (icons != null && icons.Length > 0)
                {
                    command.Thumbnail = icons[0].ToBitmap().ImageToBase64(System.Drawing.Imaging.ImageFormat.Png);
                }

                cmdList.Add(command);
            }
        }
    }
}