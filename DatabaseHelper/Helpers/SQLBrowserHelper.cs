using DevFromDownUnder.SQLBrowser;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseHelper.Helpers
{
    public class SQLBrowserHelper
    {
        public static Browser Instance { get; set; } = new Browser();

        public static async Task<List<string>> GetRegisteredNetworkServers()
        {
            var servers = await Instance.DiscoverNetworkServers(new()).ConfigureAwait(false);

            return servers?.Select((s) => s.ServerName)?.ToList();
        }

        public static async Task<List<string>> GetRegisteredLocalServers()
        {
            var servers = await Instance.DiscoverLocalServers(new()).ConfigureAwait(false);

            return servers?.Select((s) => s.ServerName)?.ToList();
        }
    }
}