﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Messages
{

    /// <summary>
    /// Get the rows count cleaned for all tables, during a DeleteMetadatasAsync call
    /// </summary>
    [DataContract(Name = "dcs"), Serializable]
    public class DatabaseMetadatasCleaned
    {
        /// <summary>
        /// Get the changes selected to be applied for a current table
        /// </summary> 
        [DataMember(Name = "tcs", IsRequired = false, EmitDefaultValue = false)]
        public List<TableMetadatasCleaned> Tables { get; set; } = new List<TableMetadatasCleaned>();

        /// <summary>
        /// Gets the total number of rows cleaned
        /// </summary>
        [IgnoreDataMember]
        public int RowsCleanedCount => Tables.Sum(tcs => tcs.RowsCleanedCount);

        /// <summary>
        /// Gets or Sets the last timestamp used as the limit to clean the table metadatas. All rows below this limit have beed cleaned.
        /// </summary>
        [DataMember(Name = "ttl", IsRequired = true)]
        public long TimestampLimit { get; set; }

        public override string ToString() => $"{RowsCleanedCount} rows cleaned.";

    }



}