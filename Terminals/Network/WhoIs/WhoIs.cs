using System;
using System.Threading;
using System.Windows.Forms;
using Kohl.Framework.Info;
using Kohl.Framework.Localization;

namespace Terminals.Network.WhoIs
{
    public partial class WhoIs : UserControl
    {
        public WhoIs()
        {
            this.InitializeComponent();
            this.hostTextbox.Text = AssemblyInfo.Url;
            Localization.SetLanguage(this);
        }

        private void whoisButton_Click(object sender, EventArgs e)
        {
            this.whoisButton.Enabled = false;
            this.textBox2.Text = null;
            String server = this.hostTextbox.Text.Trim();
            if (server != String.Empty)
            {
                if (!server.StartsWith("=") && !server.ToLower().EndsWith(".ca"))
                    server = "=" + server;

                server = server.Replace("https://", "").Replace("http://", "").Replace("www.","");

                string result = null;

                Thread serverThread = new Thread((ThreadStart)delegate { result = WhoisResolver.Whois(server); });

                serverThread.Start();

                while (serverThread.IsAlive)
                {
                    Application.DoEvents();
                }

                if (string.IsNullOrEmpty(result))
                {
                    this.textBox2.Text = "Can't resolve host.";
                    this.whoisButton.Enabled = true;
                    return;
                }
                
                result = result.Replace("\n", Environment.NewLine);

                // Don't localize the below text -> this would break the functionality
                Int32 pos = result.IndexOf("Whois Server:");

                if (pos > 0)
                {
                    String newServer = result.Substring(pos + 13, result.IndexOf("\r\n", pos) - pos - 13);

                    if (server.StartsWith("="))
                        server = this.hostTextbox.Text.Trim();

                    string newResults = null;

                    Thread t = new Thread((ThreadStart)delegate { newResults = WhoisResolver.Whois(server, newServer.Trim()); });

                    t.Start();

                    while (t.IsAlive)
                    {
                        Application.DoEvents();
                    }

                    if (!String.IsNullOrEmpty(newResults))
                        newResults = newResults.Replace("\n", Environment.NewLine);

                    result =
                        String.Format(
                            "{0}\r\n----------------------" + Localization.Text("Network.WhoIs.WhoIs_SubQuery") +
                            ":{1}--------------------------\r\n{2}", result, newServer, newResults);
                }

                this.textBox2.Text = result;
                this.whoisButton.Enabled = true;
            }
        }

        private void hostTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.whoisButton_Click(null, null);
            }
        }
    }
}