﻿using ISynergy.Framework.Synchronization.Core.Extensions;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Set
{

    [DataContract(Name = "s"), Serializable]
    public class SyncSet : IDisposable
    {
        /// <summary>
        /// Gets or Sets the sync set tables
        /// </summary>
        [DataMember(Name = "t", IsRequired = false, EmitDefaultValue = false)]
        public SyncTables Tables { get; set; }

        /// <summary>
        /// Gets or Sets an array of every SchemaRelation belong to this Schema
        /// </summary>
        [DataMember(Name = "r", IsRequired = false, EmitDefaultValue = false)]
        public SyncRelations Relations { get; set; }

        /// <summary>
        /// Filters applied on tables
        /// </summary>
        [DataMember(Name = "f", IsRequired = false, EmitDefaultValue = false)]
        public SyncFilters Filters { get; set; }

        /// <summary>
        /// Create a new SyncSet, empty
        /// </summary>
        public SyncSet()
        {
            Tables = new SyncTables(this);
            Relations = new SyncRelations(this);
            Filters = new SyncFilters(this);
        }


        /// <summary>
        /// Creates a new SyncSet based on a Sync setup (containing tables)
        /// </summary>
        /// <param name="setup"></param>
        public SyncSet(SyncSetup setup) : this()
        {
            foreach (var filter in setup.Filters)
                Filters.Add(filter);

            foreach (var setupTable in setup.Tables)
                Tables.Add(new SyncTable(setupTable.TableName, setupTable.SchemaName));

            EnsureSchema();
        }

        /// <summary>
        /// Ensure all tables, filters and relations has the correct reference to this schema
        /// </summary>
        public void EnsureSchema()
        {
            if (Tables is not null)
                Tables.EnsureTables(this);

            if (Relations is not null)
                Relations.EnsureRelations(this);

            if (Filters is not null)
                Filters.EnsureFilters(this);
        }

        /// <summary>
        /// Clone the SyncSet schema (without data)
        /// </summary>
        public SyncSet Clone(bool includeTables = true)
        {
            var clone = new SyncSet();

            if (!includeTables)
                return clone;

            foreach (var f in Filters)
                clone.Filters.Add(f.Clone());

            foreach (var r in Relations)
                clone.Relations.Add(r.Clone());

            foreach (var t in Tables)
                clone.Tables.Add(t.Clone());

            // Ensure all elements has the correct ref to its parent
            clone.EnsureSchema();

            return clone;
        }



        ///// <summary>
        ///// Import a container set in a SyncSet instance
        ///// </summary>
        //public void ImportContainerSet(ContainerSet containerSet, bool checkType)
        //{
        //    foreach (var table in containerSet.Tables)
        //    {
        //        var syncTable = Tables[table.TableName, table.SchemaName];

        //        if (syncTable is null)
        //            throw new ArgumentNullException($"Table {table.TableName} does not exist in the SyncSet");

        //        syncTable.Rows.ImportContainerTable(table, checkType);
        //    }

        //}

        ///// <summary>
        ///// Get the rows inside a container.
        ///// ContainerSet is a serialization container for rows
        ///// </summary>
        //public ContainerSet GetContainerSet()
        //{
        //    var containerSet = new ContainerSet();
        //    foreach (var table in Tables)
        //    {
        //        var containerTable = new ContainerTable(table)
        //        {
        //            Rows = table.Rows.ExportToContainerTable().ToList()
        //        };

        //        if (containerTable.Rows.Count > 0)
        //            containerSet.Tables.Add(containerTable);
        //    }

        //    return containerSet;
        //}


        /// <summary>
        /// Clear the SyncSet
        /// </summary>
        public void Clear() => Dispose(true);


        /// <summary>
        /// Dispose the whole SyncSet
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            if (Tables is not null)
                Tables.Schema = null;
            if (Relations is not null)
                Relations.Schema = null;
            if (Filters is not null)
                Filters.Schema = null;

            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanup)
        {
            // Dispose managed ressources
            if (cleanup)
            {
                if (Tables is not null)
                {
                    Tables.Clear();
                    Tables = null;
                }

                if (Relations is not null)
                {
                    Relations.Clear();
                    Relations = null;
                }

                if (Filters is not null)
                {
                    Filters.Clear();
                    Filters = null;
                }
            }

            // Dispose unmanaged ressources
        }


        public bool EqualsByProperties(SyncSet otherSet)
        {
            if (otherSet is null)
                return false;

            // Checking inner lists
            if (!Tables.CompareWith(otherSet.Tables))
                return false;

            if (!Filters.CompareWith(otherSet.Filters))
                return false;

            if (!Relations.CompareWith(otherSet.Relations))
                return false;

            return true;
        }
        public override string ToString() => $"{Tables.Count} tables";

        /// <summary>
        /// Check if Schema has tables
        /// </summary>
        public bool HasTables => Tables?.Count > 0;

        /// <summary>
        /// Check if Schema has at least one table with columns
        /// </summary>
        public bool HasColumns => Tables?.SelectMany(t => t.Columns).Count() > 0;  // using SelectMany to get columns and not Collection<Column>


        /// <summary>
        /// Gets if at least one table as at least one row
        /// </summary>
        public bool HasRows
        {
            get
            {
                if (!HasTables)
                    return false;

                // Check if any of the tables has rows inside
                return Tables.Any(t => t.Rows is not null && t.Rows.Count > 0);
            }
        }

        /// <summary>
        /// Gets a true boolean if other instance is defined as same based on all properties
        /// </summary>
        public bool Equals(SyncSet other) => EqualsByProperties(other);

        /// <summary>
        /// Gets a true boolean if other instance is defined as same based on all properties
        /// </summary>
        public override bool Equals(object obj) => EqualsByProperties(obj as SyncSet);

        public override int GetHashCode() => base.GetHashCode();

    }
}