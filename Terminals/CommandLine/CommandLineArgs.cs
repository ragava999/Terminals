using System;
using System.Runtime.Serialization;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection;
using Terminals.Connection.Manager;
using Terminals.Connections;

namespace Terminals.CommandLine
{
    /// <summary>
    ///     Container for parsed command line arguments for whole application
    /// </summary>
    [DataContract]
    public class CommandLineArgs
    {
        [DataMember] [LocalizedArgument(ArgumentType.AtMostOnce, LongName = "AutomaticallyUpdate", ShortName = "u",
            HelpText = "CommandLine.CommandLineArgs.AutomaticallyUpdate")] public bool AutomaticallyUpdate;

        [DataMember] [LocalizedArgument(ArgumentType.AtMostOnce, HelpText = "CommandLine.CommandLineArgs.Config")] public string Config;

        [DataMember]
        [LocalizedArgument(ArgumentType.AtMostOnce, HelpText = "CommandLine.CommandLineArgs.Cred")]
        public string Cred;

        [DataMember] [LocalizedArgument(ArgumentType.AtMostOnce, LongName = "console", ShortName = "c",
            HelpText = "CommandLine.CommandLineArgs.Console")] public bool Console;

        [DataMember] [LocalizedArgument(ArgumentType.AtMostOnce, LongName = "favorites",
            HelpText = "CommandLine.CommandLineArgs.Favs")] public string favorites;

        [DataMember] [LocalizedArgument(ArgumentType.AtMostOnce, LongName = "fullscreen", ShortName = "f",
            HelpText = "CommandLine.CommandLineArgs.FullScreen")] public bool Fullscreen;

        [DataMember] [LocalizedArgument(ArgumentType.AtMostOnce, ShortName = "v", HelpText = "CommandLine.CommandLineArgs.Machine")] public string Machine;

        [DataMember] [LocalizedArgument(ArgumentType.LastOccurenceWins, LongName = "protocol", ShortName = "p",
            HelpText = "CommandLine.CommandLineArgs.Protocol")] public string Protocol;

        [DataMember] [LocalizedArgument(ArgumentType.AtMostOnce, LongName = "reuse", ShortName = "r",
            HelpText = "CommandLine.CommandLineArgs.Reuse")] public bool Reuse;

        [DataMember] [LocalizedArgument(ArgumentType.AtMostOnce, HelpText = "CommandLine.CommandLineArgs.Url")] public
            string Url;

        [DataMember]
        [LocalizedArgument(ArgumentType.AtMostOnce, HelpText = "CommandLine.CommandLineArgs.UseDbFavorite", DefaultValue = false)]
        public bool UseDbFavorite;

        public string[] Favorites
        {
            get
            {
                if (this.HasFavorites)
                {
                    if (this.favorites.Contains(","))
                        return this.favorites.Split(',');

                    return new[] {this.favorites};
                }

                return new string[0];
            }
        }

        private bool HasFavorites
        {
            get { return !String.IsNullOrEmpty(this.favorites); }
        }

        public bool HasUrlDefined
        {
            get { return !String.IsNullOrEmpty(this.Url); }
        }

        public string UrlServer
        {
            get
            {
                String server;
                Int32 port;
                ProtocolHandler.Parse(this.Url, out server, out port);
                return server;
            }
        }

        public string ProtcolName
        {
            get
            {
                if (string.IsNullOrEmpty(this.Protocol))
                    if (this.HasUrlDefined)
                        return typeof (HTTPConnection).GetProtocolName();
                    else
                        return typeof (RDPConnection).GetProtocolName();

                foreach (string prot in ConnectionManager.GetProtocols())
                {
                    if (prot == this.Protocol.ToUpper())
                        return prot;
                }

                return typeof (RDPConnection).GetProtocolName();
            }
        }

        public int UrlPort
        {
            get
            {
                if (this.HasUrlDefined)
                {
                    String server;
                    Int32 port;
                    ProtocolHandler.Parse(this.Url, out server, out port);
                    return port;
                }
                return 0;
            }
        }

        public bool HasMachineDefined
        {
            get { return !String.IsNullOrEmpty(this.Machine); }
        }

        public string MachineName
        {
            get
            {
                if (this.HasMachineDefined)
                {
                    Int32 index = this.Machine.IndexOf(":");
                    if (index > 0)
                        return this.Machine.Substring(0, index);
                }

                return this.Machine;
            }
        }

        public int Port
        {
            get
            {
                if (this.HasMachineDefined)
                {
                    Int32 index = this.Machine.IndexOf(":");
                    if (index > 0)
                    {
                        Int32 port;
                        String portText = this.Machine.Substring(index + 1);
                        if (Int32.TryParse(portText, out port))
                            return port;
                    }
                }

                return 0;
            }
        }

        public bool SingleInstance
        {
            get { return Settings.SingleInstance || this.Reuse; }
        }
    }
}