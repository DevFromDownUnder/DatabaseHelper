using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using Microsoft;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseHelper.Pages
{
    /// <summary>
    /// Interaction logic for pagBackupDatabase.xaml
    /// </summary>
    public partial class pagBackupDatabase : Page
    {
        public pagBackupDatabase()
        {
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            FormHelper.ExceptionDialogHandler(() =>
            {
                var database = txtSourceDatabase.Text.SafeTrim();
                var backupFilename = txtBackupDataFilename.Text.SafeTrim();
                var compress = chkCompress.IsChecked ?? false;
                var copyOnly = chkCopyOnlyBackup.IsChecked ?? false;

                Requires.NotNullOrWhiteSpace(database, "Database must be selected");
                Requires.NotNullOrWhiteSpace(backupFilename, "Backup filename cannot be empty");

                var query = SQLQueriesHelper.GetBackupDatabase(database, backupFilename, copyOnly, compress);

                ComandProcessor processor = FormHelper.GetNewCommandProcessor();
                processor.KillExistingConnections = false;
                processor.DatabasesToKill = new[] { database };
                processor.Queries = new[] { query };

                processor.ShowDialog();
            });
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await FormHelper.ExceptionDialogHandlerAsync(
                FormHelper.LoadingFlatDarkBgButton(Refresh(), (Button)sender)
            );
        }

        private void chkAddTimestamp_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (dgDatabases.SelectedItem is DataRowView)
            {
                UpdateBackupNames();
            }
        }

        private void dgDatabases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDatabases.SelectedItem is DataRowView row)
            {
                txtSourceDatabase.Text = row.Row.Field<string>("DatabaseName") ?? string.Empty;
                txtSourceDataname.Text = row.Row.Field<string>("DatabaseDataName") ?? string.Empty;

                UpdateBackupNames();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetDefaults();
        }

        private async Task Refresh()
        {
            var results = await SQLQueriesHelper.GetDatabases(SettingsHelper.GetSQLConnectionDetails());

            dgDatabases.ItemsSource = results?.Tables?.FirstOrDefault()?.AddSelectColumn()?.DefaultView;
        }

        private void SetDefaults()
        {
            chkCopyOnlyBackup.IsChecked = true;
            chkCompress.IsChecked = true;
        }

        private void UpdateBackupNames()
        {
            string folder = txtBackupFolder.Text.SafeTrim().AddTrailingDirectorySeparator();
            string database = txtSourceDatabase.Text.SafeTrim();
            string timestamp = (chkAddTimestamp.IsChecked ?? false) ? DateTime.Now.ToString("_yyyyMMdd_HHmmss") : string.Empty;
            string extension = ".bak";

            txtBackupName.Text = $"{database}{timestamp}";
            txtBackupDataFilename.Text = $"{folder}{database}{timestamp}{extension}";
        }
    }
}