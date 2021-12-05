using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Tests.Base;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Tests.Helpers
{
    internal class DatabaseHelper : BaseDatabaseHelper
    {
        /// <summary>
        /// Returns the database connection string for Sql
        /// </summary>
        public override string GetConnectionString(string dbName)
        {
            var cstring = string.Format(_configuration.GetSection("ConnectionStrings")["SqlConnection"], dbName);
            var builder = new SqlConnectionStringBuilder(cstring);

            if (IsOnAzureDev)
            {
                builder.IntegratedSecurity = false;
                builder.DataSource = @"localhost";
                builder.UserID = "sa";
                builder.Password = "Password12!";
            }

            return builder.ToString();
        }

        /// <summary>
        /// Create a new Sql Server database
        /// </summary>
        public override async Task CreateDatabaseAsync(string dbName, bool recreateDb = true)
        {
            var onRetry = new Func<Exception, int, TimeSpan, object, Task>((ex, cpt, ts, arg) =>
            {
                Console.WriteLine($"Creating SQL Server database failed when connecting to master ({ex.Message}). Wating {ts.Milliseconds}. Try number {cpt}");
                return Task.CompletedTask;
            });

            var policy = SyncPolicy.WaitAndRetry(3, TimeSpan.FromMilliseconds(500), null, onRetry);

            await policy.ExecuteAsync(async () =>
            {
                using var masterConnection = new SqlConnection(GetConnectionString("master"));
                masterConnection.Open();

                using (var cmdDb = new SqlCommand(GetCreationScript(dbName, recreateDb), masterConnection))
                    await cmdDb.ExecuteNonQueryAsync();

                masterConnection.Close();
            });
        }

        /// <summary>
        /// Delete a database
        /// </summary>
        public override void DropDatabase(string dbName)
        {
            using var masterConnection = new SqlConnection(GetConnectionString("master"));
            try
            {
                masterConnection.Open();

                using (var cmdDb = new SqlCommand(GetDropDatabaseScript(dbName), masterConnection))
                    cmdDb.ExecuteNonQuery();

                masterConnection.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public override async Task ExecuteScriptAsync(string dbName, string script)
        {
            using var connection = new SqlConnection(GetConnectionString(dbName));
            connection.Open();

            //split the script on "GO" commands
            var splitter = new string[] { "\r\nGO\r\n" };
            var commandTexts = script.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            foreach (var commandText in commandTexts)
            {
                using var cmdDb = new SqlCommand(commandText, connection);
                await cmdDb.ExecuteNonQueryAsync();
            }
            connection.Close();
        }

        /// <summary>
        /// Gets the Create or Re-create a database script text
        /// </summary>
        private string GetCreationScript(string dbName, bool recreateDb = true)
        {
            if (recreateDb)
                return $@"if (exists (Select * from sys.databases where name = '{dbName}'))
                    begin
	                    alter database [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	                    drop database {dbName}
                    end
                    Create database {dbName}";
            else
                return $@"if not (exists (Select * from sys.databases where name = '{dbName}')) 
                          Create database {dbName}";
        }

        /// <summary>
        /// Gets the drop sql database script
        /// </summary>
        private string GetDropDatabaseScript(string dbName)
        {
            return $@"if (exists (Select * from sys.databases where name = '{dbName}'))
            begin
	            alter database [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	            drop database {dbName};
            end";
        }

        /// <summary>
        /// Restore a sql backup file
        /// </summary>
        private void RestoreSqlDatabase(string dbName, string filePath)
        {
            var dataName = Path.GetFileNameWithoutExtension(dbName) + ".mdf";
            var logName = Path.GetFileNameWithoutExtension(dbName) + ".ldf";
            var script = $@"
                if (exists (select * from sys.databases where name = '{dbName}'))
                    begin                
                        ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
                    End
                else
                    begin
                        CREATE DATABASE [{dbName}]
                    end

                -- the backup contains the full path to the database files
                -- in order to be able to restore them on different developer machines
                -- we retrieve the default data path from the server
                -- and use it in RESTORE with the MOVE option
                declare @databaseFolder as nvarchar(256);
                set @databaseFolder = Convert(nvarchar(256), (SELECT ServerProperty(N'InstanceDefaultDataPath') AS default_file));

                declare @dataFile as nvarchar(256);
                declare @logFile as nvarchar(256);
                set @dataFile =@databaseFolder + '{dataName}';
                set @logFile =@databaseFolder + '{logName}';

                RESTORE DATABASE [{dbName}] FROM  DISK = N'{filePath}' WITH  RESTRICTED_USER, REPLACE,
                    MOVE '{dbName}' TO @dataFile,
                    MOVE '{dbName}_log' TO @logFile;
                ALTER DATABASE [{dbName}] SET MULTI_USER";


            using var connection = new SqlConnection(GetConnectionString("master"));
            connection.Open();

            using (var cmdDb = new SqlCommand(script, connection))
                cmdDb.ExecuteNonQuery();

            connection.Close();
        }

        public override void ClearAllPools()
        {
            SqlConnection.ClearAllPools();
        }
    }
}
