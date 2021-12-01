using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Database
{
    /// <summary>
    /// Represents a table schema
    /// </summary>
    [DataContract(Name = "st"), Serializable]
    public class SyncTable : SyncNamedItem<SyncTable>, IDisposable
    {
        [NonSerialized]
        private SyncRows rows;

        /// <summary>
        /// Gets or sets the name of the table that the DmTableSurrogate object represents.
        /// </summary>
        [DataMember(Name = "n", IsRequired = true, Order = 1)]
        public string TableName { get; set; }

        /// <summary>
        /// Get or Set the schema used for the DmTableSurrogate
        /// </summary>
        [DataMember(Name = "s", IsRequired = false, EmitDefaultValue = false, Order = 2)]
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets or Sets the original provider (SqlServer, MySql, Sqlite, Oracle, PostgreSQL)
        /// </summary>
        [DataMember(Name = "op", IsRequired = false, EmitDefaultValue = false, Order = 3)]
        public string OriginalProvider { get; set; }

        /// <summary>
        /// Gets or Sets the Sync direction (may be Bidirectional, DownloadOnly, UploadOnly) 
        /// Default is Bidirectional
        /// </summary>
        [DataMember(Name = "sd", IsRequired = false, EmitDefaultValue = false, Order = 4)]
        public SyncDirection SyncDirection { get; set; }

        /// <summary>
        /// Gets or Sets the table columns
        /// </summary>
        [DataMember(Name = "c", IsRequired = false, EmitDefaultValue = false, Order = 5)]
        public SyncColumns Columns { get; set; }

        /// <summary>
        /// Gets or Sets the table primary keys
        /// </summary>
        [DataMember(Name = "pk", IsRequired = false, EmitDefaultValue = false, Order = 6)]
        public Collection<string> PrimaryKeys { get; set; } = new Collection<string>();


        /// <summary>
        /// Gets the ShemaTable's rows
        /// </summary>
        [IgnoreDataMember]
        public SyncRows Rows
        {
            // Use of field property because of attribute [NonSerialized] necessary for binaryformatter
            get => rows;
            private set => rows = value;
        }


        /// <summary>
        /// Gets the ShemaTable's SyncSchema
        /// </summary>
        [IgnoreDataMember]
        public SyncSet Schema { get; set; }

        public SyncTable()
        {
            Rows = new SyncRows(this);
            Columns = new SyncColumns(this);
        }

        /// <summary>
        /// Create a new sync table with the given name
        /// </summary>
        public SyncTable(string tableName) : this(tableName, string.Empty) { }

        /// <summary>
        /// Create a new sync table with the given name
        /// </summary>
        public SyncTable(string tableName, string schemaName) : this()
        {
            TableName = tableName;
            SchemaName = schemaName;
        }

        /// <summary>
        /// Ensure table as the correct schema (since the property is not serialized
        /// </summary>
        public void EnsureTable(SyncSet schema)
        {
            Schema = schema;

            if (Columns != null)
                Columns.EnsureColumns(this);

            if (Rows != null)
                Rows.EnsureRows(this);
        }

        /// <summary>
        /// Clear the Table's rows
        /// </summary>
        public void Clear() => Dispose(true);


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanup)
        {
            // Dispose managed ressources
            if (cleanup)
            {
                if (Rows != null)
                    Rows.Clear();

                if (Columns != null)
                    Columns.Clear();

                Schema = null;
            }

            // Dispose unmanaged ressources
        }

        /// <summary>
        /// Clone the table structure (without rows)
        /// </summary>
        public SyncTable Clone()
        {
            var clone = new SyncTable
            {
                OriginalProvider = OriginalProvider,
                SchemaName = SchemaName,
                SyncDirection = SyncDirection,
                TableName = TableName
            };

            foreach (var c in Columns)
                clone.Columns.Add(c.Clone());

            foreach (var pkey in PrimaryKeys)
                clone.PrimaryKeys.Add(pkey);

            return clone;
        }

        /// <summary>
        /// Create a new row
        /// </summary>
        public SyncRow NewRow(DataRowState state = DataRowState.Unchanged)
        {
            var row = new SyncRow(Columns.Count)
            {
                RowState = state,
                Table = this
            };

            return row;
        }

        public IEnumerable<SyncRelation> GetRelations()
        {
            if (Schema == null)
                return Enumerable.Empty<SyncRelation>();

            return Schema.Relations.Where(r => r.GetTable().EqualsByName(this));
        }


        /// <summary>
        /// Gets the full name of the table, based on schema name + "." + table name (if schema name exists)
        /// </summary>
        /// <returns></returns>
        public string GetFullName()
            => string.IsNullOrEmpty(SchemaName) ? TableName : $"{SchemaName}.{TableName}";


        /// <summary>
        /// Get all columns that can be updated
        /// </summary>
        public IEnumerable<SyncColumn> GetMutableColumns(bool includeAutoIncrement = true, bool includePrimaryKeys = false)
        {
            foreach (var column in Columns.OrderBy(c => c.Ordinal))
                if (!column.IsCompute && !column.IsReadOnly)
                {
                    var isPrimaryKey = PrimaryKeys.Any(pkey => column.ColumnName.Equals(pkey, SyncGlobalization.DataSourceStringComparison));

                    if (includePrimaryKeys && isPrimaryKey)
                        yield return column;
                    else if (!isPrimaryKey && (includeAutoIncrement || !includeAutoIncrement && !column.IsAutoIncrement))
                        yield return column;
                }
        }

        /// <summary>
        /// Get all columns that can be queried
        /// </summary>
        public IEnumerable<SyncColumn> GetMutableColumnsWithPrimaryKeys()
        {
            foreach (var column in Columns.OrderBy(c => c.Ordinal))
                if (!column.IsCompute && !column.IsReadOnly)
                    yield return column;
        }

        /// <summary>
        /// Get all columns that are Primary keys, based on the names we have in PrimariKeys property
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SyncColumn> GetPrimaryKeysColumns()
        {
            foreach (var column in Columns.OrderBy(c => c.Ordinal))
            {
                var isPrimaryKey = PrimaryKeys.Any(pkey => column.ColumnName.Equals(pkey, SyncGlobalization.DataSourceStringComparison));

                if (isPrimaryKey)
                    yield return column;
            }
        }

        /// <summary>
        /// Get all filters for a selected sync table
        /// </summary>
        public SyncFilter GetFilter()
        {
            if (Schema == null || Schema.Filters == null || Schema.Filters.Count <= 0)
                return null;

            return Schema.Filters.FirstOrDefault(sf =>
            {
                var sc = SyncGlobalization.DataSourceStringComparison;

                var sn = sf.SchemaName == null ? string.Empty : sf.SchemaName;
                var otherSn = SchemaName == null ? string.Empty : SchemaName;

                return TableName.Equals(sf.TableName, sc) && sn.Equals(otherSn, sc);
            });
        }

        public void Load(DbDataReader reader)
        {
            var readerFieldCount = reader.FieldCount;

            if (readerFieldCount == 0 || !reader.HasRows)
                return;

            if (Columns.Count == 0)
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var columnType = reader.GetFieldType(i);
                    Columns.Add(columnName, columnType);
                }

            while (reader.Read())
            {
                var row = NewRow();

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var columnValueObject = reader.GetValue(i);
                    //var columnValue = columnValueObject == DBNull.Value ? null : columnValueObject;

                    row[columnName] = columnValueObject;
                }

                Rows.Add(row);
            }
        }

        /// <summary>
        /// Check if a column name is a primary key
        /// </summary>
        public bool IsPrimaryKey(string columnName) => PrimaryKeys.Any(pkey => columnName.Equals(pkey, SyncGlobalization.DataSourceStringComparison));

        /// <summary>
        /// Gets a value returning if the SchemaTable contains an auto increment column
        /// </summary>
        public bool HasAutoIncrementColumns => Columns.Any(c => c.IsAutoIncrement);

        /// <summary>
        /// Gets a value indicating if the synctable has rows
        /// </summary>
        public bool HasRows => Rows != null && Rows.Count > 0;

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(SchemaName))
                return $"{SchemaName}.{TableName}";
            else
                return TableName;
        }

        /// <summary>
        /// Get all comparable fields to determine if two instances are identifed as same by name
        /// </summary>
        public override IEnumerable<string> GetAllNamesProperties()
        {
            yield return TableName;
            yield return SchemaName;
        }

        public override bool EqualsByProperties(SyncTable other)
        {
            if (other == null)
                return false;

            var sc = SyncGlobalization.DataSourceStringComparison;

            if (!EqualsByName(other))
                return false;

            // checking properties
            if (SyncDirection != other.SyncDirection)
                return false;

            if (!string.Equals(OriginalProvider, other.OriginalProvider, sc))
                return false;

            // Check list
            if (!Columns.CompareWith(other.Columns))
                return false;

            if (!PrimaryKeys.CompareWith(other.PrimaryKeys))
                return false;


            return true;

        }
    }
}
