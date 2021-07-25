using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseHelper.Pages
{
    /// <summary>
    /// Interaction logic for pagDeleteDatabase.xaml
    /// </summary>
    public partial class pagDeleteDatabase : Page
    {
        public pagDeleteDatabase()
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
                        var databases = view?.ToRows()?.Where((row) => row["Select"] is bool selected && selected)?.Select((row) => row["DatabaseName"].ToString())?.ToArray();

                        if (databases != null)
                        {
                            var queries = SQLQueriesHelper.GetDeleteDatabases(databases);

                            ComandProcessor processor = FormHelper.GetNewCommandProcessor();
                            processor.CanKillExistingConnections = true;
                            processor.KillExistingConnections = true;
                            processor.DatabasesToKill = databases;
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
            var results = await SQLQueriesHelper.GetDatabases(SettingsHelper.GetSQLConnectionDetails());

            dgDatabases.ItemsSource = results?.Tables?.FirstOrDefault()?.AddSelectColumn()?.DefaultView;
        }
    }
}