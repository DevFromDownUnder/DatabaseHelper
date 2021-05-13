using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
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
            ComandProcessor processor = FormHelper.GetNewCommandProcessor("test");
            processor.ShowDialog();
        }
    }
}