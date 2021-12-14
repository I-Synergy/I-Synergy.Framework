using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Messages;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace ISynergy.Framework.AspNetCore.Synchronization.Cache
{
    /// <summary>
    /// Cache object used by each client to cache sync process batches
    /// </summary>
    [DataContract(Name = "sc"), Serializable]
    public class SessionCache
    {
        [DataMember(Name = "rct", IsRequired = false, EmitDefaultValue = false, Order = 1)]
        public long RemoteClientTimestamp { get; set; }

        [DataMember(Name = "sbi", IsRequired = false, EmitDefaultValue = false, Order = 2)]
        public BatchInfo ServerBatchInfo { get; set; }

        [DataMember(Name = "cbi", IsRequired = false, EmitDefaultValue = false, Order = 3)]
        public BatchInfo ClientBatchInfo { get; set; }

        [DataMember(Name = "dcs", IsRequired = false, EmitDefaultValue = false, Order = 4)]
        public DatabaseChangesSelected ServerChangesSelected { get; set; }

        [DataMember(Name = "dca", IsRequired = false, EmitDefaultValue = false, Order = 5)]
        public DatabaseChangesApplied ClientChangesApplied { get; set; }


        public override string ToString()
        {
            var serverBatchInfoStr = "Null";
            if (ServerBatchInfo is not null)
            {
                var serverBatchPartsCountStr = ServerBatchInfo.BatchPartsInfo is null ? "Null" : ServerBatchInfo.BatchPartsInfo.Count.ToString();
                var serverBatchTablesCountStr = ServerBatchInfo.SanitizedSchema is null ? "Null" : ServerBatchInfo.SanitizedSchema.Tables.Count.ToString();
                serverBatchInfoStr = $"Parts:{serverBatchPartsCountStr}. Rows Count:{ServerBatchInfo.RowsCount}. Tables:{serverBatchTablesCountStr}";
            }

            var clientBatchInfoStr = "Null";
            if (ClientBatchInfo is not null)
            {
                var clientBatchPartsCountStr = ClientBatchInfo.BatchPartsInfo is null ? "Null" : ClientBatchInfo.BatchPartsInfo.Count.ToString();
                var clientBatchTablesCountStr = ClientBatchInfo.SanitizedSchema is null ? "Null" : ClientBatchInfo.SanitizedSchema.Tables.Count.ToString();
                clientBatchInfoStr = $"Parts:{clientBatchPartsCountStr}. Rows Count:{ClientBatchInfo.RowsCount}. Tables:{clientBatchTablesCountStr}";
            }

            var debug = new StringBuilder();
            debug.Append($" \"RemoteClientTimestamp\":\"{RemoteClientTimestamp}\",");
            debug.Append($" \"ClientBatchInfo\":\"{clientBatchInfoStr}\",");
            debug.Append($" \"ServerBatchInfo\":\"{serverBatchInfoStr}\"");

            return debug.ToString();
        }
    }
}
