using System.IO;
using Terminals.Connection.Panels.FavoritePanels;
using Terminals.Connection.Panels.OptionPanels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Kohl.Framework.Info;
using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Credentials;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.TabControl;

namespace Terminals.Connection.Manager
{
    public static class ConnectionManager
    {
        #region Private Fields (8)
        private static List<ushort> ports;
        private static List<string> protocols;
        private static List<string> protocolsCamalCase;
        private static List<Type> types;
        private delegate bool IsSpecificType(Type type);
        private static List<Type> optionDialogs = null;
        private static List<Type> afterConnectSupportTypes = null;
        private static List<Type> favoritesPanels = null;
        #endregion

        public struct ParsingConstants
        {
            public const string ParserStart = "{%";

            public const string ParserEnd = "%}";

            public const string ParserSeperator = ":";

            internal const string UserNameMiddle = "USER";
            public const string UserName = ParserStart + UserNameMiddle + ParserEnd;

            internal const string PasswordMiddle = "PASSWORD";
            public const string Password = ParserStart + PasswordMiddle + ParserEnd;

            internal const string DomainNameMiddle = "DOMAIN";
            public const string DomainName = ParserStart + DomainNameMiddle + ParserEnd;

            internal const string UserNameWithDomainMiddle = "USER+DOMAIN";
            public const string UserNameWithDomain = ParserStart + UserNameWithDomainMiddle + ParserEnd;

            internal const string DateTimeMiddle = "DT";
            public const string DateTime = ParserStart + DateTimeMiddle + ParserEnd;

            public const string Click = ParserStart + "CLICK" + ParserEnd;

            public const string Script = ParserStart + "SCRIPT" + ParserEnd;

            public const string Redirect = ParserStart + "REDIRECT" + ParserEnd;

            public const string WeekNumber = ParserStart + "WeekNumber" + ParserEnd;

            public static string GetDateTimeFormat(string locale, string seperator = " ", bool useShortDatePattern = true, bool useShortTimePattern = true)
            {
                System.Globalization.CultureInfo localeDependent = new System.Globalization.CultureInfo(locale);

                string localeDependentDateFormatString = useShortDatePattern ? localeDependent.DateTimeFormat.ShortDatePattern : localeDependent.DateTimeFormat.LongDatePattern;
                string localeDependentTimeFormatString = useShortTimePattern ? localeDependent.DateTimeFormat.ShortTimePattern : localeDependent.DateTimeFormat.LongTimePattern;

                return ParserStart + DateTimeMiddle + ParserSeperator + localeDependentDateFormatString + seperator + localeDependentTimeFormatString + ParserEnd;
            }
        }

        #region Public Properties (2)
        /// <summary>
        /// A list of forbidden features.
        /// </summary>
        public static string[] Limit { set; get; }

        public static string[] ProtocolsCamalCase
        {
            get
            {
                if (protocolsCamalCase == null)
                {
                    protocolsCamalCase = new List<string>();

                    string[] protocols = GetProtocols();

                    foreach (string protocol in protocols)
                    {
                        string tmp = protocol.ToLower();
                        tmp = tmp[0].ToString().ToUpper() + tmp.Remove(0, 1);
                        protocolsCamalCase.Add(tmp);
                    }

                    protocolsCamalCase.Sort();
                }

                return protocolsCamalCase.ToArray();
            }
        }
        #endregion

