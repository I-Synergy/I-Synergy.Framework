﻿using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Messages
{
    [DataContract(Name = "tmc"), Serializable]
    public class TableMetadatasCleaned
    {
        /// <summary>
        /// Gets or sets the name of the table that the DmTableSurrogate object represents.
        /// </summary>
        [DataMember(Name = "tn", IsRequired = true)]
        public string TableName { get; set; }

        /// <summary>
        /// Get or Set the schema used for the DmTableSurrogate
        /// </summary>
        [DataMember(Name = "sn", IsRequired = false, EmitDefaultValue = false)]
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets or Sets the last timestamp used as the limit to clean the table metadatas. All rows below this limit have beed cleaned.
        /// </summary>
        [DataMember(Name = "ttl", IsRequired = true)]
        public long TimestampLimit { get; set; }


        /// <summary>
        /// Createa new instance of a summary of metadatas cleaned for one table
        /// </summary>
        public TableMetadatasCleaned(string tableName, string schemaName)
        {
            TableName = tableName;
            SchemaName = schemaName;
        }

        /// <summary>
        /// Gets or Sets the metadatas rows count, that have been cleaned
        /// </summary>
        [DataMember(Name = "rcc", IsRequired = true)]
        public int RowsCleanedCount { get; set; }

    }
}