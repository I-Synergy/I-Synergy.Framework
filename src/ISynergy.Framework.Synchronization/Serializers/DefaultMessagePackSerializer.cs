using Dotmim.Sync.Serialization;
using MessagePack;
using MessagePack.Resolvers;

namespace ISynergy.Framework.Synchronization.Serializers;

public class DefaultMessagePackSerializer : ISerializer
{
    public async Task<T> DeserializeAsync<T>(Stream memoryStream) => 
        (T)await DeserializeAsync(memoryStream, typeof(T)).ConfigureAwait(false);

    public Task<byte[]> SerializeAsync<T>(T obj) => 
        SerializeAsync((object)obj);


    public async Task<object> DeserializeAsync(Stream memoryStream, Type type)
    {
        var result = await MessagePackSerializer.DeserializeAsync(type, memoryStream, MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance));
        return result;
    }

    public async Task<byte[]> SerializeAsync(object obj)
    {
        using var ms = new MemoryStream();
        await MessagePackSerializer.SerializeAsync(ms, obj, MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance));
        return ms.ToArray();
    }
}
