using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Serialization;
using ISynergy.Framework.Synchronization.Core.Set;
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
        /// <param name="localScopeId"></param>
        /// <param name="senderScopeId"></param>
        /// <param name="isNew"></param>
        /// <param name="lastTimestamp"></param>
        /// <param name="schema"></param>
        /// <param name="setup"></param>
        /// <param name="policy"></param>
        /// <param name="disableConstraintsOnApplyChanges"></param>
        /// <param name="cleanMetadatas"></param>
        /// <param name="cleanFolder"></param>
        /// <param name="snapshotApplied"></param>
        /// <param name="changes"></param>
        /// <param name="localSerializerFactory"></param>
        public MessageApplyChanges(
            Guid localScopeId, 
            Guid senderScopeId, 
            bool isNew, 
            long? lastTimestamp, 
            SyncSet schema, 
            SyncSetup setup,
            ConflictResolutionPolicy policy, 
            bool disableConstraintsOnApplyChanges, 
            bool cleanMetadatas,
            bool cleanFolder, 
            bool snapshotApplied, 
            BatchInfo changes, 
            ILocalSerializerFactory localSerializerFactory)
        {
            Argument.IsNotNull(schema);
            Argument.IsNotNull(setup);
            Argument.IsNotNull(changes);

            LocalScopeId = localScopeId;
            SenderScopeId = senderScopeId;
            IsNew = isNew;
            LastTimestamp = lastTimestamp;
            Schema = schema;
            Setup = setup;
            Policy = policy;
            DisableConstraintsOnApplyChanges = disableConstraintsOnApplyChanges;
            CleanMetadatas = cleanMetadatas;
            CleanFolder = cleanFolder;
            BatchInfo = changes;
            LocalSerializerFactory = localSerializerFactory;
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
        /// Gets or Sets the batch info containing the changes to apply
        /// </summary>
        public BatchInfo BatchInfo { get; set; }

        /// <summary>
        /// Gets or Sets the local Serializer used to buffer rows on disk
        /// </summary>
        public ILocalSerializerFactory LocalSerializerFactory { get; set; }

        /// <summary>
        /// Gets or Sets if we have already applied a snapshot. So far, we don't reset the tables, even if we are in reinit mode.
        /// </summary>
        public bool SnapshoteApplied { get; }
    }
}
