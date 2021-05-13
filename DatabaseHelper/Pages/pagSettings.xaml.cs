using DatabaseHelper.Contracts;
using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using Microsoft.Toolkit.Uwp.UI;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            await FormHelper.LoadingOriginalButton(async () =>
             {
                 var servers = await SQLBrowserHelper.GetRegisteredNetworkServers();

                 if (servers != null)
                 {
                     SettingsHelper.Settings.Server_Servers = SettingsHelper.Settings.Server_Servers.Union(servers).ToObservableCollection();
                 }
             }, (Button)sender);
        }

        private async void btnAddRegisteredLocalServers_Click(object sender, RoutedEventArgs e)
        {
            await FormHelper.LoadingOriginalButton(async () =>
            {
                var servers = await SQLBrowserHelper.GetRegisteredLocalServers();

                if (servers != null)
                {
                    SettingsHelper.Settings.Server_Servers = SettingsHelper.Settings.Server_Servers.Union(servers).ToObservableCollection();
                }
            }, (Button)sender);
        }

        private void btnAddServer_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SecurtiyType_CheckChange(object sender, RoutedEventArgs e)
        {
            SettingsHelper.UpdateConnectionDetails(SettingsHelper.Settings);
        }

        private async void ImportExportDialog_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button btnSource)
            {
                if (sender is FrameworkElement dialogElement)
                {
                    if (dialogElement.FindDescendant("txtJson") is TextBox txtJson)
                    {
                        if (btnSource.Tag != null)
                        {
                            var json = txtJson.Text;

                            switch (btnSource.Tag.ToString())
                            {
                                case "Import":
                                    await FormHelper.ExceptionDialogHandler(() =>
                                          FormHelper.LoadingFlatDarkBgButton(() => SettingsHelper.Settings = ParseUserSettingsJson(json), btnSource)
                                    );

                                    break;

                                case "Export":
                                    await FormHelper.ExceptionDialogHandler(() =>
                                        FormHelper.LoadingFlatDarkBgButton(() => Dispatcher.Invoke(() => txtJson.Text = GetSettingsJson()), btnSource)
                                    );

                                    break;

                                case "Parse":
                                    await FormHelper.ExceptionDialogHandler(() =>
                                        FormHelper.LoadingFlatDarkBgButton(() => ParseUserSettingsJson(json), btnSource)
                                    );

                                    break;

                                case "Format":
                                    await FormHelper.ExceptionDialogHandler(() =>
                                        FormHelper.LoadingFlatDarkBgButton(() => Dispatcher.Invoke(() => txtJson.Text = FormatJson(json)), btnSource)
                                    );
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private string GetSettingsJson()
        {
            return JsonSerializer.Serialize(SettingsHelper.Settings, new() { WriteIndented = true, Converters = { new JsonStringEnumConverter() } });
        }

        private string FormatJson(string json)
        {
            var settings = ParseUserSettingsJson(json);

            return JsonSerializer.Serialize(settings, new() { WriteIndented = true, Converters = { new JsonStringEnumConverter() } });
        }

        private UserSettings ParseUserSettingsJson(string json)
        {
            return JsonSerializer.Deserialize<UserSettings>(json);
        }

        private void ImportExportDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (e.Source is Grid source)
            {
                if (sender is FrameworkElement dialogElement)
                {
                    if (dialogElement.FindDescendant("txtJson") is TextBox txtScript)
                    {
                        txtScript.Text = GetSettingsJson();
                    }
                }
            }
        }
    }
}