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
    /// Interaction logic for pagCreateSnapshot.xaml
    /// </summary>
    public partial class pagCreateSnapshot : Page
    {
        public pagCreateSnapshot()
        {
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            FormHelper.ExceptionDialogHandler(() =>
            {
                var database = txtSourceDatabase.Text.SafeTrim();
                var databaseDataName = txtSourceDataname.Text.SafeTrim();
                var snapshot = txtSnapshotName.Text.SafeTrim();
                var snapshotFilename = txtSnapshotDataFilename.Text.SafeTrim();

                Requires.NotNullOrWhiteSpace(database, "Database must be selected");
                Requires.NotNullOrWhiteSpace(databaseDataName, "Database must be selected");
                Requires.NotNullOrWhiteSpace(snapshot, "Snapshot name cannot be empty");
                Requires.NotNullOrWhiteSpace(snapshotFilename, "Snapshot filename cannot be empty");

                var query = SQLQueriesHelper.GetCreateSnapshot(database, databaseDataName, snapshot, snapshotFilename);

                ComandProcessor processor = FormHelper.GetNewCommandProcessor();
                processor.CanKillExistingConnections = true;
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
                UpdateSnapshotNames();
            }
        }

        private void dgDatabases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDatabases.SelectedItem is DataRowView row)
            {
                txtSourceDatabase.Text = row.Row.Field<string>("DatabaseName") ?? string.Empty;
                txtSourceDataname.Text = row.Row.Field<string>("DatabaseDataName") ?? string.Empty;

                UpdateSnapshotNames();
            }
        }

        private async Task Refresh()
        {
            var results = await SQLQueriesHelper.GetDatabases(SettingsHelper.GetSQLConnectionDetails());

            dgDatabases.ItemsSource = results?.Tables?.FirstOrDefault()?.DefaultView;
        }

        private void UpdateSnapshotNames()
        {
            string folder = txtSnapshotFolder.Text.SafeTrim().AddTrailingDirectorySeparator();
            string database = txtSourceDatabase.Text.SafeTrim();
            string timestamp = (chkAddTimestamp.IsChecked ?? false) ? DateTime.Now.ToString("_yyyyMMdd_HHmmss") : string.Empty;
            string extension = ".ss";

            txtSnapshotName.Text = $"{database}{timestamp}";
            txtSnapshotDataFilename.Text = $"{folder}{database}{timestamp}{extension}";
        }
    }
}