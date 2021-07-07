﻿using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseHelper.Pages
{
    /// <summary>
    /// Interaction logic for pagRestoreSnapshot.xaml
    /// </summary>
    public partial class pagRestoreSnapshot : Page
    {
        public pagRestoreSnapshot()
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
            var results = await SQLQueriesHelper.GetSnapshots(SettingsHelper.GetSQLConnectionDetails());

            dgDatabases.ItemsSource = results?.Tables?.FirstOrDefault()?.DefaultView;
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await FormHelper.ExceptionDialogHandler(
                FormHelper.LoadingFlatDarkBgButton(Refresh(), (Button)sender)
            );
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (dgDatabases.SelectedItem != null)
            {
                if (dgDatabases.SelectedItem is System.Data.DataRowView row)
                {
                    var (query, parameters) = SQLQueriesHelper.GetRestoreSnapshot(row["SnapshotName"].ToString());

                    ComandProcessor processor = FormHelper.GetNewCommandProcessor();
                    processor.Query = query;
                    processor.Parameters = parameters;
                    processor.ShowDialog();
                }
            }
        }
    }
}