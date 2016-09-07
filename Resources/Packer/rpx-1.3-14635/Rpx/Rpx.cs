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
using System.Collections.Generic;
using System.IO;
using Rpx.Packing;
using Rug.Cmd;
using Rug.Cmd.Arguments;
using Rug.Cmd.Colors;

namespace Rpx
{
    public static class RpxInterface
    {
        #region Static Argument / Parser Definitions

        public static readonly string Title; 

        public static readonly ArgumentParser Parser;

        public static readonly StringArgument PathString;
        public static readonly StringListArgument FileList;
        public static readonly StringArgument AssemblyInfoPath;
        public static readonly StringArgument IconPath;
        public static readonly StringArgument OutputPath;

        public static readonly PlusMinusSwitch ConsoleSwitch;
        public static readonly BoolSwitch PassArgsSwitch;
        public static readonly BoolSwitch DecorateSwitch;
        public static readonly OptionalStringArgument ProtectZipSwitch;
        public static readonly CsvArgument ToolsCsv;

        public static readonly BoolSwitch BuildSwitch;
        public static readonly BoolSwitch QuietSwitch;
        public static readonly EnumSwitch VerboseSwitch;
        public static readonly PlusMinusSwitch WarningsAsErrors;

        #endregion 

        #region Setup Argument Parser

        static RpxInterface()
        {
            Title = Strings.App_Title + " " + typeof(Program).Assembly.GetName().Version.ToString();

            Parser = new ArgumentParser("Rpx", Title + "\n" + Strings.App_AboutText);

            Parser.AboutTitleText = Title;
            Parser.AboutText = Strings.App_AboutText;
            Parser.AboutTextLong = Strings.App_AboutTextLong;
            Parser.CreditsText = Strings.App_CreditsText;
            Parser.LegalText = Strings.App_LegalText;
            Parser.HasApplicationDocument = true;

            PathString = new StringArgument(Strings.Arg_PathString_Name, Strings.Arg_PathString_ShortHelp, Strings.Arg_PathString_Help);
            FileList = new StringListArgument(Strings.Arg_FileList_Name, "", Strings.Arg_FileList_Help);

            Parser.FirstArgument = PathString;
            Parser.DefaultArgument = FileList;

            AssemblyInfoPath = new StringArgument(Strings.Arg_AssemblyInfoPath_Name, Strings.Arg_AssemblyInfoPath_ShortHelp, Strings.Arg_AssemblyInfoPath_Help);
            IconPath = new StringArgument(Strings.Arg_IconPath_Name, Strings.Arg_IconPath_ShortHelp, Strings.Arg_IconPath_Help);
            OutputPath = new StringArgument(Strings.Arg_OutputPath_Name, Strings.Arg_OutputPath_ShortHelp, Strings.Arg_OutputPath_Help);
            ConsoleSwitch = new PlusMinusSwitch(Strings.Arg_ConsoleSwitch_ShortHelp, Strings.Arg_ConsoleSwitch_Help, true);
            PassArgsSwitch = new BoolSwitch(Strings.Arg_PassArgsSwitch_ShortHelp, Strings.Arg_PassArgsSwitch_Help);
            DecorateSwitch = new BoolSwitch(Strings.Arg_Decorate_ShortHelp, Strings.Arg_Decorate_Help);
            ProtectZipSwitch = new OptionalStringArgument(Strings.Arg_Hide_Name, Strings.Arg_Hide_ShortHelp, Strings.Arg_Hide_Help);

            ToolsCsv = new CsvArgument(Strings.Arg_Toolkit_Name, Strings.Arg_Toolkit_ShortHelp, Strings.Arg_Toolkit_Help);

            Parser.Add(PlusMinusSwitch.KeyPrefix, Strings.Arg_ConsoleSwitch_Symbol, Strings.Arg_ConsoleSwitch_Key, ConsoleSwitch);
            Parser.Add("/", Strings.Arg_PassArgsSwitch_Symbol, Strings.Arg_PassArgsSwitch_Key, PassArgsSwitch);
            Parser.Add("/", Strings.Arg_Toolkit_Key, ToolsCsv);
            Parser.Add("/", Strings.Arg_OutputPath_Symbol, Strings.Arg_OutputPath_Key, OutputPath);
            Parser.Add("/", Strings.Arg_Decorate_Key, DecorateSwitch);
            Parser.Add("/", Strings.Arg_IconPath_Symbol, Strings.Arg_IconPath_Key, IconPath);
            Parser.Add("/", Strings.Arg_AssemblyInfoPath_Symbol, Strings.Arg_AssemblyInfoPath_Key, AssemblyInfoPath);
            Parser.Add("/", Strings.Arg_Hide_Key, ProtectZipSwitch);


            BuildSwitch = new BoolSwitch(Strings.Arg_BuildSwitch_ShortHelp, Strings.Arg_BuildSwitch_Help);
            QuietSwitch = new BoolSwitch(Strings.Arg_QuietSwitch_ShortHelp, Strings.Arg_QuietSwitch_Help);
            VerboseSwitch = new EnumSwitch(Strings.Arg_Verbose_ShortHelp, Strings.Arg_Verbose_Help, typeof(ConsoleVerbosity));

            Parser.Add("/", Strings.Arg_BuildSwitch_Key, BuildSwitch);
            Parser.Add("/", Strings.Arg_QuietSwitch_Key, QuietSwitch);
            Parser.Add("/", Strings.Arg_Verbose_Key, VerboseSwitch);

            WarningsAsErrors = new PlusMinusSwitch(Strings.Arg_WarningsAsErrors_ShortHelp, Strings.Arg_WarningsAsErrors_Help, false);

            Parser.Add(PlusMinusSwitch.KeyPrefix, Strings.Arg_WarningsAsErrors_Key, WarningsAsErrors);
        }
        
