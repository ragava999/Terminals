using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using Kohl.Framework.Localization;
using Kohl.Framework.Logging;

namespace Terminals.Network.DNS
{
    public partial class DnsLookup : UserControl
    {
        public DnsLookup()
        {
            this.InitializeComponent();
            Localization.SetLanguage(this);
        }

        public void ForceDNS(string Host)
        {
            this.hostnameTextBox.Text = Host;
            this.lookupButton_Click(null, null);
        }

        private void lookupButton_Click(object sender, EventArgs e)
        {
            string serverIP = this.serverComboBox.Text.Trim();

            if (serverIP == "")
            {
                MessageBox.Show("Please enter the DNS server you want to lookup!");
                return;
            }

            this.serverComboBox.Text = serverIP.Trim();

            string domain = this.hostnameTextBox.Text.Trim();

            if (domain == "")
            {
                MessageBox.Show("Please enter the domain you want to lookup!");
                return;
            }

            domain = domain.Replace("https://", "").Replace("http://", "").Replace("www.", "");

            this.hostnameTextBox.Text = domain.Trim();

            try
            {
                List<Answer> responses = new List<Answer>();

                IPAddress dnsServer = IPAddress.Parse(serverIP.Contains(": ") ? serverIP.Split(new string[] { ": " }, StringSplitOptions.None)[1] : serverIP);
                // create a DNS request
                Request request = new Request();
                request.AddQuestion(new Question(domain, DnsType.ANAME, DnsClass.IN));
                responses.Add(Resolver.Lookup(request, dnsServer).Answers[0]);

                request = new Request();
                request.AddQuestion(new Question(domain, DnsType.MX, DnsClass.IN));
                responses.Add(Resolver.Lookup(request, dnsServer).Answers[0]);

                request = new Request();
                request.AddQuestion(new Question(domain, DnsType.NS, DnsClass.IN));
                responses.Add(Resolver.Lookup(request, dnsServer).Answers[0]);

                request = new Request();
                request.AddQuestion(new Question(domain, DnsType.SOA, DnsClass.IN));
                responses.Add(Resolver.Lookup(request, dnsServer).Answers[0]);

                this.dataGridView1.DataSource = responses;
                //this.propertyGrid1.SelectedObject = records;
                // send it to the DNS server and get the response
                //
                //this.dataGridView1.DataSource = response.Answers;
            }
            catch (Exception exc)
            {
                Log.Info("Could not resolve host.", exc);
                MessageBox.Show("Could not resolve host.");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void DNSLookup_Load(object sender, EventArgs e)
        {
            this.serverComboBox.DataSource = AdapterInfo.DnsServers;
        }

        private void hostnameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.lookupButton_Click(null, null);
            }
        }
    }
}