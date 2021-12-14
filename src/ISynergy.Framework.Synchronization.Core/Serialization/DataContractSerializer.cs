using ISynergy.Framework.Synchronization.Core.Database;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Serialization
{

    public class ContractSerializerFactory : ISerializerFactory
    {
        public string Key => "dc";
        private static ContractSerializerFactory instance = null;
        public static ContractSerializerFactory Current => instance ?? new ContractSerializerFactory();

        public ISerializer<T> GetSerializer<T>() => new ContractSerializer<T>();

    }

    public class ContractSerializer<T> : ISerializer<T>
    {

        public ContractSerializer()
        {
        }

        public string Extension => "bin";

        public Task CloseFileAsync(string path, SyncTable shemaTable) => throw new NotImplementedException();

        public async Task<T> DeserializeAsync(Stream ms)
        {
            using (var ims = new MemoryStream())
            {
                // Quick fix to not being IO Synchronous, new refused feature from .Net Core 3.1
                // Even if you try a Task.Run or a ThreadPool.Queue and so on, you will enventually had this exception:
                // "Synchronous operations are disallowed. Call ReadAsync or set AllowSynchronousIO to true instead"
                await ms.CopyToAsync(ims);
                ims.Seek(0, SeekOrigin.Begin);

                var serializer = new DataContractSerializer(typeof(T));

                var res = (T)serializer.ReadObject(ims);

                await ims.FlushAsync();
                return res;
            }


        }

        public Task<long> GetCurrentFileSizeAsync() => throw new NotImplementedException();
        public Task OpenFileAsync(string path, SyncTable shemaTable) => throw new NotImplementedException();

        public async Task<byte[]> SerializeAsync(T obj)
        {
            var serializer = new DataContractSerializer(typeof(T));

            using var ms = new MemoryStream();

            serializer.WriteObject(ms, obj);
            var a = ms.ToArray();
            await ms.FlushAsync();

            return a;
        }

        public Task WriteRowToFileAsync(object[] row, SyncTable shemaTable) => throw new NotImplementedException();
    }
}

