using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.UI.Helpers;

/// <summary>
/// Thread-safe singleton container keyed by type.
/// </summary>
/// <typeparam name="T">
/// The type of singleton. Must have a public parameterless constructor.
/// The <see cref="DynamicallyAccessedMembersAttribute"/> annotation ensures the
/// parameterless constructor is preserved under NativeAOT trimming.
/// </typeparam>
public static class Singleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
    where T : new()
{
    /// <summary>
    /// The instances
    /// </summary>
    private static readonly ConcurrentDictionary<Type, T> _instances = new ConcurrentDictionary<Type, T>();

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static T Instance => _instances.GetOrAdd(typeof(T), (_) => new T());
}
