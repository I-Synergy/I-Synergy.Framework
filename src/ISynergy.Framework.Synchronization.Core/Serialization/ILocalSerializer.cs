using ISynergy.Framework.Synchronization.Core.Database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Serialization
{
    public interface ILocalSerializerFactory
    {
        string Key { get; }
        ILocalSerializer GetLocalSerializer();
    }

    public interface ILocalSerializer
    {
        Task CloseFileAsync(string path, SyncTable shemaTable);
        Task OpenFileAsync(string path, SyncTable shemaTable);
        Task WriteRowToFileAsync(SyncRow row, SyncTable shemaTable);
        Task<long> GetCurrentFileSizeAsync();
        IEnumerable<SyncRow> ReadRowsFromFile(string path, SyncTable shemaTable);
        string Extension { get; }
    }
}