        #endregion

        #region Run Command

        public static void RunCommand(string argString)
        {
            RunCommand(CommandLineSplitter.SplitArguments(argString));
        }

        public static void RunCommand(params string[] args)
        {
            if (ParseArgs(args))
                Run();
        }

        #endregion 

        #region Parse Args

        public static bool ParseArgs(string argString)
        {
            return ParseArgs(CommandLineSplitter.SplitArguments(argString)); 
        }

        public static bool ParseArgs(string[] args)
        {
            ConsoleColorState state = RC.ColorState;

            Parser.Reset();

            try
            {
                Parser.Parse(args);

                return true; 
            }
            catch (Exception ex)
            {
                RC.WriteException(0199, ex);

                return false; 
            }
            finally
            {
                #region Reset The Color State

                RC.ColorState = state;

                #endregion
            }

        }

        #endregion 

        #region Actaully Run the process 

        public static void Run()
        {
            ConsoleColorState state = RC.ColorState;            

            try
            {
                if (!Parser.HelpMode)
                {
                    #region Setup

                    if (VerboseSwitch.Defined)
                    {
                        RC.Verbosity = (ConsoleVerbosity)VerboseSwitch.Value;
                    }
                    else if (QuietSwitch.Defined)
                    {
                        RC.Verbosity = ConsoleVerbosity.Quiet;
                    }

                    if (WarningsAsErrors.Defined)
                    {
                        RC.WarningsAsErrors = WarningsAsErrors.Value;
                        RC.ReportWarnings = WarningsAsErrors.Value;
                    }                    

                    RC.IsBuildMode = BuildSwitch.Defined;

                    RC.WriteLine(ConsoleVerbosity.Normal, "");
                    RC.WriteLine(ConsoleVerbosity.Minimal, ConsoleThemeColor.TitleText, Title);
                    //RC.WriteLine(ConsoleVerbosity.Minimal, "");

                    RC.ForegroundThemeColor = ConsoleThemeColor.Text;

                    #endregion

                    #region Check First Argument

                    if (Helper.IsNullOrEmpty(PathString.Value))
                    {
                        RC.WriteError(0101, Strings.Error_0101);
                        return;
                    }

                    #endregion

                    #region Create Process

                    CompileProcess compressionProcess = new CompileProcess();

                    compressionProcess.LogToConsole = true;

                    // ExecutableTypeMode
                    if (ConsoleSwitch.Defined)
                    {
                        compressionProcess.ExecutableTypeLookupMode = ExecutableTypeMode.Explicit;
                        compressionProcess.ExecutableType = ConsoleSwitch.Value ? ExecutableType.Console : ExecutableType.Forms;
                    }
                    else
                    {
                        compressionProcess.ExecutableTypeLookupMode = ExecutableTypeMode.Reflected;
                        compressionProcess.ExecutableType = ExecutableType.Console;
                    }

                    #endregion

                    #region Parse Optional Arguments

                    #region ToolsCsv Argument Switch Arround

                    if (ToolsCsv.Defined)
                    {
                        if (!OutputPath.Defined)
                        {
                            // If the output path argument has not been defined then manually set its value
                            // to the value of the first argument. e.g. 'rug-tool.exe' 
                            OutputPath.Defined = true;
                            OutputPath.SetValue(PathString.Value);
                        }
                        else
                        {
                            // if the output path argument has been defined then the fist argument must be an
                            // additional .dll file so move the value to additional file list argument                            
                            FileList.Value.Add(PathString.Value);
                        }

                        // Zero the value of the fist argument
                        PathString.Reset();

                        if (compressionProcess.ExecutableTypeLookupMode != ExecutableTypeMode.Explicit)
                        {
                            compressionProcess.ExecutableTypeLookupMode = ExecutableTypeMode.Default;
                        }
                    }

                    #endregion

                    #region Additional Assembly Paths

                    List<string> assemblyPaths = new List<string>(FileList.Value.Count + 1);

                    if (PathString.Defined)
                    {
                        assemblyPaths.Add(PathString.Value);
                    }

                    foreach (object obj in FileList.Value)
                    {
                        assemblyPaths.Add(obj.ToString());
                    }

                    compressionProcess.Assemblys.AddRange(assemblyPaths);
                    compressionProcess.Compression.Files.AddRange(assemblyPaths);

                    #endregion

                    #region Output Path

                    string output;

                    if (OutputPath.Defined)
                    {
                        output = OutputPath.Value;
                    }
                    else if (PathString.Defined)
                    {
                        output = PathString.Value;
                    }
                    else
                    {
                        RC.WriteError(0104, Strings.Error_0104);
                        return;
                    }

                    #endregion

                    #region Initial Assembly Path

                    string initial;

                    if (PathString.Defined)
                    {
                        initial = PathString.Value;
                        initial = "/" + initial.Substring(initial.LastIndexOf('\\') + 1);
                    }
                    else
                    {
                        initial = null;
                    }

                    compressionProcess.InitialAssemblyPath = initial;

                    #endregion

                    #region Icon Path

                    string icon = "";

                    if (IconPath.Defined)
                    {
                        icon = IconPath.Value;
                    }

                    compressionProcess.IconFile = icon;

                    #endregion

                    #region AssemblyInfoPath

                    string assemblyinfoFile = null;

                    if (AssemblyInfoPath.Defined)
                    {
                        if (!File.Exists(AssemblyInfoPath.Value))
                        {
                            RC.WriteError(0102, string.Format(Strings.Error_0102, AssemblyInfoPath.Value));
                            return;
                        }

                        assemblyinfoFile = AssemblyInfoPath.Value;
                    }
                    else if (!PathString.Defined)
                    {
                        RC.WriteError(0106, Strings.Error_0106);
                        return;
                    }

                    compressionProcess.AssembyInfoSourceFilePath = assemblyinfoFile;

                    #endregion

                    #region Pass Arguments

                    bool passArgs = compressionProcess.ExecutableType == ExecutableType.Console ? true : false;

                    if (PassArgsSwitch.Defined)
                    {
                        passArgs = PassArgsSwitch.Value;
                    }

                    compressionProcess.PassArguments = passArgs;

                    #endregion

                    #region Decorate

                    compressionProcess.Decorate = DecorateSwitch.Defined;

                    #endregion

                    #region Protect Zip

                    compressionProcess.Compression.Protected = ProtectZipSwitch.Defined;
                    compressionProcess.Compression.Password = ProtectZipSwitch.Value;

                    #endregion

                    #region Build Tools

                    foreach (string toolString in ToolsCsv.Value)
                    {
                        int index = toolString.IndexOf(':');

                        if (index < 1)
                        {
                            RC.WriteError(0105, string.Format(Strings.Error_0105, toolString));
                            return;
                        }

                        string toolName = toolString.Substring(0, index);
                        string toolPath = toolString.Substring(index + 1);

                        compressionProcess.Tools.Add(toolName, toolPath);
                        compressionProcess.Compression.Files.Add(toolPath);
                    }

                    #endregion

                    #endregion

                    #region Execute Bundle and Compile

                    RC.ForegroundThemeColor = ConsoleThemeColor.Text;

                    compressionProcess.Compression.CreatePackage();

                    #endregion

                    if (compressionProcess.RunProcess(output))
                    {
                        #region Print Size Summary

                        RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.TitleText, "\n\n" + Strings.Text_FinalSummary);
                        RC.WriteLine(ConsoleThemeColor.SubText, " " + new string(ConsoleChars.SingleLines[1], 41));

                        Rug.Cmd.CmdHelper.WriteInfoToConsole(Strings.Text_UncompressedSize, CmdHelper.GetMemStringFromBytes(compressionProcess.InitialSize, true), RC.Theme[ConsoleThemeColor.Text]);

                        if (compressionProcess.IconFileSize > 0)
                        {
                            Rug.Cmd.CmdHelper.WriteInfoToConsole(Strings.Text_IconFileSize, CmdHelper.GetMemStringFromBytes(compressionProcess.IconFileSize, true), RC.Theme[ConsoleThemeColor.Text]);
                            Rug.Cmd.CmdHelper.WriteInfoToConsole(Strings.Text_TotalSize, CmdHelper.GetMemStringFromBytes(compressionProcess.InitialSize + compressionProcess.IconFileSize, true), RC.Theme[ConsoleThemeColor.Text]);
                        }

                        Rug.Cmd.CmdHelper.WriteInfoToConsole(Strings.Text_StartupOverhead, CmdHelper.GetMemStringFromBytes(compressionProcess.OverheadSize, true), RC.Theme[ConsoleThemeColor.Text]);

                        ConsoleThemeColor compColor = ConsoleThemeColor.Text;

                        if (compressionProcess.FinalFileSize < compressionProcess.TotalInitialFileSize / 2)
                        {
                            compColor = ConsoleThemeColor.TextGood;
                        }
                        else if (compressionProcess.FinalFileSize < (compressionProcess.TotalInitialFileSize / 4) * 3)
                        {
                            compColor = ConsoleThemeColor.SubTextGood;
                        }
                        else if ((compressionProcess.FinalFileSize >= ((compressionProcess.TotalInitialFileSize / 4) * 3)) &&
                                 (compressionProcess.FinalFileSize < compressionProcess.TotalInitialFileSize))
                        {
                            compColor = ConsoleThemeColor.SubTextNutral;
                        }
                        else if (compressionProcess.FinalFileSize == compressionProcess.TotalInitialFileSize)
                        {
                            compColor = ConsoleThemeColor.Text;
                        }
                        else if (compressionProcess.FinalFileSize > compressionProcess.TotalInitialFileSize)
                        {
                            compColor = ConsoleThemeColor.TextBad;
                        }

                        Rug.Cmd.CmdHelper.WriteInfoToConsole(Strings.Text_FinalSize, CmdHelper.GetMemStringFromBytes(compressionProcess.FinalFileSize, true), RC.Theme[compColor]);

                        RC.WriteLine(ConsoleThemeColor.SubText, " " + new string(ConsoleChars.SingleLines[1], 41));

                        RC.Write(ConsoleVerbosity.Minimal, ConsoleThemeColor.TitleText, " " + Strings.Text_OverallCompression);

                        #endregion

                        if (compressionProcess.FinalFileSize > 0)
                        {
                            #region Print Final Success Message

                            string valueString = ((100 - (((double)compressionProcess.FinalFileSize / (double)compressionProcess.TotalInitialFileSize) * 100.0)).ToString("N2") + "%");

                            RC.Write(ConsoleVerbosity.Minimal, ConsoleThemeColor.SubText, " :".PadRight(22 - valueString.Length, '.'));
                            RC.WriteLine(ConsoleVerbosity.Minimal, compColor, valueString);

                            int offset = 6;

                            if (compColor == ConsoleThemeColor.TextBad)
                            {
                                offset += 3;
                                RC.WriteLine("");
                                RC.WriteWarning(0103, " " + Strings.Error_0103);
                            }

                            if (RC.CanManipulateBuffer && (int)RC.Verbosity >= (int)ConsoleVerbosity.Normal)
                            {
                                Rug.Cmd.CmdHelper.WriteRuglandLogo(48, RC.CursorTop - offset, 3, 2, false, ConsoleShade.Opaque);
                            }

                            #endregion
                        }
                        else
                        {
                            #region The Final Size Is Invlaid So Print A Message

                            RC.Write(ConsoleVerbosity.Minimal, ConsoleThemeColor.SubText, new string('.', 19 - Strings.Text_Invalid.Length));
                            RC.WriteLine(ConsoleVerbosity.Minimal, ConsoleThemeColor.ErrorColor1, Strings.Text_Invalid);

                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RC.WriteException(0199, ex);
            }
            finally
            {
                #region Reset The Color State
                
                RC.ColorState = state;
                RC.WriteLine(ConsoleVerbosity.Minimal, ""); 

                #endregion
            }
        }

        #endregion
    }

    class Program
    {          
        static void Main(string[] args)
        {
            RC.Theme = ConsoleColorTheme.Load((ConsoleColor)RC.ForegroundColor, (ConsoleColor)RC.BackgroundColor, ConsoleColorDefaultThemes.Colorful);

            RpxInterface.RunCommand(args);

            RC.ResetColor();
        }
    }
}
