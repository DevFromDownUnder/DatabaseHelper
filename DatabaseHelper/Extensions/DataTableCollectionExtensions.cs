using System.Data;

namespace DatabaseHelper.Extensions
{
    public static class DataTableCollectionExtensions
    {
        public static DataTable FirstOrDefault(this DataTableCollection source)
        {
            return (source == null || source.Count == 0) ? null : source[0];
        }
    }
}