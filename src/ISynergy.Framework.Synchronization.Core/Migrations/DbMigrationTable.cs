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
        private readonly SyncTable _currentTable;
        private readonly SyncTable _newTable;
        private readonly bool _preserveTracking;
        private readonly CoreProvider _provider;

        public DbMigrationTable(CoreProvider provider, SyncTable currentTable, SyncTable newTable, bool preserveTracking = true)
        {
            _currentTable = currentTable ?? throw new ArgumentNullException(nameof(currentTable));
            _newTable = newTable ?? throw new ArgumentNullException(nameof(newTable));
            _preserveTracking = preserveTracking;
            _provider = provider;
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
            var currentSchema = _currentTable.Schema;
            var newSchema = _newTable.Schema;

            // if tracking table name changed, we need to:
            // - Alter tracking table name
            // - Regenerate stored procedure
            // - Regenerate triggers
            //if (currentSchema.TrackingTablesPrefix != newSchema.TrackingTablesPrefix ||
            //    currentSchema.TrackingTablesSuffix != newSchema.TrackingTablesSuffix)
            //{
            //    NeedRenameTrackingTable = true;
            //    NeedRecreateTriggers = true;
            //    NeedRecreateStoredProcedures = true;
            //}

            //if (currentSchema.TriggersPrefix != newSchema.TriggersPrefix ||
            //    currentSchema.TriggersSuffix != newSchema.TriggersSuffix)
            //{
            //    NeedRecreateTriggers = true;
            //}

            //if (currentSchema.StoredProceduresPrefix != newSchema.StoredProceduresPrefix ||
            //    currentSchema.StoredProceduresSuffix != newSchema.StoredProceduresSuffix)
            //{
            //    NeedRecreateStoredProcedures = true;
            //}
        }

        private void CheckAddedOrRemovedColumns()
        {
            // Search for column added
            foreach (var column in _newTable.Columns)
                // if current table does not contains this new column, so we've added a new column
                if (!_currentTable.Columns.Any(c => c.EqualsByName(column)))
                    AddedColumns.Add(column.Clone());

            // Search for columns removed
            foreach (var column in _currentTable.Columns)
                // if current table does not contains this new column, so we've added a new column
                if (!_newTable.Columns.Any(c => c.EqualsByName(column)))
                    RemovedColumns.Add(column.Clone());


        }


        /// <summary>
        /// Check if we need to recreate tracking table due to primary keys changing
        /// </summary>
        private void CheckPrimaryKeys()
        {
            // Check primary keys
            foreach (var pkey in _currentTable.PrimaryKeys)
                if (!_newTable.PrimaryKeys.Contains(pkey))
                    if (_preserveTracking)
                        throw new Exception($"New table {_newTable.TableName} has primary keys different from the last values stored in schema");
                    else
                        NeedRecreateTrackingTable = true;

            foreach (var pkey in _newTable.PrimaryKeys)
                if (!_currentTable.PrimaryKeys.Contains(pkey))

                    if (_preserveTracking)
                        throw new Exception($"New table {_newTable.TableName} has primary keys different from the last values stored in schema");
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
