using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DatabaseHelper.Extensions
{
    public static class DataTableCollectionExtensions
    {
        public static DataTable FirstOrDefault(this DataTableCollection source)
        {
            return (source == null || source.Count == 0) ? null : source[0];
        }

        public static List<DataTable> ToList(this DataTableCollection source)
        {
            return source?.Cast<DataTable>()?.ToList();
        }
    }
}