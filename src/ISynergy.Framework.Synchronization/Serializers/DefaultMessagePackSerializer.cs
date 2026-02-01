using Dotmim.Sync.Serialization;
using MessagePack;
using MessagePack.Resolvers;
using System.Text;

namespace ISynergy.Framework.Synchronization.Serializers;

public class DefaultMessagePackSerializer : ISerializer
{
    private readonly MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);

    public async Task<T> DeserializeAsync<T>(Stream ms) =>
        (T)await this.DeserializeAsync(ms, typeof(T)).ConfigureAwait(false);

    public Task<byte[]> SerializeAsync<T>(T obj) =>
        this.SerializeAsync(obj);

    public async Task<object> DeserializeAsync(Stream ms, Type type)
    {
        var result = await MessagePackSerializer.DeserializeAsync(type, ms, this.options).ConfigureAwait(false);
        return result!;
    }

    public T Deserialize<T>(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        using var ms = new MemoryStream(bytes);
        var result = MessagePackSerializer.Deserialize(typeof(T), ms, this.options);
        return (T)result!;
    }

    public async Task<byte[]> SerializeAsync(object obj)
    {
        using var ms = new MemoryStream();
        await MessagePackSerializer.SerializeAsync(ms, obj, this.options).ConfigureAwait(false);
        return ms.ToArray();
    }

    public byte[] Serialize<T>(T obj)
    {
        using var ms = new MemoryStream();
        MessagePackSerializer.Serialize(ms, obj, this.options);
        return ms.ToArray();
    }

    public async Task<byte[]> SerializeAsync(object obj, Type type)
    {
        using var ms = new MemoryStream();
        await MessagePackSerializer.SerializeAsync(type, ms, obj, this.options).ConfigureAwait(false);
        return ms.ToArray();
    }

    public byte[] Serialize(object obj, Type type)
    {
        var result = MessagePackSerializer.Serialize(type, obj, this.options);
        return result;
    }
}
