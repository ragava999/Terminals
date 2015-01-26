namespace Rug.Cmd
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
    internal class Strings
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        internal Strings()
        {
        }

        internal static string ArgumentParser_AboutScreen_Title
        {
            get
            {
                return ResourceManager.GetString("ArgumentParser_AboutScreen_Title", resourceCulture);
            }
        }

        internal static string ArgumentParser_AboutScreen_Usage
        {
            get
            {
                return ResourceManager.GetString("ArgumentParser_AboutScreen_Usage", resourceCulture);
            }
        }

        internal static string ArgumentParser_Add_AllreadyContainsKey
        {
            get
            {
                return ResourceManager.GetString("ArgumentParser_Add_AllreadyContainsKey", resourceCulture);
            }
        }

        internal static string ArgumentParser_Add_CannotMatchSymbolForKey
        {
            get
            {
                return ResourceManager.GetString("ArgumentParser_Add_CannotMatchSymbolForKey", resourceCulture);
            }
        }

        internal static string ArgumentParser_Add_SymbolInUse
        {
            get
            {
                return ResourceManager.GetString("ArgumentParser_Add_SymbolInUse", resourceCulture);
            }
        }

        internal static string ArgumentParser_Documentation_NameForPath
        {
            get
            {
                return ResourceManager.GetString("ArgumentParser_Documentation_NameForPath", resourceCulture);
            }
        }

        internal static string ArgumentParser_DocumentationWrittenToPath
        {
            get
            {
                return ResourceManager.GetString("ArgumentParser_DocumentationWrittenToPath", resourceCulture);
            }
        }

        internal static string ArgumentParser_DocumentGenerator_Title
        {
            get
            {
                return ResourceManager.GetString("ArgumentParser_DocumentGenerator_Title", resourceCulture);
            }
        }

        internal static string ArgumentParser_InvalidArgument
        {
            get
            {
                return ResourceManager.GetString("ArgumentParser_InvalidArgument", resourceCulture);
            }
        }

        internal static string ArgumentParser_InvalidSwitch
        {
            get
            {
                return ResourceManager.GetString("ArgumentParser_InvalidSwitch", resourceCulture);
            }
        }

        internal static string Commads_CommandNotFound
        {
            get
            {
                return ResourceManager.GetString("Commads_CommandNotFound", resourceCulture);
            }
        }

        internal static string ConsoleControl_CtrlOutOfBounds
        {
            get
            {
                return ResourceManager.GetString("ConsoleControl_CtrlOutOfBounds", resourceCulture);
            }
        }

        internal static string ConsoleExt_BufferHeight_Error
        {
            get
            {
                return ResourceManager.GetString("ConsoleExt_BufferHeight_Error", resourceCulture);
            }
        }

        internal static string ConsoleExt_BufferWidth_Error
        {
            get
            {
                return ResourceManager.GetString("ConsoleExt_BufferWidth_Error", resourceCulture);
            }
        }

        internal static string ConsoleExt_CannotAcceptInput
        {
            get
            {
                return ResourceManager.GetString("ConsoleExt_CannotAcceptInput", resourceCulture);
            }
        }

        internal static string ConsoleExt_CursorManipulate_GetSetError
        {
            get
            {
                return ResourceManager.GetString("ConsoleExt_CursorManipulate_GetSetError", resourceCulture);
            }
        }

        internal static string ConsoleInterpreter_ParseColour_Error
        {
            get
            {
                return ResourceManager.GetString("ConsoleInterpreter_ParseColour_Error", resourceCulture);
            }
        }

        internal static string ConsoleInterpreter_UnexpectedEndTag
        {
            get
            {
                return ResourceManager.GetString("ConsoleInterpreter_UnexpectedEndTag", resourceCulture);
            }
        }

        internal static string ConsoleProgressBar_ValueOfMax
        {
            get
            {
                return ResourceManager.GetString("ConsoleProgressBar_ValueOfMax", resourceCulture);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        internal static string EnumSwitch_Posible
        {
            get
            {
                return ResourceManager.GetString("EnumSwitch_Posible", resourceCulture);
            }
        }

        internal static string EnumSwitch_Unknown
        {
            get
            {
                return ResourceManager.GetString("EnumSwitch_Unknown", resourceCulture);
            }
        }

        internal static string Error_0010
        {
            get
            {
                return ResourceManager.GetString("Error_0010", resourceCulture);
            }
        }

        internal static string Package_FileCouldNotBeAdded
        {
            get
            {
                return ResourceManager.GetString("Package_FileCouldNotBeAdded", resourceCulture);
            }
        }

        internal static string Package_ResolveError
        {
            get
            {
                return ResourceManager.GetString("Package_ResolveError", resourceCulture);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager("Rug.Cmd.Strings", typeof(Strings).Assembly);
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }
    }
}

