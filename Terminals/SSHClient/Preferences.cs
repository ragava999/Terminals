namespace Terminals.SSHClient
{
    using System;
    using System.Windows.Forms;
    using Configuration.Files.Main.Favorites;
    using Configuration.Files.Main.Keys;

    /// <summary>
    ///     Description of Preferences.
    /// </summary>
    public partial class Preferences : UserControl
    {
        private KeysSection keysSection;

        public Preferences()
        {
            this.InitializeComponent();

            this.buttonPublicKey.Checked = true;
            this.buttonSSH2.Checked = true;
        }

        public KeysSection Keys
        {
            set
            {
                this.keysSection = value;
                foreach (KeyConfigElement k in this.keysSection.Keys)
                    this.comboBoxKey.Items.Add(k.Name);
                if (this.comboBoxKey.Items.Count > 0)
                    this.comboBoxKey.SelectedIndex = 0;
            }
        }

        public AuthMethod AuthMethod
        {
            get
            {
                if (this.buttonPublicKey.Checked)
                    return AuthMethod.PublicKey;
                if (this.buttonPassword.Checked)
                    return AuthMethod.Password;

                return AuthMethod.KeyboardInteractive;
            }
            set
            {
                switch (value)
                {
                    case AuthMethod.Password:
                        this.buttonPassword.Checked = true;
                        break;
                    case AuthMethod.PublicKey:
                        this.buttonPublicKey.Checked = true;
                        break;
                    default:
                        this.buttonKbd.Checked = true;
                        break;
                }
            }
        }

        public bool SSH1
        {
            get { return this.buttonSSH1.Checked; }
            set { this.buttonSSH1.Checked = value; }
        }

        public string KeyTag
        {
            get { return this.comboBoxKey.Text; }
            set { this.comboBoxKey.Text = value; }
        }

        public ComboBox.ObjectCollection KeyTags
        {
            get { return this.comboBoxKey.Items; }
        }

        private void ButtonGenerateKeyClick(object sender, EventArgs e)
        {
            if (this.SSH1)
            {
                this.LoadSSH1Key();
            }
            else
            {
                this.GenerateSSH2Key();
            }
        }

        private void LoadSSH1Key()
        {
            MessageBox.Show("not done yet");
        }

        private void GenerateSSH2Key()
        {
            KeyGenForm dlg = new KeyGenForm {KeyTag = this.comboBoxKey.Text};
            dlg.ShowDialog();
            if (dlg.Key != null)
            {
                string tag = dlg.KeyTag;
                int i = this.comboBoxKey.FindString(tag);
                if (i >= 0)
                {
                    this.comboBoxKey.SelectedIndex = i;
                }
                else
                {
                    this.keysSection.AddKey(tag, dlg.Key.toBase64String());
                    this.comboBoxKey.Items.Add(tag);
                    this.comboBoxKey.SelectedIndex = this.comboBoxKey.FindString(tag);
                }
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            // copy the public key to the clipboard
            this.openSSHTextBox.SelectAll();
            this.openSSHTextBox.Copy();
        }

        private void comboBoxKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tag = (string) this.comboBoxKey.SelectedItem;
            string keytext = this.keysSection.Keys[tag].Key;
            SSH2UserAuthKey key = SSH2UserAuthKey.FromBase64String(keytext);
            this.openSSHTextBox.Text = key.PublicPartInOpenSSHStyle() + " " + tag;
        }

        private void ButtonSSH1CheckedChanged(object sender, EventArgs e)
        {
			// TODO: KOHL> Import PUTTY keys!
            this.buttonGenerateKey.Text = this.buttonSSH1.Checked ? "Load" : "New";
        }
    }
}