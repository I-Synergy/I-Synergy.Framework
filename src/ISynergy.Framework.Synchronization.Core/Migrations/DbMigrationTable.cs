using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Providers;
using System;
using System.Collections.Generic;

/* Unmerged change from project 'ISynergy.Framework.Synchronization.Core.Core (net5.0)'
Before:
using System.Data.Common;
After:
using System.Data;
using System.Data.Common;
*/

/* Unmerged change from project 'ISynergy.Framework.Synchronization.Core.Core (net6.0)'
Before:
using System.Data.Common;
After:
using System.Data;
using System.Data.Common;
*/
using System.Linq;
/* Unmerged change from project 'ISynergy.Framework.Synchronization.Core.Core (net5.0)'
Before:
using System.Threading.Tasks;
using System.Data;
After:
using System.Threading.Tasks;
*/

/* Unmerged change from project 'ISynergy.Framework.Synchronization.Core.Core (net6.0)'
Before:
using System.Threading.Tasks;
using System.Data;
After:
using System.Threading.Tasks;
*/


namespace ISynergy.Framework.Synchronization.Core.Migrations
{
    public class DbMigrationTable
    {
        private readonly SyncTable currentTable;
        private readonly SyncTable newTable;
        private readonly bool preserveTracking;
        private readonly CoreProvider provider;

        public DbMigrationTable(CoreProvider provider, SyncTable currentTable, SyncTable newTable, bool preserveTracking = true)
        {
            this.currentTable = currentTable ?? throw new ArgumentNullException(nameof(currentTable));
            this.newTable = newTable ?? throw new ArgumentNullException(nameof(newTable));
            this.preserveTracking = preserveTracking;
            this.provider = provider;
        }

        public void Compare()
        {
            // Checking if prefixes or suffixes have changed on Tracking tables, Triggers or Stored procedures
            CheckSchema();

            // Checking if primary keys have changed
            CheckPrimaryKeys();

            // Checking if we have some new or removed columns
            CheckAddedOrRemovedColumns();
        }

        /// <summary>
        /// Check if prefixes or suffixes have changed on Tracking tables, Triggers or Stored procedures
        /// </summary>
        private void CheckSchema()
        {
            var currentSchema = currentTable.Schema;
            var newSchema = newTable.Schema;

            // if tracking table name changed, we need to:
            // - Alter tracking table name
            // - Regenerate stored procedure
            // - Regenerate triggers
            //if (currentSchema.TrackingTablesPrefix != newSchema.TrackingTablesPrefix ||
            //    currentSchema.TrackingTablesSuffix != newSchema.TrackingTablesSuffix)
            //{
            //    this.NeedRenameTrackingTable = true;
            //    this.NeedRecreateTriggers = true;
            //    this.NeedRecreateStoredProcedures = true;
            //}

            //if (currentSchema.TriggersPrefix != newSchema.TriggersPrefix ||
            //    currentSchema.TriggersSuffix != newSchema.TriggersSuffix)
            //{
            //    this.NeedRecreateTriggers = true;
            //}

            //if (currentSchema.StoredProceduresPrefix != newSchema.StoredProceduresPrefix ||
            //    currentSchema.StoredProceduresSuffix != newSchema.StoredProceduresSuffix)
            //{
            //    this.NeedRecreateStoredProcedures = true;
            //}
        }

        private void CheckAddedOrRemovedColumns()
        {
            // Search for column added
            foreach (var column in newTable.Columns)
                // if current table does not contains this new column, so we've added a new column
                if (!currentTable.Columns.Any(c => c.EqualsByName(column)))
                    AddedColumns.Add(column.Clone());

            // Search for columns removed
            foreach (var column in currentTable.Columns)
                // if current table does not contains this new column, so we've added a new column
                if (!newTable.Columns.Any(c => c.EqualsByName(column)))
                    RemovedColumns.Add(column.Clone());


        }


        /// <summary>
        /// Check if we need to recreate tracking table due to primary keys changing
        /// </summary>
        private void CheckPrimaryKeys()
        {
            // Check primary keys
            foreach (var pkey in currentTable.PrimaryKeys)
                if (!newTable.PrimaryKeys.Contains(pkey))
                    if (preserveTracking)
                        throw new Exception($"New table {newTable.TableName} has primary keys different from the last values stored in schema");
                    else
                        NeedRecreateTrackingTable = true;

            foreach (var pkey in newTable.PrimaryKeys)
                if (!currentTable.PrimaryKeys.Contains(pkey))

                    if (preserveTracking)
                        throw new Exception($"New table {newTable.TableName} has primary keys different from the last values stored in schema");
                    else
                        NeedRecreateTrackingTable = true;


        }


        /// <summary>
        /// Gets or Sets added columns
        /// </summary>
        public List<SyncColumn> AddedColumns { get; set; } = new List<SyncColumn>();

        /// <summary>
        /// Gets or Sets removed columns
        /// </summary>
        public List<SyncColumn> RemovedColumns { get; set; } = new List<SyncColumn>();

        /// <summary>
        /// Gets or Sets if we need to rename tracking table
        /// </summary>
        public bool NeedRenameTrackingTable { get; private set; }

        /// <summary>
        /// Gets or Sets if we need to recreate the whole tracking table
        /// </summary>
        public bool NeedRecreateTrackingTable { get; private set; }

        /// <summary>
        /// Gets or Sets if we need to recreate triggers
        /// </summary>
        public bool NeedRecreateTriggers { get; set; }

        /// <summary>
        /// Gets or Sets if we need to recreate triggers
        /// </summary>
        public bool NeedRecreateStoredProcedures { get; set; }



    }
}
