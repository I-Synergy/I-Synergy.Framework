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
        InsertRow,
        DeleteRow,
        DisableConstraints,
        EnableConstraints,
        DeleteMetadata,
        UpdateMetadata,
        InsertTrigger,
        UpdateTrigger,
        DeleteTrigger,
        UpdateRows,
        InsertRows,
        DeleteRows,
        BulkTableType,
        UpdateUntrackedRows,
        Reset
    }
}
