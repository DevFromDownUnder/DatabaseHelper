using Microsoft.Data.SqlClient;
using System;

namespace DatabaseHelper.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string source)
        {
            return !String.IsNullOrWhiteSpace(source);
        }

        public static bool HasNoValue(this string source)
        {
            return !HasValue(source);
        }

        public static string SmartSQLFormat(this string source, SqlParameterCollection parameters)
        {
            if (parameters == null)
            {
                return source;
            }

            var formatter = SmartFormat.Smart.Default;
            formatter.AddExtensions(new SmartFormatSqlParameterCollectionSource(formatter));

            return formatter.Format(source, (parameters));
        }
    }
}