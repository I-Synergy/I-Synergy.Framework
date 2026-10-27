using ISynergy.Framework.EventSourcing.EntityFramework.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Services;

/// <summary>
/// Resolves event types by first attempting <see cref="Type.GetType(string)"/> (handles
/// assembly-qualified names), then falling back to scanning all loaded assemblies for a type
/// whose <c>FullName</c> or <c>Name</c> matches.
/// </summary>
/// <remarks>
/// Register this as a singleton via the reflection-based overload of
/// <c>AddEventSourcingEntityFramework(IServiceCollection, Action&lt;DbContextOptionsBuilder&gt;)</c>.
/// Not NativeAOT/trim-safe: under full trimming an event type not otherwise referenced by the
/// application may be removed, and assembly scanning itself requires unreferenced-code access.
/// Use <see cref="DictionaryEventTypeResolver"/> instead when publishing with
/// <c>PublishAot</c> or <c>PublishTrimmed</c>.
/// </remarks>
public sealed class DefaultEventTypeResolver : IEventTypeResolver
{
    /// <inheritdoc />
    [RequiresUnreferencedCode("Scans all loaded assemblies for a type matching the stored event type name; not trim-compatible. Use DictionaryEventTypeResolver instead under NativeAOT/trimming.")]
    public Type? Resolve(string eventTypeName)
    {
        // Fast path: assembly-qualified or fully-qualified name already resolvable.
        var type = Type.GetType(eventTypeName);
        if (type is not null)
            return type;

        // Fallback: scan loaded assemblies for FullName or short Name match.
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch (Exception) { return []; }
            })
            .FirstOrDefault(t => t.FullName == eventTypeName || t.Name == eventTypeName);
    }

    /// <inheritdoc />
    public string GetTypeName(Type eventType) =>
        eventType.FullName ?? eventType.Name;
}
