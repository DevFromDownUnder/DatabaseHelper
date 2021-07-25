using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using Microsoft;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseHelper.Pages
{
    /// <summary>
    /// Interaction logic for pagExecuteFile.xaml
    /// </summary>
    public partial class pagExecuteFile : Page
    {
        public pagExecuteFile()
        {
            InitializeComponent();
        }

        private async void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            await FormHelper.ExceptionDialogHandlerAsync(async () =>
            {
                if (dgDatabases.SelectedItem != null)
                {
                    if (dgDatabases.SelectedItem is System.Data.DataRowView row)
                    {
                        var database = row["DatabaseName"].ToString();
                        var filename = txtFilename.Text.SafeTrim();

                        Requires.NotNullOrWhiteSpace(filename, "Script filename must be specified");

                        var script = await File.ReadAllTextAsync(filename);

                        Requires.NotNullOrWhiteSpace(script, "Script must not be empty");

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

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            FormHelper.ExceptionDialogHandler(() =>
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    AddExtension = true,
                    InitialDirectory = Directory.GetCurrentDirectory(),
                    Filter = "SQL files|*.sql|Text files|*.txt|All files|*.*"
                };

                if (openFileDialog.ShowDialog() ?? false)
                {
                    txtFilename.Text = openFileDialog.SafeFileName;
                }
            });
        }

        private async Task Refresh()
        {
            var results = await SQLQueriesHelper.GetAllDatabases(SettingsHelper.GetSQLConnectionDetails());

            dgDatabases.ItemsSource = results?.Tables?.FirstOrDefault()?.AddSelectColumn()?.DefaultView;
        }
    }
}