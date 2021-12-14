﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Database
{
    [DataContract(Name = "ct"), Serializable]
    public class ContainerTable : SyncNamedItem<ContainerTable>
    {
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
        /// List of rows
        /// </summary>
        [DataMember(Name = "r", IsRequired = false, Order = 3)]
        public List<object[]> Rows { get; set; } = new List<object[]>();

        public ContainerTable()
        {

        }

        public ContainerTable(SyncTable table)
        {
            TableName = table.TableName;
            SchemaName = table.SchemaName;
        }

        /// <summary>
        /// Check if we have rows in this container table
        /// </summary>
        public bool HasRows => Rows.Count > 0;

        public void Clear() => Rows.Clear();
        public override IEnumerable<string> GetAllNamesProperties()
        {
            yield return TableName;
            yield return SchemaName;

        }

    }

}
