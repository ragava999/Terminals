namespace Terminals.Plugins.AutoIt.Panels.OptionPanels
{
    using Terminals.Connection.Panels.OptionPanels;
    using MainSettings = Configuration.Files.Main.Settings;

    public partial class AutoItOptionPanel : OptionPanel
    {
        #region Constructor (1)
        public AutoItOptionPanel()
        {
            this.InitializeComponent();
        }
        #endregion

        #region Public Methods (2)
        public override void LoadSettings()
        {
            this.txtAutoItPath.Text = Settings.AutoItProgramPath();
        }

        public override void SaveSettings()
        {
            Settings.AutoItProgramPath(this.txtAutoItPath.Text);
        }
        #endregion

        #region Private Methods (1)
        private void btnBrowse_Click(object sender, System.EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txtAutoItPath.Text = openFileDialog1.FileName;
            }
        }
        #endregion
    }
}