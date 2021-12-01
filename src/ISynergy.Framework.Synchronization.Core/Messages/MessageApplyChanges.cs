﻿using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Serialization;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;

namespace ISynergy.Framework.Synchronization.Core.Messages
{
    /// <summary>
    /// Message exchanged during the Begin session sync stage
    /// </summary>
    public class MessageApplyChanges
    {

        /// <summary>
        /// Applying changes message.
        /// Be careful policy could be differente from the schema (especially on client side, it's the reverse one, by default)
        /// </summary>
        public MessageApplyChanges(Guid localScopeId, Guid senderScopeId, bool isNew, long? lastTimestamp, SyncSet schema, SyncSetup setup,
                                    ConflictResolutionPolicy policy, bool disableConstraintsOnApplyChanges,
                                    bool useBulkOperations, bool cleanMetadatas, bool cleanFolder, bool snapshotApplied, BatchInfo changes, ISerializerFactory serializerFactory)
        {
            LocalScopeId = localScopeId;
            SenderScopeId = senderScopeId;
            IsNew = isNew;
            LastTimestamp = lastTimestamp;
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            Setup = setup ?? throw new ArgumentNullException(nameof(setup));
            Policy = policy;
            DisableConstraintsOnApplyChanges = disableConstraintsOnApplyChanges;
            UseBulkOperations = useBulkOperations;
            CleanMetadatas = cleanMetadatas;
            CleanFolder = cleanFolder;
            Changes = changes ?? throw new ArgumentNullException(nameof(changes));
            SerializerFactory = serializerFactory;
            SnapshoteApplied = snapshotApplied;
        }


        /// <summary>
        /// Gets the local Scope Id
        /// </summary>
        public Guid LocalScopeId { get; }

        /// <summary>
        /// Gets the sender Scope Id
        /// </summary>
        public Guid SenderScopeId { get; }


        /// <summary>
        /// Gets or Sets if the sync is a first sync. In this case, the last sync timestamp is ignored
        /// </summary>
        public bool IsNew { get; }

        /// <summary>
        /// Gets or Sets the last date timestamp from where we want rows
        /// </summary>
        public long? LastTimestamp { get; }

        /// <summary>
        /// Gets or Sets the schema used for this sync
        /// </summary>
        public SyncSet Schema { get; set; }

        /// <summary>
        /// Gets or Sets the setup used for this sync
        /// </summary>
        public SyncSetup Setup { get; }

        /// <summary>
        /// Gets or Sets the current Conflict resolution policy
        /// </summary>
        public ConflictResolutionPolicy Policy { get; set; }

        /// <summary>
        /// Gets or Sets if we should disable all constraints on apply changes.
        /// </summary>
        public bool DisableConstraintsOnApplyChanges { get; set; }

        /// <summary>
        /// Gets or Sets if during appy changes, we are using bulk operations
        /// </summary>
        public bool UseBulkOperations { get; set; }

        /// <summary>
        /// Gets or Sets if we should cleaning tracking table metadatas
        /// </summary>
        public bool CleanMetadatas { get; set; }

        /// <summary>
        /// Gets or Sets if we should cleaning tmp dir files after sync.
        /// </summary>
        public bool CleanFolder { get; set; }

        /// <summary>
        /// Gets or Sets the changes to apply
        /// </summary>
        public BatchInfo Changes { get; set; }

        /// <summary>
        /// Gets or Sets the Serializer factory used when batch mode is enabled
        /// </summary>
        public ISerializerFactory SerializerFactory { get; set; }

        /// <summary>
        /// Gets or Sets if we have already applied a snapshot. So far, we don't reset the tables, even if we are in reinit mode.
        /// </summary>
        public bool SnapshoteApplied { get; }
    }
}
