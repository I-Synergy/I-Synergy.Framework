using ISynergy.Framework.Synchronization.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Database
{
    [DataContract(Name = "sr"), Serializable]
    public class SyncRelation : SyncNamedItem<SyncRelation>, IDisposable
    {

        /// <summary>
        /// Gets or Sets the relation name 
        /// </summary>
        [DataMember(Name = "n", IsRequired = true, Order = 1)]
        public string RelationName { get; set; }

        /// <summary>
        /// Gets or Sets a list of columns that represent the parent key.
        /// </summary>
        [DataMember(Name = "pks", IsRequired = true, Order = 2)]
        public IList<SyncColumnIdentifier> ParentKeys { get; set; } = new List<SyncColumnIdentifier>();

        /// <summary>
        /// Gets or Sets a list of columns that represent the parent key.
        /// </summary>
        [DataMember(Name = "cks", IsRequired = true, Order = 3)]
        public IList<SyncColumnIdentifier> Keys { get; set; } = new List<SyncColumnIdentifier>();

        /// <summary>
        /// Gets the ShemaFilter's SyncSchema
        /// </summary>
        [IgnoreDataMember]
        public SyncSet Schema { get; set; }

        public SyncRelation() { }

        public SyncRelation(string relationName, SyncSet schema = null)
        {
            RelationName = relationName;
            Schema = schema;
        }

        public SyncRelation(string relationName, IList<SyncColumnIdentifier> columns, IList<SyncColumnIdentifier> parentColumns, SyncSet schema = null)
        {
            RelationName = relationName;
            ParentKeys = parentColumns;
            Keys = columns;
            Schema = schema;
        }


        public SyncRelation Clone()
        {
            var clone = new SyncRelation();
            clone.RelationName = RelationName;

            foreach (var pk in ParentKeys)
                clone.ParentKeys.Add(pk.Clone());

            foreach (var ck in Keys)
                clone.Keys.Add(ck.Clone());

            return clone;
        }

        /// <summary>
        /// Clear 
        /// </summary>
        public void Clear() => Dispose(true);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanup)
        {
            // Dispose managed ressources
            if (cleanup)
            {
                // clean rows
                Keys.Clear();
                ParentKeys.Clear();
                Schema = null;
            }

            // Dispose unmanaged ressources
        }

        /// <summary>
        /// Ensure this relation has correct Schema reference
        /// </summary>
        public void EnsureRelation(SyncSet schema) => Schema = schema;


        /// <summary>
        /// Get parent table
        /// </summary>
        public SyncTable GetParentTable()
        {
            if (Schema == null || ParentKeys.Count() <= 0)
                return null;

            var id = ParentKeys.First();

            return Schema.Tables[id.TableName, id.SchemaName];
        }

        /// <summary>
        /// Get child table
        /// </summary>
        public SyncTable GetTable()
        {
            if (Schema == null || Keys.Count() <= 0)
                return null;

            var id = Keys.First();

            return Schema.Tables[id.TableName, id.SchemaName];
        }

        public override IEnumerable<string> GetAllNamesProperties()
        {
            yield return RelationName;
        }


        public override bool EqualsByProperties(SyncRelation other)
        {
            if (other == null)
                return false;

            if (!EqualsByName(other))
                return false;

            // Check list
            if (!Keys.CompareWith(other.Keys))
                return false;

            if (!ParentKeys.CompareWith(other.ParentKeys))
                return false;


            return true;

        }
    }
}

