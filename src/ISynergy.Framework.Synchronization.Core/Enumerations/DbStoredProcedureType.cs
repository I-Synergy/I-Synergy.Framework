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
        BulkTableType,
        BulkUpdateRows,
        BulkDeleteRows,
        Reset
    }
}