        #region Public Methods (14)
        /// <summary>
        /// Extension method that returns the tool tip text.
        /// </summary>
        /// <param name="favorite"></param>
        /// <returns></returns>
        public static String GetToolTipText(this FavoriteConfigurationElement favorite)
        {
            string captionComputer = "Computer: ";
            string captionURL = "Url: ";

            bool isUrl = false;

            string serverName = favorite.GetServersAndIPs(ref isUrl);

            if (isUrl)
                serverName = captionURL + serverName;
            else
                serverName = captionComputer + serverName;

            if (!string.IsNullOrEmpty(serverName) && serverName.Length > 100)
                serverName = serverName.Remove(90, serverName.Length - 90) + " ...";

            String toolTip = string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(serverName.Replace(captionComputer, string.Empty).Replace(captionURL, string.Empty)) ? string.Empty : serverName + Environment.NewLine;

            if (favorite.Port > 0)
                toolTip += "Port: " + favorite.Port + Environment.NewLine;

            if (Credential.GetCredentials(favorite).IsSetUserNameAndDomainName)
                toolTip += "User: " + Credential.GetCredentials(favorite).UserNameWithDomain + Environment.NewLine;

            if (Settings.ShowFullInformationToolTips)
            {
                string notes = string.IsNullOrEmpty(favorite.Notes) ? "-" : favorite.Notes;

                if (!string.IsNullOrEmpty(notes) && notes != "-")
                {
                    bool appendWithDots = false;

                    string[] notesArray = notes.Split('\n');

                    if (notesArray.Length == 1 && notes.Length > 300)
                    {
                        notes = notes.Remove(300, notes.Length - 300);
                        appendWithDots = true;
                    }

                    if (notesArray.Length > 5)
                    {
                        notes = notesArray[0] +
                            notesArray[1] +
                            notesArray[2] +
                            notesArray[3] +
                            notesArray[4].TrimEnd();

                        appendWithDots = true;
                    }

                    if (appendWithDots)
                        notes += " ...";
                }

                toolTip += String.Format("Tag: {1}{0}Connect to Console: {2}{0}Notes: {3}", Environment.NewLine, string.IsNullOrEmpty(favorite.Tags) ? "-" : favorite.Tags, favorite.ConnectToConsole, notes);
            }

            return toolTip;
        }

        public static PopupTerminal CreateConnectionInPopup(IHostingForm parentForm, string protocol, string name, string url = null, string server = null)
        {
            if (url == null && server == null)
                throw new Exception("Either the url or the server must be set.");

            return CreateConnectionInPopup(parentForm, new FavoriteConfigurationElement
                                                           {
                                                               Protocol = protocol,
                                                               Url = url,
                                                               ServerName = server,
                                                               Name = name
                                                           });
        }

        public static PopupTerminal CreateConnectionInPopup(IHostingForm parentForm, FavoriteConfigurationElement favorite)
        {
            TerminalTabControlItem item = CreateTerminalTabPageByFavoriteName(favorite);
            CreateConnection(favorite, parentForm, false, item);
            PopupTerminal popup = new PopupTerminal();
            popup.AddTerminal(item);
            return popup;
        }

