namespace ISynergy.Framework.Synchronization.Core.Enumerations
{
    /// <summary>
    /// Defines the types of conflicts that can occur during synchronization.
    /// </summary>
    public enum ConflictType
    {
        /// <summary>
        /// The peer database threw an exception while applying a change.
        /// </summary>
        ErrorsOccurred,
        /// <summary>
        /// The remote datasource raised an unique key constraint error
        /// </summary>
        UniqueKeyConstraint,
        /// <summary>
        /// The Remote and Local datasources have both updated the same row.
        /// </summary>
        RemoteExistsLocalExists,
        /// <summary>
        /// The Remote and Local datasource have both deleted the same row.
        /// </summary>
        RemoteIsDeletedLocalIsDeleted,
        /// <summary>
        /// The Remote datasource has updated or inserted a row that does not exists in the local datasource.
        /// </summary>
        RemoteExistsLocalNotExists,
        /// <summary>
        /// The Local datasource has inserted or updated a row that does not exists in the Remote datasource 
        /// </summary>
        RemoteNotExistsLocalExists,
        /// <summary>
        /// The Remote datasource has inserted or updated a row that the Local datasource has deleted.
        /// </summary>
        RemoteExistsLocalIsDeleted,
        /// <summary>
        /// The Remote datasource has deleted a row that the Local datasource has inserted or updated.
        /// </summary>
        RemoteIsDeletedLocalExists,
        /// <summary>
        /// The Remote datasource has deleted a row that does not exists in the Local datasource 
        /// </summary>
        RemoteIsDeletedLocalNotExists
    }
}
