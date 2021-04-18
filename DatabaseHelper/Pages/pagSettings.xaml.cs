using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DatabaseHelper.Pages
{
    /// <summary>
    /// Interaction logic for pagSettings.xaml
    /// </summary>
    public partial class pagSettings : Page
    {
        public pagSettings()
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

        private void hypRemove_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Hyperlink item)
            {
                if (item.DataContext != null && SettingsHelper.Settings.Server_Servers.Contains(item.DataContext.ToString()))
                {
                    SettingsHelper.Settings.Server_Servers.Remove(item.DataContext.ToString());
                }
            }
        }

        private async void btnAddRegisteredNetworkServers_Click(object sender, RoutedEventArgs e)
        {
            await FormHelper.LoadingOriginalButton(() =>
             {
                 var registeredServers = SQLHelper.GetRegisteredNetworkServers().Result;

                 SettingsHelper.Settings.Server_Servers = SettingsHelper.Settings.Server_Servers.Union(registeredServers).ToObservableCollection();
             }, (Button)sender);
        }

        private async void btnAddRegisteredLocalServers_Click(object sender, RoutedEventArgs e)
        {
            await FormHelper.LoadingOriginalButton(() =>
            {
                var registeredServers = SQLHelper.GetRegisteredLocalServers().Result;

                SettingsHelper.Settings.Server_Servers = SettingsHelper.Settings.Server_Servers.Union(registeredServers).ToObservableCollection();
            }, (Button)sender);
        }

        private void btnAddServer_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SecurtiyType_CheckChange(object sender, RoutedEventArgs e)
        {
            SettingsHelper.UpdateConnectionDetails(SettingsHelper.Settings);
        }
    }
}