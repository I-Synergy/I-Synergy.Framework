using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Synchronization.Core.Abstractions;
using ISynergy.Framework.Synchronization.Core.Providers;
using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ISynergy.Framework.Synchronization.Core.Merger
{
    public class DbMigrationTable
    {
        private readonly SyncTable _currentTable;
        private readonly SyncTable _newTable;
        private readonly bool _preserveTracking;
        private readonly IProvider _provider;

        public DbMigrationTable(IProvider provider, SyncTable currentTable, SyncTable newTable, bool preserveTracking = true)
        {
            Argument.IsNotNull(currentTable);
            Argument.IsNotNull(newTable);

            _currentTable = currentTable;
            _newTable = newTable;
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
            {
                // if current table does not contains this new column, so we've added a new column
                if (!_currentTable.Columns.Any(c => c.EqualsByName(column)))
                    AddedColumns.Add(column.Clone());
            }

            // Search for columns removed
            foreach (var column in _currentTable.Columns)
            {
                // if current table does not contains this new column, so we've added a new column
                if (!_newTable.Columns.Any(c => c.EqualsByName(column)))
                    RemovedColumns.Add(column.Clone());
            }


        }


        /// <summary>
        /// Check if we need to recreate tracking table due to primary keys changing
        /// </summary>
        private void CheckPrimaryKeys()
        {
            // Check primary keys
            foreach (var pkey in _currentTable.PrimaryKeys)
            {
                if (!_newTable.PrimaryKeys.Contains(pkey))
                {
                    if (_preserveTracking)
                        throw new Exception($"New table {_newTable.TableName} has primary keys different from the last values stored in schema");
                    else
                        NeedRecreateTrackingTable = true;
                }

            }

            foreach (var pkey in _newTable.PrimaryKeys)
            {
                if (!_currentTable.PrimaryKeys.Contains(pkey))
                {

                    if (_preserveTracking)
                        throw new Exception($"New table {_newTable.TableName} has primary keys different from the last values stored in schema");
                    else
                        NeedRecreateTrackingTable = true;
                }
            }


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
