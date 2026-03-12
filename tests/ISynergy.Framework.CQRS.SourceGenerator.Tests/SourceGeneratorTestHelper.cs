using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ISynergy.Framework.CQRS.SourceGenerator.Tests;

/// <summary>
/// Runs <see cref="CqrsSourceGenerator"/> against a synthetic compilation built
/// from the supplied C# source strings and returns all files produced by the generator.
/// </summary>
internal static class SourceGeneratorTestHelper
{
    /// <summary>
    /// Compiles <paramref name="sources"/> with references to ISynergy.Framework.CQRS
    /// and returns all files produced by the source generator keyed by file name.
    /// </summary>
    /// <param name="sources">One or more C# source strings to compile.</param>
    /// <returns>
    /// A dictionary mapping generated file names (e.g. <c>CqrsHandlerRegistrations.g.cs</c>)
    /// to their text content.
    /// </returns>
    public static IReadOnlyDictionary<string, string> RunGenerator(params string[] sources)
    {
        var syntaxTrees = sources
            .Select(s => CSharpSyntaxTree.ParseText(s))
            .ToArray();

        // Collect references: all loaded assemblies + explicit CQRS + DI references
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();

        // Ensure CQRS assembly types are resolvable in the test compilation
        references.Add(MetadataReference.CreateFromFile(
            typeof(ISynergy.Framework.CQRS.Commands.ICommand).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(
            typeof(Microsoft.Extensions.DependencyInjection.IServiceCollection).Assembly.Location));

        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: syntaxTrees,
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new CqrsSourceGenerator();
        var driver = CSharpGeneratorDriver
            .Create(generator)
            .RunGenerators(compilation);

        var result = driver.GetRunResult();

        return result.GeneratedTrees
            .ToDictionary(
                t => Path.GetFileName(t.FilePath),
                t => t.GetText().ToString());
    }
}
