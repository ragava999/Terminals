using System;
using System.Windows.Forms;
using Granados.PKI;

namespace Terminals.SSHClient
{
    public partial class KeyGenForm : Form
    {
        private String _OpenSSHstring;
        private bool _gotKey;
        private SSH2UserAuthKey _key;
        private MouseEventHandler _mmhandler;

        public KeyGenForm()
        {
            this.InitializeComponent();
            this.buttonSave.Enabled = false;
            this.algorithmBox.SelectedIndex = 0;
            this.bitCountBox.SelectedIndex = 0;
            this.labelfingerprint.Hide();
            this.labelpublicKey.Hide();
            this.labelRandomness.Hide();
            this.labelTag.Hide();
            this.textBoxTag.Hide();
            this.progressBarGenerate.Hide();
            this.publicKeyBox.Hide();
            this.fingerprintBox.Hide();
            this._gotKey = false;
        }

        public string KeyTag
        {
            get { return this.textBoxTag.Text; }
            set { this.textBoxTag.Text = value; }
        }

        public SSH2UserAuthKey Key
        {
            get { return this._key; }
        }

        public bool needMoreEntropy
        {
            get { return this._gotKey == false; }
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            PublicKeyAlgorithm algorithm;
            algorithm = this.algorithmBox.Text == "RSA" ? PublicKeyAlgorithm.RSA : PublicKeyAlgorithm.DSA;
            this.labelfingerprint.Hide();
            this.labelpublicKey.Hide();
            this.labelTag.Hide();
            this.textBoxTag.Hide();
            this.publicKeyBox.Hide();
            this.fingerprintBox.Hide();
            this.labelRandomness.Show();
            this.progressBarGenerate.Show();
            this._gotKey = false;
            this._key = null;
            this._OpenSSHstring = "";
            KeyGenThread t = new KeyGenThread(this, algorithm, Int32.Parse(this.bitCountBox.Text));
            this._mmhandler = t.OnMouseMove;
            this.progressBarGenerate.MouseMove += this._mmhandler;
            t.Start();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this._key = null;
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //Terminals.Configuration.Settings.Settings.AddSSHKey(textBoxTag.Text,_key);
            this.Close();
        }

        private void hideGenKey()
        {
            this.progressBarGenerate.Value = this.progressBarGenerate.Maximum;
            this.progressBarGenerate.Value = this.progressBarGenerate.Maximum;
            this.progressBarGenerate.Hide();
            this.labelRandomness.Hide();
        }

        private void showKey()
        {
            this._OpenSSHstring = this._key.PublicPartInOpenSSHStyle();
            String alg;
            alg = this._key.Algorithm == PublicKeyAlgorithm.DSA ? "dsa" : "rsa";
            DateTime n = DateTime.Now;
            String comment = alg + "-key-" + n.ToString("yyyyMMdd");
            this.labelpublicKey.Show();
            this.publicKeyBox.Text = this._OpenSSHstring + " " + this.textBoxTag.Text;
            this.publicKeyBox.Show();
            //labelfingerprint.Show();
            //fingerprintBox.Text = _key.
            //fingerprintBox.Show();
            this.labelTag.Show();
            this.textBoxTag.Show();
            this.textBoxTag.Text = comment;
        }

        public void SetProgressValue(int v)
        {
            if (v < this.progressBarGenerate.Maximum)
                this.progressBarGenerate.Value = v;
            if (this._key != null)
            {
                this.hideGenKey();
                this.showKey();
                this.progressBarGenerate.MouseMove -= this._mmhandler;
                this.buttonSave.Enabled = true;
                this._gotKey = true;
            }
        }

        public void SetResultKey(SSH2UserAuthKey k)
        {
            this._key = k;
        }

        private void textBoxTag_TextChanged(object sender, EventArgs e)
        {
            if (this.textBoxTag.Text == "")
            {
                this.buttonSave.Enabled = false;
            }
            else
            {
                this.publicKeyBox.Text = this._OpenSSHstring + " " + this.textBoxTag.Text;
                if (this.buttonSave.Enabled)
                    this.buttonSave.Enabled = true;
            }
        }
    }
}