﻿using ISynergy.Framework.Synchronization.Core.Database;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Builders
{
    public abstract class DbBuilder
    {
        /// <summary>
        /// First step before creating schema
        /// </summary>
        public abstract Task EnsureDatabaseAsync(DbConnection connection, DbTransaction transaction = null);

        /// <summary>
        /// First step before creating schema
        /// </summary>
        public abstract Task<SyncTable> EnsureTableAsync(string tableName, string schemaName, DbConnection connection, DbTransaction transaction = null);

        /// <summary>
        /// Make a hello test on the current database
        /// </summary>
        public abstract Task<(string DatabaseName, string Version)> GetHelloAsync(DbConnection connection, DbTransaction transaction = null);

    }
}
