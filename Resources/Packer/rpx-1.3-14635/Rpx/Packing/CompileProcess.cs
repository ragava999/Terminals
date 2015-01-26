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
using System.Text;
using System.IO;
using System.Reflection;
using System.IO.Packaging;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Xml;
using Rug.Cmd;
using Rpx.Packing.Embedded;
using Rpx.Packing.PEFile;
using Rug.Packing.Reflection;

namespace Rpx.Packing
{
    #region Suport Classes and Enumerations

    internal enum ExecutableType { Console, Forms }
    internal enum ExecutableTypeMode { Default, Reflected, Explicit } 
    internal enum AssemblyInfoMode { Reflected, Explicit, SourceFile } 

    #endregion 

    internal class CompileProcess : MarshalByRefObject
    {
        #region Public Members

        public readonly PackingProcess Compression;
        public readonly List<string> Assemblys = new List<string>();
        public readonly Dictionary<string, string> Tools = new Dictionary<string, string>();

        public bool LogToConsole = false;
        public bool Debug = false;

        public ExecutableTypeMode ExecutableTypeLookupMode;
        public ExecutableType ExecutableType;
        public bool PassArguments;
        public string InitialAssemblyPath;
        public string IconFile;
        public bool DeleteIconWhenDone = false; 
        public string AssembyInfoSourceFilePath;
        public AssemblyInfo ExplicitInfo;
        public bool Decorate = true;

        public long FinalFileSize = 0;

        #endregion

        #region Properties

        public AssemblyInfoMode GetAssembyInfoFrom
        {
            get
            {
                if (Helper.IsNotNullOrEmpty(AssembyInfoSourceFilePath))
                    return AssemblyInfoMode.SourceFile;
                else if (ExplicitInfo != null)
                    return AssemblyInfoMode.Explicit;
                else
                    return AssemblyInfoMode.Reflected;
            }
        }

        public long InitialSize
        {
            get
            {
                return Compression.TotalInitialSize;
            }
        }

        public long IconFileSize
        {
            get
            {
                if (Helper.IsNotNullOrEmpty(IconFile))
                {
                    if (File.Exists(IconFile))
                        return new FileInfo(IconFile).Length;
                }

                return 0;
            }
        }

        public long TotalInitialFileSize
        {
            get { return InitialSize + IconFileSize; }
        }

        public long OverheadSize
        {
            get
            {
                return FinalFileSize - (Compression.CompressedSize + IconFileSize);
            }
        }

        #endregion

        #region Constructors

        public CompileProcess()
        {
            Compression = new PackingProcess();
        }

        public CompileProcess(XmlNode node, PackingProcess compressionSettings)
        {
            Compression = compressionSettings;

            // ExecutableType
            // PassArguments
            // Debug
            // InitialAssemblyPath
            // IconFile
            // Assemblys
            // AssembyInfoSourceFilePath
            // ExplicitInfo
            // Decorate
            // Tools
        }

        #endregion

        #region Process 
        
        public bool RunProcess(string outputFile)
        {
            string replacedFile = null;
            bool allGood = false;

            #region Keep House First

            try
            {
                RC.WriteLine(ConsoleVerbosity.Debug, ConsoleThemeColor.TitleText, "\n" + Rpx.Strings.Compiler_CleaningJunkFiles);

                foreach (string str in Directory.GetFiles(Application.UserAppDataPath, "*.junk"))
                {
                    RC.WriteLine(ConsoleVerbosity.Debug, ConsoleThemeColor.Text, " " + str);
                    File.Delete(str);
                }
            }
            catch (Exception ex)
            {
                RC.WriteError(01, Rpx.Strings.Compiler_CannotDeleteJunkFiles + " " + ex.Message);
            }

            #endregion

            #region Check Archive

            string archivePath = Compression.OutputFile;

            if (Helper.IsNullOrEmpty(archivePath))
            {
                RC.WriteError(02, Rpx.Strings.Compiler_ResourceNotDefined);
                return false;
            }

            if (!File.Exists(archivePath))
            {
                RC.WriteError(03, string.Format(Rpx.Strings.Compiler_ResourceDoesNotExist, archivePath));
                return false;
            }

            #endregion


            RC.WriteLine(ConsoleVerbosity.Verbose, "");
            RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.TitleText, Rpx.Strings.Compiler_CompileStart);

