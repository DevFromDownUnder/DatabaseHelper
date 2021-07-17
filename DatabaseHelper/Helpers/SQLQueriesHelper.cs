using DatabaseHelper.Contracts;
using DatabaseHelper.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using System;
using System.Data;
using System.IO;
using System.Linq;
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

        public static SQLQuery GetBackupDatabase(string database, string backupFilename, bool copyOnly, bool compression)
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
            parameters.AddWithValue("@DatabaseName", database);
            parameters.AddWithValue("@BackupFilename", backupFilename);

            return new() { Query = query, Parameters = parameters };
        }

        public static SQLQuery GetCreateSnapshot(string database, string databaseDataName, string snapshot, string snapshotFilename)
        {
            var query = File.ReadAllText(CREATE_SNAPSHOT);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@DatabaseName", database);
            parameters.AddWithValue("@SnapshotName", snapshot);
            parameters.AddWithValue("@DatabaseDataName", databaseDataName);
            parameters.AddWithValue("@SnapshotFilename", snapshotFilename);

            return new() { Query = query, Parameters = parameters };
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

        public static SQLQuery GetDeleteDatabase(string database)
        {
            var query = File.ReadAllText(DELETE_DATABASE);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@DatabaseName", database);

            return new() { Query = query, Parameters = parameters };
        }

        public static SQLQuery[] GetDeleteDatabases(string[] databases)
        {
            return databases.Select((database) => GetDeleteDatabase(database)).ToArray();
        }

        public static SQLQuery GetDeleteSnapshot(string snapshot)
        {
            var query = File.ReadAllText(DELETE_SNAPSHOT);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@SnapshotName", snapshot);

            return new() { Query = query, Parameters = parameters };
        }

        public static SQLQuery[] GetDeleteSnapshots(string[] snapshots)
        {
            return snapshots.Select((snapshot) => GetDeleteSnapshot(snapshot)).ToArray();
        }

        public static SQLQuery GetRestoreSnapshot(string snapshot, string database)
        {
            var query = File.ReadAllText(RESTORE_SNAPSHOTS);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@DatabaseName", database);
            parameters.AddWithValue("@SnapshotName", snapshot);

            return new() { Query = query, Parameters = parameters };
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

        public static string JoinAndFormat(SQLQuery[] queries)
        {
            if (queries is null)
            {
                return string.Empty;
            }

            return string.Join($"{Environment.NewLine}GO {Environment.NewLine}", queries.Select((query) => query.Query.SmartSQLFormat(query.Parameters)));
        }

        public static async Task<DataSet> KillExistingConnections(SQLConnectionDetails connectionDetails,
                                                                  string database,
                                                                  ServerMessageEventHandler fncServerMessage = null,
                                                                  SqlInfoMessageEventHandler fncInfoMessage = null)
        {
            var connectionString = SQLHelper.GetConnectionString(connectionDetails);

            var query = File.ReadAllText(KILL_EXISTING_CONNECTIONS);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@DatabaseName", database);

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