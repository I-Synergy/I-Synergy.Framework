namespace ISynergy.Framework.CQRS.SourceGenerator.Parsers;

/// <summary>
/// The kind of CQRS handler represented.
/// </summary>
internal enum HandlerKind
{
    /// <summary>ICommandHandler&lt;TCommand&gt; — command with no return value.</summary>
    Command,

    /// <summary>ICommandHandler&lt;TCommand, TResult&gt; — command with return value.</summary>
    CommandWithResult,

    /// <summary>IQueryHandler&lt;TQuery, TResult&gt; — query with return value.</summary>
    Query,
}

/// <summary>
/// Fully-qualified type names for a discovered handler and its interface.
/// Equatable so Roslyn's incremental pipeline can diff and cache.
/// </summary>
/// <param name="HandlerTypeName">Fully-qualified name of the handler implementation class.</param>
/// <param name="ServiceInterfaceName">Fully-qualified name of the service interface to register against.</param>
/// <param name="FirstTypeArg">Fully-qualified name of the first type argument (TCommand or TQuery).</param>
/// <param name="SecondTypeArg">
/// Fully-qualified name of the second type argument (TResult), or <c>null</c>
/// for <see cref="HandlerKind.Command"/> (no return value).
/// </param>
/// <param name="Kind">The kind of handler.</param>
internal sealed record HandlerInfo(
    string HandlerTypeName,
    string ServiceInterfaceName,
    string FirstTypeArg,
    string? SecondTypeArg,
    HandlerKind Kind);
