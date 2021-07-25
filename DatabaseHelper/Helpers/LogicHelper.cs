using System.IO;
using System.Threading.Tasks;

namespace DatabaseHelper.Helpers
{
    public class LogicHelper
    {
        public static async Task<string[]> GetFiles(string path, string filter)
        {
            return await Task.Run(() =>
            {
                if (!Directory.Exists(path))
                {
                    return null;
                }

                return Directory.GetFiles(path, filter);
            }).ConfigureAwait(false);
        }
    }
}