﻿using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Setup
{
    [DataContract(Name = "sfw"), Serializable]
    public class SetupFilterWhere : SyncNamedItem<SetupFilterWhere>
    {
        [DataMember(Name = "tn", IsRequired = true)]
        public string TableName { get; set; }

        [DataMember(Name = "sn", IsRequired = false, EmitDefaultValue = false)]
        public string SchemaName { get; set; }

        [DataMember(Name = "cn", IsRequired = true)]
        public string ColumnName { get; set; }

        [DataMember(Name = "pn", IsRequired = true)]
        public string ParameterName { get; set; }


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