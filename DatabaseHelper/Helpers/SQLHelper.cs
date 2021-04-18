using DatabaseHelper.Contracts;
using DatabaseHelper.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseHelper.Helpers
{
    public class SQLHelper
    {
        public const string MASTER_DB = "master";

        public static async Task<List<string>> GetRegisteredNetworkServers()
        {
            return await Task.Run(() =>
            {
                return new List<string>();
            });
        }

        public static async Task<List<string>> GetRegisteredLocalServers()
        {
            return await Task.Run(() =>
            {
                return SmoApplication.EnumAvailableSqlServers(true)
                                     .AsEnumerable()
                                     .Select((r) => string.IsNullOrEmpty(r.Field<string>("InstanceName")) ? r.Field<string>("ServerName") : r.Field<string>("ServerName") + "\\" + r.Field<string>("InstanceName"))
                                     .ToList();
            });
        }

        public static async Task<DataSet> RunSmartCommand(string connectionString,
                                                          string command,
                                                          SqlParameterCollection parameters,
                                                          ServerMessageEventHandler fncServerMessage)
        {
            return await Task.Run(() =>
            {
                DataSet results = null;

                using (SqlConnection connection = new(connectionString))
                {
                    var server = new Server(new ServerConnection(connection));

                    server.ConnectionContext.ServerMessage += fncServerMessage;

                    results = server.ConnectionContext.ExecuteWithResults(command.SmartSQLFormat(parameters));
                }

                return results;
            });
        }

        public static async Task<DataSet> RunCommand(string connectionString,
                                                     string command,
                                                     SqlParameterCollection parameters,
                                                     ServerMessageEventHandler fncServerMessage)
        {
            return await Task.Run(() =>
            {
                DataSet results = null;

                using (SqlConnection connection = new(connectionString))
                {
                    var server = new Server(new ServerConnection(connection));

                    server.ConnectionContext.ServerMessage += fncServerMessage;

                    //Handle paramaters normally not weird embedded stuff I have going
                    results = server.ConnectionContext.ExecuteWithResults(command.SmartSQLFormat(parameters));
                }

                return results;
            });
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

        public static string GetConnectionString(SQLConnectionDetails connectionDetails, bool useMasterOverride = false)
        {
            var connectionBuilder = new SqlConnectionStringBuilder()
            {
                ApplicationName = System.AppDomain.CurrentDomain.FriendlyName,
                DataSource = connectionDetails.Server,
                InitialCatalog = useMasterOverride ? MASTER_DB : connectionDetails.Database
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
    }
}