namespace ISynergy.Framework.AspNetCore.Blazor.SourceGenerator
{
    /// <summary>
    /// Classifies the UI type kind discovered by the source generator.
    /// </summary>
    internal enum BlazorTypeKind
    {
        /// <summary>Implements <c>IView</c>.</summary>
        View,

        /// <summary>Implements <c>IWindow</c>.</summary>
        Window,

        /// <summary>Implements <c>IViewModel</c>.</summary>
        ViewModel,
    }

    /// <summary>
    /// Service lifetime to use when registering the type in DI.
    /// Mirrors <c>ISynergy.Framework.Core.Enumerations.Lifetimes</c>.
    /// </summary>
    internal enum BlazorLifetime
    {
        /// <summary>Transient lifetime (default).</summary>
        Transient,

        /// <summary>Scoped lifetime.</summary>
        Scoped,

        /// <summary>Singleton lifetime.</summary>
        Singleton,
    }

    /// <summary>
    /// Holds all information needed by the emitter for a single discovered Blazor UI type.
    /// </summary>
    internal record BlazorTypeInfo(
        string ConcreteTypeName,
        string? AbstractionTypeName,
        BlazorTypeKind Kind,
        BlazorLifetime Lifetime);
}
