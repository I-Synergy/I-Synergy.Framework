using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Abstractions.Tests
{
    public interface IDatabaseHelper
    {
        Task CreateDatabaseAsync(string dbName, bool recreateDb = true);
        void DropDatabase(string dbName);
        Task ExecuteScriptAsync(string dbName, string script);
        string GetConnectionString(string dbName);
        string GetRandomName(string pref = null);
        void ClearAllPools();
    }
}