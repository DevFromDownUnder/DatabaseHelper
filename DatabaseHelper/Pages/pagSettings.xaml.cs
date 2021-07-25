using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using FolderBrowserEx;
using Microsoft;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

//using MaterialDesignExtensions.Controls;

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

        private void AddServerDialog_Click(object sender, RoutedEventArgs e)
        {
            FormHelper.ExceptionDialogHandler(() =>
            {
                if (e.Source is Button btnSource)
                {
                    if (sender is FrameworkElement dialogElement)
                    {
                        if (dialogElement.FindDescendant("txtAddServerServer") is TextBox txtAddServerServer)
                        {
                            if (dialogElement.FindDescendant("txtAddServerPort") is TextBox txtAddServerPort)
                            {
                                if (btnSource.Tag != null)
                                {
                                    var server = txtAddServerServer.Text.Trim();
                                    var port = txtAddServerPort.Text.Trim();

                                    switch (btnSource.Tag.ToString())
                                    {
                                        case "Add":
                                            Requires.NotNullOrEmpty(server, "Server must be populated");
                                            Requires.NotNullOrEmpty(port, "Port must be populated");

                                            if (!ushort.TryParse(port, out ushort serverPort))
                                            {
                                                throw new ArgumentException($"Port must be between {ushort.MinValue} and {ushort.MaxValue}");
                                            }

                                            if (!SettingsHelper.Settings.Server_Servers.Any((x) => x.Server == server))
                                            {
                                                Dispatcher.Invoke(() => SettingsHelper.Settings.Server_Servers.Add(new() { Server = server, Port = serverPort }));
                                            }

                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        private void AddServerDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (e.Source is Grid)
            {
                if (sender is FrameworkElement dialogElement)
                {
                    if (dialogElement.FindDescendant("txtAddServerPort") is TextBox txtAddServerPort)
                    {
                        txtAddServerPort.Text = DevFromDownUnder.SQLBrowser.SQL.Architecture.DEFAULT_TCP_PORT.ToString();
                    }
                }
            }
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

        private async void btnExport_Click(object sender, RoutedEventArgs e)
        {
            await FormHelper.ExceptionDialogHandlerAsync(async () =>
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog()
                {
                    AddExtension = true,
                    DefaultExt = ".json",
                    InitialDirectory = Directory.GetCurrentDirectory(),
                    Filter = "JSON files|*.json|All files|*.*"
                };

                if (saveFileDialog.ShowDialog() ?? false)
                {
                    await SettingsHelper.SaveSettings(false, saveFileDialog.FileName);
                }
            });
        }

        private async void btnImport_Click(object sender, RoutedEventArgs e)
        {
            await FormHelper.ExceptionDialogHandlerAsync(async () =>
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    AddExtension = true,
                    InitialDirectory = Directory.GetCurrentDirectory(),
                    Filter = "JSON files|*.json|All files|*.*"
                };

                if (openFileDialog.ShowDialog() ?? false)
                {
                    _ = await SettingsHelper.LoadSettings(true, openFileDialog.FileName);
                }
            });
        }

        private void btnResetToDefault_Click(object sender, RoutedEventArgs e)
        {
            FormHelper.ExceptionDialogHandler(() =>
            {
                SettingsHelper.UpdateSettings(SettingsHelper.GetDefaultSettings());
            });
        }

        private void btnRestoreDatabase_DefaultBackupFolder_Click(object sender, RoutedEventArgs e)
        {
            FormHelper.ExceptionDialogHandler(() =>
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
            });
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            await FormHelper.ExceptionDialogHandlerAsync(async () =>
            {
                await SettingsHelper.SaveSettings();
            });
        }

        private async void EditJsonDialog_Click(object sender, RoutedEventArgs e)
        {
            await FormHelper.ExceptionDialogHandlerAsync(async () =>
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
                                    case "Save":
                                        await FormHelper.LoadingFlatDarkBgButton(() => SettingsHelper.UpdateSettings(SettingsHelper.ParseUserSettingsJson(json)), btnSource);
                                        break;

                                    case "Parse":
                                        await FormHelper.LoadingFlatDarkBgButton(() => SettingsHelper.ParseUserSettingsJson(json), btnSource);
                                        break;

                                    case "Format":
                                        await FormHelper.LoadingFlatDarkBgButton(() => Dispatcher.Invoke(() => txtJson.Text = SettingsHelper.FormatUserSettingsJson(json)), btnSource);
                                        break;
                                }
                            }
                        }
                    }
                }
            });
        }

        private void EditJsonDialog_Loaded(object sender, RoutedEventArgs e)
        {
            FormHelper.ExceptionDialogHandler(() =>
            {
                if (e.Source is Grid)
                {
                    if (sender is FrameworkElement dialogElement)
                    {
                        if (dialogElement.FindDescendant("txtJson") is TextBox txtJson)
                        {
                            txtJson.Text = SettingsHelper.GetSettingsJson();
                        }
                    }
                }
            });
        }

        private void hypRemove_Click(object sender, RoutedEventArgs e)
        {
            FormHelper.ExceptionDialogHandler(() =>
            {
                if (e.Source is Hyperlink item)
                {
                    if (item.DataContext != null && SettingsHelper.Settings.Server_Servers.Any((x) => x.Server == item.DataContext.ToString()))
                    {
                        SettingsHelper.Settings.Server_Servers.Remove(SettingsHelper.Settings.Server_Servers.First((x) => x.Server == item.DataContext.ToString()));
                    }
                }
            });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetDefaults();
        }

        private void SecurtiyType_CheckChange(object sender, RoutedEventArgs e)
        {
            FormHelper.ExceptionDialogHandler(() =>
            {
                SettingsHelper.UpdateConnectionDetails(SettingsHelper.Settings);
            });
        }

        private void SetDefaults()
        {
        }
    }
}