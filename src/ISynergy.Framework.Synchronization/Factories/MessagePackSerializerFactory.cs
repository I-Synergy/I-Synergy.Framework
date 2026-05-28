using Dotmim.Sync.Serialization;
using ISynergy.Framework.Synchronization.Serializers;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Synchronization.Factories;

/// <summary>
/// Factory that creates <see cref="DefaultMessagePackSerializer"/> instances for use with
/// <c>Dotmim.Sync</c> serialization pipelines.
/// </summary>
/// <remarks>
/// <para>
/// This factory is not compatible with Native AOT or trimmed builds because it returns a
/// <see cref="DefaultMessagePackSerializer"/> which depends on
/// <c>ContractlessStandardResolver</c> — a reflection-based MessagePack resolver.
/// See <see cref="DefaultMessagePackSerializer"/> for the full AOT compatibility note and
/// migration guidance.
/// </para>
/// </remarks>
[RequiresUnreferencedCode("Returns DefaultMessagePackSerializer which is not AOT-compatible. " +
    "See DefaultMessagePackSerializer remarks for migration guidance.")]
[RequiresDynamicCode("Returns DefaultMessagePackSerializer which uses dynamic code generation.")]
public class MessagePackSerializerFactory : ISerializerFactory
{
    /// <summary>
    /// Gets the serializer key used to identify this factory within the <c>Dotmim.Sync</c> pipeline.
    /// </summary>
    public string Key => "mpack";

    /// <summary>
    /// Creates and returns a new <see cref="DefaultMessagePackSerializer"/> instance.
    /// </summary>
    /// <returns>A <see cref="ISerializer"/> backed by <c>ContractlessStandardResolver</c>.</returns>
    public ISerializer GetSerializer() => new DefaultMessagePackSerializer();
}
