using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            chkCopyOnlyBackup.IsChecked = true;
            chkCompress.IsChecked = true;
        }

        private async Task Refresh()
        {
            var results = await SQLQueriesHelper.GetDatabases(SettingsHelper.GetSQLConnectionDetails());

            dgDatabases.ItemsSource = results?.Tables?.FirstOrDefault()?.DefaultView;
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await FormHelper.ExceptionDialogHandler(() =>
                FormHelper.LoadingFlatDarkBgButton(Refresh, (Button)sender)
            );
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            ComandProcessor processor = FormHelper.GetNewCommandProcessor(null);
            processor.ShowDialog();
        }
    }
}