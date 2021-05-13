using DatabaseHelper.Helpers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetDefaults();

            ThemeHelper.BindTheme();
        }

        private static void SetDefaults()
        {
            MenuHelper.MenuItems.First().Selected = true;
        }

        private void Menu_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rdbSender)
            {
                if (rdbSender.Tag != null)
                {
                    frmContentFrame.Source = new Uri(rdbSender.Tag.ToString(), UriKind.RelativeOrAbsolute);
                }
            }
        }

        private void SecurtiyType_CheckChange(object sender, RoutedEventArgs e)
        {
            SettingsHelper.UpdateConnectionDetails(SettingsHelper.Settings);
        }

        private void DialogHost_DialogOpened(object sender, MaterialDesignThemes.Wpf.DialogOpenedEventArgs eventArgs)
        {
        }
    }
}