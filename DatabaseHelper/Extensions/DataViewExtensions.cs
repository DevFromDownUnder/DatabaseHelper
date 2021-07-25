using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DatabaseHelper.Extensions
{
    public static class DataViewExtensions
    {
        public static List<DataRow> ToRows(this DataView source)
        {
            return source?.Table?.Rows?.Cast<DataRow>()?.ToList();
        }
    }
}