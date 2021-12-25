using ISynergy.Framework.Synchronization.Core.Enumerations;
using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Client.Exceptions
{
    [Serializable, DataContract(Name = "ex")]
    public class WebSyncException
    {
        /// <summary>
        /// Gets or Sets type name of inner exception
        /// </summary>
        [DataMember(Name = "tn", IsRequired = false, EmitDefaultValue = false)]
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or Sets the MEssage associated to the exception
        /// </summary>
        [DataMember(Name = "m", IsRequired = false, EmitDefaultValue = false)]
        public string Message { get; set; }

        /// <summary>
        /// Sync stage when exception occured
        /// </summary>
        [DataMember(Name = "ss", IsRequired = false, EmitDefaultValue = false)]
        public SyncStage SyncStage { get; set; }

        /// <summary>
        /// Data source error number if available
        /// </summary>
        [DataMember(Name = "n", IsRequired = false, EmitDefaultValue = false)]
        public int Number { get; set; }

        /// <summary>
        /// Gets or Sets data source if available
        /// </summary>
        [DataMember(Name = "d", IsRequired = false, EmitDefaultValue = false)]
        public string DataSource { get; set; }

        /// <summary>
        /// Gets or Sets initial catalog if available
        /// </summary>
        [DataMember(Name = "ic", IsRequired = false, EmitDefaultValue = false)]
        public string InitialCatalog { get; set; }

        /// <summary>
        /// Gets or Sets if error is Local or Remote side
        /// </summary>
        [DataMember(Name = "s", IsRequired = false, EmitDefaultValue = false)]
        public SyncSide Side { get; set; }

    }
}
