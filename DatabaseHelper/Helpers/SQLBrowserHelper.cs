using DatabaseHelper.Contracts;
using DevFromDownUnder.SQLBrowser;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseHelper.Helpers
{
    public class SQLBrowserHelper
    {
        public static Browser Instance { get; set; } = new Browser();

        public static async Task<List<SQLServer>> GetRegisteredNetworkServers()
        {
            var servers = await Instance.DiscoverNetworkServers(new()).ConfigureAwait(false);

            return servers?.Select((s) => new SQLServer() { Server = s.ServerName, Port = s.Port })?.ToList();
        }

        public static async Task<List<SQLServer>> GetRegisteredLocalServers()
        {
            var servers = await Instance.DiscoverLocalServers(new()).ConfigureAwait(false);

            return servers?.Select((s) => new SQLServer() { Server = s.ServerName, Port = s.Port })?.ToList();
        }
    }
}