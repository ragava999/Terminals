using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Kohl.Framework.Lists;
using Kohl.Framework.Localization;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Forms.Controls;

namespace Terminals.Network.AD
{
    public partial class ImportFromAD : Form
    {
        private readonly ActiveDirectoryClient adClient;

        public ImportFromAD()
        {
            this.InitializeComponent();
            this.gridComputers.AutoGenerateColumns = false;

            this.adClient = new ActiveDirectoryClient();
            this.adClient.ListComputersDone += this.AdClient_OnListComputersDone;
            this.adClient.ComputerFound += this.OnClientComputerFound;

            SortableList<ActiveDirectoryComputer> computers = new SortableList<ActiveDirectoryComputer>();
            this.bsComputers.DataSource = computers;

            Localization.SetLanguage(this);
        }

        private void ImportFromAD_Load(object sender, EventArgs e)
        {
            this.progressBar1.Visible = false;
            this.lblProgressStatus.Text = String.Empty;

            this.domainTextbox.Text = !String.IsNullOrEmpty(Settings.DefaultDomain) ? Settings.DefaultDomain : Environment.UserDomainName;
        }

        private void ScanADButton_Click(object sender, EventArgs e)
        {
            if (!this.adClient.IsRunning)
            {
                this.bsComputers.Clear();
                this.adClient.FindComputers(this.domainTextbox.Text);
                this.lblProgressStatus.Text = Localization.Text("Network.AD.ImportFromAD.ScanADButton_Message1");
                this.SwitchToRunningMode();
            }
            else
            {
                this.adClient.Stop();
                this.lblProgressStatus.Text = Localization.Text("Network.AD.ImportFromAD.ScanADButton_Message2");
            }
        }

        private void SwitchToRunningMode()
        {
            this.progressBar1.Visible = true;
            this.ButtonScanAD.Text = Localization.Text("Network.AD.ImportFromAD.SwitchToRunningMode");
            this.btnSelectAll.Enabled = false;
            this.btnSelectNone.Enabled = false;
            this.ButtonImport.Enabled = false;
        }

        private void SwitchToStoppedMode()
        {
            this.progressBar1.Visible = false;
            this.ButtonScanAD.Text = Localization.Text("Network.AD.ImportFromAD.SwitchToStoppedMode");
            this.btnSelectAll.Enabled = true;
            this.btnSelectNone.Enabled = true;
            this.ButtonImport.Enabled = true;
        }

        private void OnClientComputerFound(ActiveDirectoryComputer computer)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ComputerFoundDelegate(this.OnClientComputerFound), new object[] {computer});
            }
            else
            {
                this.bsComputers.Add(computer);
                this.lblProgressStatus.Text =
                    String.Format(Localization.Text("Network.AD.ImportFromAD.OnClientComputerFound"),
                                  this.bsComputers.Count);
                this.gridComputers.Refresh();
            }
        }

        private void AdClient_OnListComputersDone(bool success)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ListComputersDoneDelegate(this.AdClient_OnListComputersDone), new object[] {success});
            }
            else
            {
                if (success)
                {
                    this.lblProgressStatus.Text =
                        String.Format(Localization.Text("Network.AD.ImportFromAD.OnClientComputersDone"),
                                      this.bsComputers.Count);
                }
                else
                {
                    this.lblProgressStatus.Text =
                        Localization.Text("Network.AD.ImportFromAD.OnClientComputersDone_Cancel");
                }

                this.SwitchToStoppedMode();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnButtonImportClick(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            List<FavoriteConfigurationElement> favoritesToImport =
                this.GetFavoritesFromBindingSource(this.domainTextbox.Text);
            ImportWithDialogs managedImport = new ImportWithDialogs(this);
            managedImport.Import(favoritesToImport);
        }

        private List<FavoriteConfigurationElement> GetFavoritesFromBindingSource(String domain)
        {
            return (from DataGridViewRow computerRow in this.gridComputers.SelectedRows select computerRow.DataBoundItem as ActiveDirectoryComputer into computer select computer.ToFavorite(domain)).ToList();
        }

        private void OnBtnSelectAllClick(object sender, EventArgs e)
        {
            this.gridComputers.SelectAll();
        }

        private void OnBtnSelectNoneClick(object sender, EventArgs e)
        {
            this.gridComputers.ClearSelection();
        }

        private void ImportFromAD_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.adClient.ListComputersDone -= this.AdClient_OnListComputersDone;
            this.adClient.ComputerFound -= this.OnClientComputerFound;
            this.adClient.Stop();
        }

        private void gridComputers_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn lastSortedColumn = this.gridComputers.FindLastSortedColumn();
            DataGridViewColumn column = this.gridComputers.Columns[e.ColumnIndex];

            SortOrder newSortDirection = SortableUnboundGrid.GetNewSortDirection(lastSortedColumn, column);
            SortableList<ActiveDirectoryComputer> data =
                this.bsComputers.DataSource as SortableList<ActiveDirectoryComputer>;
            this.bsComputers.DataSource = data.SortByProperty(column.DataPropertyName, newSortDirection);
            column.HeaderCell.SortGlyphDirection = newSortDirection;
        }
    }
}