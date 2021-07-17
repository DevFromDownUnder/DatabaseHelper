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
                        var snapshotName = row["SnapshotName"].ToString();
                        var databaseName = row["DatabaseName"].ToString();

                        var (query, parameters) = SQLQueriesHelper.GetRestoreSnapshot(snapshotName, databaseName);

                        ComandProcessor processor = FormHelper.GetNewCommandProcessor();
                        processor.KillExistingConnections = true;
                        processor.DatabaseToKill = databaseName;
                        processor.Queries = new[] { query };
                        processor.Parameters = parameters;

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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetDefaults();
        }

        private async Task Refresh()
        {
            var results = await SQLQueriesHelper.GetSnapshots(SettingsHelper.GetSQLConnectionDetails());

            dgDatabases.ItemsSource = results?.Tables?.FirstOrDefault()?.AddSelectColumn()?.DefaultView;
        }

        private void SetDefaults()
        {
        }
    }
}