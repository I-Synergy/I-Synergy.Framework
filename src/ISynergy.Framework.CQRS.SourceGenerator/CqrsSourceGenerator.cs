using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using ISynergy.Framework.CQRS.SourceGenerator.Emitters;
using ISynergy.Framework.CQRS.SourceGenerator.Parsers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ISynergy.Framework.CQRS.SourceGenerator;

/// <summary>
/// Roslyn incremental source generator that emits AOT-safe CQRS handler registrations
/// and query dispatch table setup for all assemblies that reference ISynergy.Framework.CQRS.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class CqrsSourceGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Initializes the generator pipeline.
    /// </summary>
    /// <param name="context">The incremental generator initialization context.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 1. Find all class declarations in the current compilation.
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax { BaseList: not null },
                transform: TransformToHandlerInfos)
            .Where(static infos => infos.Length > 0)
            .SelectMany(static (infos, _) => infos);

        // 2. Collect all handlers.
        var allHandlers = classDeclarations.Collect();

        // 3. Get the assembly root namespace from the compilation options.
        var rootNamespace = context.CompilationProvider.Select(
            static (compilation, _) =>
                compilation.AssemblyName?.Replace('-', '_') ?? "GeneratedCqrs");

        // 4. Combine and emit.
        var combined = allHandlers.Combine(rootNamespace);

        context.RegisterSourceOutput(combined, static (spc, data) =>
        {
            var (handlers, ns) = data;

            if (handlers.IsDefaultOrEmpty)
                return;

            var handlerList = handlers.ToList();
            var queryHandlers = handlerList.Where(h => h.Kind == HandlerKind.Query).ToList();

            // Emit handler registration
            var registrationSource = HandlerRegistrationEmitter.Emit(handlerList, ns);
            spc.AddSource("CqrsHandlerRegistrations.g.cs", registrationSource);

            // Emit query dispatch table (only when there are query handlers)
            if (queryHandlers.Count > 0)
            {
                var tableSource = QueryDispatchTableEmitter.Emit(queryHandlers, ns);
                spc.AddSource("CqrsQueryDispatchTable.g.cs", tableSource);
            }
        });
    }

    private static ImmutableArray<HandlerInfo> TransformToHandlerInfos(
        GeneratorSyntaxContext context,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDecl = (ClassDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(classDecl, cancellationToken);

        if (symbol is not INamedTypeSymbol namedSymbol)
            return ImmutableArray<HandlerInfo>.Empty;

        var infos = HandlerParser.GetHandlerInfos(namedSymbol).ToList();

        return infos.Count > 0
            ? infos.ToImmutableArray()
            : ImmutableArray<HandlerInfo>.Empty;
    }
}
