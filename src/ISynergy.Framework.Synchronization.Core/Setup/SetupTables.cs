﻿using ISynergy.Framework.Synchronization.Core.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Setup
{
    /// <summary>
    /// Represents a list of tables to be added to the sync process
    /// </summary>
    [CollectionDataContract(Name = "tbls", ItemName = "tbl"), Serializable]
    public class SetupTables : ICollection<SetupTable>, IList<SetupTable>
    {

        /// <summary>
        /// Exposing the InnerCollection for serialization purpose
        /// </summary>
        [DataMember(Name = "c", IsRequired = true)]
        public Collection<SetupTable> InnerCollection = new Collection<SetupTable>();

        public SetupTables()
        {

        }
        /// <summary>
        /// Create a list of tables to be added to the sync process
        /// </summary>
        /// <param name="tables"></param>
        public SetupTables(IEnumerable<string> tables)
        {
            foreach (var table in tables)
                Add(table);
        }

        /// <summary>
        /// Add a new table to the collection of tables to be added to the sync process
        /// </summary>
        public SetupTable Add(string tableName, string schemaName = null)
        {
            var st = new SetupTable(tableName, schemaName);
            Add(st);
            return st;
        }

        /// <summary>
        /// Add a new table to the collection of tables to be added to the sync process
        /// </summary>
        public void Add(SetupTable table)
        {
            if (this[table.TableName, table.SchemaName] is not null)
                throw new Exception($"Table {table.TableName} already exists in the collection");

            InnerCollection.Add(table);
        }


        /// <summary>
        /// Add a collection of tables to be added to the sync process
        /// </summary>
        public void AddRange(IEnumerable<SetupTable> tables)
        {
            foreach (var table in tables)
                InnerCollection.Add(table);
        }


        /// <summary>
        /// Add a collection of tables to be added to the sync process
        /// </summary>
        public void AddRange(IEnumerable<string> tables)
        {
            if (tables is null)
                return;

            foreach (var table in tables)
                InnerCollection.Add(new SetupTable(table));
        }

        /// <summary>
        /// Get a table by its name
        /// </summary>
        public SetupTable this[string tableName]
        {
            get
            {
                if (string.IsNullOrEmpty(tableName))
                    throw new ArgumentNullException("tableName");

                var parser = ParserName.Parse(tableName);
                var tblName = parser.ObjectName;
                var schemaName = parser.SchemaName;
                schemaName = schemaName is null ? string.Empty : schemaName;

                var sc = SyncGlobalization.DataSourceStringComparison;

                var table = InnerCollection.FirstOrDefault(innerTable =>
                {
                    var innerTableSchemaName = string.IsNullOrEmpty(innerTable.SchemaName) ? string.Empty : innerTable.SchemaName;
                    return string.Equals(innerTable.TableName, tblName, sc) && string.Equals(innerTableSchemaName, schemaName);
                });

                return table;
            }
        }

        /// <summary>
        /// Get a table by its name
        /// </summary>
        public SetupTable this[string tableName, string schemaName]
        {
            get
            {
                if (string.IsNullOrEmpty(tableName))
                    throw new ArgumentNullException("tableName");

                schemaName = schemaName is null ? string.Empty : schemaName;

                var sc = SyncGlobalization.DataSourceStringComparison;

                var table = InnerCollection.FirstOrDefault(innerTable =>
                {
                    var innerTableSchemaName = string.IsNullOrEmpty(innerTable.SchemaName) ? string.Empty : innerTable.SchemaName;
                    return string.Equals(innerTable.TableName, tableName, sc) && string.Equals(innerTableSchemaName, schemaName);
                });

                return table;
            }
        }

        /// <summary>
        /// Check if Setup has tables
        /// </summary>
        public bool HasTables => InnerCollection?.Count > 0;

        /// <summary>
        /// Check if Setup has at least one table with columns
        /// </summary>
        public bool HasColumns => InnerCollection?.SelectMany(t => t.Columns).Count() > 0;  // using SelectMany to get columns and not Collection<Column>

        public void Clear() => InnerCollection.Clear();
        public SetupTable this[int index] => InnerCollection[index];
        public int Count => InnerCollection.Count;
        public bool IsReadOnly => false;
        SetupTable IList<SetupTable>.this[int index] { get => InnerCollection[index]; set => InnerCollection[index] = value; }
        public bool Remove(SetupTable item) => InnerCollection.Remove(item);
        public bool Contains(SetupTable item) => this[item.TableName, item.SchemaName] is not null;
        public void CopyTo(SetupTable[] array, int arrayIndex) => InnerCollection.CopyTo(array, arrayIndex);
        public int IndexOf(SetupTable item) => InnerCollection.IndexOf(item);
        public void RemoveAt(int index) => InnerCollection.RemoveAt(index);
        public override string ToString() => InnerCollection.Count.ToString();
        public void Insert(int index, SetupTable item) => InnerCollection.Insert(index, item);
        public IEnumerator<SetupTable> GetEnumerator() => InnerCollection.GetEnumerator();
        IEnumerator<SetupTable> IEnumerable<SetupTable>.GetEnumerator() => InnerCollection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => InnerCollection.GetEnumerator();
    }
}