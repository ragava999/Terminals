using System;
using System.Diagnostics;
using System.Windows.Forms;
using Kohl.Framework.Info;

namespace Terminals.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            this.InitializeComponent();
        }

        private void lblTerminals_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(AssemblyInfo.Url);
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            this.lblVersion.Text = AssemblyInfo.AboutText;
            DateTime dt = AssemblyInfo.BuildDate;
			this.textBox1.Text += string.Format("This version of " + Kohl.Framework.Info.AssemblyInfo.Title + " has been built for you on {0} at {1}", dt.ToLongDateString(), dt.ToLongTimeString());
			this.textBox1.Text += Environment.NewLine + Environment.NewLine + String.Format("Author: {0}", AssemblyInfo.Author);
			this.textBox1.Text += Environment.NewLine + Environment.NewLine + new HumanReadableInfo().ToString();
			}

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}