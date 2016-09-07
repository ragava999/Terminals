namespace Terminals.Updates
{
    using System.IO;
    using System.Net;
    using System.Text;
    using Terminals.Configuration.Files.Main.Settings;

    public static class Web
    {
        /// <summary>
        ///     its a default of 1024 bytes for the buffer because of download speeds and such
        ///     some arbritray number which to buffer the downloaded content
        ///     too high of a number and the responsestream cant keep up
        ///     too low and the more loops, and ore time it takes to get the content
        ///     if your constantly tearing the same URL down you may want to test 
        ///     with different buffersizes for optimal performance
        ///     with my tests 1024 (1kb/s) was optimal for text and binary data
        /// </summary>
        private const int BufferSize = 1024;

        /// <summary>
        ///     Generic HTTP String Reader
        /// </summary>
        private static string HTTPAsString(string URL, byte[] Data, bool DoPOST)
        {
            return Encoding.ASCII.GetString(HTTPAsBytes(URL, Data, DoPOST));
        }

        private static void SetCredentials(IWebProxy webProxy)
        {
            // if UseAuto := false -> no credentials will be used
            // if UseAuto := true -> use Windows default credentials
            if (Settings.ProxyUseAuth)
            {
            	webProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            	//webProxy.UseDefaultCredentials = true;
            }
            
            if (Settings.ProxyUseAuthCustom)
            {
            	webProxy.Credentials = new NetworkCredential("", "");
            }    
        }

        private static WebResponse HTTPAsWebResponse(string URL, byte[] Data,  bool DoPOST)
        {
			// Proxy auto detection is true in .NET by default
        	bool UseProxy_AutoDetect = false;
			string Username = Settings.ProxyUserName;
			string Password = Settings.ProxyPassword;
			string Domain = Settings.ProxyDomainName;
			string ProxyAddress = string.Empty;
			int ProxyPort = 0;

            if (Settings.ProxyUse)
            {
                ProxyAddress = Settings.ProxyAddress;
                ProxyPort = Settings.ProxyPort;
                UseProxy_AutoDetect = Settings.ProxyAutoDetect;
            }

            if (!DoPOST && Data != null && Data.Length > 0)
            {
                string restoftheurl = Encoding.ASCII.GetString(Data);
                if (URL.IndexOf("?") <= 0)
                    URL = URL + "?";

                URL = URL + restoftheurl;
            }

            WebRequest wreq = WebRequest.Create(URL);
            wreq.Method = "GET";
            if (DoPOST)
                wreq.Method = "POST";

			if (Settings.ProxyUse)
			if (ProxyAddress != null && ProxyAddress.Trim () != string.Empty && ProxyPort > 0) {
				// Proxy autodetection is default in .NET
				// if neither set nor null -> auto detected proxy will be used.
				if (!UseProxy_AutoDetect) {
					WebProxy webProxy = new WebProxy (ProxyAddress, ProxyPort) { BypassProxyOnLocal = true };
					SetCredentials (webProxy);
					wreq.Proxy = webProxy;
				}
			} else {
				// Proxy autodetection is default in .NET
				// if neither set nor null -> auto detected proxy will be used.
				if (!UseProxy_AutoDetect) {
					wreq.Proxy = WebRequest.DefaultWebProxy;
					SetCredentials (wreq.Proxy);
				}
			}
			// Nothing has been changed in the proxy option panel ->
			// use the default IE settings with the default auth.
			else if (Settings.ProxyAutoDetect) {
				wreq.Proxy = WebRequest.DefaultWebProxy;
				SetCredentials (wreq.Proxy);
			}
			else
				wreq.Proxy = null;
	        
            if (Username != null && Password != null && Domain != null && Username.Trim() != string.Empty &&
                Password.Trim() != null && Domain.Trim() != null)
                wreq.Credentials = new NetworkCredential(Username, Password, Domain);
            else if (Username != null && Password != null && Username.Trim() != string.Empty && Password.Trim() != null)
                wreq.Credentials = new NetworkCredential(Username, Password);

            if (DoPOST && Data != null && Data.Length > 0)
            {
                wreq.ContentType = "application/x-www-form-urlencoded";
                Stream request = wreq.GetRequestStream();
                request.Write(Data, 0, Data.Length);
                request.Close();
            }

            WebResponse wrsp = wreq.GetResponse();
            return wrsp;
        }

        private static byte[] ConvertWebResponseToByteArray(WebResponse res)
        {
            BinaryReader br = new BinaryReader(res.GetResponseStream());

            // Download and buffer the binary stream into a memory stream
            MemoryStream stm = new MemoryStream();
            int pos = 0;
            int maxread = BufferSize;
            while (true)
            {
                byte[] content = br.ReadBytes(maxread);
                if (content.Length <= 0)
                    break;

                if (content.Length < maxread)
                    maxread = maxread - content.Length;

                stm.Write(content, 0, content.Length);
                pos += content.Length;
            }

            br.Close();
            stm.Position = 0;
            byte[] final = new byte[(int) stm.Length];
            stm.Read(final, 0, final.Length);
            stm.Close();
            return final;
        }

        /// <summary>
        ///     Generic HTTP Byte Array Reader
        /// </summary>
        private static byte[] HTTPAsBytes(string URL, byte[] Data, bool DoPOST)
        {
            WebResponse res = HTTPAsWebResponse(URL, Data, DoPOST);
            return ConvertWebResponseToByteArray(res);
        }

        /// <summary>
        ///     Save content at any URL to disk, either with a POST or a GET
        /// </summary>
        private static bool SaveHTTPToFile(string URL, byte[] Data, string Filename, bool DoPost)
        {
            byte[] data = HTTPAsBytes(URL, Data, DoPost);
            if (data != null)
            {
                FileStream fs = new FileStream(Filename, FileMode.Create);
                fs.Write(data, 0, data.Length);
                fs.Close();
                return true;
            }

            return false;
        }

		public static string HTTPAsString(string URL)
		{
			return HTTPAsString(URL, null, false);
		}

        public static bool SaveHTTPToFile(string URL, string Filename)
        {
			return SaveHTTPToFile(URL, null, Filename, false);
        }

        /// <summary>
        ///     Upload content to HTTP
        /// </summary>
        public static bool SendFileToHTTP(string URL, string Filename, bool DoPost)
        {
            if (!File.Exists(Filename))
                return false;

            FileStream fs = new FileStream(Filename, FileMode.Open);
            byte[] d = new byte[(int) fs.Length];
            fs.Read(d, 0, d.Length);
            fs.Close();
            if (d != null)
            {
                HTTPAsBytes(URL, d, DoPost);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}