//using ISynergy.Framework.Synchronization.Core.Builders;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;

//namespace ISynergy.Framework.Synchronization.Core
//{
//    [CollectionDataContract(Name = "cols", ItemName = "col"), Serializable]
//    public class SyncColumns : ICollection<SyncColumn>, IList<SyncColumn>
//    {
//        /// <summary>
//        /// Exposing the InnerCollection for serialization purpose
//        /// </summary>
//        [DataMember(Name = "c", IsRequired = true, Order = 1)]
//        public Collection<SyncColumn> InnerCollection { get; set; } = new Collection<SyncColumn>();


//        [DataMember(Name = "i", IsRequired = true, Order = 2)]
//        public Dictionary<string, int> InnerIndexes { get; set; } = new Dictionary<string, int>();

//        /// <summary>
//        /// Column's schema
//        /// </summary>
//        [IgnoreDataMember]
//        public SyncTable Table { get; internal set; }

//        /// <summary>
//        /// Create a default collection for Serializers
//        /// </summary>
//        public SyncColumns()
//        {
//        }

//        /// <summary>
//        /// Create a new collection of tables for a SyncSchema
//        /// </summary>
//        public SyncColumns(SyncTable table) => Table = table;

//        /// <summary>
//        /// Since we don't serializer the reference to the schema, this method will reaffect the correct schema
//        /// </summary>
//        public void EnsureColumns(SyncTable table)
//        {
//            Table = table;
//        }

//        /// <summary>
//        /// Get a Column by its name
//        /// </summary>
//        public SyncColumn this[string columnName]
//        {
//            get
//            {
//                var col = InnerCollection[InnerIndexes[columnName.ToLowerInvariant()]];
//                //var col = InnerCollection.FirstOrDefault(c => string.Equals(c.ColumnName, columnName, SyncGlobalization.DataSourceStringComparison));
//                return col;
//            }
//        }


//        /// <summary>
//        /// Add a new Column to the Schema Column collection
//        /// </summary>
//        public void Add(SyncColumn item)
//        {
//            InnerCollection.Add(item);
//            AffectOrder();
//        }

//        public void Add(string columnName, Type type = null)
//        {
//            var item = new SyncColumn(columnName, type);
//            InnerCollection.Add(item);
//            AffectOrder();

//        }



//        /// <summary>
//        /// Add a collection of columns
//        /// </summary>
//        public void AddRange(SyncColumn[] addedColumns)
//        {
//            foreach (var item in addedColumns)
//                InnerCollection.Add(item);

//            AffectOrder();

//        }


//        /// <summary>
//        /// Reorganize columns order
//        /// </summary>
//        public void Reorder(SyncColumn column, int newPosition)
//        {
//            if (newPosition < 0 || newPosition > InnerCollection.Count - 1)
//                throw new Exception($"InvalidOrdinal(ordinal, {newPosition}");

//            // Remove column from collection
//            InnerCollection.Remove(column);

//            // Add at the end or insert in new positions
//            if (newPosition > InnerCollection.Count - 1)
//                InnerCollection.Add(column);
//            else
//                InnerCollection.Insert(newPosition, column);

//            AffectOrder();
//        }
//        private void AffectOrder()
//        {
//            InnerIndexes.Clear();
//            // now reordered correctly, affect new Ordinal property
//            for (int i = 0; i < InnerCollection.Count; i++)
//            {
//                var c = InnerCollection[i];
//                c.Ordinal = i;
//                InnerIndexes[c.ColumnName.ToLowerInvariant()] = i;
//            }

//        }

//        /// <summary>
//        /// Clear all the relations
//        /// </summary>
//        public void Clear()
//        {
//            InnerCollection.Clear();
//            InnerIndexes.Clear();
//        }

//        public SyncColumn this[int index] => InnerCollection[index];
//        public int Count => InnerCollection.Count;
//        public bool IsReadOnly => false;
//        SyncColumn IList<SyncColumn>.this[int index]
//        {
//            get => InnerCollection[index];
//            set
//            {
//                if (value is null)
//                    throw new Exception("Can't be null");

//                InnerCollection[index] = value;
//                InnerIndexes[value.ColumnName.ToLowerInvariant()] = index;
//            }
//        }
//        public bool Remove(SyncColumn item)
//        {
//            var isDeleted = InnerCollection.Remove(item);

//            if (isDeleted)
//                InnerIndexes.Remove(item.ColumnName.ToLowerInvariant());

//            return isDeleted;
//        }

//        public bool Contains(SyncColumn item) => InnerCollection.Contains(item);
//        public void CopyTo(SyncColumn[] array, int arrayIndex) => InnerCollection.CopyTo(array, arrayIndex);
//        public int IndexOf(SyncColumn item) => InnerCollection.IndexOf(item);
//        public void RemoveAt(int index)
//        {
//            InnerIndexes.Remove(InnerCollection[index].ColumnName.ToLowerInvariant());
//            InnerCollection.RemoveAt(index);
//        }
//        IEnumerator IEnumerable.GetEnumerator() => InnerCollection.GetEnumerator();
//        public IEnumerator<SyncColumn> GetEnumerator() => InnerCollection.GetEnumerator();
//        public override string ToString() => InnerCollection.Count.ToString();
//        public void Insert(int index, SyncColumn item)
//        {
//            InnerIndexes[item.ColumnName.ToLowerInvariant()] = index;
//            InnerCollection.Insert(index, item);
//        }
//    }

//}
