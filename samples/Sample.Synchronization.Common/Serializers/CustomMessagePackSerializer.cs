using ISynergy.Framework.Synchronization.Core.Serialization;
using MessagePack;

namespace Sample.Serializers
{

    public class CustomMessagePackSerializerFactory : ISerializerFactory
    {
        public string Key => "mpack";
        public ISerializer<T> GetSerializer<T>() => new CustomMessagePackSerializer<T>();
    }

    public class CustomMessagePackSerializer<T> : ISerializer<T>
    {
        private MessagePackSerializerOptions options;

        public CustomMessagePackSerializer() => this.options = MessagePack.Resolvers.ContractlessStandardResolver.Options;

        public async Task<T> DeserializeAsync(Stream ms)
        {
            var t = await MessagePackSerializer.DeserializeAsync<T>(ms, options);

            return t;

        }
        public async Task<byte[]> SerializeAsync(T obj)
        {
            using var ms = new MemoryStream();
            await MessagePackSerializer.SerializeAsync(ms, obj, options);
            return ms.ToArray();
        }
    }
}





