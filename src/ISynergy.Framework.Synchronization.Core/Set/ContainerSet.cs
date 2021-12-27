using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Set
{
    [DataContract(Name = "c"), Serializable]
    public class ContainerSet : IDisposable
    {
        /// <summary>
        /// List of tables
        /// </summary>
        [DataMember(Name = "t", IsRequired = false, EmitDefaultValue = false)]
        public Collection<ContainerTable> Tables { get; set; } = new Collection<ContainerTable>();



        /// <summary>
        /// Check if we have some tables in the container
        /// </summary>
        public bool HasTables => Tables?.Count > 0;

        /// <summary>
        /// Check if we have at least one table with one row
        /// </summary>
        public bool HasRows
        {
            get
            {
                if (!HasTables)
                    return false;

                // Check if any of the tables has rows inside
                return Tables.Any(t => t.Rows.Count > 0);
            }
        }

        /// <summary>
        /// Getting the container rows count
        /// </summary>
        public int RowsCount()
        {
            if (!HasTables)
                return 0;

            return Tables.Sum(t => t.Rows.Count);
        }

        public ContainerSet() { }

        public void Clear()
        {
            foreach (var t in Tables)
                t.Clear();

            Tables.Clear();
        }
        public void Dispose()
        {
            Dispose(true);

            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanup)
        {
            // Dispose managed ressources
            if (cleanup)
            {
                Clear();
                Tables = null;
            }

            // Dispose unmanaged ressources
        }
    }
}
