using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DatabaseHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MaterialWindow
    {
        public MainWindow()
        {
            ThemeHelper.BindTheme();

            SettingsHelper.LoadSettings(false).Wait();

            InitializeComponent();
        }

        private static void SetDefaults()
        {
            MenuHelper.MenuItems.First().Selected = true;

            if (SettingsHelper.Settings.Server_PreferredServer != null)
            {
                SettingsHelper.Settings.Server_CurrentServer = SettingsHelper.Settings.Server_PreferredServer;
            }
        }

        private void DialogHost_DialogOpened(object sender, MaterialDesignThemes.Wpf.DialogOpenedEventArgs eventArgs)
        {
        }

        private void Menu_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rdbSender)
            {
                if (rdbSender.Tag != null)
                {
                    frmContentFrame.Navigate(MenuHelper.MenuItems.FirstOrDefault((x) => x.PageKey == rdbSender.Tag.ToString())?.Content);
                }
            }
        }

        private void SecurtiyType_CheckChange(object sender, RoutedEventArgs e)
        {
            SettingsHelper.UpdateConnectionDetails(SettingsHelper.Settings);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (SettingsHelper.Settings.IsChanged)
            {
                if (MessageBox.Show("Settings have changed\nDo you want to save changes?", "Unsaved changes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    SettingsHelper.SaveSettings(false).Wait();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetDefaults();
        }

        #region Custom Open Dialog

        public void OpenSpecificDialogCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        public async void OpenSpecificDialogCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Just setup for abutton currently as it has Tag we can use
            if (e.Command is RoutedCommand command)
            {
                if (command.Name.HasValue())
                {
                    await DialogHost.Show(e.Parameter, command.Name);
                }
            }
        }

        #endregion Custom Open Dialog
    }
}