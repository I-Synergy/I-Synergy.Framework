﻿using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Batch
{
    /// <summary>
    /// Represents a Batch, containing a full or serialized change set
    /// </summary>
    [DataContract(Name = "bi"), Serializable]
    public class BatchInfo
    {

        /// <summary>
        /// Ctor for serializer
        /// </summary>
        public BatchInfo()
        {

        }

        /// <summary>
        /// Create a new BatchInfo, containing all BatchPartInfo
        /// </summary>
        public BatchInfo(bool isInMemory, SyncSet inSchema, string rootDirectory = null, string directoryName = null)
        {
            this.InMemory = isInMemory;

            // We need to create a change table set, containing table with columns not readonly
            foreach (var table in inSchema.Tables)
                DbSyncAdapter.CreateChangesTable(inSchema.Tables[table.TableName, table.SchemaName], this.SanitizedSchema);

            // If not in memory, generate a directory name and initialize batch parts list
            if (!this.InMemory)
            {
                this.DirectoryRoot = rootDirectory;
                this.BatchPartsInfo = new List<BatchPartInfo>();
                this.DirectoryName = string.IsNullOrEmpty(directoryName) ? string.Concat(DateTime.UtcNow.ToString("yyyy_MM_dd_ss"), Path.GetRandomFileName().Replace(".", "")) : directoryName;
            }
        }


        /// <summary>
        /// Is the batch parts are in memory
        /// If true, only one BPI
        /// If false, several serialized BPI
        /// </summary>
        [IgnoreDataMember]
        public bool InMemory { get; set; }

        /// <summary>
        /// If in memory, return the in memory Dm
        /// </summary>
        [IgnoreDataMember]
        public SyncSet InMemoryData { get; set; }

        /// <summary>
        /// Gets or Sets directory name
        /// </summary>
        [DataMember(Name = "dirname", IsRequired = false, EmitDefaultValue = false, Order = 1)]
        public string DirectoryName { get; set; }

        /// <summary>
        /// Gets or sets directory root
        /// </summary>
        [DataMember(Name = "dir", IsRequired = false, EmitDefaultValue = false, Order = 2)]
        public string DirectoryRoot { get; set; }

        /// <summary>
        /// Gets or sets server timestamp
        /// </summary>
        [DataMember(Name = "ts", IsRequired = false, EmitDefaultValue = false, Order = 3)]
        public long Timestamp { get; set; }

        /// <summary>
        /// List of batch parts if not in memory
        /// </summary>
        [DataMember(Name = "parts", IsRequired = false, EmitDefaultValue = false, Order = 4)]
        public List<BatchPartInfo> BatchPartsInfo { get; set; }

        /// <summary>
        /// Gets or Sets the rows count contained in the batch info
        /// </summary>
        [DataMember(Name = "count", IsRequired = true, Order = 5)]
        public int RowsCount { get; set; }

        /// <summary>
        /// Gets or Sets the Serialization Factory Key used to serialize this batch info (if not in memory)
        /// </summary>
        [DataMember(Name = "ser", IsRequired = false, EmitDefaultValue = false, Order = 6)]
        public string SerializerFactoryKey { get; set; }


        /// <summary>
        /// Internally setting schema
        /// </summary>
        //[IgnoreDataMember]
        [DataMember(Name = "schema", IsRequired = true, EmitDefaultValue = false, Order = 7)]
        public SyncSet SanitizedSchema { get; set; } = new SyncSet();

        /// <summary>
        /// Get the full path of the Batch directory
        /// </summary>
        /// <returns></returns>
        public string GetDirectoryFullPath()
        {
            if (this.InMemory)
                return null;

            return Path.Combine(this.DirectoryRoot, this.DirectoryName);
        }


        /// <summary>
        /// Check if this batchinfo has some data (in memory or not)
        /// </summary>
        public bool HasData()
        {
            if (this.SanitizedSchema is null)
                throw new NullReferenceException("Batch info schema should not be null");

            if (InMemory && InMemoryData is not null && InMemoryData.HasTables && InMemoryData.HasRows)
                return true;

            if (!InMemory && BatchPartsInfo is not null && BatchPartsInfo.Count > 0)
            {
                var rowsCount = BatchPartsInfo.Sum(bpi => bpi.RowsCount);

                return rowsCount > 0;
            }

            return false;
        }



        /// <summary>
        /// Check if this batchinfo has some data (in memory or not)
        /// </summary>
        public bool HasData(string tableName, string schemaName)
        {
            if (this.SanitizedSchema is null)
                throw new NullReferenceException("Batch info schema should not be null");

            if (InMemory && InMemoryData is not null && InMemoryData.HasTables)
            {
                var table = InMemoryData.Tables[tableName, schemaName];
                if (table is null)
                    return false;

                return table.HasRows;
            }

            if (!InMemory && BatchPartsInfo is not null && BatchPartsInfo.Count > 0)
            {
                var tableInfo = new BatchPartTableInfo(tableName, schemaName);

                var bptis = BatchPartsInfo.SelectMany(bpi => bpi.Tables.Where(t => t.EqualsByName(tableInfo)));

                if (bptis is null)
                    return false;


                return bptis.Sum(bpti => bpti.RowsCount) > 0;
            }

            return false;
        }

        public async IAsyncEnumerable<(SyncTable SyncTable, BatchPartInfo BatchPartInfo)> GetTableAsync(string tableName, string schemaName, ISerializerFactory serializerFactory = default, BaseOrchestrator orchestrator = null)
        {
            if (this.SanitizedSchema is null)
                throw new NullReferenceException("Batch info schema should not be null");

            var tableInfo = new BatchPartTableInfo(tableName, schemaName);

            if (InMemory)
            {
                this.SerializerFactoryKey = null;

                if (this.InMemoryData is not null && this.InMemoryData.HasTables)
                    yield return (this.InMemoryData.Tables[tableName, schemaName], null);
            }
            else
            {
                this.SerializerFactoryKey = serializerFactory.Key;

                var bpiTables = BatchPartsInfo.Where(bpi => bpi.RowsCount > 0 && bpi.Tables.Any(t => t.EqualsByName(tableInfo))).OrderBy(t => t.Index);

                if (bpiTables is not null)
                    foreach (var batchPartinInfo in bpiTables)
                    {
                        // load only if not already loaded in memory
                        if (batchPartinInfo.Data is null)
                            await batchPartinInfo.LoadBatchAsync(this.SanitizedSchema, GetDirectoryFullPath(), serializerFactory, orchestrator).ConfigureAwait(false);

                        // Get the table from the batchPartInfo
                        // generate a tmp SyncTable for 
                        var batchTable = batchPartinInfo.Data.Tables.FirstOrDefault(bt => bt.EqualsByName(new SyncTable(tableName, schemaName)));

                        if (batchTable is not null)
                            yield return (batchTable, batchPartinInfo);
                    }
            }
        }

        /// <summary>
        /// Ensure the last batch part (if not in memory) has the correct IsLastBatch flag
        /// </summary>
        public void EnsureLastBatch()
        {
            if (this.InMemory)
                return;

            if (this.BatchPartsInfo.Count == 0)
                return;

            // get last index
            var maxIndex = this.BatchPartsInfo.Max(tBpi => tBpi.Index);

            // Set corret last batch 
            foreach (var bpi in this.BatchPartsInfo)
                bpi.IsLastBatch = bpi.Index == maxIndex;
        }

        /// <summary>
        /// Add changes to batch info.
        /// </summary>
        public async Task AddChangesAsync(SyncSet changes, int batchIndex = 0, bool isLastBatch = true, ISerializerFactory serializerFactory = default, BaseOrchestrator orchestrator = null)
        {
            if (this.InMemory)
            {
                this.SerializerFactoryKey = null;
                this.InMemoryData = changes;
            }
            else
            {
                this.SerializerFactoryKey = serializerFactory.Key;
                var bpId = GenerateNewFileName(batchIndex.ToString());
                var bpi = await BatchPartInfo.CreateBatchPartInfoAsync(batchIndex, changes, bpId, GetDirectoryFullPath(), isLastBatch, serializerFactory, orchestrator).ConfigureAwait(false);

                // add the batchpartinfo tp the current batchinfo
                this.BatchPartsInfo.Add(bpi);
            }
        }

        /// <summary>
        /// generate a batch file name
        /// </summary>
        public static string GenerateNewFileName(string batchIndex)
        {
            if (batchIndex.Length == 1)
                batchIndex = $"000{batchIndex}";
            else if (batchIndex.Length == 2)
                batchIndex = $"00{batchIndex}";
            else if (batchIndex.Length == 3)
                batchIndex = $"0{batchIndex}";
            else if (batchIndex.Length == 4)
                batchIndex = $"{batchIndex}";
            else
                throw new OverflowException("too much batches !!!");

            return $"{batchIndex}_{Path.GetRandomFileName().Replace(".", "_")}.batch";
        }


        /// <summary>
        /// try to delete the Batch tmp directory and all the files stored in it
        /// </summary>
        public void TryRemoveDirectory()
        {
            // Once we have applied all the batch, we can safely remove the temp dir and all it's files
            if (!this.InMemory && !string.IsNullOrEmpty(this.DirectoryRoot) && !string.IsNullOrEmpty(this.DirectoryName))
            {
                var tmpDirectory = new DirectoryInfo(this.GetDirectoryFullPath());

                if (tmpDirectory is null || !tmpDirectory.Exists)
                    return;

                try
                {
                    tmpDirectory.Delete(true);
                }
                // do nothing here 
                catch { }
            }
        }


        /// <summary>
        /// Clear all batch parts info and try to delete tmp folder if needed
        /// </summary>
        public void Clear(bool deleteFolder)
        {
            if (this.InMemory && this.InMemoryData is not null)
            {
                this.InMemoryData.Dispose();
                return;
            }

            // Delete folders before deleting batch parts
            if (deleteFolder)
                this.TryRemoveDirectory();

            if (this.BatchPartsInfo is not null)
            {
                foreach (var bpi in this.BatchPartsInfo)
                    bpi.Clear();

                this.BatchPartsInfo.Clear();
            }

        }

    }
}
