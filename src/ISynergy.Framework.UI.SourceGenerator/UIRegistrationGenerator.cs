using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ISynergy.Framework.UI.SourceGenerator
{
    /// <summary>
    /// Roslyn incremental source generator that emits AOT-safe UI type registrations
    /// for all assemblies that reference ISynergy.Framework.UI.
    /// Scans for types implementing <c>IView</c>, <c>IWindow</c>, and <c>IViewModel</c>
    /// and emits <c>AddUITypes(IServiceCollection)</c> and a compile-time view-name map.
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public sealed class UIRegistrationGenerator : IIncrementalGenerator
    {
        // Fully-qualified names of the base UI interfaces (without global::)
        private const string IViewFullName = "ISynergy.Framework.Mvvm.Abstractions.IView";
        private const string IWindowFullName = "ISynergy.Framework.Mvvm.Abstractions.IWindow";
        private const string IViewModelFullName = "ISynergy.Framework.Mvvm.Abstractions.ViewModels.IViewModel";

        private const string ViewModelSuffix = "ViewModel";
        private const string ViewSuffix = "View";

        /// <summary>
        /// Initializes the generator pipeline.
        /// </summary>
        /// <param name="context">The incremental generator initialization context.</param>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Find all class declarations that have a base list (they could implement an interface)
            var typeInfoProvider = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is ClassDeclarationSyntax { BaseList: not null },
                    transform: TransformToUITypeInfo)
                .Where(static info => info != null)
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                .Select(static (info, _) => info);
#pragma warning restore CS8619

            var allTypes = typeInfoProvider.Collect();

            var rootNamespace = context.CompilationProvider.Select(
                static (compilation, _) =>
                    compilation.AssemblyName?.Replace('-', '_') ?? "GeneratedUI");

            var combined = allTypes.Combine(rootNamespace);

            context.RegisterSourceOutput(combined, static (spc, data) =>
            {
                var (types, ns) = data;

                if (types.IsDefaultOrEmpty)
                    return;

                var typeList = new List<UITypeInfo>();
                foreach (var t in types)
                {
                    if (t != null)
                        typeList.Add(t);
                }

                if (typeList.Count == 0)
                    return;

                // Emit the service registration extension method
                var registrationSource = UIRegistrationEmitter.EmitRegistrations(typeList, ns);
                spc.AddSource("UIGeneratedRegistrations.g.cs", registrationSource);

                // Emit the ViewModel -> View name map
                var viewModelTypes = new List<UITypeInfo>();
                foreach (var t in typeList)
                {
                    if (t.Kind == UITypeKind.ViewModel && t.RelatedViewName != null)
                        viewModelTypes.Add(t);
                }

                if (viewModelTypes.Count > 0)
                {
                    var mapSource = UIRegistrationEmitter.EmitViewModelViewMap(viewModelTypes, ns);
                    spc.AddSource("ViewModelViewMap.g.cs", mapSource);
                }
            });
        }

        private static UITypeInfo? TransformToUITypeInfo(
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

            // Check for IView implementation
            var viewAbstraction = FindAbstractionInterface(namedSymbol, IViewFullName);
            if (viewAbstraction != null || ImplementsInterface(namedSymbol, IViewFullName))
            {
                return new UITypeInfo(
                    ConcreteTypeName: concreteTypeName,
                    AbstractionTypeName: viewAbstraction,
                    Kind: UITypeKind.View,
                    RelatedViewName: null);
            }

            // Check for IWindow implementation
            var windowAbstraction = FindAbstractionInterface(namedSymbol, IWindowFullName);
            if (windowAbstraction != null || ImplementsInterface(namedSymbol, IWindowFullName))
            {
                return new UITypeInfo(
                    ConcreteTypeName: concreteTypeName,
                    AbstractionTypeName: windowAbstraction,
                    Kind: UITypeKind.Window,
                    RelatedViewName: null);
            }

            // Check for IViewModel implementation
            if (ImplementsInterface(namedSymbol, IViewModelFullName))
            {
                var viewModelAbstraction = FindAbstractionInterface(namedSymbol, IViewModelFullName);
                var relatedViewName = GetRelatedViewName(namedSymbol);
                return new UITypeInfo(
                    ConcreteTypeName: concreteTypeName,
                    AbstractionTypeName: viewModelAbstraction,
                    Kind: UITypeKind.ViewModel,
                    RelatedViewName: relatedViewName);
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
        /// that is derived from the given base interface full name.
        /// Mirrors the runtime logic in ReflectionExtensions.RegisterView/RegisterWindow/RegisterViewModel.
        /// </summary>
        private static string? FindAbstractionInterface(INamedTypeSymbol typeSymbol, string baseInterfaceFullName)
        {
            foreach (var iface in typeSymbol.AllInterfaces)
            {
                var ifaceName = iface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                                     .Replace("global::", string.Empty);

                // Skip the base interface itself
                if (ifaceName == baseInterfaceFullName)
                    continue;

                // Check whether this interface itself implements the base interface
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
        /// Computes the expected view type name for a ViewModel type by replacing the
        /// "ViewModel" suffix with "View" — mirroring GetRelatedView() in ViewModelExtensions.
        /// </summary>
        private static string? GetRelatedViewName(INamedTypeSymbol viewModelSymbol)
        {
            var name = viewModelSymbol.Name;

            if (viewModelSymbol.IsGenericType)
            {
                var backtickIndex = name.IndexOf('`');
                if (backtickIndex >= 0)
                    name = name.Substring(0, backtickIndex);
            }

            if (!name.EndsWith(ViewModelSuffix))
                return null;

            return name.Substring(0, name.Length - ViewModelSuffix.Length) + ViewSuffix;
        }
    }
}
