using DatabaseHelper.Contracts;
using DatabaseHelper.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data;
using System.Threading.Tasks;

namespace DatabaseHelper.Helpers
{
    public class SQLHelper
    {
        public const string MASTER_DB = "master";

        public static bool CanKillConnectionsForDatabase(string database)
        {
            //At the very least you can't kill the master DB, that would cause all sorts of problems
            return !string.Equals(database, MASTER_DB, System.StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetConnectionString(SQLConnectionDetails connectionDetails, bool useMasterOverride = false)
        {
            var connectionBuilder = new SqlConnectionStringBuilder()
            {
                ApplicationName = System.AppDomain.CurrentDomain.FriendlyName,
                DataSource = connectionDetails.Server,
                InitialCatalog = useMasterOverride ? MASTER_DB : connectionDetails.Database,
                ConnectTimeout = connectionDetails.ConnectTimeout,
                CommandTimeout = connectionDetails.CommandTimeout
            };

            if (connectionDetails.UseActiveDirectory)
            {
                connectionBuilder.Authentication = SqlAuthenticationMethod.ActiveDirectoryPassword;
                connectionBuilder.UserID = GetDomainUsername(connectionDetails.Domain, connectionDetails.Username);
                connectionBuilder.Password = connectionDetails.Password;
            }

            if (connectionDetails.UseIntegratedSecurity)
            {
                connectionBuilder.Authentication = SqlAuthenticationMethod.NotSpecified;
                connectionBuilder.IntegratedSecurity = true;
            }

            if (connectionDetails.UseSQLPassword)
            {
                connectionBuilder.Authentication = SqlAuthenticationMethod.SqlPassword;
                connectionBuilder.UserID = connectionDetails.Username;
                connectionBuilder.Password = connectionDetails.Password;
            }

            return connectionBuilder.ToString();
        }

        public static string GetDomainUsername(string domain, string username)
        {
            if (domain.HasValue())
            {
                return username.Trim() + '@' + username.Trim();
            }
            else
            {
                return username.Trim();
            }
        }

        public static async Task<DataSet> RunCommand(string connectionString,
                                                     string command,
                                                     SqlParameterCollection parameters,
                                                     ServerMessageEventHandler fncServerMessage,
                                                     SqlInfoMessageEventHandler fncInfoMessage)
        {
            return await Task.Run(() =>
            {
                DataSet results = null;

                using (SqlConnection connection = new(connectionString))
                {
                    var server = new Server(new ServerConnection(connection));

                    if (fncServerMessage != null)
                    {
                        server.ConnectionContext.ServerMessage += fncServerMessage;
                    }

                    if (fncInfoMessage != null)
                    {
                        server.ConnectionContext.InfoMessage += fncInfoMessage;
                    }

                    //Handle paramaters normally not weird embedded stuff I have going
                    results = server.ConnectionContext.ExecuteWithResults(command.SmartSQLFormat(parameters));
                }

                return results;
            });
        }

        public static async Task<DataSet> RunSmartCommand(string connectionString,
                                                                                  string command,
                                                          SqlParameterCollection parameters,
                                                          ServerMessageEventHandler fncServerMessage,
                                                          SqlInfoMessageEventHandler fncInfoMessage)
        {
            return await Task.Run(() =>
            {
                DataSet results = null;

                using (SqlConnection connection = new(connectionString))
                {
                    var server = new Server(new ServerConnection(connection));

                    if (fncServerMessage != null)
                    {
                        server.ConnectionContext.ServerMessage += fncServerMessage;
                    }

                    if (fncInfoMessage != null)
                    {
                        server.ConnectionContext.InfoMessage += fncInfoMessage;
                    }

                    results = server.ConnectionContext.ExecuteWithResults(command.SmartSQLFormat(parameters));
                }

                return results;
            });
        }
    }
}