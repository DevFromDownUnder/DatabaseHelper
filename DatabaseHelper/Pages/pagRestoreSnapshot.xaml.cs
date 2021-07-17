using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseHelper.Pages
{
    /// <summary>
    /// Interaction logic for pagRestoreSnapshot.xaml
    /// </summary>
    public partial class pagRestoreSnapshot : Page
    {
        public pagRestoreSnapshot()
        {
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            FormHelper.ExceptionDialogHandler(() =>
            {
                if (dgDatabases.SelectedItem != null)
                {
                    if (dgDatabases.SelectedItem is System.Data.DataRowView row)
                    {
                        var snapshot = row["SnapshotName"].ToString();
                        var database = row["DatabaseName"].ToString();

                        var query = SQLQueriesHelper.GetRestoreSnapshot(snapshot, database);

                        ComandProcessor processor = FormHelper.GetNewCommandProcessor();
                        processor.KillExistingConnections = true;
                        processor.DatabasesToKill = new[] { database };
                        processor.Queries = new[] { query };

                        processor.ShowDialog();
                    }
                }
            });
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await FormHelper.ExceptionDialogHandlerAsync(
                FormHelper.LoadingFlatDarkBgButton(Refresh(), (Button)sender)
            );
        }

        private async Task Refresh()
        {
            var results = await SQLQueriesHelper.GetSnapshots(SettingsHelper.GetSQLConnectionDetails());

            dgDatabases.ItemsSource = results?.Tables?.FirstOrDefault()?.AddSelectColumn()?.DefaultView;
        }
    }
}