using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace ISynergy.Framework.CQRS.SourceGenerator.Parsers;

internal static class HandlerParser
{
    // Display format for emitting type names in generated code:
    // - Does NOT include "global::" prefix (we handle this carefully in emitters)
    // - Does include full namespace qualification
    // - Uses language keyword aliases (string, int, bool, etc.) for built-in types
    private static readonly SymbolDisplayFormat FullyQualifiedNoGlobal = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions:
            SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
            SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

    // Format for interface identity comparison (open generics with type parameter names):
    // Needs global:: stripped for comparison against string constants.
    private static readonly SymbolDisplayFormat OpenGenericComparison = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

    // These are the fully-qualified names of the open generic interfaces, for identity comparison.
    // FullyQualifiedFormat renders open generics as "Namespace.Interface<TypeParamName>".
    private const string CommandHandlerOpen = "ISynergy.Framework.CQRS.Commands.ICommandHandler<TCommand>";
    private const string CommandHandlerWithResult = "ISynergy.Framework.CQRS.Abstractions.Commands.ICommandHandler<TCommand, TResult>";
    private const string QueryHandlerOpen = "ISynergy.Framework.CQRS.Queries.IQueryHandler<TQuery, TResult>";

    /// <summary>
    /// Inspects <paramref name="typeSymbol"/> and returns all <see cref="HandlerInfo"/> records
    /// for each CQRS handler interface it implements. Returns an empty enumerable when the
    /// type implements no handler interfaces.
    /// </summary>
    /// <param name="typeSymbol">The type symbol to inspect.</param>
    /// <returns>All handler infos for CQRS interfaces implemented by the type.</returns>
    public static IEnumerable<HandlerInfo> GetHandlerInfos(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.IsAbstract || typeSymbol.TypeKind != TypeKind.Class)
            yield break;

        // Skip non-public types — private/internal/protected types cannot be referenced in
        // generated DI registration code (e.g., private nested classes inside test step definitions).
        if (typeSymbol.DeclaredAccessibility != Accessibility.Public)
            yield break;

        // Skip open generic type definitions (e.g., decorator<TCommand>) — they have unbound type parameters.
        // We only want concrete, fully-closed types.
        if (typeSymbol.IsGenericType && typeSymbol.TypeArguments.Any(
            static t => t.TypeKind == TypeKind.TypeParameter))
            yield break;

        foreach (var iface in typeSymbol.AllInterfaces)
        {
            if (!iface.IsGenericType)
                continue;

            var originalDef = iface.OriginalDefinition.ToDisplayString(OpenGenericComparison);
            var typeArgs = iface.TypeArguments;

            if (originalDef == CommandHandlerOpen && typeArgs.Length == 1)
            {
                var tCommand = typeArgs[0].ToDisplayString(FullyQualifiedNoGlobal);
                var serviceName = $"ISynergy.Framework.CQRS.Commands.ICommandHandler<{tCommand}>";
                yield return new HandlerInfo(
                    HandlerTypeName: typeSymbol.ToDisplayString(FullyQualifiedNoGlobal),
                    ServiceInterfaceName: serviceName,
                    FirstTypeArg: tCommand,
                    SecondTypeArg: null,
                    Kind: HandlerKind.Command);
            }
            else if (originalDef == CommandHandlerWithResult && typeArgs.Length == 2)
            {
                var tCommand = typeArgs[0].ToDisplayString(FullyQualifiedNoGlobal);
                var tResult = typeArgs[1].ToDisplayString(FullyQualifiedNoGlobal);
                var serviceName = $"ISynergy.Framework.CQRS.Abstractions.Commands.ICommandHandler<{tCommand}, {tResult}>";
                yield return new HandlerInfo(
                    HandlerTypeName: typeSymbol.ToDisplayString(FullyQualifiedNoGlobal),
                    ServiceInterfaceName: serviceName,
                    FirstTypeArg: tCommand,
                    SecondTypeArg: tResult,
                    Kind: HandlerKind.CommandWithResult);
            }
            else if (originalDef == QueryHandlerOpen && typeArgs.Length == 2)
            {
                var tQuery = typeArgs[0].ToDisplayString(FullyQualifiedNoGlobal);
                var tResult = typeArgs[1].ToDisplayString(FullyQualifiedNoGlobal);
                var serviceName = $"ISynergy.Framework.CQRS.Queries.IQueryHandler<{tQuery}, {tResult}>";
                yield return new HandlerInfo(
                    HandlerTypeName: typeSymbol.ToDisplayString(FullyQualifiedNoGlobal),
                    ServiceInterfaceName: serviceName,
                    FirstTypeArg: tQuery,
                    SecondTypeArg: tResult,
                    Kind: HandlerKind.Query);
            }
        }
    }
}