        /// <summary>
        ///     Creates a new connection by spawning a new thread.
        /// </summary>
        /// <param name="favorite"> </param>
        /// <param name="TerminalTabPage"> </param>
        /// <param name="parentForm"> </param>
        /// <remarks>
        ///     This method calls the <see cref="ConnectionBase.Connect" /> method and requires it to be thread safe.
        /// </remarks>
        public static void CreateConnection(FavoriteConfigurationElement favorite, IHostingForm parentForm, bool waitforEnd, TerminalTabControlItem terminalTabPage, ConnectionBase conn = null)
        {
            // This might happen if the user is not allowed to
            // use all available connections e.g. 
            // if the user has a freeware version.
            if (Limit.Contains(favorite.Protocol.ToUpper()) || terminalTabPage == null)
            {
                MessageBox.Show("You are not allowed to use that kind of connection! Please upgrade your license.", AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (!waitforEnd)
            {
               Thread t = new Thread((ThreadStart)delegate
                                                    {
                                                        Code(terminalTabPage, parentForm, favorite, conn);
                                                    });

                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
            else
            {
                Code(terminalTabPage, parentForm, favorite, conn);
            }
        }

        public static ushort[] GetPorts()
        {
            if (ports == null)
            {
                ports = new List<ushort>();

                foreach (Type type in GetConnectionTypes())
                {
                    Connection con = (Connection)Activator.CreateInstance(type);

                    if (con.Port != 0 && !ports.Contains(con.Port))
                        ports.Add(con.Port);
                }
            }

            return ports.ToArray();
        }

        public static ushort GetPort(string name)
        {
            return GetConnection(name).Port;
        }

        public static string[] GetProtocols()
        {
            if (protocols == null)
            {
                protocols = new List<string>();

                foreach (Type type in GetConnectionTypes())
                {
                    if (!type.IsAbstract)
                        protocols.Add(type.GetProtocolName());
                }
            }

            return protocols.ToArray();
        }

        public static bool IsEqual(this Type type, string protocolName)
        {
            return type.GetProtocolName().Equals(protocolName.ToUpper());
        }

        public static string GetProtcolNameCamalCase(string protocolName)
        {
            string[] protocolsCamalCase = ProtocolsCamalCase;

            foreach (string camalCase in protocolsCamalCase)
            {
                if (protocolName.ToUpper() == camalCase.ToUpper())
                    return camalCase;
            }

            return protocolName;
        }

        public static string GetProtocolName(this Type type)
        {
            return type.Name.ToUpper().Replace("CONNECTION", "").Replace("FAVORITEPANEL", "");
        }

        public static List<Type> GetFavoritePanels()
        {
            return GetConnectionTypes(ref favoritesPanels, IsFavoritePanelType);
        }

        public static List<Type> GetConnectionTypes()
        {
            return GetConnectionTypes(ref types, IsConnectionType);
        }

        public static List<Type> GetOptionDialogTypes()
        {
            return GetConnectionTypes(ref optionDialogs, IsOptionDialogType);
        }

        public static List<Type> GetIAfterConnectSupportTypes()
        {
            if (afterConnectSupportTypes == null)
            {
                List<Type> types = GetConnectionTypes();

                afterConnectSupportTypes = (from t in types
                                            where t.FindInterfaces(new TypeFilter(InterfaceFilter), (object)"IAfterConnectSupport").ToList().Count > 0
                                            select t).ToList<Type>();
            }

            return afterConnectSupportTypes;
        }

        public static string ParseValue(Credential credential, string value)
        {
            if (string.IsNullOrEmpty(value) || credential == null)
                return value;

            string output = value;

            // Get the week number
            System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            string weekNo = ci.Calendar.GetWeekOfYear(DateTime.Now, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek).ToString();

            // loop five times and replace a maximum of 5 occurences per FIELDCONSTANT
            for (int i = 0; i < 5; i++)
            {
                output = ReplaceEx(output, ParsingConstants.WeekNumber, weekNo);
                output = ReplaceEx(output, ParsingConstants.UserName, credential.UserName);
                output = ReplaceEx(output, ParsingConstants.Password, credential.Password);
                output = ReplaceEx(output, ParsingConstants.DomainName, credential.DomainName);
                output = ReplaceEx(output, ParsingConstants.UserNameWithDomain, credential.UserNameWithDomain);
                output = ReplaceEx(output, ParsingConstants.DateTime, DateTime.Now.ToString());

                // date time feature
                int startIndex = output.IndexOf(ParsingConstants.ParserStart + ParsingConstants.DateTimeMiddle + ParsingConstants.ParserSeperator);
                if (startIndex > -1)
                {
                    // Unable to pare logic incorrect
                    if (output.IndexOf(ParsingConstants.ParserEnd, startIndex) < output.IndexOf(ParsingConstants.ParserStart, startIndex))
                        continue;

                    int start = startIndex + (ParsingConstants.ParserStart + ParsingConstants.DateTimeMiddle + ParsingConstants.ParserSeperator).Length;
                    int end = output.IndexOf(ParsingConstants.ParserEnd, startIndex);

                    // Parsing error
                    if (start < 0 || end < 0)
                    {
                        continue;
                    }

                    // get the date time format string
                    string format = output.Substring(start, end - start);

                    // format the date time string
                    output = ReplaceEx(output, ParsingConstants.ParserStart + ParsingConstants.DateTimeMiddle + ParsingConstants.ParserSeperator + format + ParsingConstants.ParserEnd, DateTime.Now.ToString(format));
                }
            }

            return output;
        }
        #endregion

        #region Private Methods (12)
        private static void Code(TerminalTabControlItem terminalTabPage, IHostingForm parentForm, FavoriteConfigurationElement favorite, ConnectionBase conn = null)
        {
            if (conn == null)
            {
                conn = CreateConnection(favorite);
                conn.TerminalTabPage = terminalTabPage;
                terminalTabPage.TabColor = FavoriteConfigurationElement.TranslateColor(favorite.TabColor);
                terminalTabPage.Connection = conn;
            }

            conn.Favorite = favorite;
            conn.ParentForm = parentForm;

            if (conn.Connect())
            {
                if (conn.InvokeRequired)
                    conn.Invoke(new MethodInvoker(delegate
                    {
                        conn.BringToFront();
                        conn.Update();
                    }));
                else
                {
                    conn.BringToFront();
                    conn.Update();
                }
                
                if (parentForm.InvokeRequired)
                    parentForm.Invoke(new MethodInvoker(delegate
                    {
                        parentForm.UpdateControls();

                        if (favorite.DesktopSize == DesktopSize.FullScreen)
                            parentForm.FullScreen = true;
                    }));
                else
                {
                    parentForm.UpdateControls();

                    if (favorite.DesktopSize == DesktopSize.FullScreen)
                        parentForm.FullScreen = true;
                }

                conn.AfterConnectPlugins();
            }
            else
            {
            	string message = "Sorry, " + AssemblyInfo.Title + " was unable to create the connection. Try again or check the log for more information.";

                Log.Error(message);
                MessageBox.Show(message, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (parentForm.InvokeRequired)
                    parentForm.Invoke(new MethodInvoker(delegate { parentForm.RemoveAndUnSelect(terminalTabPage); }));
                else
                    parentForm.RemoveAndUnSelect(terminalTabPage);
            }

            if (conn.Connected && favorite.NewWindow)
            {
                if (parentForm.InvokeRequired)
                    parentForm.Invoke(new MethodInvoker(delegate { parentForm.DetachTabToNewWindow(terminalTabPage); }));
                else
                    parentForm.DetachTabToNewWindow(terminalTabPage);
            }
        }

        private static bool InterfaceFilter(Type typeObj, Object criteriaObj)
        {
            if (typeObj.Name == criteriaObj.ToString())
                return true;
            else
                return false;
        }

        private static List<Type> GetConnectionTypes(ref List<Type> types, IsSpecificType isSpecificType)
        {
            if (types == null)
            {
                // Load internal types
                types = GetConnectionTypes(AssemblyInfo.Assembly, isSpecificType);

                // Load external types
                string pluginPath = Path.Combine(AssemblyInfo.Directory, "Plugins");

                // Check if the plugin folder exists
                if (Directory.Exists(pluginPath))
                {
                    // Get all dlls in the plugin folder and under it.
                    var assemblies = from assembly in new DirectoryInfo(pluginPath).GetFiles("*.dll", SearchOption.AllDirectories) where !assembly.FullName.ToUpperInvariant().StartsWith(AssemblyInfo.UpgradeDirectory.ToUpperInvariant()) select assembly.FullName;

                    // Now load the external types and add them to the list.
                    foreach (string assemblyFile in assemblies)
                    {
                        try
                        {
                            types.AddRange(GetConnectionTypes(Assembly.LoadFile(assemblyFile), isSpecificType));
                        }
                        catch (Exception ex)
                        {
                            Log.Debug("Ignoring native assembly " + assemblyFile, ex);
                        }
                    }
                }
            }

            return types;
        }

        private static List<Type> GetConnectionTypes(Assembly assembly, IsSpecificType isSpecificType)
        {
        	if (Limit == null)
        		Limit = new string[0];
        	
            IEnumerable<Type> a = null;
            try
            {
                a =
                    from t in assembly.GetTypes()
                    where t != null && t.IsClass && t.IsPublic
                    select t;
            }
            catch (ReflectionTypeLoadException ex)
            {
                a =
                    from t in ex.Types
                    where t != null && t.IsClass && t.IsPublic
                    select t;
            }


            List<Type> types = new List<Type>();
            
            foreach (Type type in a)
            {
            	if (type != null)
	                if (isSpecificType(type))//(IsConnectionType(type))
	                {
	                    // If the user is not allowed to
	                    // use all available connections e.g. 
	                    // if the user has a freeware version.
	                    if (!Limit.Contains(GetProtocolName(type)))
	                    {
	                    	types.Add(type);
	                    }
	                }
            }

            return types;
        }
        
        public static TerminalTabControlItem CreateTerminalTabPageByFavoriteName(FavoriteConfigurationElement favorite)
        {
            String terminalTabTitle = favorite.Name;
            if (Settings.ShowUserNameInTitle)
            {
                Credential.GetCredentials(favorite).AppendTabPageTitleWithUserName(ref terminalTabTitle);
            }

            return new TerminalTabControlItem(terminalTabTitle, favorite.Name);
        }

        private static string GetServersAndIPs(this FavoriteConfigurationElement favorite, ref bool isUrl)
        {
            isUrl = true;

            if (favorite == null || string.IsNullOrEmpty(favorite.Url))
                isUrl = false;

            string serverName = string.IsNullOrEmpty(favorite.Url) ? favorite.ServerName : favorite.Url;

            if (string.IsNullOrEmpty(favorite.Url) && !string.IsNullOrEmpty(favorite.ServerName))
            {
                // throws a SocketException but the message is set to The requested name is valid, but no data of the requested type was found.
                // Both the stack traces are the same in each program, but the first ones error is WSAHOST_NOT_FOUND where the second one is
                // WSANO_DATA.
                // There's many reasons why a SocketException will occur in GetHostAddresses. 
                // In the case of WSA_NODATA, that means there is an entry in DNS; but no IP address (i.e. no A record).
                // You may get a different error if you can't reach the DNS server but the data is cached. Or, you may get a different
                // error if the DNS server is unreachable and the data isn't cached. Etc. Unfortunately GetHostAddresses wrap all those
                // errors in one exception type. If you want to do something different for each type of error, you'll have to check a
                // relevant property in SocketException. If you just want to detect that "it didn't work", then you don't have to check.
                // But, it's clear that SocketException message is different depending on the failure so I wouldn't recommend just using Message.
                // Not to mention, that will get translated on other language versions of Windows--so, it might look strange if the rest of your
                // application is English, except for this message. Technically it could have one of the following errors: WSANOTINITIALIZED,
                // WSAENETDOWN, WSAHOST_NOT_FOUND, WSATRY_AGAIN, WSANO_RECOVERY, WSANO_DATA, WSAEINPROGRESS, WSAEFAULT and WSAEINTR.
                // Although some are extremely unlikely. I also think you'll get different messages depending on whether IPv6 or IPv4 is in play.
                try
                {
                    System.Net.IPAddress[] addresses = System.Net.Dns.GetHostAddresses(favorite.ServerName);

                    if (addresses != null)
                    {
                        serverName = "";
                        if (addresses.Length > 1)
                            foreach (var address in addresses)
                            {
                                string hostName = System.Net.Dns.GetHostEntry(address).HostName;
                                string ip = address.ToString();

                                serverName += hostName + " [" + ip + "], ";
                            }
                        else
                        {
                            string ip = addresses[0].ToString();
                            string hostName = ip;

                            try
                            {
                                hostName = System.Net.Dns.GetHostEntry(addresses[0]).HostName.ToUpper();
                            }
                            catch (Exception ex)
                            { 
                                Log.Warn("Error resolving host name: \"" + favorite.ServerName + "\", connection name: \"" + favorite.Name + "\". " + ex.Message); 
                            }

                            if (hostName == ip || string.IsNullOrEmpty(hostName))
                                if (favorite.ServerName != ip && !string.IsNullOrEmpty(hostName))
                                    serverName += favorite.ServerName + " [" + ip + "]";
                                else
                                    serverName += ip;
                            else
                                serverName += hostName + " [" + ip + "]";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("The following error occured while trying to resolve the connection's server or ip: " + ex.Message + ". Host name: \"" + favorite.ServerName + "\". Connection name: \"" + favorite.Name  + "\". ");
                    return serverName;
                }
            }

            return serverName;
        }

        private static ConnectionBase CreateConnection(FavoriteConfigurationElement favorite)
        {
            return GetConnection(favorite.Protocol);
        }

        public static Connection GetConnection(string name)
        {
			try
			{
            	return (from type in GetConnectionTypes() where type.GetProtocolName() == name.ToUpper() select (Connection)Activator.CreateInstance(type)).FirstOrDefault();
			}
			catch
			{
				Log.Debug("Type for connection '" + name +  "' can't be found in the file system or can't be instantiated.");
				return null;
			}
        }

        private static bool IsFavoritePanelType(Type type)
        {
            return IsType(type, typeof(FavoritePanel));
        }

        private static bool IsConnectionType(Type type)
        {
            return IsType(type, typeof(Connection));
        }

        private static bool IsOptionDialogType(Type type)
        {
            return IsType(type, typeof(OptionPanel));
        }

        private static bool IsType(Type type, Type comparisonType)
        {
            do
            {
                if (type == typeof(object) || type == comparisonType)
                    break;

                if (type.BaseType == comparisonType)
                    return true;

                type = type.BaseType;
            } while (true);

            return false;
        }
        #endregion

        private static string ReplaceEx(string original, string pattern, string replacement)
        {
            if (string.IsNullOrEmpty(replacement) || string.IsNullOrEmpty(original) || string.IsNullOrEmpty(pattern))
                return original;

            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) *
                      (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];

            while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                    chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i)
                chars[count++] = original[i];
            return new string(chars, 0, count);
        }
    }
}