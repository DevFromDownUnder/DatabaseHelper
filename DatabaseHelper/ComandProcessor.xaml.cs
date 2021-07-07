using DatabaseHelper.Contracts;
using MaterialDesignExtensions.Controls;
using Microsoft.Data.SqlClient;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using DatabaseHelper.Extensions;
using DatabaseHelper.Helpers;
using Microsoft.SqlServer.Management.Common;

namespace DatabaseHelper
{
    /// <summary>
    /// Interaction logic for ComandProcessor.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class ComandProcessor : MaterialWindow
    {
        private BackgroundWorker mBackgroundWorker;
        private Dictionary<Guid, DataView> mResults;

        public SQLConnectionDetails ConnectionDetails { get; set; }
        public string Query { get; set; }
        public SqlParameterCollection Parameters { get; set; }

        public ComandProcessor(SQLConnectionDetails connectionDetails)
        {
            InitializeComponent();

            //Need to set context for some reason it doesn't default
            DataContext = this;

            ConnectionDetails = connectionDetails;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupDefaults();
        }

        private void SetupDefaults()
        {
            btnAbort.IsEnabled = false;
            btnOK.IsEnabled = false;
            pgbProgressTop.Visibility = Visibility.Collapsed;
            pgbProgressBottom.Visibility = Visibility.Collapsed;

            txtResults.Visibility = Visibility.Collapsed;
            cmbResultSet.Visibility = Visibility.Collapsed;
            dgResults.Visibility = Visibility.Collapsed;

            txtCommand.Text = Query.SmartSQLFormat(Parameters);
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
            dgResults.Visibility = Visibility.Visible;

            txtResults.Clear();
            cmbResultSet.ItemsSource = null;
            dgResults.ItemsSource = null;

            mResults = new Dictionary<Guid, DataView>();

            cmbResultSet.ItemsSource = mResults.Keys;

            mBackgroundWorker = new();
            mBackgroundWorker.WorkerSupportsCancellation = true;
            mBackgroundWorker.WorkerReportsProgress = true;
            mBackgroundWorker.DoWork += mBackgroundWorker_DoWork;
            mBackgroundWorker.ProgressChanged += mBackgroundWorker_ProgressChanged;
            mBackgroundWorker.RunWorkerCompleted += mBackgroundWorker_RunWorkerCompleted;

            mBackgroundWorker.RunWorkerAsync();
        }

        private void mBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is Tuple<Guid, String, DataView> tplResult)
            {
                txtResults.Text += tplResult.Item2 + Environment.NewLine;

                if (tplResult.Item3 != null)
                {
                    mResults.Add(tplResult.Item1, tplResult.Item3);
                    cmbResultSet.Items.Refresh();
                }
            }
        }

        private void mBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                txtResults.Text += "Cancelled by user" + Environment.NewLine;
            }

            Exception error = e.Error;
            while (error != null)
            {
                txtResults.Text += error.Message + Environment.NewLine;

                error = error.InnerException;
            }

            btnAbort.IsEnabled = false;
            btnCancel.IsEnabled = true;
            btnExecute.IsEnabled = true;
            btnOK.IsEnabled = true;

            pgbProgressTop.Visibility = Visibility.Collapsed;
            pgbProgressBottom.Visibility = Visibility.Collapsed;
        }

        private void mBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            List<string> queries = new();

            //Need to double check if this needs to split for GO's
            queries.Add(Query);

            var connectionString = SQLHelper.GetConnectionString(ConnectionDetails);

            foreach (var query in queries)
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
                                                              (object sender, ServerMessageEventArgs e) =>
                                                              {
                                                                  //Just throw our errors up for now
                                                                  throw new Exception(e.Error.Message);
                                                              });

                    while (!execution.IsCompleted)
                    {
                        execution.Wait(1);
                    }
                }
            }
        }

        private void msgReport(object sender, ServerMessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void StopExecution()
        {
            if (mBackgroundWorker != null && mBackgroundWorker.IsBusy)
            {
                mBackgroundWorker.CancelAsync();
            }
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            StartExecution();
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            StopExecution();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void cmbResultSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Guid? selectedGuid = e.AddedItems[0] as Guid?;
                if (selectedGuid != null)
                {
                    if (mResults != null && mResults.ContainsKey(selectedGuid.Value))
                    {
                        dgResults.ItemsSource = mResults[selectedGuid.Value];
                    }
                    else
                    {
                        dgResults.ItemsSource = null;
                    }
                }
            }
        }
    }
}