            RC.WriteLine(ConsoleThemeColor.SubText, " " + new string(ConsoleChars.SingleLines[1], 41));

            try
            {
                #region Get the Assembly Info

                AssemblyInfo assemblyInfo = null;

                if (Helper.IsNotNullOrEmpty(InitialAssemblyPath))
                {
                    try
                    {
                        if (ExecutableTypeLookupMode == ExecutableTypeMode.Reflected || Helper.IsNullOrEmpty(this.IconFile))
                        {
                            RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.TitleText, " Reading PE file header");

                            ReflectPESubsytemType(archivePath, PackageHelper.MakeUriSafe(InitialAssemblyPath), this);
                        }

                        if (GetAssembyInfoFrom == AssemblyInfoMode.Reflected)
                        {
                            RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.TitleText, " Reflecting assembly info");

                            assemblyInfo = ReflectAssemblyInfo(archivePath, PackageHelper.MakeUriSafe(InitialAssemblyPath), this);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (RC.ShouldWrite(ConsoleVerbosity.Verbose))
                        {
                            RC.WriteException(03, Rpx.Strings.Error_0108, ex);
                        }
                        else
                        {
                            RC.WriteError(03, Rpx.Strings.Error_0108 + "\n" + ex.Message);
                        }

                        return false;
                    }
                }
                #endregion

                allGood = Compile(archivePath, outputFile, ref replacedFile, assemblyInfo);
            }
            catch (Exception ex)
            {
                RC.WriteException(03, ex);
            }
            finally
            {
                #region Clean Up

                if (replacedFile != null)
                {
                    if (allGood)
                    {
                        #region What Does This Achieve?

                        File.SetAttributes(replacedFile, FileAttributes.Temporary);
                        //File.Delete(replacedFile);
                        replacedFile = null;

                        #endregion
                    }
                    else
                    {
                        #region Rollback the Assembly

                        RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.TitleText, "\n" + Rpx.Strings.Compiler_RestoreBackupAsm);
                        RC.WriteLine(ConsoleVerbosity.Debug, ConsoleThemeColor.Text, " " + outputFile);

                        File.Move(replacedFile, outputFile);
                        File.SetAttributes(outputFile, FileAttributes.Normal);
                        replacedFile = null;

                        #endregion
                    }
                }

                #region Clean Up Temp Files

                if (archivePath != null)
                {
                    RC.WriteLine(ConsoleVerbosity.Debug, ConsoleThemeColor.TitleText, "\n" + Rpx.Strings.Compiler_DeleteTempArchive);
                    RC.WriteLine(ConsoleVerbosity.Debug, ConsoleThemeColor.Text, " " + archivePath);

                    File.SetAttributes(archivePath, FileAttributes.Normal);
                    File.Delete(archivePath);
                }

                if (DeleteIconWhenDone)
                {
                    RC.WriteLine(ConsoleVerbosity.Debug, ConsoleThemeColor.Text, " " + IconFile);

                    File.SetAttributes(IconFile, FileAttributes.Normal);
                    File.Delete(IconFile);
                }

                #endregion

