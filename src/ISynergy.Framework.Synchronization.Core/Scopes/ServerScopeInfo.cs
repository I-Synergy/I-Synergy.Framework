﻿using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Scopes
{
    /// <summary>
    /// Mapping sur la table ScopeInfo
    /// </summary>
    [DataContract(Name = "server_scope"), Serializable]
    public class ServerScopeInfo
    {
        /// <summary>
        /// Scope name. Shared by all clients and the server
        /// </summary>
        [DataMember(Name = "n", IsRequired = true)]
        public string Name { get; set; }

        /// <summary>
        /// Scope schema. stored locally
        /// </summary>
        [IgnoreDataMember]
        public SyncSet Schema { get; set; }

        /// <summary>
        /// Setup. stored locally
        /// </summary>
        [DataMember(Name = "s", IsRequired = true)]
        public SyncSetup Setup { get; set; }

        /// <summary>
        /// Gets or Sets the schema version
        /// </summary>
        [DataMember(Name = "v", IsRequired = false, EmitDefaultValue = false)]
        public string Version { get; set; }

        /// <summary>
        /// Gets or Sets the last timestamp a sync has occured. This timestamp is set just 'before' sync start.
        /// </summary>
        [DataMember(Name = "lst", IsRequired = false, EmitDefaultValue = false)]
        public long LastCleanupTimestamp { get; set; }



    }
}