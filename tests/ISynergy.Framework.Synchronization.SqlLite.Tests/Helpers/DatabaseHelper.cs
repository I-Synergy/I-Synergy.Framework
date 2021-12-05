using ISynergy.Framework.Synchronization.Core.Tests.Base;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlLite.Tests.Helpers
{
    internal class DatabaseHelper : BaseDatabaseHelper
    {
        private readonly IConfigurationRoot _configuration;

        public DatabaseHelper()
        {
            _configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", false, true)
              .AddJsonFile("appsettings.local.json", true, true)
              .Build();
        }

        /// <summary>
        /// Gets the connection string used to open a sqlite database
        /// </summary>
        public override string GetConnectionString(string dbName)
        {
            var builder = new SqliteConnectionStringBuilder { DataSource = GetSqliteFilePath(dbName) };
            return builder.ConnectionString;
        }

        public override Task CreateDatabaseAsync(string dbName, bool recreateDb = true)
        {
            return Task.CompletedTask;
        }

        public override void DropDatabase(string dbName)
        {
            string filePath = null;
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                filePath = GetSqliteFilePath(dbName);

                if (File.Exists(filePath))
                    File.Delete(filePath);

            }
            catch (Exception)
            {
                Debug.WriteLine($"Sqlite file seems loked. ({filePath})");
            }
        }

        public override async Task ExecuteScriptAsync(string dbName, string script)
        {
            using var connection = new SqliteConnection(GetConnectionString(dbName));
            connection.Open();
            using (var cmdDb = new SqliteCommand(script, connection))
                await cmdDb.ExecuteNonQueryAsync();
            connection.Close();
        }

        /// <summary>
        /// Get the Sqlite file path (ie: /Dir/mydatabase.db)
        /// </summary>
        private string GetSqliteFilePath(string dbName)
        {
            var fi = new FileInfo(dbName);

            if (string.IsNullOrEmpty(fi.Extension))
                dbName = $"{dbName}.db";

            return Path.Combine(Directory.GetCurrentDirectory(), dbName);
        }

        private async Task ExecuteSqliteScriptAsync(string dbName, string script)
        {
            using var connection = new SqliteConnection(GetConnectionString(dbName));
            connection.Open();
            using (var cmdDb = new SqliteCommand(script, connection))
                await cmdDb.ExecuteNonQueryAsync();
            connection.Close();
        }

        /// <summary>
        /// Drop a sqlite database
        /// </summary>
        private void DropSqliteDatabase(string dbName)
        {
            string filePath = null;
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                filePath = GetSqliteFilePath(dbName);

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception)
            {
                Debug.WriteLine($"Sqlite file seems loked. ({filePath})");
            }
        }


        #region MariaDB
        ///// <summary>
        ///// Create a new MySql Server database
        ///// </summary>
        //private static async Task CreateMariaDBDatabaseAsync(string dbName, bool recreateDb = true)
        //{
        //    var onRetry = new Func<Exception, int, TimeSpan, object, Task>((ex, cpt, ts, arg) =>
        //    {
        //        Console.WriteLine($"Creating MariaDB database failed when connecting to information_schema ({ex.Message}). Wating {ts.Milliseconds}. Try number {cpt}");
        //        return Task.CompletedTask;
        //    });

        //    var policy = SyncPolicy.WaitAndRetry(3, TimeSpan.FromMilliseconds(500), null, onRetry);

        //    await policy.ExecuteAsync(async () =>
        //    {
        //        using var sysConnection = new MySqlConnection(Setup.GetMariaDBDatabaseConnectionString("information_schema"));
        //        sysConnection.Open();

        //        if (recreateDb)
        //        {
        //            using var cmdDrop = new MySqlCommand($"Drop schema if exists  {dbName};", sysConnection);
        //            await cmdDrop.ExecuteNonQueryAsync();
        //        }

        //        using (var cmdDb = new MySqlCommand($"create schema {dbName};", sysConnection))
        //            cmdDb.ExecuteNonQuery();

        //        sysConnection.Close();
        //    });
        //}

        //private static async Task ExecuteMariaDBScriptAsync(string dbName, string script)
        //{
        //    using var connection = new MySqlConnection(Setup.GetMariaDBDatabaseConnectionString(dbName));
        //    connection.Open();

        //    using (var cmdDb = new MySqlCommand(script, connection))
        //        await cmdDb.ExecuteNonQueryAsync();

        //    connection.Close();
        //}

        ///// <summary>
        ///// Drop a MariaDB database
        ///// </summary>
        //private static void DropMariaDBDatabase(string dbName)
        //{
        //    using var sysConnection = new MySqlConnection(Setup.GetMariaDBDatabaseConnectionString("information_schema"));
        //    sysConnection.Open();

        //    using (var cmdDb = new MySqlCommand($"drop database if exists {dbName};", sysConnection))
        //        cmdDb.ExecuteNonQuery();

        //    sysConnection.Close();
        //}
        #endregion

        #region MySql
        ///// <summary>
        ///// Create a new MySql Server database
        ///// </summary>
        //private static async Task CreateMySqlDatabaseAsync(string dbName, bool recreateDb = true)
        //{
        //    var onRetry = new Func<Exception, int, TimeSpan, object, Task>((ex, cpt, ts, arg) =>
        //    {
        //        Console.WriteLine($"Creating MySql database failed when connecting to information_schema ({ex.Message}). Wating {ts.Milliseconds}. Try number {cpt}");
        //        return Task.CompletedTask;
        //    });

        //    var policy = SyncPolicy.WaitAndRetry(3, TimeSpan.FromMilliseconds(500), null, onRetry);

        //    await policy.ExecuteAsync(async () =>
        //    {
        //        using var sysConnection = new MySqlConnection(Setup.GetMySqlDatabaseConnectionString("information_schema"));
        //        sysConnection.Open();

        //        if (recreateDb)
        //        {
        //            using var cmdDrop = new MySqlCommand($"Drop schema if exists  {dbName};", sysConnection);
        //            await cmdDrop.ExecuteNonQueryAsync();
        //        }

        //        using (var cmdDb = new MySqlCommand($"create schema {dbName};", sysConnection))
        //            cmdDb.ExecuteNonQuery();

        //        sysConnection.Close();
        //    });
        //}

        //private static async Task ExecuteMySqlScriptAsync(string dbName, string script)
        //{
        //    using var connection = new MySqlConnection(Setup.GetMySqlDatabaseConnectionString(dbName));
        //    connection.Open();

        //    using (var cmdDb = new MySqlCommand(script, connection))
        //        await cmdDb.ExecuteNonQueryAsync();

        //    connection.Close();
        //}

        ///// <summary>
        ///// Drop a mysql database
        ///// </summary>
        //private static void DropMySqlDatabase(string dbName)
        //{
        //    using var sysConnection = new MySqlConnection(Setup.GetMySqlDatabaseConnectionString("information_schema"));
        //    sysConnection.Open();

        //    using (var cmdDb = new MySqlCommand($"drop database if exists {dbName};", sysConnection))
        //        cmdDb.ExecuteNonQuery();

        //    sysConnection.Close();
        //}
        #endregion
    }
}
