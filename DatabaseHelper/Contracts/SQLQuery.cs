using Microsoft.Data.SqlClient;
using PropertyChanged;

namespace DatabaseHelper.Contracts
{
    [AddINotifyPropertyChangedInterface]
    public class SQLQuery
    {
        public SqlParameterCollection Parameters { get; set; }
        public string Query { get; set; }
    }
}