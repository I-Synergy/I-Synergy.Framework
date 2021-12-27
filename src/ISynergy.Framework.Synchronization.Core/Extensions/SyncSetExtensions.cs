using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Collections.Generic;
using System.IO;

namespace ISynergy.Framework.Synchronization.Core.Extensions
{
    internal static class SyncSetExtensions
    {
        public static BatchInfo ToBatchInfo(this SyncSet schema, string rootDirectory = null, string directoryName = null)
        {
            var result = new BatchInfo();

            // We need to create a change table set, containing table with columns not readonly
            foreach (var table in schema.Tables)
                DbSyncAdapter.CreateChangesTable(schema.Tables[table.TableName, table.SchemaName], result.SanitizedSchema);

            result.DirectoryRoot = rootDirectory;
            result.BatchPartsInfo = new List<BatchPartInfo>();
            result.DirectoryName = string.IsNullOrEmpty(directoryName) ? string.Concat(DateTime.UtcNow.ToString("yyyy_MM_dd_ss"), Path.GetRandomFileName().Replace(".", "")) : directoryName;

            return result;
        }
    }
}
