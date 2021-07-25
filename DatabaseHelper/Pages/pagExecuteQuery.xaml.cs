using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using Microsoft;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseHelper.Pages
{
    /// <summary>
    /// Interaction logic for pagExecuteQuery.xaml
    /// </summary>
    public partial class pagExecuteQuery : Page
    {
        public pagExecuteQuery()
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
                        var database = row["DatabaseName"].ToString();
                        var script = txtScript.Text.SafeTrim();

                        Requires.NotNullOrWhiteSpace(script, "Script must be specified");

                        var queries = SQLQueriesHelper.Split(script, null);

                        ComandProcessor processor = FormHelper.GetNewCommandProcessor();
                        processor.CanKillExistingConnections = true;
                        processor.KillExistingConnections = false;
                        processor.DatabasesToKill = new[] { database };
                        processor.Queries = queries;

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
            var results = await SQLQueriesHelper.GetAllDatabases(SettingsHelper.GetSQLConnectionDetails());

            dgDatabases.ItemsSource = results?.Tables?.FirstOrDefault()?.DefaultView;
        }
    }
}