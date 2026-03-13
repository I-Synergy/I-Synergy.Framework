using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ISynergy.Framework.AspNetCore.Blazor.SourceGenerator
{
    /// <summary>
    /// Roslyn incremental source generator that emits AOT-safe Blazor UI type registrations
    /// for all assemblies that reference ISynergy.Framework.AspNetCore.Blazor.
    /// Scans for types implementing <c>IView</c>, <c>IWindow</c>, and <c>IViewModel</c>
    /// and emits an <c>AddBlazorRegistrations(IServiceCollection)</c> extension method.
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public sealed class BlazorRegistrationGenerator : IIncrementalGenerator
    {
        // Fully-qualified names of the base UI interfaces (without global::)
        private const string IViewFullName = "ISynergy.Framework.Mvvm.Abstractions.IView";
        private const string IWindowFullName = "ISynergy.Framework.Mvvm.Abstractions.IWindow";
        private const string IViewModelFullName = "ISynergy.Framework.Mvvm.Abstractions.ViewModels.IViewModel";

        // Fully-qualified name of the LifetimeAttribute
        private const string LifetimeAttributeFullName = "ISynergy.Framework.Core.Attributes.LifetimeAttribute";

        // Lifetimes enum values (from ISynergy.Framework.Core.Enumerations.Lifetimes)
        // Scoped = 0, Singleton = 1
        private const int LifetimesScoped = 0;
        private const int LifetimesSingleton = 1;

        /// <summary>
        /// Initializes the generator pipeline.
        /// </summary>
        /// <param name="context">The incremental generator initialization context.</param>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var typeInfoProvider = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is ClassDeclarationSyntax { BaseList: not null },
                    transform: TransformToBlazorTypeInfo)
                .Where(static info => info != null)
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                .Select(static (info, _) => info);
#pragma warning restore CS8619

            var allTypes = typeInfoProvider.Collect();

            var rootNamespace = context.CompilationProvider.Select(
                static (compilation, _) =>
                    compilation.AssemblyName?.Replace('-', '_') ?? "GeneratedBlazor");

            var combined = allTypes.Combine(rootNamespace);

            context.RegisterSourceOutput(combined, static (spc, data) =>
            {
                var (types, ns) = data;

                if (types.IsDefaultOrEmpty)
                    return;

                var typeList = new List<BlazorTypeInfo>();
                foreach (var t in types)
                {
                    if (t != null)
                        typeList.Add(t);
                }

                if (typeList.Count == 0)
                    return;

                var registrationSource = BlazorRegistrationEmitter.Emit(typeList, ns);
                spc.AddSource("GeneratedBlazorRegistrations.g.cs", registrationSource);
            });
        }

        private static BlazorTypeInfo? TransformToBlazorTypeInfo(
            GeneratorSyntaxContext context,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var classDecl = (ClassDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(classDecl, cancellationToken);

            if (symbol is not INamedTypeSymbol namedSymbol)
                return null;

            // Skip abstract types and interfaces
            if (namedSymbol.IsAbstract || namedSymbol.TypeKind != TypeKind.Class)
                return null;

            // Skip non-public types
            if (namedSymbol.DeclaredAccessibility != Accessibility.Public)
                return null;

            // Skip open generic type definitions
            if (namedSymbol.IsGenericType)
            {
                var hasUnboundTypeParam = false;
                foreach (var typeArg in namedSymbol.TypeArguments)
                {
                    if (typeArg.TypeKind == TypeKind.TypeParameter)
                    {
                        hasUnboundTypeParam = true;
                        break;
                    }
                }

                if (hasUnboundTypeParam)
                    return null;
            }

            var concreteTypeName = namedSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var lifetime = ResolveLifetime(namedSymbol);

            // Check for IView implementation
            if (ImplementsInterface(namedSymbol, IViewFullName))
            {
                var abstraction = FindAbstractionInterface(namedSymbol, IViewFullName);
                return new BlazorTypeInfo(
                    ConcreteTypeName: concreteTypeName,
                    AbstractionTypeName: abstraction,
                    Kind: BlazorTypeKind.View,
                    Lifetime: lifetime);
            }

            // Check for IWindow implementation
            if (ImplementsInterface(namedSymbol, IWindowFullName))
            {
                var abstraction = FindAbstractionInterface(namedSymbol, IWindowFullName);
                return new BlazorTypeInfo(
                    ConcreteTypeName: concreteTypeName,
                    AbstractionTypeName: abstraction,
                    Kind: BlazorTypeKind.Window,
                    Lifetime: lifetime);
            }

            // Check for IViewModel implementation
            if (ImplementsInterface(namedSymbol, IViewModelFullName))
            {
                var abstraction = FindAbstractionInterface(namedSymbol, IViewModelFullName);
                return new BlazorTypeInfo(
                    ConcreteTypeName: concreteTypeName,
                    AbstractionTypeName: abstraction,
                    Kind: BlazorTypeKind.ViewModel,
                    Lifetime: lifetime);
            }

            return null;
        }

        /// <summary>
        /// Returns true when the type directly or indirectly implements the interface
        /// identified by <paramref name="baseInterfaceFullName"/>.
        /// </summary>
        private static bool ImplementsInterface(INamedTypeSymbol typeSymbol, string baseInterfaceFullName)
        {
            foreach (var iface in typeSymbol.AllInterfaces)
            {
                var name = iface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                               .Replace("global::", string.Empty);
                if (name == baseInterfaceFullName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Finds the first specific abstraction interface (not the base marker interface itself)
        /// that is derived from the given base interface.
        /// Mirrors the runtime logic in ReflectionExtensions.RegisterView/RegisterWindow/RegisterViewModel.
        /// </summary>
        private static string? FindAbstractionInterface(INamedTypeSymbol typeSymbol, string baseInterfaceFullName)
        {
            foreach (var iface in typeSymbol.AllInterfaces)
            {
                var ifaceName = iface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                                     .Replace("global::", string.Empty);

                if (ifaceName == baseInterfaceFullName)
                    continue;

                foreach (var parentIface in iface.AllInterfaces)
                {
                    var parentName = parentIface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                                                .Replace("global::", string.Empty);
                    if (parentName == baseInterfaceFullName)
                    {
                        return iface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Reads the <c>[Lifetime]</c> attribute from the type symbol to determine the DI lifetime.
        /// Defaults to <see cref="BlazorLifetime.Transient"/> when no attribute is present.
        /// </summary>
        private static BlazorLifetime ResolveLifetime(INamedTypeSymbol typeSymbol)
        {
            foreach (var attribute in typeSymbol.GetAttributes())
            {
                var attrClass = attribute.AttributeClass;
                if (attrClass is null)
                    continue;

                var attrFullName = attrClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                                            .Replace("global::", string.Empty);

                if (attrFullName != LifetimeAttributeFullName)
                    continue;

                if (attribute.ConstructorArguments.Length > 0)
                {
                    var lifetimeValue = attribute.ConstructorArguments[0].Value;
                    if (lifetimeValue is int intValue)
                    {
                        return intValue == LifetimesSingleton
                            ? BlazorLifetime.Singleton
                            : BlazorLifetime.Scoped;
                    }
                }
            }

            return BlazorLifetime.Transient;
        }
    }
}
