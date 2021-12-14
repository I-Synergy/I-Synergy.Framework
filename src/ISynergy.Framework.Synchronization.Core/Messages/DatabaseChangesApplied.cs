using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Messages
{
    /// <summary>
    /// All table changes applied on a provider
    /// </summary>
    [DataContract(Name = "dca"), Serializable]
    public class DatabaseChangesApplied
    {

        /// <summary>
        /// ctor for serialization purpose
        /// </summary>
        public DatabaseChangesApplied()
        {

        }

        /// <summary>
        /// Get the view to be applied 
        /// </summary>
        [DataMember(Name = "tca", IsRequired = false, EmitDefaultValue = false, Order = 1)]
        public List<TableChangesApplied> TableChangesApplied { get; } = new List<TableChangesApplied>();


        /// <summary>
        /// Gets the total number of conflicts that have been applied resolved during the synchronization session.
        /// </summary>
        [IgnoreDataMember]
        public int TotalResolvedConflicts
        {
            get
            {
                var conflicts = 0;
                foreach (var tableProgress in TableChangesApplied)
                    conflicts = conflicts + tableProgress.ResolvedConflicts;
                return conflicts;
            }
        }


        /// <summary>
        /// Gets the total number of changes that have been applied during the synchronization session.
        /// </summary>
        [IgnoreDataMember]
        public int TotalAppliedChanges
        {
            get
            {
                var changesApplied = 0;
                foreach (var tableProgress in TableChangesApplied)
                    changesApplied += tableProgress.Applied;
                return changesApplied;
            }
        }

        /// <summary>
        /// Gets the total number of changes that have failed to be applied during the synchronization session.
        /// </summary>
        [IgnoreDataMember]
        public int TotalAppliedChangesFailed
        {
            get
            {
                var changesFailed = 0;
                foreach (var tableProgress in TableChangesApplied)
                    changesFailed += tableProgress.Failed;

                return changesFailed;
            }
        }

        public override string ToString() => $"{TotalAppliedChanges} changes applied for {TableChangesApplied.Count} tables";
    }

}
