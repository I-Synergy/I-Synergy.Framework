namespace ISynergy.Framework.UI.SourceGenerator
{
    /// <summary>
    /// Classifies the UI type kind discovered by the source generator.
    /// </summary>
    internal enum UITypeKind
    {
        /// <summary>Implements <c>IView</c>.</summary>
        View,

        /// <summary>Implements <c>IWindow</c>.</summary>
        Window,

        /// <summary>Implements <c>IViewModel</c>.</summary>
        ViewModel,
    }

    /// <summary>
    /// Holds all information needed by the emitter for a single discovered UI type.
    /// </summary>
    internal record UITypeInfo(
        string ConcreteTypeName,
        string? AbstractionTypeName,
        UITypeKind Kind,
        string? RelatedViewName);
}
