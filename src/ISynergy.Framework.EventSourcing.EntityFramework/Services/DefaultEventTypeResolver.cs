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
/// For applications with many assemblies, consider supplying a custom <see cref="IEventTypeResolver"/>
/// that maintains an explicit type map to avoid assembly scanning on the hot path.
/// </remarks>
[RequiresUnreferencedCode("Scans AppDomain assemblies at runtime. Use a custom IEventTypeResolver with an explicit type map for AOT/trim-safe scenarios.")]
public sealed class DefaultEventTypeResolver : IEventTypeResolver
{
    /// <inheritdoc />
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
