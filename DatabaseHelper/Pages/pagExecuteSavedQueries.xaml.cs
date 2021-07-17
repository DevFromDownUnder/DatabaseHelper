using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseHelper.Pages
{
    /// <summary>
    /// Interaction logic for pagExecuteSavedQueries.xaml
    /// </summary>
    public partial class pagExecuteSavedQueries : Page
    {
        public pagExecuteSavedQueries()
        {
            InitializeComponent();
        }

        public enum eDialogResult
        {
            Cancel,
            Save,
            Delete
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            ComandProcessor processor = FormHelper.GetNewCommandProcessor("test");
            processor.ShowDialog();
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await FormHelper.ExceptionDialogHandlerAsync(
                FormHelper.LoadingFlatDarkBgButton(Refresh(), (Button)sender)
            );
        }

        private void DeleteDialog_Click(object sender, RoutedEventArgs e)
        {
            dgScripts.UnselectAllCells();

            if (e.Source is Button btnSource)
            {
                if (btnSource.Tag != null && btnSource.Tag.ToString() == "Delete")
                {
                }
            }
        }

        private void EditDialog_Click(object sender, RoutedEventArgs e)
        {
            dgScripts.UnselectAllCells();

            if (e.Source is Button btnSource)
            {
                if (btnSource.Tag != null && btnSource.Tag.ToString() == "Save")
                {
                    if (sender is FrameworkElement dialogElement)
                    {
                        if (dialogElement.FindName("txtEditScript") is TextBox txtScript)
                        {
                            Debugger.Break();
                        }
                    }
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetDefaults();
        }

        private async Task Refresh()
        {
            var results = await SQLQueriesHelper.GetDatabases(SettingsHelper.GetSQLConnectionDetails());

            dgDatabases.ItemsSource = results?.Tables?.FirstOrDefault()?.DefaultView;
        }

        private void SetDefaults()
        {
        }
    }
}