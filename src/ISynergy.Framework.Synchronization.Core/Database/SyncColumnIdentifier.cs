using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Database
{
    [DataContract(Name = "sci"), Serializable]
    public class SyncColumnIdentifier : SyncNamedItem<SyncColumnIdentifier>
    {
        [DataMember(Name = "c", IsRequired = true, Order = 1)]
        public string ColumnName { get; set; }

        [DataMember(Name = "t", IsRequired = true, Order = 2)]
        public string TableName { get; set; }

        [DataMember(Name = "s", IsRequired = false, EmitDefaultValue = false, Order = 3)]
        public string SchemaName { get; set; }

        public SyncColumnIdentifier()
        {

        }

        public SyncColumnIdentifier(string columnName, string tableName, string schemaName = null)
        {
            TableName = tableName;
            SchemaName = schemaName;
            ColumnName = columnName;
        }


        public SyncColumnIdentifier Clone() => new SyncColumnIdentifier
        {
            ColumnName = ColumnName,
            SchemaName = SchemaName,
            TableName = TableName
        };

        public override string ToString()
        {
            if (string.IsNullOrEmpty(SchemaName))
                return $"{TableName}-{ColumnName}";
            else
                return $"{SchemaName}.{TableName}-{ColumnName}";

        }

        public override IEnumerable<string> GetAllNamesProperties()
        {
            yield return TableName;
            yield return SchemaName;
            yield return ColumnName;
        }

    }
}
