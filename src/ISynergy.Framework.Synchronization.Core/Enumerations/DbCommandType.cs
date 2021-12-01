namespace ISynergy.Framework.Synchronization.Core.Enumerations
{
    public enum DbCommandType
    {
        SelectChanges,
        SelectInitializedChanges,
        SelectInitializedChangesWithFilters,
        SelectChangesWithFilters,
        SelectRow,
        UpdateRow,
        InitializeRow,
        DeleteRow,
        DisableConstraints,
        EnableConstraints,
        DeleteMetadata,
        UpdateMetadata,
        InsertTrigger,
        UpdateTrigger,
        DeleteTrigger,
        BulkTableType,
        BulkUpdateRows,
        BulkInitializeRows,
        BulkDeleteRows,
        UpdateUntrackedRows,
        Reset
    }
}
