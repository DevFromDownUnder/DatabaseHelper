using DatabaseHelper.Contracts;
using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using MaterialDesignExtensions.Controls;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using PropertyChanged;
using Swordfish.NET.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseHelper
{
    /// <summary>
    /// Interaction logic for ComandProcessor.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class ComandProcessor : MaterialWindow
    {
        private BackgroundWorker mBackgroundWorker;

        public ComandProcessor(SQLConnectionDetails connectionDetails)
        {
            InitializeComponent();

            //Need to set context for some reason it doesn't default
            DataContext = this;

            ConnectionDetails = connectionDetails;
        }

        public SQLConnectionDetails ConnectionDetails { get; set; }
        public string DatabaseToKill { get; set; }
        public bool KillExistingConnections { get; set; }
        public SqlParameterCollection Parameters { get; set; }
        public string[] Queries { get; set; }
        public ConcurrentObservableDictionary<Guid, DataSet> Results { get; set; }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            StopExecution();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            StartExecution();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void cmbResultSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is KeyValuePair<Guid, DataSet> selected)
                {
                    cmbTables.ItemsSource = selected.Value?.Tables?.ToList()?.Select((x, i) => i);

                    return;
                }
            }

            dgResults.ItemsSource = null;
            cmbTables.ItemsSource = null;
        }

        private void cmbTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is int index)
                {
                    if (cmbResultSet.SelectedItem is KeyValuePair<Guid, DataSet> selected)
                    {
                        if (selected.Value.Tables.Count > index)
                        {
                            dgResults.ItemsSource = selected.Value.Tables[index].DefaultView;

                            return;
                        }
                    }
                }
            }

            dgResults.ItemsSource = null;
        }

        private void mBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            var connectionString = SQLHelper.GetConnectionString(ConnectionDetails);

            if (KillExistingConnections)
            {
                SQLQueriesHelper.KillExistingConnections(ConnectionDetails,
                                                         DatabaseToKill,
                                                         (object sender, ServerMessageEventArgs e) => mBackgroundWorker.ReportProgress(0, e.Error.ToString()),
                                                         (object sender, SqlInfoMessageEventArgs e) => mBackgroundWorker.ReportProgress(0, e.Message)).Wait();

                mBackgroundWorker.ReportProgress(0, ("Existing connections killed"));
            }

            foreach (var query in Queries)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    var execution = SQLHelper.RunSmartCommand(connectionString,
                                                              query,
                                                              Parameters,
                                                              (object sender, ServerMessageEventArgs e) => mBackgroundWorker.ReportProgress(0, e.Error.ToString()),
                                                              (object sender, SqlInfoMessageEventArgs e) => mBackgroundWorker.ReportProgress(0, e.Message));

                    while (!execution.IsCompleted)
                    {
                        //Will let it at least try for 100ms before cancelling
                        execution.Wait(100);

                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }

                    mBackgroundWorker.ReportProgress(0, (Guid.NewGuid(), "Query completed", execution.Result));
                }
            }
        }

        private void mBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is (Guid id, string message, DataSet results))
            {
                txtResults.Text += message + Environment.NewLine;

                Results.Add(id, results);

                if (Results.Count == 1)
                {
                    cmbResultSet.SelectedIndex = 0;
                }
            }

            if (e.UserState is string error)
            {
                txtResults.Text += error + Environment.NewLine;
            }
        }

        private void mBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                txtResults.Text += "Cancelled by user" + Environment.NewLine;
            }

            txtResults.Text += e.Error.FlattenExceptionMessagesToString();

            btnAbort.IsEnabled = false;
            btnCancel.IsEnabled = true;
            btnExecute.IsEnabled = true;
            btnOK.IsEnabled = true;

            pgbProgressTop.Visibility = Visibility.Collapsed;
            pgbProgressBottom.Visibility = Visibility.Collapsed;
        }

        private void SetupDefaults()
        {
            btnAbort.IsEnabled = false;
            btnOK.IsEnabled = false;
            pgbProgressTop.Visibility = Visibility.Collapsed;
            pgbProgressBottom.Visibility = Visibility.Collapsed;

            txtResults.Visibility = Visibility.Collapsed;
            cmbResultSet.Visibility = Visibility.Collapsed;
            cmbTables.Visibility = Visibility.Collapsed;
            dgResults.Visibility = Visibility.Collapsed;

            txtCommand.Text = SQLQueriesHelper.Join(Queries).SmartSQLFormat(Parameters);
        }

        private void StartExecution()
        {
            btnAbort.IsEnabled = true;
            btnCancel.IsEnabled = false;
            btnExecute.IsEnabled = false;
            btnOK.IsEnabled = false;

            pgbProgressTop.Visibility = Visibility.Visible;
            pgbProgressBottom.Visibility = Visibility.Visible;

            txtCommand.Visibility = Visibility.Collapsed;
            txtResults.Visibility = Visibility.Visible;
            cmbResultSet.Visibility = Visibility.Visible;
            cmbTables.Visibility = Visibility.Visible;
            dgResults.Visibility = Visibility.Visible;

            txtResults.Clear();
            cmbTables.ItemsSource = null;
            dgResults.ItemsSource = null;

            Results = new();

            mBackgroundWorker = new();
            mBackgroundWorker.WorkerSupportsCancellation = true;
            mBackgroundWorker.WorkerReportsProgress = true;
            mBackgroundWorker.DoWork += mBackgroundWorker_DoWork;
            mBackgroundWorker.ProgressChanged += mBackgroundWorker_ProgressChanged;
            mBackgroundWorker.RunWorkerCompleted += mBackgroundWorker_RunWorkerCompleted;

            mBackgroundWorker.RunWorkerAsync();
        }

        private void StopExecution()
        {
            if (mBackgroundWorker != null && mBackgroundWorker.IsBusy)
            {
                mBackgroundWorker.CancelAsync();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupDefaults();
        }
    }
}