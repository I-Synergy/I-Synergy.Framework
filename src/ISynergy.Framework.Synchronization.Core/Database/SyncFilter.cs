using ISynergy.Framework.Synchronization.Core.Extensions;
using ISynergy.Framework.Synchronization.Core.Model.Parsers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Database
{
    /// <summary>
    /// Design a filter clause on Dmtable
    /// </summary>
    [DataContract(Name = "sf"), Serializable]
    public class SyncFilter : SyncNamedItem<SyncFilter>, IDisposable
    {

        [DataMember(Name = "t", IsRequired = true, Order = 1)]
        public string TableName { get; set; }

        [DataMember(Name = "s", IsRequired = false, EmitDefaultValue = false, Order = 2)]
        public string SchemaName { get; set; }


        /// <summary>
        /// Gets or Sets the parameters list, used as input in the stored procedure
        /// </summary>
        [DataMember(Name = "p", IsRequired = false, EmitDefaultValue = false, Order = 3)]
        public SyncFilterParameters Parameters { get; set; } = new SyncFilterParameters();

        /// <summary>
        /// Gets or Sets side where filters list
        /// </summary>
        [DataMember(Name = "w", IsRequired = false, EmitDefaultValue = false, Order = 4)]
        public SyncFilterWhereSideItems Wheres { get; set; } = new SyncFilterWhereSideItems();

        /// <summary>
        /// Gets or Sets side where filters list
        /// </summary>
        [DataMember(Name = "j", IsRequired = false, EmitDefaultValue = false, Order = 5)]
        public SyncFilterJoins Joins { get; set; } = new SyncFilterJoins();

        /// <summary>
        /// Gets or Sets customs where
        /// </summary>
        [DataMember(Name = "cw", IsRequired = false, EmitDefaultValue = false, Order = 6)]
        public List<string> CustomWheres { get; set; } = new List<string>();


        /// <summary>
        /// Gets the ShemaFilter's SyncSchema
        /// </summary>
        [IgnoreDataMember]
        public SyncSet Schema { get; set; }


        /// <summary>
        /// Creates a filterclause allowing to specify a different DbType.
        /// If you specify the columnType, ISynergy.Framework.Synchronization.Core will expect that the column does not exist on the table, and the filter is only
        /// used as a parameter for the selectchanges stored procedure. Thus, IsVirtual would be true
        /// </summary>
        public SyncFilter(string tableName, string schemaName = null)
        {
            TableName = tableName;
            SchemaName = schemaName;
        }


        /// <summary>
        /// Clone the SyncFilter
        /// </summary>
        public SyncFilter Clone()
        {
            var clone = new SyncFilter();
            clone.SchemaName = SchemaName;
            clone.TableName = TableName;

            return clone;
        }


        public override IEnumerable<string> GetAllNamesProperties()
        {
            yield return TableName;
            yield return SchemaName;

        }

        /// <summary>
        /// Ensure filter has the correct schema (since the property is not serialized
        /// </summary>
        public void EnsureFilter(SyncSet schema)
        {
            Schema = schema;

            Parameters.EnsureFilters(Schema);
            Wheres.EnsureFilters(Schema);
            Joins.EnsureFilters(Schema);
        }

        /// <summary>
        /// For Serializer
        /// </summary>
        public SyncFilter()
        {
        }


        /// <summary>
        /// Get unique filter name, composed by all parameters name
        /// </summary>
        public string GetFilterName()
        {
            var name = string.Empty;
            var sep = "";
            foreach (var parameterName in Parameters.Select(f => f.Name))
            {
                var columnName = ParserName.Parse(parameterName).Unquoted().Normalized().ToString();
                name += $"{columnName}{sep}";
                sep = "_";
            }

            return name;
        }



        /// <summary>
        /// Compare all properties to see if object are Equals by all properties
        /// </summary>
        public override bool EqualsByProperties(SyncFilter other)
        {
            if (other is null)
                return false;

            var sc = SyncGlobalization.DataSourceStringComparison;

            // Check name properties
            if (!EqualsByName(other))
                return false;

            // Compare all list properties
            // For each, check if they are both null or not null
            // If not null, compare each item

            if (!CustomWheres.CompareWith(other.CustomWheres, (cw, ocw) => string.Equals(ocw, cw, sc)))
                return false;

            if (!Joins.CompareWith(other.Joins))
                return false;

            if (!Parameters.CompareWith(other.Parameters))
                return false;

            if (!Wheres.CompareWith(other.Wheres))
                return false;


            return true;
        }
        /// <summary>
        /// Clear
        /// </summary>
        public void Clear() => Dispose(true);


        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool cleanup)
        {
            // Dispose managed ressources
            if (cleanup)
                // clean rows
                Schema = null;

            // Dispose unmanaged ressources
        }
    }
}
