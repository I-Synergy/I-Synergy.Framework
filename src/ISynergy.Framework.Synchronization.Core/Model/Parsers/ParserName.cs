using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Parsers;
using System.Text;

namespace ISynergy.Framework.Synchronization.Core.Model.Parsers
{

    public class ParserName
    {
        private string key;

        private bool withDatabase = false;
        private bool withSchema = false;
        private bool withQuotes = false;
        private bool withNormalized = false;

        public string SchemaName => GlobalParser.GetParserString(key).SchemaName;
        public string ObjectName => GlobalParser.GetParserString(key).ObjectName;
        public string DatabaseName => GlobalParser.GetParserString(key).DatabaseName;

        /// <summary>
        /// Add database name if available to the final string
        /// </summary>
        public ParserName Database()
        {
            withDatabase = true;
            return this;

        }

        /// <summary>
        /// Add schema if available to the final string
        /// </summary>
        public ParserName Schema()
        {
            withSchema = true;
            return this;

        }

        /// <summary>
        /// Add quotes ([] or ``) on all objects 
        /// </summary>
        /// <returns></returns>
        public ParserName Quoted()
        {
            withQuotes = true;
            return this;
        }

        public ParserName Unquoted()
        {
            withQuotes = false;
            return this;
        }

        public ParserName Normalized()
        {
            withNormalized = true;
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
            input = input == null ? string.Empty : input.Trim();
            key = input;

            if (!string.IsNullOrEmpty(leftQuote) && !string.IsNullOrEmpty(rightQuote))
                key = $"{leftQuote}^{rightQuote}^{input}";
            else if (!string.IsNullOrEmpty(leftQuote))
                key = $"{leftQuote}^{leftQuote}^{input}";

            GlobalParser.GetParserString(key);

        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var parsedName = GlobalParser.GetParserString(key);


            if (withDatabase && !string.IsNullOrEmpty(DatabaseName))
            {
                sb.Append(withQuotes ? parsedName.QuotedDatabaseName : DatabaseName);
                sb.Append(withNormalized ? "_" : ".");
            }
            if (withSchema && !string.IsNullOrEmpty(SchemaName))
            {
                sb.Append(withQuotes ? parsedName.QuotedSchemaName : SchemaName);
                sb.Append(withNormalized ? "_" : ".");
            }

            var name = withQuotes ? parsedName.QuotedObjectName : ObjectName;
            name = withNormalized ? name.Replace(" ", "_").Replace(".", "_") : name;
            sb.Append(name);

            // now we have the correct string, reset options for the next time we call the same instance
            withDatabase = false;
            withSchema = false;
            withQuotes = false;
            withNormalized = false;

            return sb.ToString();


        }

        //public string ToString(bool addQuote = false, bool addSchema = false, bool isNormalized = false, bool addDatabase = false)
        //{
        //    var sb = new StringBuilder();

        //    var parsedName = GlobalParser.GetParserString(this.key);


        //    if (addDatabase && !string.IsNullOrEmpty(this.DatabaseName))
        //    {
        //        sb.Append(addQuote ? parsedName.QuotedDatabaseName : this.DatabaseName);
        //        sb.Append(isNormalized ? "_" : ".");
        //    }
        //    if (addSchema && !string.IsNullOrEmpty(this.SchemaName))
        //    {
        //        sb.Append(addQuote ? parsedName.QuotedSchemaName : this.SchemaName);
        //        sb.Append(isNormalized ? "_" : ".");
        //    }

        //    var name = addQuote ? parsedName.QuotedObjectName : this.ObjectName;
        //    name = isNormalized ? name.Replace(" ", "_").Replace(".", "_") : name;
        //    sb.Append(name);

        //    return sb.ToString();

        //}
    }

}
