﻿using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Serialization;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;

namespace ISynergy.Framework.Synchronization.Core.Messages
{
    /// <summary>
    /// Message exchanged during the Get Changes Batch sync stage
    /// </summary>
    public class MessageGetChangesBatch
    {
        public MessageGetChangesBatch(Guid? excludingScopeId, Guid localScopeId, bool isNew, long? lastTimestamp, SyncSet schema, SyncSetup setup,
                                      int batchSize, string batchDirectory, bool supportsMultiActiveResultSets, ILocalSerializerFactory localSerializerFactory)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            Setup = setup ?? throw new ArgumentNullException(nameof(setup));
            BatchDirectory = batchDirectory ?? throw new ArgumentNullException(nameof(batchDirectory));
            SupportsMultiActiveResultSets = supportsMultiActiveResultSets;
            //SerializerFactory = serializerFactory;
            LocalSerializerFactory = localSerializerFactory;
            ExcludingScopeId = excludingScopeId;
            LocalScopeId = localScopeId;
            IsNew = isNew;
            LastTimestamp = lastTimestamp;
            BatchSize = batchSize;
        }

        /// <summary>
        /// Gets or Sets the Scope Id that should be excluded when we get lines from the local store
        /// Usable only from Server side
        /// </summary>
        public Guid? ExcludingScopeId { get; set; }

        /// <summary>
        /// Gets or Sets the local Scope Id that will replace NULL values when creating the row
        /// </summary>
        public Guid LocalScopeId { get; set; }


        /// <summary>
        /// Gets or Sets if the sync is a first sync. In this case, the last sync timestamp is ignored
        /// </summary>
        public bool IsNew { get; set; }


        /// <summary>
        /// Gets or Sets the last date timestamp from where we want rows
        /// </summary>
        public long? LastTimestamp { get; set; }

        /// <summary>
        /// Gets or Sets the schema used for this sync
        /// </summary>
        public SyncSet Schema { get; set; }

        /// <summary>
        /// Gets or Sets the Setup used for this sync
        /// </summary>
        public SyncSetup Setup { get; }

        /// <summary>
        /// Gets or Sets the download batch size, if needed
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or Sets the batch directory used to serialize the datas
        /// </summary>
        public string BatchDirectory { get; set; }
        public bool SupportsMultiActiveResultSets { get; }

        ///// <summary>
        ///// Gets or Sets the Serializer used to serialize rows
        ///// </summary>
        //public ISerializerFactory SerializerFactory { get; set; }

        /// <summary>
        /// Gets or Sets the Local Serializer factory, used to buffer rows when reading from datasource
        /// </summary>
        public ILocalSerializerFactory LocalSerializerFactory { get; set; }
    }
}
