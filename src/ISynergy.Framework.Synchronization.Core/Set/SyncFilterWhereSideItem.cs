﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Set
{
    [DataContract(Name = "sfwsi"), Serializable]
    public class SyncFilterWhereSideItem : SyncNamedItem<SyncFilterWhereSideItem>
    {

        [DataMember(Name = "c", IsRequired = true)]
        public string ColumnName { get; set; }

        [DataMember(Name = "t", IsRequired = true)]
        public string TableName { get; set; }

        [DataMember(Name = "s", IsRequired = false, EmitDefaultValue = false)]
        public string SchemaName { get; set; }

        [DataMember(Name = "p", IsRequired = true)]
        public string ParameterName { get; set; }


        /// <summary>
        /// Gets the ShemaTable's SyncSchema
        /// </summary>
        [IgnoreDataMember]
        public SyncSet Schema { get; set; }


        /// <summary>
        /// Ensure filter parameter as the correct schema (since the property is not serialized)
        /// </summary>
        public void EnsureFilterWhereSideItem(SyncSet schema)
        {
            Schema = schema;
        }

        /// <summary>
        /// Get all comparable fields to determine if two instances are identifed as same by name
        /// </summary>
        public override IEnumerable<string> GetAllNamesProperties()
        {
            yield return TableName;
            yield return SchemaName;
            yield return ColumnName;
            yield return ParameterName;
        }
    }
}