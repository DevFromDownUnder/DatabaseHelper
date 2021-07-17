using Microsoft.Data.SqlClient;
using SmartFormat;
using SmartFormat.Core.Extensions;

namespace DatabaseHelper.Extensions
{
    public class SmartFormatSqlParameterCollectionSource : ISource
    {
        public SmartFormatSqlParameterCollectionSource(SmartFormatter formatter)
        {
            // Add some special info to the parser:
            formatter.Parser.AddAlphanumericSelectors(); // (A-Z + a-z)
            formatter.Parser.AddAdditionalSelectorChars("_@");
            formatter.Parser.AddOperators(".");
        }

        public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
        {
            var current = selectorInfo.CurrentValue;
            var selector = selectorInfo.SelectorText;

            if (current is SqlParameterCollection rawSqlParameterCollection)
            {
                foreach (SqlParameter parameter in rawSqlParameterCollection)
                {
                    var key = parameter.ParameterName;
                    if (key.Equals(selector, selectorInfo.FormatDetails.Settings.CaseSensitivity == SmartFormat.Core.Settings.CaseSensitivityType.CaseSensitive ? System.StringComparison.Ordinal : System.StringComparison.OrdinalIgnoreCase))
                    {
                        selectorInfo.Result = parameter.Value.ToString();
                        return true;
                    }
                }
            }

            //Will output straight placeholder name instead of erroring
            selectorInfo.Result = selector;
            return true;
        }
    }
}