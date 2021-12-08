using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Parsers;
using System.Text;

namespace ISynergy.Framework.Synchronization.Core.Model.Parsers
{

    public class ParserName
    {
        private string _key;

        private bool _withDatabase = false;
        private bool _withSchema = false;
        private bool _withQuotes = false;
        private bool _withNormalized = false;

        public string SchemaName => GlobalParser.GetParserString(_key).SchemaName;
        public string ObjectName => GlobalParser.GetParserString(_key).ObjectName;
        public string DatabaseName => GlobalParser.GetParserString(_key).DatabaseName;

        /// <summary>
        /// Add database name if available to the final string
        /// </summary>
        public ParserName Database()
        {
            _withDatabase = true;
            return this;

        }

        /// <summary>
        /// Add schema if available to the final string
        /// </summary>
        public ParserName Schema()
        {
            _withSchema = true;
            return this;

        }

        /// <summary>
        /// Add quotes ([] or ``) on all objects 
        /// </summary>
        /// <returns></returns>
        public ParserName Quoted()
        {
            _withQuotes = true;
            return this;
        }

        public ParserName Unquoted()
        {
            _withQuotes = false;
            return this;
        }

        public ParserName Normalized()
        {
            _withNormalized = true;
            return this;
        }


        public static ParserName Parse(SyncTable syncTable, string leftQuote = null, string rightQuote = null) => new ParserName(syncTable, leftQuote, rightQuote);
        public static ParserName Parse(SyncColumn syncColumn, string leftQuote = null, string rightQuote = null) => new ParserName(syncColumn, leftQuote, rightQuote);
        public static ParserName Parse(string input, string leftQuote = null, string rightQuote = null) => new ParserName(input, leftQuote, rightQuote);


        private ParserName() { }

        private ParserName(string input, string leftQuote = null, string rightQuote = null) => ParseString(input, leftQuote, rightQuote);
        private ParserName(SyncColumn column, string leftQuote = null, string rightQuote = null) => ParseString(column.ColumnName, leftQuote, rightQuote);
        private ParserName(SyncTable table, string leftQuote = null, string rightQuote = null)
        {
            var input = string.IsNullOrEmpty(table.SchemaName) ? table.TableName : $"{table.SchemaName}.{table.TableName}";
            ParseString(input, leftQuote, rightQuote);
        }

        /// <summary>
        /// Parse the input string and Get a non bracket object name :
        ///   "[Client] ==> Client "
        ///   "[dbo].[client] === > dbo client "
        ///   "dbo.client === > dbo client "
        ///   "Fabrikam.[dbo].[client] === > Fabrikam dbo client "
        /// </summary>
        private void ParseString(string input, string leftQuote = null, string rightQuote = null)
        {
            input = input is null ? string.Empty : input.Trim();
            _key = input;

            if (!string.IsNullOrEmpty(leftQuote) && !string.IsNullOrEmpty(rightQuote))
                _key = $"{leftQuote}^{rightQuote}^{input}";
            else if (!string.IsNullOrEmpty(leftQuote))
                _key = $"{leftQuote}^{leftQuote}^{input}";

            GlobalParser.GetParserString(_key);

        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var parsedName = GlobalParser.GetParserString(_key);


            if (_withDatabase && !string.IsNullOrEmpty(DatabaseName))
            {
                sb.Append(_withQuotes ? parsedName.QuotedDatabaseName : DatabaseName);
                sb.Append(_withNormalized ? "_" : ".");
            }
            if (_withSchema && !string.IsNullOrEmpty(SchemaName))
            {
                sb.Append(_withQuotes ? parsedName.QuotedSchemaName : SchemaName);
                sb.Append(_withNormalized ? "_" : ".");
            }

            var name = _withQuotes ? parsedName.QuotedObjectName : ObjectName;
            name = _withNormalized ? name.Replace(" ", "_").Replace(".", "_") : name;
            sb.Append(name);

            // now we have the correct string, reset options for the next time we call the same instance
            _withDatabase = false;
            _withSchema = false;
            _withQuotes = false;
            _withNormalized = false;

            return sb.ToString();
        }
    }
}
