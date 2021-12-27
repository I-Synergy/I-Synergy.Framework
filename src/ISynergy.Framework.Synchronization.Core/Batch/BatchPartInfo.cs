using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Batch
{
    /// <summary>
    /// Info about a BatchPart
    /// Will be serialized in the BatchInfo file
    /// </summary>
    [DataContract(Name = "batchpartinfo"), Serializable]
    public class BatchPartInfo
    {
        [DataMember(Name = "file", IsRequired = true)]
        public string FileName { get; set; }

        [DataMember(Name = "index", IsRequired = true)]
        public int Index { get; set; }

        [DataMember(Name = "last", IsRequired = true)]
        public bool IsLastBatch { get; set; }

        /// <summary>
        /// Tables contained in the SyncSet (serialiazed or not) (NEW v0.9.3 : Only One table per file)
        /// </summary>
        [DataMember(Name = "tables", IsRequired = true)]
        public BatchPartTableInfo[] Tables { get; set; }

        /// <summary>
        /// Tables contained rows count
        /// </summary>
        [DataMember(Name = "rc", IsRequired = false)]
        public int RowsCount { get; set; }

        public BatchPartInfo()
        {
        }

        public override string ToString()
        {
            if (Tables is null || Tables.Length <= 0)
                return base.ToString();

            var table = Tables[0];

            if (!string.IsNullOrEmpty(table.SchemaName))
                return $"{table.SchemaName}.{table.TableName}";
            else
                return table.TableName;
        }
    }

}
