using ISynergy.Framework.Synchronization.Core.Extensions;
using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Setup
{
    /// <summary>
    /// Design a filter clause on Dmtable
    /// </summary>
    [DataContract(Name = "sf"), Serializable]
    public class SetupFilter : SyncNamedItem<SetupFilter>
    {

        /// <summary>
        /// Gets or Sets the name of the table where the filter will be applied (and so the _Changes stored proc)
        /// </summary>
        [DataMember(Name = "tn", IsRequired = true)]
        public string TableName { get; set; }

        /// <summary>
        /// Gets or Sets the schema name of the table where the filter will be applied (and so the _Changes stored proc)
        /// </summary>
        [DataMember(Name = "sn", IsRequired = false, EmitDefaultValue = false)]
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets the custom joins list, used with custom wheres
        /// </summary>
        [DataMember(Name = "j", IsRequired = false, EmitDefaultValue = false)]
        public List<SetupFilterJoin> Joins { get; } = new List<SetupFilterJoin>();

        /// <summary>
        /// Gets the custom joins list, used with custom wheres
        /// </summary>
        [DataMember(Name = "cw", IsRequired = false, EmitDefaultValue = false)]
        public List<string> CustomWheres { get; } = new List<string>();

        /// <summary>
        /// Gets the parameters list, used as input in the stored procedure
        /// </summary>
        [DataMember(Name = "p", IsRequired = false, EmitDefaultValue = false)]
        public List<SetupFilterParameter> Parameters { get; } = new List<SetupFilterParameter>();

        /// <summary>
        /// Side where filters list
        /// </summary>
        [DataMember(Name = "w", IsRequired = false, EmitDefaultValue = false)]
        public List<SetupFilterWhere> Wheres { get; } = new List<SetupFilterWhere>();

        /// <summary>
        /// Creates a filterclause allowing to specify a different DbType.
        /// If you specify the columnType, ISynergy.Framework.Synchronization will expect that the column does not exist on the table, and the filter is only
        /// used as a parameter for the selectchanges stored procedure. Thus, IsVirtual would be true
        /// </summary>
        public SetupFilter(string tableName, string schemaName = null)
        {
            TableName = tableName;
            SchemaName = schemaName;
        }

        /// <summary>
        /// Add a parameter as input to stored procedure
        /// For SQL Server, parameter will be added as @{parameterName}
        /// For MySql, parameter will be added as in_{parameterName}
        /// </summary>
        public void AddParameter(string parameterName, DbType type, bool allowNull = false, string defaultValue = null, int maxLength = 0)
        {

            if (Parameters.Any(p => string.Equals(p.Name, parameterName, SyncGlobalization.DataSourceStringComparison)))
                throw new FilterParameterAlreadyExistsException(parameterName, TableName);

            var parameter = new SetupFilterParameter { Name = parameterName, DbType = type, DefaultValue = defaultValue, AllowNull = allowNull, MaxLength = maxLength };

            Parameters.Add(parameter);
        }

        /// <summary>
        /// Add a parameter based on a column. 
        /// For SQL Server, parameter will be added as @{parameterName}
        /// For MySql, parameter will be added as in_{parameterName}
        /// </summary>
        public void AddParameter(string columnName, string tableName, string schemaName, bool allowNull = false, string defaultValue = null)
        {

            if (Parameters.Any(p => string.Equals(p.Name, columnName, SyncGlobalization.DataSourceStringComparison)))
                throw new FilterParameterAlreadyExistsException(columnName, TableName);

            Parameters.Add(new SetupFilterParameter { Name = columnName, TableName = tableName, SchemaName = schemaName, DefaultValue = defaultValue, AllowNull = allowNull });
        }


        /// <summary>
        /// Add a parameter based on a column. 
        /// For SQL Server, parameter will be added as @{parameterName}
        /// For MySql, parameter will be added as in_{parameterName}
        /// </summary>
        public void AddParameter(string columnName, string tableName, bool allowNull = false, string defaultValue = null)
        {
            if (Parameters.Any(p => string.Equals(p.Name, columnName, SyncGlobalization.DataSourceStringComparison)))
                throw new FilterParameterAlreadyExistsException(columnName, TableName);

            Parameters.Add(new SetupFilterParameter { Name = columnName, TableName = tableName, DefaultValue = defaultValue, AllowNull = allowNull });
        }


        /// <summary>
        /// Add a custom filter clause
        /// </summary>
        public SetupFilterOn AddJoin(Join join, string tableName) => new SetupFilterOn(this, join, tableName);

        /// <summary>
        /// Internal add custom join
        /// </summary>
        internal void AddJoin(SetupFilterJoin setupFilterJoin)
        {
            Joins.Add(setupFilterJoin);
        }


        /// <summary>
        /// Add a Where clause. 
        /// </summary>
        public SetupFilter AddWhere(string columnName, string tableName, string parameterName, string schemaName = null)
        {
            if (!Parameters.Any(p => string.Equals(p.Name, parameterName, SyncGlobalization.DataSourceStringComparison)))
                throw new FilterTrackingWhereException(parameterName);

            Wheres.Add(new SetupFilterWhere { ColumnName = columnName, TableName = tableName, ParameterName = parameterName, SchemaName = schemaName });
            return this;
        }

        /// <summary>
        /// Add a custom Where clause. 
        /// </summary>
        public SetupFilter AddCustomWhere(string where)
        {
            // check we don't add a null value
            where = where ?? string.Empty;

            CustomWheres.Add(where);
            return this;
        }

        /// <summary>
        /// For Serializer
        /// </summary>
        public SetupFilter()
        {
        }

        /// <summary>
        /// Get all comparable fields to determine if two instances are identifed as same by name
        /// </summary>
        public override IEnumerable<string> GetAllNamesProperties()
        {
            yield return TableName;
            yield return SchemaName;
        }


        /// <summary>
        /// Compare all properties to see if object are Equals by all properties
        /// </summary>
        public override bool EqualsByProperties(SetupFilter other)
        {
            if (other is null)
                return false;

            // Check name properties
            if (!EqualsByName(other))
                return false;

            // Compare all list properties
            // For each, check if they are both null or not null
            // If not null, compare each item

            if (!Joins.CompareWith(other.Joins))
                return false;

            if (!Parameters.CompareWith(other.Parameters))
                return false;

            if (!Wheres.CompareWith(other.Wheres))
                return false;

            // since it's string comparison, don't rely on internal comparison and provide our own comparison func, using StringComparison
            var sc = SyncGlobalization.DataSourceStringComparison;
            if (!CustomWheres.CompareWith(other.CustomWheres, (c, oc) => string.Equals(c, oc, sc)))
                return false;

            return true;
        }


    }
}
