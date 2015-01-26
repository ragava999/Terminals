using System.Linq;

namespace Terminals.Forms.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Kohl.Framework.Localization;
    using Terminals.Configuration.Files.Main.Favorites;
    using Terminals.Configuration.Files.Main.Settings;

    /// <summary>
    ///     Calls import, asking user what to do, if there are duplicit items to import.
    ///     Handles showing result message box after import and wait cursor for import source form.
    /// </summary>
    public class ImportWithDialogs
    {
        private static readonly string importSuffix = Localization.Text("Forms.Controls.ImportWithDialogs_Imported");
        private readonly Form sourceForm;

        public bool SaveInDB { get; set; }

        public ImportWithDialogs(Form sourceForm)
        {
            this.sourceForm = sourceForm;
        }

        public Boolean Import(List<FavoriteConfigurationElement> favoritesToImport)
        {
            this.sourceForm.Cursor = Cursors.WaitCursor;
            bool imported = this.ImportPreservingNames(favoritesToImport);
            this.sourceForm.Cursor = Cursors.Default;
            if (imported)
                ShowImportResultMessage(favoritesToImport.Count);

            return imported;
        }

        private static void ShowImportResultMessage(Int32 importedItemsCount)
        {
            string message = Localization.Text("Forms.Controls.ImportWithDialogs.ShowImportResultMessage_Singular");

            if (importedItemsCount > 1)
                message =
                    String.Format(Localization.Text("Forms.Controls.ImportWithDialogs.ShowImportResultMessage_Plural"),
                                  importedItemsCount);

            MessageBox.Show(message, Localization.Text("Forms.Controls.ImportWithDialogs.ShowImportResultMessage"),
                            MessageBoxButtons.OK);
        }

        private Boolean ImportPreservingNames(List<FavoriteConfigurationElement> favoritesToImport)
        {
            List<FavoriteConfigurationElement> conflictingFavorites = GetConflictingFavorites(favoritesToImport);
            DialogResult renameAnswer = AskIfOverwriteOrRename(conflictingFavorites.Count);
            return this.PerformImport(favoritesToImport, conflictingFavorites, renameAnswer);
        }

        private Boolean PerformImport(List<FavoriteConfigurationElement> favoritesToImport,
                                      List<FavoriteConfigurationElement> conflictingFavorites, DialogResult renameAnswer)
        {
            if (renameAnswer == DialogResult.Yes)
                this.RenameConflictingFavorites(conflictingFavorites);

            if (renameAnswer != DialogResult.Cancel)
            {
                Settings.AddFavorites(favoritesToImport);
                return true;
            }

            return false;
        }

        private void RenameConflictingFavorites(List<FavoriteConfigurationElement> conflictingFavorites)
        {
            FavoriteConfigurationElementCollection savedFavorites = Settings.GetFavorites(SaveInDB);
            foreach (FavoriteConfigurationElement favoriteToRename in conflictingFavorites)
            {
                this.AddImportSuffixToFavorite(favoriteToRename, savedFavorites);
            }
        }

        private void AddImportSuffixToFavorite(FavoriteConfigurationElement favoriteToRename,
                                               FavoriteConfigurationElementCollection savedFavorites)
        {
            favoriteToRename.Name += importSuffix;
            if (savedFavorites[favoriteToRename.Name] != null)
                this.AddImportSuffixToFavorite(favoriteToRename, savedFavorites);
        }

        private static DialogResult AskIfOverwriteOrRename(Int32 conflictingFavoritesCount)
        {
            DialogResult overwriteResult = DialogResult.No;

            if (conflictingFavoritesCount > 0)
            {
                overwriteResult =
                    MessageBox.Show(
                        String.Format(
                            Localization.Text("Forms.Controls.ImportWithDialogs.AskIfOverwriteOrRename_Message"),
                            conflictingFavoritesCount, importSuffix),
                        Localization.Text("Forms.Controls.ImportWithDialogs.AskIfOverwriteOrRename_Caption"),
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            }

            return overwriteResult;
        }

        private List<FavoriteConfigurationElement> GetConflictingFavorites(
            List<FavoriteConfigurationElement> favorites)
        {
            FavoriteConfigurationElementCollection savedFavorites = Settings.GetFavorites(SaveInDB);
            return favorites.Where(favorite => savedFavorites[favorite.Name] != null).ToList();
        }
    }
}