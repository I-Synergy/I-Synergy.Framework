﻿using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Serialization;
using ISynergy.Framework.Synchronization.Core.Set;
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
        /// Gets or Sets directory name
        /// </summary>
        [DataMember(Name = "dirname", IsRequired = false, EmitDefaultValue = false)]
        public string DirectoryName { get; set; }

        /// <summary>
        /// Gets or sets directory root
        /// </summary>
        [DataMember(Name = "dir", IsRequired = false, EmitDefaultValue = false)]
        public string DirectoryRoot { get; set; }

        /// <summary>
        /// Gets or sets server timestamp
        /// </summary>
        [DataMember(Name = "ts", IsRequired = false, EmitDefaultValue = false)]
        public long Timestamp { get; set; }

        /// <summary>
        /// List of batch parts
        /// </summary>
        [DataMember(Name = "parts", IsRequired = false, EmitDefaultValue = false)]
        public List<BatchPartInfo> BatchPartsInfo { get; set; }

        /// <summary>
        /// Gets or Sets the rows count contained in the batch info
        /// </summary>
        [DataMember(Name = "count", IsRequired = true)]
        public int RowsCount { get; set; }

        /// <summary>
        /// Gets or Sets the Serialization Factory Key used to serialize this batch info
        /// </summary>
        [DataMember(Name = "ser", IsRequired = false, EmitDefaultValue = false)]
        public string SerializerFactoryKey { get; set; }


        /// <summary>
        /// Internally setting schema
        /// </summary>
        //[IgnoreDataMember]
        [DataMember(Name = "schema", IsRequired = true, EmitDefaultValue = false)]
        public SyncSet SanitizedSchema { get; set; } = new SyncSet();

        /// <summary>
        /// Get the full path of the Batch directory
        /// </summary>
        /// <returns></returns>
        public string GetDirectoryFullPath() => Path.Combine(DirectoryRoot, DirectoryName);


        /// <summary>
        /// Create batch info directory
        /// </summary>
        public void CreateDirectory()
        {
            if (!Directory.Exists(GetDirectoryFullPath()))
                Directory.CreateDirectory(GetDirectoryFullPath());
        }

        /// <summary>
        /// Check if this batchinfo has some data
        /// </summary>
        public bool HasData()
        {
            if (SanitizedSchema is null)
                throw new NullReferenceException("Batch info schema should not be null");

            if (BatchPartsInfo is not null && BatchPartsInfo.Count > 0)
            {
                var rowsCount = BatchPartsInfo.Sum(bpi => bpi.RowsCount);

                return rowsCount > 0;
            }

            return false;
        }

        /// <summary>
        /// Generate a new full path to store a new batch part info file
        /// </summary>
        public (string FullPath, string FileName) GetNewBatchPartInfoPath(SyncTable syncTable, int batchIndex, string extension)
        {
            var tableName = ParserName.Parse(syncTable).Unquoted().Schema().Normalized().ToString();
            var fileName = GenerateNewFileName(batchIndex.ToString(), tableName, extension);
            var fullPath = Path.Combine(GetDirectoryFullPath(), fileName);
            return (fullPath, fileName);
        }

        public (string FullPath, string FileName) GetBatchPartInfoPath(BatchPartInfo batchPartInfo)
        {
            if (BatchPartsInfo is null)
                return (default, default);

            var fullPath = Path.Combine(GetDirectoryFullPath(), batchPartInfo.FileName);

            return (fullPath, batchPartInfo.FileName);

        }

        /// <summary>
        /// Check if this batchinfo has some data
        /// </summary>
        public bool HasData(string tableName, string schemaName)
        {
            if (SanitizedSchema is null)
                throw new NullReferenceException("Batch info schema should not be null");

            if (BatchPartsInfo is not null && BatchPartsInfo.Count > 0)
            {
                var tableInfo = new BatchPartTableInfo(tableName, schemaName);

                var bptis = BatchPartsInfo.SelectMany(bpi => bpi.Tables.Where(t => t.EqualsByName(tableInfo)));

                if (bptis is null)
                    return false;


                return bptis.Sum(bpti => bpti.RowsCount) > 0;
            }

            return false;
        }

        /// <summary>
        /// Get all batch part for 1 particular table
        /// </summary>
        public IEnumerable<BatchPartInfo> GetBatchPartsInfo(SyncTable syncTable)
        {
            if (syncTable is null) return Enumerable.Empty<BatchPartInfo>();

            if (BatchPartsInfo is null) return Enumerable.Empty<BatchPartInfo>();

            // Get all batch part
            var bpiTables = BatchPartsInfo.Where(bpi => bpi.RowsCount > 0
                                            && bpi.Tables.Any(t => t.EqualsByName(new BatchPartTableInfo(syncTable.TableName, syncTable.SchemaName)))).OrderBy(t => t.Index);

            if (bpiTables is null) return Enumerable.Empty<BatchPartInfo>();

            return bpiTables;
        }

        /// <summary>
        /// Get all batch part for 1 particular table
        /// </summary>
        public IEnumerable<BatchPartInfo> GetBatchPartsInfo(string tableName, string schemaName = default) =>
            GetBatchPartsInfo(new SyncTable(tableName, schemaName));

        /// <summary>
        /// Ensure the last batch part has the correct IsLastBatch flag
        /// </summary>
        public void EnsureLastBatch()
        {
            if (BatchPartsInfo.Count == 0)
                return;

            // get last index
            var maxIndex = BatchPartsInfo.Max(tBpi => tBpi.Index);

            // Set corret last batch 
            foreach (var bpi in BatchPartsInfo)
                bpi.IsLastBatch = bpi.Index == maxIndex;
        }

        /// <summary>
        /// generate a batch file name
        /// </summary>
        public static string GenerateNewFileName(string batchIndex, string tableName, string extension)
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

            return $"{tableName}_{batchIndex}_{Path.GetRandomFileName().Replace(".", "_")}.{extension}";
        }


        /// <summary>
        /// Load the Batch part info in memory, in a SyncTable
        /// </summary>
        public Task<SyncTable> LoadBatchPartInfoAsync(BatchPartInfo batchPartInfo, ILocalSerializerFactory localSerializerFactory = null)
        {
            if (localSerializerFactory is null)
                localSerializerFactory = new LocalJsonSerializerFactory();

            // Get full path of my batchpartinfo
            var fullPath = GetBatchPartInfoPath(batchPartInfo).FullPath;

            if (!File.Exists(fullPath))
                return Task.FromResult<SyncTable>(null);

            if (SanitizedSchema is null || batchPartInfo.Tables is null || batchPartInfo.Tables.Count() < 1)
                return Task.FromResult<SyncTable>(null);

            var schemaTable = SanitizedSchema.Tables[batchPartInfo.Tables[0].TableName, batchPartInfo.Tables[0].SchemaName];

            var localSerializer = localSerializerFactory.GetLocalSerializer();

            var table = schemaTable.Clone();

            foreach (var syncRow in localSerializer.ReadRowsFromFile(fullPath, schemaTable))
                table.Rows.Add(syncRow);

            return Task.FromResult(table);
        }

        public async Task SaveBatchPartInfoAsync(BatchPartInfo batchPartInfo, SyncTable syncTable, ILocalSerializerFactory localSerializerFactory = null)
        {
            if (localSerializerFactory is null)
                localSerializerFactory = new LocalJsonSerializerFactory();

            // Get full path of my batchpartinfo
            var fullPath = GetBatchPartInfoPath(batchPartInfo).FullPath;

            if (!File.Exists(fullPath))
                return;

            File.Delete(fullPath);

            var localSerializer = localSerializerFactory.GetLocalSerializer();

            // open the file and write table header
            await localSerializer.OpenFileAsync(fullPath, syncTable).ConfigureAwait(false);

            foreach (var row in syncTable.Rows)
                await localSerializer.WriteRowToFileAsync(row, syncTable).ConfigureAwait(false);

            // Close file
            await localSerializer.CloseFileAsync(fullPath, syncTable).ConfigureAwait(false);

        }

        /// <summary>
        /// try to delete the Batch tmp directory and all the files stored in it
        /// </summary>
        public void TryRemoveDirectory()
        {
            // Once we have applied all the batch, we can safely remove the temp dir and all it's files
            if (!string.IsNullOrEmpty(DirectoryRoot) && !string.IsNullOrEmpty(DirectoryName))
            {
                var tmpDirectory = new DirectoryInfo(GetDirectoryFullPath());

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
            // Delete folders before deleting batch parts
            if (deleteFolder)
                TryRemoveDirectory();
        }

    }
}