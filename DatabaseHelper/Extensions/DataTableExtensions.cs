using System.Data;

namespace DatabaseHelper.Extensions
{
    public static class DataTableExtensions
    {
        public static DataTable AddSelectColumn(this DataTable source)
        {
            if (source == null)
            {
                return source;
            }

            source.Columns.Add("Select", typeof(bool)).DefaultValue = false;

            //Update existing rows
            foreach (DataRow row in source.Rows)
            {
                row["Select"] = false;
            }

            return source;
        }
    }
}