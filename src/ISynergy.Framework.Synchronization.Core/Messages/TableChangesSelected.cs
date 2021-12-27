using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Messages
{
    /// <summary>
    /// Get changes to be applied (contains Deletes AND Inserts AND Updates)
    /// </summary>
    [DataContract(Name = "tcs"), Serializable]
    public class TableChangesSelected
    {

        /// <summary>
        /// Ctor for serialization purpose
        /// </summary>
        public TableChangesSelected()
        {

        }

        public TableChangesSelected(string tableName, string schemaName)
        {
            TableName = tableName;
            SchemaName = schemaName;
        }

        /// <summary>
        /// Gets the table name
        /// </summary>
        [DataMember(Name = "n", IsRequired = false, EmitDefaultValue = false)]
        public string TableName { get; set; }


        /// <summary>
        /// Get or Set the schema used for the DmTableSurrogate
        /// </summary>
        [DataMember(Name = "s", IsRequired = false, EmitDefaultValue = false)]
        public string SchemaName { get; set; }


        /// <summary>
        /// Gets or sets the number of deletes that should be applied to a table during the synchronization session.
        /// </summary>
        [DataMember(Name = "d", IsRequired = false, EmitDefaultValue = false)]
        public int Deletes { get; set; }

        /// <summary>
        /// Gets or sets the number of updates OR inserts that should be applied to a table during the synchronization session.
        /// </summary>
        [DataMember(Name = "u", IsRequired = false, EmitDefaultValue = false)]
        public int Upserts { get; set; }

        /// <summary>
        /// Gets the total number of changes that are applied to a table during the synchronization session.
        /// </summary>
        [IgnoreDataMember()]
        public int TotalChanges => Upserts + Deletes;
    }
}
