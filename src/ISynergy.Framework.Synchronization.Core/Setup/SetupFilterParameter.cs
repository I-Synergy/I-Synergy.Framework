﻿using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Setup
{
    /// <summary>
    /// Represents a filter parameters
    /// For example : @CustomerID int NULL = 12
    /// </summary>
    [DataContract(Name = "sfp"), Serializable]
    public class SetupFilterParameter : SyncNamedItem<SetupFilterParameter>
    {
        /// <summary>
        /// Gets or sets the name of the parameter.
        /// for SQL, will be named @{ParamterName}
        /// for MySql, will be named in_{ParameterName}
        /// </summary>
        [DataMember(Name = "n", IsRequired = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets of Sets the table name if parameter is a column 
        /// </summary>
        [DataMember(Name = "tn", IsRequired = true)]
        public string TableName { get; set; }


        /// <summary>
        /// Gets of Sets the table schema name if parameter is a column 
        /// </summary>
        [DataMember(Name = "sn", IsRequired = true)]
        public string SchemaName { get; set; }


        /// <summary>
        /// Gets or Sets the parameter db type
        /// </summary>
        [DataMember(Name = "dt", IsRequired = false, EmitDefaultValue = false)]
        public DbType? DbType { get; set; }

        /// <summary>
        /// Gets or Sets the parameter default value expression.
        /// Be careful, must be expresse in data source language
        /// </summary>
        [DataMember(Name = "dv", IsRequired = false, EmitDefaultValue = false)]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or Sets if the parameter is default null
        /// </summary>
        [DataMember(Name = "an", IsRequired = false, EmitDefaultValue = false)]
        public bool AllowNull { get; set; } = false;

        /// <summary>
        /// Gets or Sets the parameter max length (if needed)
        /// </summary>
        [DataMember(Name = "ml", IsRequired = false, EmitDefaultValue = false)]
        public int MaxLength { get; set; }


        public override IEnumerable<string> GetAllNamesProperties()
        {
            yield return TableName;
            yield return SchemaName;
            yield return Name;
        }

        public override bool EqualsByProperties(SetupFilterParameter other)
        {
            if (other is null)
                return false;

            // Check names properties
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