using DatabaseHelper.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace DatabaseHelper.Helpers
{
    /// <summary>
    /// Not really necessary, a bit dirty, but makes things a little easier
    /// </summary>
    internal class SQLQueriesHelper
    {
        private const string CREATE_SNAPSHOT = "SQL\\CreateSnapshot.sql";
        private const string DELETE_DATABASE = "SQL\\DeleteDatabase.sql";
        private const string DELETE_SNAPSHOT = "SQL\\DeleteSnapshot.sql";
        private const string GET_DATABASES = "SQL\\GetDatabases.sql";
        private const string GET_SNAPSHOTS = "SQL\\GetSnapshots.sql";
        private const string KILL_EXISTING_CONNECTIONS = "SQL\\KillExistingConnections.sql";
        private const string RESTORE_SNAPSHOTS = "SQL\\RestoreSnapshot.sql";

        public static async Task<DataSet> DeleteDatabase(SQLConnectionDetails connectionDetails,
                                                         string databaseName,
                                                         ServerMessageEventHandler fncServerMessage = null)
        {
            var connectionString = SQLHelper.GetConnectionString(connectionDetails);

            var query = File.ReadAllText(DELETE_DATABASE);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@DatabaseName", databaseName);

            return await SQLHelper.RunSmartCommand(connectionString,
                                                   query,
                                                   parameters,
                                                   fncServerMessage);
        }

        public static async Task<DataSet> DeleteSnapshot(SQLConnectionDetails connectionDetails,
                                                         string snapshotName,
                                                         ServerMessageEventHandler fncServerMessage = null)
        {
            var connectionString = SQLHelper.GetConnectionString(connectionDetails);

            var query = File.ReadAllText(DELETE_SNAPSHOT);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@SnapshotName", snapshotName);

            return await SQLHelper.RunSmartCommand(connectionString,
                                                   query,
                                                   parameters,
                                                   fncServerMessage);
        }

        public static async Task<DataSet> GetDatabases(SQLConnectionDetails connectionDetails,
                                                       ServerMessageEventHandler fncServerMessage = null)
        {
            var connectionString = SQLHelper.GetConnectionString(connectionDetails);

            var query = File.ReadAllText(GET_DATABASES);

            return await SQLHelper.RunCommand(connectionString,
                                              query,
                                              null,
                                              fncServerMessage);
        }

        public static async Task<DataSet> GetSnapshots(SQLConnectionDetails connectionDetails,
                                                       ServerMessageEventHandler fncServerMessage = null)
        {
            var connectionString = SQLHelper.GetConnectionString(connectionDetails);

            var query = File.ReadAllText(GET_SNAPSHOTS);

            return await SQLHelper.RunCommand(connectionString,
                                              query,
                                              null,
                                              fncServerMessage);
        }

        public static async Task<DataSet> KillExistingConnections(SQLConnectionDetails connectionDetails,
                                                                  string databaseName,
                                                                  ServerMessageEventHandler fncServerMessage = null)
        {
            var connectionString = SQLHelper.GetConnectionString(connectionDetails);

            var query = File.ReadAllText(DELETE_SNAPSHOT);

            var parameters = new SqlCommand().Parameters;
            parameters.AddWithValue("@DatabaseName", databaseName);

            return await SQLHelper.RunSmartCommand(connectionString,
                                                   query,
                                                   parameters,
                                                   fncServerMessage);
        }
    }
}