﻿using System;
using System.Data;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Messages
{
    /// <summary>
    /// Summary of table changes applied on a source
    /// </summary>
    [DataContract(Name = "tca"), Serializable]
    public class TableChangesApplied
    {
        /// <summary>
        /// ctor for serialization purpose
        /// </summary>
        public TableChangesApplied()
        {

        }

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
        /// Gets the RowState of the applied rows
        /// </summary>
        [DataMember(Name = "st", IsRequired = true)]
        public DataRowState State { get; set; }

        /// <summary>
        /// Gets the resolved conflict rows applied count
        /// </summary>
        [DataMember(Name = "rc", IsRequired = true)]
        public int ResolvedConflicts { get; set; }

        /// <summary>
        /// Gets the rows changes applied count. This count contains resolved conflicts count also
        /// </summary>
        [DataMember(Name = "a", IsRequired = true)]
        public int Applied { get; set; }

        /// <summary>
        /// Gets the rows changes failed count
        /// </summary>
        [DataMember(Name = "f", IsRequired = true)]
        public int Failed { get; set; }

        /// <summary>
        /// Gets the total rows count to apply for all tables
        /// </summary>
        [DataMember(Name = "trc", IsRequired = false)]
        public int TotalRowsCount { get; set; }

        /// <summary>
        /// Gets the total rows count applied on all tables
        /// </summary>
        [DataMember(Name = "tac", IsRequired = false)]
        public int TotalAppliedCount { get; set; }
    }

}