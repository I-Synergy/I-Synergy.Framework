using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Sample.Synchronization.SqlServer.Helpers
{
    public class DBHelper
    {
        private static IConfiguration configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", false, true)
              .AddJsonFile("appsettings.local.json", true, true)
              .Build();

        protected DBHelper()
        {
        }

        public static string GetDatabaseConnectionString(string dbName) =>
            string.Format(configuration.GetSection("ConnectionStrings")["SqlConnection"], dbName);

        /// <summary>
        /// create a server database with datas and an empty client database
        /// </summary>
        /// <returns></returns>
        public static async Task EnsureDatabasesAsync(string databaseName, bool useSeeding = true)
        {
            // Create server database with items
            using var dbServer = new AdventureWorksContext(GetDatabaseConnectionString(databaseName), useSeeding);
            await dbServer.Database.EnsureDeletedAsync();
            await dbServer.Database.EnsureCreatedAsync();
        }

        public static async Task DeleteDatabaseAsync(string dbName)
        {
            var masterConnection = new SqlConnection(GetDatabaseConnectionString("master"));
            await masterConnection.OpenAsync();
            var cmdDb = new SqlCommand(GetDeleteDatabaseScript(dbName), masterConnection);
            await cmdDb.ExecuteNonQueryAsync();
            masterConnection.Close();
        }

        public static async Task CreateDatabaseAsync(string dbName, bool recreateDb = true)
        {
            var masterConnection = new SqlConnection(GetDatabaseConnectionString("master"));
            await masterConnection.OpenAsync();
            var cmdDb = new SqlCommand(GetCreationDBScript(dbName, recreateDb), masterConnection);
            await cmdDb.ExecuteNonQueryAsync();
            masterConnection.Close();
        }

        private static string GetDeleteDatabaseScript(string dbName) =>
                  $@"if (exists (Select * from sys.databases where name = '{dbName}'))
            begin
	            alter database [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	            drop database {dbName}
            end";

        private static string GetCreationDBScript(string dbName, bool recreateDb = true)
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
    }
}
