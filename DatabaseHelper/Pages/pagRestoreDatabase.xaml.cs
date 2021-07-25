using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using FolderBrowserEx;
using Microsoft;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseHelper.Pages
{
    /// <summary>
    /// Interaction logic for pagRestoreDatabase.xaml
    /// </summary>
    public partial class pagRestoreDatabase : Page
    {
        public pagRestoreDatabase()
        {
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            FormHelper.ExceptionDialogHandler(() =>
            {
                if (dgDatabases.SelectedItem != null)
                {
                    if (dgDatabases.SelectedItem is Result result)
                    {
                        var database = result.Name;
                        var filename = result.Filename;

                        Requires.NotNullOrWhiteSpace(database, "Database name must be specified");

                        var query = SQLQueriesHelper.GetRestoreDatabase(database, filename);

                        ComandProcessor processor = FormHelper.GetNewCommandProcessor();
                        processor.CanKillExistingConnections = true;
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

        private void btnRestoreDatabase_DefaultBackupFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog()
            {
                InitialFolder = SettingsHelper.Settings.RestoreDatabase_DefaultBackupFolder.SafeTrim(),
                AllowMultiSelect = false,
                Title = "Select SQL backup folder"
            };

            if (folderBrowserDialog.ShowDialogB())
            {
                SettingsHelper.Settings.RestoreDatabase_DefaultBackupFolder = folderBrowserDialog.SelectedFolder.SafeTrim();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetDefaults();
        }

        private async Task Refresh()
        {
            var files = await LogicHelper.GetFiles(SettingsHelper.Settings.RestoreDatabase_DefaultBackupFolder, "*.bak");

            var results = files?.Select((f) => new Result() { Name = Path.GetFileNameWithoutExtension(f), Filename = f })?.ToList();

            dgDatabases.ItemsSource = results;
        }

        private void SetDefaults()
        {
        }

        public class Result
        {
            public string Filename { get; set; }
            public string Name { get; set; }
            public bool Select { get; set; } = false;
        }
    }
}