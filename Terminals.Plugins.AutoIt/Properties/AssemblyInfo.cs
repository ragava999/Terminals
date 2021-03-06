﻿using System.Reflection;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Terminals.Plugins.AutoIt")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Oliver Kohl D.Sc.")]
[assembly: AssemblyProduct("Terminals.Plugins.AutoIt by Oliver Kohl D.Sc.")]
[assembly: AssemblyCopyright("Copyright © Oliver Kohl D.Sc. 2016")]
[assembly: AssemblyTrademark("http://www.kohl.bz")]
[assembly: AssemblyCulture("")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//

[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]
[assembly: AssemblyInformationalVersion("2.0.0 RTM")]

/*
AssemblyVersion

Where other assemblies that reference your assembly will look. If this number changes, other assemblies have to update their references to your assembly! The AssemblyVersion is required.

I use the format: major.minor. This would result in:
[assembly: AssemblyVersion("1.0")] 

 AssemblyFileVersion

Used for deployment. You can increase this number for every deployment. It is used by setup programs. Use it to mark assemblies that have the same AssemblyVersion, but are generated from different builds.

In Windows, it can be viewed in the file properties.

If possible, let it be generated by MSBuild. The AssemblyFileVersion is optional. If not given, the AssemblyVersion is used.

I use the format: major.minor.revision.build, where I use revision for development stage (Alpha, Beta, RC and RTM), service packs and hot fixes. This would result in:
[assembly: AssemblyFileVersion("1.0.3100.1242")] 

 AssemblyInformationalVersion

The Product version of the assembly. This is the version you would use when talking to customers or for display on your website. This version can be a string, like '1.0 Release Candidate'. Unfortunately, when you use a string, it will generate a false warning -- already reported to Microsoft (fixed in VS2010). The AssemblyInformationalVersion is optional. If not given, the AssemblyVersion is used.

I use the format: major.minor [revision as string]. This would result in:
[assembly: AssemblyInformationalVersion("1.0 RC1")] 
*/