using Dotmim.Sync.Serialization;
using ISynergy.Framework.Synchronization.Serializers;

namespace ISynergy.Framework.Synchronization.Factories;

public class MessagePackSerializerFactory : ISerializerFactory
{
    public string Key => "mpack";
    public ISerializer GetSerializer() => new DefaultMessagePackSerializer();
}
