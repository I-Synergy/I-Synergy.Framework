using Dotmim.Sync.Serialization;
using MessagePack;
using MessagePack.Resolvers;

namespace ISynergy.Framework.Synchronization.Serializers;

public class DefaultMessagePackSerializer : ISerializer
{
    public Task<byte[]> SerializeAsync<T>(T obj) =>
        SerializeAsync((object)obj);

    public async Task<byte[]> SerializeAsync(object obj)
    {
        using var ms = new MemoryStream();
        await MessagePackSerializer.SerializeAsync(ms, obj, MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance));
        return ms.ToArray();
    }

    public async Task<T> DeserializeAsync<T>(Stream ms) =>
        (T)await DeserializeAsync(ms, typeof(T)).ConfigureAwait(false);

    public async Task<object> DeserializeAsync(Stream ms, Type type)
    {
        var result = await MessagePackSerializer.DeserializeAsync(type, ms, MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance));
        return result;
    }
}
