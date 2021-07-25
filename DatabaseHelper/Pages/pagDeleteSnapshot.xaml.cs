using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseHelper.Pages
{
    /// <summary>
    /// Interaction logic for pagDeleteSnapshot.xaml
    /// </summary>
    public partial class pagDeleteSnapshot : Page
    {
        public pagDeleteSnapshot()
        {
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            FormHelper.ExceptionDialogHandler(() =>
            {
                if (dgDatabases.ItemsSource != null)
                {
                    if (dgDatabases.ItemsSource is System.Data.DataView view)
                    {
                        var snapshots = view?.ToRows()?.Where((row) => row["Select"] is bool selected && selected)?.Select((row) => row["SnapshotName"].ToString())?.ToArray();

                        if (snapshots != null)
                        {
                            var queries = SQLQueriesHelper.GetDeleteSnapshots(snapshots);

                            ComandProcessor processor = FormHelper.GetNewCommandProcessor();
                            processor.CanKillExistingConnections = false;
                            processor.KillExistingConnections = false;
                            processor.DatabasesToKill = null;
                            processor.Queries = queries;

                            processor.ShowDialog();
                        }
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