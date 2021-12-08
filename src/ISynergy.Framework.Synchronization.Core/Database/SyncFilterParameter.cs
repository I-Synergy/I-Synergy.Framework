using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Database
{
    /// <summary>
    /// Represents a filter parameters
    /// For example : @CustomerID int NULL = 12
    /// </summary>
    [DataContract(Name = "sfp"), Serializable]
    public class SyncFilterParameter : SyncNamedItem<SyncFilterParameter>
    {


        /// <summary>
        /// Gets the ShemaTable's SyncSchema
        /// </summary>
        [IgnoreDataMember]
        public SyncSet Schema { get; set; }

        public SyncFilterParameter()
        {

        }

        /// <summary>
        /// Create a new filter parameter with the given name
        /// </summary>
        public SyncFilterParameter(string tableName) : this(tableName, string.Empty) { }

        /// <summary>
        /// Create a new filter parameter with the given name
        /// </summary>
        public SyncFilterParameter(string tableName, string schemaName) : this()
        {
            TableName = tableName;
            SchemaName = schemaName;
        }

        /// <summary>
        /// Ensure filter parameter as the correct schema (since the property is not serialized)
        /// </summary>
        public void EnsureFilterParameter(SyncSet schema)
        {
            Schema = schema;
        }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// for SQL, will be named @{ParamterName}
        /// for MySql, will be named in_{ParameterName}
        /// </summary>
        [DataMember(Name = "n", IsRequired = true, Order = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets table name, if parameter is linked to a table
        /// </summary>
        [DataMember(Name = "t", IsRequired = true, Order = 2)]
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets schema name, if parameter is linked to a table
        /// </summary>
        [DataMember(Name = "s", IsRequired = false, EmitDefaultValue = false, Order = 3)]
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets or Sets the parameter db type
        /// </summary>
        [DataMember(Name = "dt", IsRequired = false, EmitDefaultValue = false, Order = 4)]
        public DbType? DbType { get; set; }

        /// <summary>
        /// Gets or Sets the parameter default value expression.
        /// Be careful, must be expresse in data source language
        /// </summary>
        [DataMember(Name = "dv", IsRequired = false, EmitDefaultValue = false, Order = 5)]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or Sets if the parameter is default null
        /// </summary>
        [DataMember(Name = "an", IsRequired = false, EmitDefaultValue = false, Order = 6)]
        public bool AllowNull { get; set; } = false;

        /// <summary>
        /// Gets or Sets the parameter max length (if needed)
        /// </summary>
        [DataMember(Name = "ml", IsRequired = false, EmitDefaultValue = false, Order = 7)]
        public int MaxLength { get; set; }

        public override string ToString() => Name;


        /// <summary>
        /// Get all comparable fields to determine if two instances are identifed as same by name
        /// </summary>
        public override IEnumerable<string> GetAllNamesProperties()
        {
            yield return TableName;
            yield return SchemaName;
            yield return Name;
        }

        public override bool EqualsByProperties(SyncFilterParameter other)
        {
            if (other is null)
                return false;

            if (!EqualsByName(other))
                return false;

            var sc = SyncGlobalization.DataSourceStringComparison;

            // Can be null since it'as a nullable value
            var sameDbType = DbType.HasValue && other.DbType.HasValue && DbType.Equals(other.DbType)
                            || !DbType.HasValue && !other.DbType.HasValue;

            return sameDbType
                && AllowNull.Equals(other.AllowNull)
                && MaxLength.Equals(other.MaxLength)
                && string.Equals(DefaultValue, other.DefaultValue, sc);
        }
    }
}
