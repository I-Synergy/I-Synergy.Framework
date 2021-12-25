using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Parameter
{
    /// <summary>
    /// Encapsulates information sent from the client to the server.
    /// </summary>
    [DataContract(Name = "par"), Serializable]
    public class SyncParameter : SyncNamedItem<SyncParameter>
    {
        /// <summary>
        /// Gets or sets the name of the column from the table involved in filter.
        /// </summary>
        [DataMember(Name = "pn", IsRequired = true, Order = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        [DataMember(Name = "v", IsRequired = true, Order = 2)]
        public object Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the SyncParameter class by using default values.
        /// </summary>
        public SyncParameter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Synchronization.Data.SyncParameter" /> class by 
        /// using name and value parameters.
        /// </summary>
        public SyncParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }


        public override string ToString() => $"{Name}: {Value}";

        public override IEnumerable<string> GetAllNamesProperties()
        {
            yield return Name;
        }

    }
}