                #endregion
            }

            return allGood;
        }

        #endregion 

        #region Compile

        private bool Compile(string archivePath, string outputFile, ref string replacedFile, AssemblyInfo assemblyInfo)
        {
            bool allGood = false; 

            #region Compile

            List<string> defines = new List<string>();
            Dictionary<string, string> replacements = new Dictionary<string, string>();

            #region Create the CodeDomProvider

            FileInfo fileInfo = new FileInfo(outputFile);

            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            CodeDomProvider csc = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();

            cp.GenerateExecutable = true;
            cp.OutputAssembly = outputFile;
            cp.CompilerOptions = " /filealign:512 /optimize+ /platform:x86";

            if (Helper.IsNotNullOrEmpty(IconFile))
                cp.CompilerOptions += string.Format(" /win32icon:\"{0}\"", IconFile);

            if (Debug)
            {
                defines.Add("DEBUG");
                cp.CompilerOptions += " /debug+ /debug:full";
            }

            #endregion

            #region Defines

            defines.Add("RPX");

            string targetName;

            ConsoleThemeColor targetColour = ConsoleThemeColor.SubTextGood; 

            switch (ExecutableType)
            {
                case ExecutableType.Console:
                    cp.CompilerOptions += " /target:exe";
                    targetName = Rpx.Strings.Compiler_ConsoleApplication;
                    break;
                case ExecutableType.Forms:
                    cp.CompilerOptions += " /target:winexe";
                    defines.Add("WinExec");
                    targetName = Rpx.Strings.Compiler_FormsApplication;
                    break;
                default:
                    cp.CompilerOptions += " /target:exe";
                    targetName = Rpx.Strings.Compiler_DefaultApplication;
                    targetColour = ConsoleThemeColor.SubTextBad; 
                    break;
            }

            if (RC.ShouldWrite(ConsoleVerbosity.Verbose))
            {
                RC.WriteLine(ConsoleVerbosity.Verbose, targetColour, " - " + string.Format(Rpx.Strings.Compiler_AssemblyTarget, targetName));
            }
            else
            {
                RC.WriteLine(ConsoleVerbosity.Minimal, targetColour, " " + string.Format(Rpx.Strings.Compiler_AssemblyTarget, targetName));
            }

            if (Decorate)
                defines.Add("Decorate");

            if (Assemblys.Count > 1 || Tools.Count > 0)
                defines.Add("HasAdditionalAssemblys");

            if (Tools.Count > 0)
                defines.Add("IsToolkit");

            if (PassArguments)
                defines.Add("PassArguments");

            if (GetAssembyInfoFrom != AssemblyInfoMode.SourceFile)
            {
                defines.Add("ExpliciteInfo");

                replacements.Add("[Asm_Title]", Helper.MakeNonNull(assemblyInfo.Title));

                string description = Helper.MakeNonNull(assemblyInfo.Description);

                if (Helper.IsNotNullOrEmpty(description)) //  Asm_Description))
                    description += "\n";

                string watermark = "RPX " + typeof(CompileProcess).Assembly.GetName().Version.ToString();

                if (!description.Contains(watermark))
                    description += watermark;

                replacements.Add("[Asm_Description]", Helper.MakeNonNullAndEscape(description));
                replacements.Add("[Asm_Configuration]", Helper.MakeNonNullAndEscape(assemblyInfo.Configuration));
                replacements.Add("[Asm_Company]", Helper.MakeNonNullAndEscape(assemblyInfo.Company));
                replacements.Add("[Asm_Product]", Helper.MakeNonNullAndEscape(assemblyInfo.Product));
                replacements.Add("[Asm_Copyright]", Helper.MakeNonNullAndEscape(assemblyInfo.Copyright));
                replacements.Add("[Asm_Trademark]", Helper.MakeNonNullAndEscape(assemblyInfo.Trademark));
                replacements.Add("[Asm_Culture]", Helper.MakeNonNullAndEscape(assemblyInfo.Culture));
                replacements.Add("[Asm_Version]", Helper.MakeNonNullAndEscape(assemblyInfo.Version));
                replacements.Add("[Asm_FileVersion]", Helper.MakeNonNullAndEscape(assemblyInfo.FileVersion));

                //RC.WriteLine(ConsoleVerbosity.Normal, ConsoleColorExt.Gray, " " + Rpx.Strings.Compiler_AssemblyInfoHasBeenReflected);

                RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubText, " - " + Rpx.Strings.AssemblyInfo_Product.PadRight(16) + ": " + Helper.MakeNonNullAndEscape(assemblyInfo.Product));
                RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubText, " - " + Rpx.Strings.AssemblyInfo_Version.PadRight(16) + ": " + Helper.MakeNonNullAndEscape(assemblyInfo.Version));
                RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubText, " - " + Rpx.Strings.AssemblyInfo_FileVersion.PadRight(16) + ": " + Helper.MakeNonNullAndEscape(assemblyInfo.FileVersion));
                RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubText, " - " + Rpx.Strings.AssemblyInfo_Configuration.PadRight(16) + ": " + Helper.MakeNonNullAndEscape(assemblyInfo.Configuration));

                RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubText, " - " + Rpx.Strings.AssemblyInfo_Company.PadRight(16) + ": " + Helper.MakeNonNullAndEscape(assemblyInfo.Company));
                RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubText, " - " + Rpx.Strings.AssemblyInfo_Description.PadRight(16) + ": " + Helper.MakeNonNullAndEscape(assemblyInfo.Description));

                RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubText, " - " + Rpx.Strings.AssemblyInfo_Copyright.PadRight(16) + ": " + Helper.MakeNonNullAndEscape(assemblyInfo.Copyright));
                RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubText, " - " + Rpx.Strings.AssemblyInfo_Trademark.PadRight(16) + ": " + Helper.MakeNonNullAndEscape(assemblyInfo.Trademark));
                RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubText, " - " + Rpx.Strings.AssemblyInfo_Culture.PadRight(16) + ": " + Helper.MakeNonNullAndEscape(assemblyInfo.Culture));

            }
            else
            {
                RC.WriteLine(ConsoleVerbosity.Normal, ConsoleThemeColor.Text, " " + Rpx.Strings.Compiler_AssemblyInfoFromFile);
                RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubText, " - " + AssembyInfoSourceFilePath);
                // 
            }


            if (Compression.Protected)
            {
                if (Helper.IsNotNullOrEmpty(Compression.Password))
                {
                    RC.WriteLine(ConsoleVerbosity.Normal, ConsoleThemeColor.Text, " " + Rpx.Strings.Compiler_IncludeCryptographyCode);

                    defines.Add("UseCryptography");
                }
                else
                {
                    RC.WriteLine(ConsoleVerbosity.Normal, ConsoleThemeColor.Text, " " + Rpx.Strings.Compiler_IncludeDisguiseCode);
                }

                defines.Add("Hidden");
            }

            foreach (string define in defines)
                cp.CompilerOptions += " /define:" + define;

            #endregion

            #region Include the Archive Resource

            string archiveName = "a.zip";

            if (Compression.Protected)
            {
                archiveName = "a";
            }

            #region Finalise Package

            Compression.FinalisePackage(Compression.OutputFile);

            #endregion

            cp.CompilerOptions += string.Format(" /resource:\"{0}\",{1}", archivePath, archiveName);

            #endregion

            #region Add Default Refrences

            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            cp.CompilerOptions += string.Format(" /reference:\"{0}\"", typeof(Package).Assembly.Location);

            #endregion

            #region Define Remaining Code Template Replacements

            if (GetAssembyInfoFrom == AssemblyInfoMode.SourceFile)
            {
                replacements.Add("[#AssemblyInfo.cs#]", File.ReadAllText(AssembyInfoSourceFilePath).Replace("using System.Reflection;", ""));
            }

            replacements.Add("#ResourceLocation#", archiveName);

            if (Compression.Protected)
            {
                if (Helper.IsNotNullOrEmpty(Compression.Password))
                {
                    replacements.Add("[#PasswordVector#]", Helpers.EncryptHelper.InitVector);
                    replacements.Add("[#PasswordSalt#]", Compression.SaltValue);
                    replacements.Add("[#Password#]", Compression.Password);
                    replacements.Add("[#PasswordStrength#]", "2");
                }
            }

            List<Dictionary<string, string>> AssemblyReplacements = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> ToolReplacements = new List<Dictionary<string, string>>();

            #region Build Assembly Code

            int asmId = 0;

            if (Assemblys.Count > 1)
            {
                foreach (string asm in Assemblys)
                {
                    string path = PackageHelper.MakeUriSafe(asm.Substring(asm.LastIndexOf("\\") + 1));

                    if (path != InitialAssemblyPath)
                    {
                        Dictionary<string, string> asmReplacements = new Dictionary<string, string>();

                        string idStr = "a" + (asmId++).ToString();

                        asmReplacements.Add("[#Asm_IdStr#]", idStr);
                        asmReplacements.Add("[#Asm_Path#]", PackageHelper.MakeUriSafe(path));
                        AssemblyReplacements.Add(asmReplacements);
                    }
                }
            }

            #endregion

            #region Build Tools Code

            if (Tools.Count > 0)
            {
                RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.TitleText, "\n" + Rpx.Strings.Compiler_Toolkit);

                if (RC.ShouldWrite(ConsoleVerbosity.Normal))
                {
                    RC.WriteLine(ConsoleVerbosity.Minimal, ConsoleThemeColor.Text, " " + string.Format(Rpx.Strings.Compiler_Tools, Tools.Count.ToString()));
                }
                else if (RC.ShouldWrite(ConsoleVerbosity.Minimal))
                {
                    RC.Write(ConsoleVerbosity.Minimal, ConsoleThemeColor.TitleText, " " + string.Format(Rpx.Strings.Compiler_Tools, Tools.Count.ToString()));
                    RC.Write(ConsoleVerbosity.Minimal, ConsoleThemeColor.SubText, " (");
                }

                StringBuilder ToolsString = new StringBuilder();

                bool firstMenu = true;

                foreach (KeyValuePair<string, string> tool in Tools)
                {
                    string toolName = tool.Key;
                    string asm = tool.Value;
                    string path = PackageHelper.MakeUriSafe(asm.Substring(asm.LastIndexOf("\\") + 1));

                    if (path != InitialAssemblyPath)
                    {
                        Dictionary<string, string> asmReplacements = new Dictionary<string, string>();
                        Dictionary<string, string> toolReplacements = new Dictionary<string, string>();

                        string idStr = "a" + (asmId++).ToString();

                        asmReplacements.Add("[#Asm_IdStr#]", idStr);
                        asmReplacements.Add("[#Asm_Path#]", PackageHelper.MakeUriSafe(path));

                        AssemblyReplacements.Add(asmReplacements);

                        toolReplacements.Add("[#Tool_Name#]", toolName);
                        toolReplacements.Add("[#Tool_Asm#]", idStr);

                        ToolReplacements.Add(toolReplacements);

                        if (RC.ShouldWrite(ConsoleVerbosity.Normal))
                        {
                            RC.Write(ConsoleVerbosity.Normal, ConsoleThemeColor.SubText, " - " + toolName.PadRight(15, ' ') + " ");
                            RC.WriteLine(ConsoleVerbosity.Normal, ConsoleThemeColor.SubText2, "(" + path + ")");
                        }
                        else if (RC.ShouldWrite(ConsoleVerbosity.Minimal))
                        {
                            if (!firstMenu)
                                RC.Write(ConsoleVerbosity.Minimal, ConsoleThemeColor.SubText, ", ");

                            RC.Write(ConsoleVerbosity.Minimal, ConsoleThemeColor.SubText2, toolName);

                            firstMenu = false;
                        }

                        ToolsString.Append("\\n  " + toolName);
                    }
                }

                if (!RC.ShouldWrite(ConsoleVerbosity.Normal) && RC.ShouldWrite(ConsoleVerbosity.Minimal))
                {
                    RC.WriteLine(ConsoleVerbosity.Minimal, ConsoleThemeColor.SubText, ")");
                }

                replacements.Add("[#ToolsString#]", ToolsString.ToString());
            }

            #endregion

            replacements.Add("#InitialAssemblyPath#", PackageHelper.MakeUriSafe(InitialAssemblyPath));

            #endregion

            #region Rename the Output File If It Exits So It Can Be Rolled Back

            if (File.Exists(outputFile))
            {
                RC.WriteLine(ConsoleVerbosity.Debug, ConsoleThemeColor.TitleText, "\n" + Rpx.Strings.Compiler_BackupAsm);

                string name = new FileInfo(outputFile).Name;

                replacedFile = Application.UserAppDataPath + @"\" + Guid.NewGuid().GetHashCode().ToString() + "_" + name + ".InUse";

                RC.WriteLine(ConsoleVerbosity.Debug, ConsoleThemeColor.Text, " " + replacedFile);

                File.Move(outputFile, replacedFile);
            }

            #endregion

            #region Compile Assembly From Code Template

            RC.WriteLine(ConsoleVerbosity.Debug, ConsoleThemeColor.TitleText, "\n" + Rpx.Strings.Compiler_Compiling);
            RC.WriteLine(ConsoleVerbosity.Debug, ConsoleThemeColor.Text, string.Format(" {0}: {1}", Rpx.Strings.Compiler_Options, cp.CompilerOptions.Replace("/", "\n    /")));

            CodeFileBuilder builder = GetCodeFileBuilderFromResource("Rpx.Packing.Embedded.RpxWrapperSource.xml");

            Dictionary<string, List<Dictionary<string, string>>> ObjReplacements = new Dictionary<string, List<Dictionary<string, string>>>();

            ObjReplacements.Add("Assemblys", AssemblyReplacements);
            ObjReplacements.Add("Tools", ToolReplacements);

            string source = builder.BuildCodeFile(replacements, ObjReplacements, defines);

            // Compile standalone executable with input files embedded as resource
            CompilerResults cr = csc.CompileAssemblyFromSource(cp, source);

            // yell if compilation error
            if (cr.Errors.Count > 0)
            {
                if (replacedFile != null)
                {
                    File.Move(replacedFile, outputFile);
                    replacedFile = null;
                }

                string msg = string.Format(Rpx.Strings.Compiler_BuildErrors, cr.PathToAssembly);

                foreach (CompilerError ce in cr.Errors)
                {
                    msg += Environment.NewLine + ce.ToString();
                }

                if (!LogToConsole)
                    MessageBox.Show(msg);
                else
                {
                    RC.WriteError(02, string.Format(Rpx.Strings.Compiler_CompileFailed, cr.Errors.Count));
                    RC.WriteLine(ConsoleThemeColor.ErrorColor2, msg);

                    if (RC.Verbosity == ConsoleVerbosity.Debug)
                    {
						if (RC.IsBuildMode == false) RC.Write(ConsoleThemeColor.SubTextBad, new string(ConsoleChars.GetShade(ConsoleShade.Dim), RC.BufferWidth));
						else RC.Write(ConsoleThemeColor.SubTextBad, new string('*', RC.BufferWidth));

                        RC.WriteLine();
                        RC.WriteLine(ConsoleThemeColor.Text, source.Replace("\t", "  "));
                        RC.WriteLine();
						
						if (RC.IsBuildMode == false) RC.Write(ConsoleThemeColor.SubTextBad, new string(ConsoleChars.GetShade(ConsoleShade.Dim), RC.BufferWidth));
						else RC.Write(ConsoleThemeColor.SubTextBad, new string('*', RC.BufferWidth));
                    }
                }
            }
            else
            {
                if (RC.Verbosity == ConsoleVerbosity.Debug)
                {
                    RC.Write(ConsoleThemeColor.SubTextGood, new string(ConsoleChars.GetShade(ConsoleShade.Dim), RC.BufferWidth));
                    RC.WriteLine();
                    RC.WriteLine(ConsoleThemeColor.Text, source.Replace("\t", "  "));
                    RC.WriteLine();
                    RC.Write(ConsoleThemeColor.SubTextGood, new string(ConsoleChars.GetShade(ConsoleShade.Dim), RC.BufferWidth));
                }

                FinalFileSize = new FileInfo(outputFile).Length;

                if (replacedFile != null)
                {                    
                    File.SetAttributes(replacedFile, FileAttributes.Temporary);

                    string junkFile = replacedFile.Substring(0, replacedFile.Length - 6) + ".junk";

                    RC.WriteLine(ConsoleVerbosity.Debug, ConsoleThemeColor.TitleText, "\n" + Rpx.Strings.Compiler_JunkingBackupAsm);
                    RC.WriteLine(ConsoleVerbosity.Debug, ConsoleThemeColor.Text, " " + junkFile);

                    File.Move(replacedFile, junkFile);
                    replacedFile = null;
                }

				allGood = true;
            }

            #endregion

            #endregion

            return allGood; 
        }

        #endregion

        #region Get CodeFileBuilder From Resource

        private CodeFileBuilder GetCodeFileBuilderFromResource(string resourceName)
        {
            Assembly assem = this.GetType().Assembly;
            using (Stream stream = assem.GetManifestResourceStream(resourceName))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(reader.ReadToEnd());

                        CodeFileBuilder builder = new CodeFileBuilder(doc);

                        return builder;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format(Rpx.Strings.Compiler_ErrorGettingResource, resourceName, e.ToString()));
                }
            }
        }

        #endregion

        #region Assebly Info Helpers

        private void ReflectPESubsytemType(string archivePath, string uri, CompileProcess compileProcess)
        {
            try
            {
                string iconOutputPath = null;

                if (Helper.IsNullOrEmpty(compileProcess.IconFile))
                {
                    iconOutputPath = Application.UserAppDataPath + @"\" + Guid.NewGuid().GetHashCode().ToString() + ".ico";

                    compileProcess.IconFile = iconOutputPath;
                    compileProcess.DeleteIconWhenDone = true; 
                }
                
                PEFileInfo info;

				if (!PEFileInterrogator.TryGetPEFileInfo(archivePath, uri, iconOutputPath, out info))
				{ 
                    throw new Exception(string.Format(Rpx.Strings.Reflector_CouldNotReadPE, uri));
				}

                if (iconOutputPath != null && !File.Exists(iconOutputPath))
                {
                    compileProcess.IconFile = null;
                    compileProcess.DeleteIconWhenDone = false;
                }

                if (compileProcess.ExecutableTypeLookupMode == ExecutableTypeMode.Reflected)
                {
                    if (info.Subsystem == SubsystemTypes.WindowsCni)
                        compileProcess.ExecutableType = ExecutableType.Console;
                    else if (info.Subsystem == SubsystemTypes.WindowsGui)
                        compileProcess.ExecutableType = ExecutableType.Forms;
                    else
                    {
                        SubsystemTypes SubsystemType = info.Subsystem;

                        throw new Exception(string.Format(Rpx.Strings.Error_0107, SubsystemType.ToString()));
                    }
                }
            }
            finally
            {                
            }
        }

        private AssemblyInfo ReflectAssemblyInfo(string archivePath, string uri, CompileProcess compileProcess)
        {
            AssemblyInfo info = new AssemblyInfo();

            AppDomain domain = null;

            try
            {
                string appDomainName = "Interrogator";                

                AppDomainSetup domainSetup = new AppDomainSetup();
                domainSetup.ApplicationName = appDomainName;
                domainSetup.ApplicationBase = new FileInfo(Application.ExecutablePath).DirectoryName; 

                domain = AppDomain.CreateDomain(appDomainName, null, domainSetup);

                AssemblyInterrogator remoteWorker = (AssemblyInterrogator)domain.CreateInstanceAndUnwrap(typeof(AssemblyInterrogator).Assembly.FullName, typeof(AssemblyInterrogator).FullName);

				string errorMessage, exceptionMessage;

				if (!remoteWorker.GetAssemblyInfo(archivePath, uri, out info, out errorMessage, out exceptionMessage))
                {
					if (RC.ShouldWrite(ConsoleVerbosity.Debug) && String.IsNullOrEmpty(exceptionMessage) == false)
					{
						throw new Exception(errorMessage + Environment.NewLine + exceptionMessage);
					}
					else
					{
						throw new Exception(errorMessage); 
					}
                    //throw new Exception(string.Format("{0} : {1}", Rpx.Strings.Compiler_UnableToLoadAsm, uri));					
                }
            
                info = AssemblyInfo.Clone(info); 

                compileProcess.PassArguments = info.PassArgs;
            }
            finally
            {
                if (domain != null)
                    AppDomain.Unload(domain);
            }

            return info;
        }

        #endregion
    }
}
