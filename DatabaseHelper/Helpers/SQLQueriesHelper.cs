using DatabaseHelper.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DatabaseHelper.Helpers
{
    /// <summary>
    /// Not really necessary, a bit dirty, but makes things a little easier
    /// </summary>
    internal class SQLQueriesHelper
    {
        private const string BACKUP_DATABASE = "SQL\\BackupDatabase.sql";
        private const string BACKUP_DATABASE_COMPRESSION = "SQL\\BackupDatabase_Compression.sql";
        private const string BACKUP_DATABASE_COPYONLY = "SQL\\BackupDatabase_CopyOnly.sql";
        private const string BACKUP_DATABASE_COPYONLY_COMPRESSION = "SQL\\BackupDatabase_CopyOnly_Compression.sql";
        private const string CREATE_SNAPSHOT = "SQL\\CreateSnapshot.sql";
        private const string DELETE_DATABASE = "SQL\\DeleteDatabase.sql";
        private const string DELETE_SNAPSHOT = "SQL\\DeleteSnapshot.sql";
        private const string GET_DATABASES = "SQL\\GetDatabases.sql";
        private const string GET_SNAPSHOTS = "SQL\\GetSnapshots.sql";
        private const string KILL_EXISTING_CONNECTIONS = "SQL\\KillExistingConnections.sql";
        private const string RESTORE_SNAPSHOTS = "SQL\\RestoreSnapshot.sql";

        public static (string, SqlParameterCollection) GetBackupDatabase(string databaseName, string backupFilename, bool copyOnly, bool compression)
        {
            var queryFile = BACKUP_DATABASE;

            if (copyOnly && compression)
            {
                queryFile = BACKUP_DATABASE_COPYONLY_COMPRESSION;
            }
            else if (compression)
            {
                queryFile = BACKUP_DATABASE_COMPRESSION;
            }
            else if (copyOnly)
            {
                queryFile = BACKUP_DATABASE_COPYONLY;
            }

            var query = File.ReadAllText(queryFile);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@DatabaseName", databaseName);
            parameters.AddWithValue("@BackupFilename", backupFilename);

            return (query, parameters);
        }

        public static (string, SqlParameterCollection) GetCreateSnapshot(string databaseName, string databaseDataName, string snapshotName, string snapshotFilename)
        {
            var query = File.ReadAllText(CREATE_SNAPSHOT);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@DatabaseName", databaseName);
            parameters.AddWithValue("@SnapshotName", snapshotName);
            parameters.AddWithValue("@DatabaseDataName", databaseDataName);
            parameters.AddWithValue("@SnapshotFilename", snapshotFilename);

            return (query, parameters);
        }

        public static async Task<DataSet> GetDatabases(SQLConnectionDetails connectionDetails,
                                                       ServerMessageEventHandler fncServerMessage = null,
                                                       SqlInfoMessageEventHandler fncInfoMessage = null)
        {
            var connectionString = SQLHelper.GetConnectionString(connectionDetails);

            var query = File.ReadAllText(GET_DATABASES);

            return await SQLHelper.RunCommand(connectionString,
                                              query,
                                              null,
                                              fncServerMessage,
                                              fncInfoMessage);
        }

        public static (string, SqlParameterCollection) GetDeleteDatabase(string databaseName)
        {
            var query = File.ReadAllText(DELETE_DATABASE);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@DatabaseName", databaseName);

            return (query, parameters);
        }

        public static (string, SqlParameterCollection) GetDeleteSnapshot(string snapshotName)
        {
            var query = File.ReadAllText(DELETE_SNAPSHOT);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@SnapshotName", snapshotName);

            return (query, parameters);
        }

        public static (string, SqlParameterCollection) GetRestoreSnapshot(string snapshotName, string databaseName)
        {
            var query = File.ReadAllText(RESTORE_SNAPSHOTS);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@DatabaseName", databaseName);
            parameters.AddWithValue("@SnapshotName", snapshotName);

            return (query, parameters);
        }

        public static async Task<DataSet> GetSnapshots(SQLConnectionDetails connectionDetails,
                                                       ServerMessageEventHandler fncServerMessage = null,
                                                       SqlInfoMessageEventHandler fncInfoMessage = null)
        {
            var connectionString = SQLHelper.GetConnectionString(connectionDetails);

            var query = File.ReadAllText(GET_SNAPSHOTS);

            return await SQLHelper.RunCommand(connectionString,
                                              query,
                                              null,
                                              fncServerMessage,
                                              fncInfoMessage);
        }

        public static string Join(string[] queries)
        {
            if (queries is null)
            {
                return string.Empty;
            }

            return string.Join($"{Environment.NewLine}GO {Environment.NewLine}", queries);
        }

        public static async Task<DataSet> KillExistingConnections(SQLConnectionDetails connectionDetails,
                                                                          string databaseName,
                                                                  ServerMessageEventHandler fncServerMessage = null,
                                                                  SqlInfoMessageEventHandler fncInfoMessage = null)
        {
            var connectionString = SQLHelper.GetConnectionString(connectionDetails);

            var query = File.ReadAllText(KILL_EXISTING_CONNECTIONS);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@DatabaseName", databaseName);

            return await SQLHelper.RunSmartCommand(connectionString,
                                                   query,
                                                   parameters,
                                                   fncServerMessage,
                                                   fncInfoMessage);
        }

        public static string[] Split(string query)
        {
            const string QUERY_SEPARATOR = "^[\\ \\t]*GO[\\ \\t;]*$";

            return Regex.Split(query, QUERY_SEPARATOR, RegexOptions.IgnoreCase);
        }
    }
}