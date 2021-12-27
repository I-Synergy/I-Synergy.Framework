namespace ISynergy.Framework.Synchronization.Core.Enumerations
{
    public enum DbStoredProcedureType
    {
        SelectChanges,
        SelectChangesWithFilters,
        SelectInitializedChanges,
        SelectInitializedChangesWithFilters,
        SelectRow,
        UpdateRow,
        DeleteRow,
        DeleteMetadata,
        BulkInitRows,
        BulkUpdateRows,
        BulkDeleteRows,
        Reset,
        BulkTableType
    }
}
