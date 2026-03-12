# CQRS AOT Source Generator Implementation Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make `ISynergy.Framework.CQRS` fully AOT-compatible by replacing reflection-based dispatch and assembly scanning with a Roslyn incremental source generator that emits compile-time handler registrations and a type-safe query dispatch table.

**Architecture:** A new `ISynergy.Framework.CQRS.SourceGenerator` project (targeting `netstandard2.0`) is bundled as a Roslyn analyzer inside the existing CQRS NuGet. It scans the consuming assembly's compilation for `ICommandHandler<>` and `IQueryHandler<>` implementations and generates two extension methods: `AddCQRSHandlers()` (replaces reflection assembly scanning) and `AddQueryDispatchTable()` (replaces `MakeGenericType`+`dynamic` in `QueryDispatcher`). The CQRS runtime library gains a `QueryDispatchTable` class and an AOT-safe doubly-generic `IQueryDispatcher.DispatchAsync<TQuery, TResult>` overload; the existing single-type-arg overload is preserved but annotated with trimmer attributes for non-AOT scenarios.

**Tech Stack:** .NET 10 / C# 14, Roslyn Incremental Source Generators (`IIncrementalGenerator`), `Microsoft.CodeAnalysis.CSharp` 4.x, MSTest + `Microsoft.CodeAnalysis.CSharp.SourceGenerators.Testing` for generator tests, central package management via `Directory.Packages.props`.

---

## File Map

### New files (source generator project)
| File | Responsibility |
|---|---|
| `src/ISynergy.Framework.CQRS.SourceGenerator/ISynergy.Framework.CQRS.SourceGenerator.csproj` | Generator project (netstandard2.0, no implicit usings) |
| `src/ISynergy.Framework.CQRS.SourceGenerator/CqrsSourceGenerator.cs` | `IIncrementalGenerator` entry point — wires pipeline |
| `src/ISynergy.Framework.CQRS.SourceGenerator/Parsers/HandlerInfo.cs` | Equatable value-type record holding parsed handler data |
| `src/ISynergy.Framework.CQRS.SourceGenerator/Parsers/HandlerParser.cs` | Roslyn syntax/symbol logic to detect handler types |
| `src/ISynergy.Framework.CQRS.SourceGenerator/Emitters/HandlerRegistrationEmitter.cs` | Emits `AddCQRSHandlers()` source text |
| `src/ISynergy.Framework.CQRS.SourceGenerator/Emitters/QueryDispatchTableEmitter.cs` | Emits `AddQueryDispatchTable()` source text |

### New files (CQRS runtime)
| File | Responsibility |
|---|---|
| `src/ISynergy.Framework.CQRS/Dispatch/QueryDispatchTable.cs` | Dictionary of pre-compiled `Func<>` dispatchers, registered generically at startup |

### New files (tests)
| File | Responsibility |
|---|---|
| `tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/ISynergy.Framework.CQRS.SourceGenerator.Tests.csproj` | Source generator test project |
| `tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/HandlerRegistrationGeneratorTests.cs` | Verifies generated handler registration code |
| `tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/QueryDispatchTableGeneratorTests.cs` | Verifies generated dispatch table code |
| `tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/SourceGeneratorTestHelper.cs` | Shared helper to run generator in-process |

### Modified files
| File | Change |
|---|---|
| `src/ISynergy.Framework.CQRS/Abstractions/Dispatchers/IQueryDispatcher.cs` | Add doubly-generic `DispatchAsync<TQuery, TResult>` overload |
| `src/ISynergy.Framework.CQRS/Dispatchers/QueryDispatcher.cs` | Use `QueryDispatchTable` for single-arg; add doubly-generic impl; annotate reflection path |
| `src/ISynergy.Framework.CQRS/Extensions/ServiceCollectionExtensions.cs` | Annotate `AddHandlers()` with `[RequiresUnreferencedCode]`; register `QueryDispatchTable` in `AddCQRS()` |
| `src/ISynergy.Framework.CQRS/ISynergy.Framework.CQRS.csproj` | Reference generator project as Analyzer |
| `Directory.Packages.props` | Add `Microsoft.CodeAnalysis.CSharp` version entry |
| `tests/ISynergy.Framework.CQRS.Tests/Dispatchers/QueryDispatcherTests.cs` | Add tests for doubly-generic overload + dispatch table path |
| `tests/ISynergy.Framework.CQRS.Tests/Extensions/ServiceCollectionExtensionsTests.cs` | Verify `AddHandlers()` trimmer attribute; update integration test |

---

## Chunk 1: Runtime Library — AOT-Safe Dispatching

### Task 1: Add doubly-generic `IQueryDispatcher` overload

**Files:**
- Modify: `src/ISynergy.Framework.CQRS/Abstractions/Dispatchers/IQueryDispatcher.cs`

- [ ] **Step 1: Read the current interface**

  ```
  src/ISynergy.Framework.CQRS/Abstractions/Dispatchers/IQueryDispatcher.cs
  ```

- [ ] **Step 2: Add the doubly-generic overload**

  Replace the file content with:

  ```csharp
  using ISynergy.Framework.CQRS.Queries;

  namespace ISynergy.Framework.CQRS.Abstractions.Dispatchers;

  /// <summary>
  /// Query dispatcher interface.
  /// </summary>
  public interface IQueryDispatcher
  {
      /// <summary>
      /// Dispatches a query. Requires a <see cref="ISynergy.Framework.CQRS.Dispatch.QueryDispatchTable"/>
      /// registered in DI for AOT compatibility; falls back to reflection when the table is absent.
      /// </summary>
      /// <typeparam name="TResult">Type of result.</typeparam>
      /// <param name="query">Query to dispatch.</param>
      /// <param name="cancellationToken">Cancellation token.</param>
      /// <returns>Query result.</returns>
      Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

      /// <summary>
      /// Dispatches a query using statically-known types. AOT-safe — no reflection required.
      /// Prefer this overload in AOT-published applications.
      /// </summary>
      /// <typeparam name="TQuery">Concrete type of query.</typeparam>
      /// <typeparam name="TResult">Type of result.</typeparam>
      /// <param name="query">Query to dispatch.</param>
      /// <param name="cancellationToken">Cancellation token.</param>
      /// <returns>Query result.</returns>
      Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
          where TQuery : IQuery<TResult>;
  }
  ```

- [ ] **Step 3: Build to confirm the interface change compiles**

  ```powershell
  dotnet build src/ISynergy.Framework.CQRS/ISynergy.Framework.CQRS.csproj
  ```

  Expected: build error in `QueryDispatcher.cs` — "does not implement interface member `DispatchAsync<TQuery, TResult>`". This is expected; Task 3 fixes it.

---

### Task 2: Create `QueryDispatchTable`

**Files:**
- Create: `src/ISynergy.Framework.CQRS/Dispatch/QueryDispatchTable.cs`

- [ ] **Step 1: Create the directory and file**

  ```csharp
  // src/ISynergy.Framework.CQRS/Dispatch/QueryDispatchTable.cs
  using ISynergy.Framework.CQRS.Queries;
  using Microsoft.Extensions.DependencyInjection;

  namespace ISynergy.Framework.CQRS.Dispatch;

  /// <summary>
  /// A compile-time-populated dispatch table that maps concrete query types to
  /// pre-compiled, AOT-safe handler invocations. Populate via the source-generated
  /// <c>AddQueryDispatchTable()</c> extension method.
  /// </summary>
  public sealed class QueryDispatchTable
  {
      private readonly Dictionary<Type, Func<IServiceProvider, object, CancellationToken, Task<object?>>> _dispatchers
          = new();

      /// <summary>
      /// Registers an AOT-safe dispatcher for <typeparamref name="TQuery"/> → <typeparamref name="TResult"/>.
      /// Call this from generated startup code only.
      /// </summary>
      /// <typeparam name="TQuery">Concrete query type.</typeparam>
      /// <typeparam name="TResult">Result type.</typeparam>
      public void Register<TQuery, TResult>()
          where TQuery : IQuery<TResult>
      {
          _dispatchers[typeof(TQuery)] = async (sp, query, ct) =>
          {
              var handler = sp.GetRequiredService<IQueryHandler<TQuery, TResult>>();
              return (object?)await handler.HandleAsync((TQuery)query, ct).ConfigureAwait(false);
          };
      }

      /// <summary>
      /// Attempts to retrieve a pre-compiled dispatcher for the given query type.
      /// </summary>
      /// <param name="queryType">Runtime type of the query.</param>
      /// <param name="dispatcher">The dispatcher delegate if found.</param>
      /// <returns><c>true</c> if a dispatcher was registered for <paramref name="queryType"/>.</returns>
      public bool TryGetDispatcher(
          Type queryType,
          out Func<IServiceProvider, object, CancellationToken, Task<object?>> dispatcher)
          => _dispatchers.TryGetValue(queryType, out dispatcher!);
  }
  ```

- [ ] **Step 2: Build to verify the new class compiles**

  ```powershell
  dotnet build src/ISynergy.Framework.CQRS/ISynergy.Framework.CQRS.csproj
  ```

  Expected: same error as before (QueryDispatcher still missing the new interface member).

---

### Task 3: Update `QueryDispatcher` — implement new overload, use dispatch table

**Files:**
- Modify: `src/ISynergy.Framework.CQRS/Dispatchers/QueryDispatcher.cs`

- [ ] **Step 1: Write the updated dispatcher**

  Replace file content:

  ```csharp
  using System.Diagnostics.CodeAnalysis;
  using ISynergy.Framework.CQRS.Abstractions.Dispatchers;
  using ISynergy.Framework.CQRS.Dispatch;
  using ISynergy.Framework.CQRS.Queries;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Logging;

  namespace ISynergy.Framework.CQRS.Dispatchers;

  /// <summary>
  /// Default query dispatcher implementation.
  /// Prefers the AOT-safe <see cref="QueryDispatchTable"/> when registered;
  /// falls back to reflection-based dispatch for non-AOT scenarios.
  /// </summary>
  public class QueryDispatcher : IQueryDispatcher
  {
      private readonly IServiceProvider _serviceProvider;
      private readonly ILogger? _logger;
      private readonly QueryDispatchTable? _dispatchTable;

      /// <summary>
      /// Initializes a new instance of <see cref="QueryDispatcher"/>.
      /// </summary>
      /// <param name="serviceProvider">The service provider.</param>
      /// <param name="logger">Logger instance.</param>
      /// <param name="dispatchTable">
      /// Optional AOT-safe dispatch table. When provided, reflection-based dispatch is bypassed.
      /// Populated by the source-generated <c>AddQueryDispatchTable()</c> call.
      /// </param>
      public QueryDispatcher(
          IServiceProvider serviceProvider,
          ILogger<QueryDispatcher> logger,
          QueryDispatchTable? dispatchTable = null)
      {
          _serviceProvider = serviceProvider;
          _logger = logger;
          _dispatchTable = dispatchTable;
      }

      /// <inheritdoc/>
      public async Task<TResult> DispatchAsync<TResult>(
          IQuery<TResult> query,
          CancellationToken cancellationToken = default)
      {
          ArgumentNullException.ThrowIfNull(query);

          try
          {
              // AOT-safe path: use pre-compiled dispatch table when available.
              if (_dispatchTable is not null &&
                  _dispatchTable.TryGetDispatcher(query.GetType(), out var dispatch))
              {
                  return (TResult)(await dispatch(_serviceProvider, query, cancellationToken).ConfigureAwait(false))!;
              }

              // Reflection fallback — not AOT-safe. Triggers trimmer warnings when table is absent.
              return await DispatchViaReflection<TResult>(query, cancellationToken).ConfigureAwait(false);
          }
          catch (Exception ex) when (ex is not OperationCanceledException)
          {
              _logger?.LogError(ex, "Error dispatching query of type {QueryType}", query.GetType().Name);
              throw;
          }
      }

      /// <inheritdoc/>
      public async Task<TResult> DispatchAsync<TQuery, TResult>(
          TQuery query,
          CancellationToken cancellationToken = default)
          where TQuery : IQuery<TResult>
      {
          ArgumentNullException.ThrowIfNull(query);

          try
          {
              var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
              return await handler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
          }
          catch (Exception ex) when (ex is not OperationCanceledException)
          {
              _logger?.LogError(ex, "Error dispatching query of type {QueryType}", typeof(TQuery).Name);
              throw;
          }
      }

      [RequiresDynamicCode("Reflection-based query dispatch is not AOT-compatible. Register a QueryDispatchTable via AddQueryDispatchTable() or use DispatchAsync<TQuery, TResult> instead.")]
      [RequiresUnreferencedCode("Reflection-based query dispatch may not work after trimming. Register a QueryDispatchTable via AddQueryDispatchTable() or use DispatchAsync<TQuery, TResult> instead.")]
      private async Task<TResult> DispatchViaReflection<TResult>(
          IQuery<TResult> query,
          CancellationToken cancellationToken)
      {
          var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
          dynamic handler = _serviceProvider.GetRequiredService(handlerType);
          return await handler.HandleAsync((dynamic)query, cancellationToken);
      }
  }
  ```

- [ ] **Step 2: Build — should now succeed**

  ```powershell
  dotnet build src/ISynergy.Framework.CQRS/ISynergy.Framework.CQRS.csproj
  ```

  Expected: **0 errors, 0 warnings**.

---

### Task 4: Update `ServiceCollectionExtensions` — register `QueryDispatchTable`, annotate `AddHandlers`

**Files:**
- Modify: `src/ISynergy.Framework.CQRS/Extensions/ServiceCollectionExtensions.cs`

- [ ] **Step 1: Add trimmer attribute to `AddHandlers` and register `QueryDispatchTable` in `AddCQRS`**

  Apply two targeted edits:

  **Edit A** — Add `using` and `QueryDispatchTable` registration inside `AddCQRS()`:

  ```csharp
  // At top of file, add:
  using ISynergy.Framework.CQRS.Dispatch;
  using System.Diagnostics.CodeAnalysis;
  ```

  Inside `AddCQRS()`, after registering dispatchers:
  ```csharp
  // Register optional AOT-safe dispatch table (populated by source-generated startup code).
  services.TryAddSingleton<QueryDispatchTable>();
  ```

  **Edit B** — Annotate `AddHandlers()`:
  ```csharp
  [RequiresUnreferencedCode("Assembly scanning for CQRS handlers uses reflection and is not AOT-compatible. Use the source-generated AddCQRSHandlers() extension method instead.")]
  public static IServiceCollection AddHandlers(this IServiceCollection services, params Assembly[] assemblies)
  ```

- [ ] **Step 2: Build**

  ```powershell
  dotnet build src/ISynergy.Framework.CQRS/ISynergy.Framework.CQRS.csproj
  ```

  Expected: **0 errors**.

---

### Task 5: Update existing tests to cover new APIs

**Files:**
- Modify: `tests/ISynergy.Framework.CQRS.Tests/Dispatchers/QueryDispatcherTests.cs`

- [ ] **Step 1: Add test for doubly-generic overload**

  Add the following test method to `QueryDispatcherTests`:

  ```csharp
  [TestMethod]
  public async Task QueryDispatcher_DoublyGenericOverload_ReturnsExpectedResult()
  {
      // Arrange
      var services = new ServiceCollection();
      services.AddScoped<IQueryHandler<TestQuery, string>>(_ => new TestQueryHandler());
      var provider = services.BuildServiceProvider();
      var dispatcher = new QueryDispatcher(provider, Mock.Of<ILogger<QueryDispatcher>>());
      var query = new TestQuery { Parameter = "Generic Test" };

      // Act
      var result = await dispatcher.DispatchAsync<TestQuery, string>(query);

      // Assert
      Assert.AreEqual("Query Result: Generic Test", result);
  }

  [TestMethod]
  public async Task QueryDispatcher_WithDispatchTable_UsesTablePath()
  {
      // Arrange
      var services = new ServiceCollection();
      services.AddScoped<IQueryHandler<TestQuery, string>>(_ => new TestQueryHandler());
      var provider = services.BuildServiceProvider();

      var table = new QueryDispatchTable();
      table.Register<TestQuery, string>();

      var dispatcher = new QueryDispatcher(provider, Mock.Of<ILogger<QueryDispatcher>>(), table);
      var query = new TestQuery { Parameter = "Table Path" };

      // Act
      var result = await dispatcher.DispatchAsync<string>(query);

      // Assert
      Assert.AreEqual("Query Result: Table Path", result);
  }
  ```

- [ ] **Step 2: Run the CQRS test suite**

  ```powershell
  dotnet test tests/ISynergy.Framework.CQRS.Tests/ISynergy.Framework.CQRS.Tests.csproj --logger "console;verbosity=normal"
  ```

  Expected: all existing tests pass + 2 new tests pass.

- [ ] **Step 3: Commit Chunk 1**

  ```powershell
  git add src/ISynergy.Framework.CQRS/ tests/ISynergy.Framework.CQRS.Tests/
  git commit -m "feat(cqrs): add AOT-safe QueryDispatchTable and doubly-generic IQueryDispatcher overload"
  ```

---

## Chunk 2: Source Generator Project

### Task 6: Create the source generator project

**Files:**
- Create: `src/ISynergy.Framework.CQRS.SourceGenerator/ISynergy.Framework.CQRS.SourceGenerator.csproj`

> **Critical**: Source generators must target `netstandard2.0`. They cannot reference `net10.0` TFMs. Implicit usings must be disabled.

- [ ] **Step 1: Create the project file**

  ```xml
  <!-- src/ISynergy.Framework.CQRS.SourceGenerator/ISynergy.Framework.CQRS.SourceGenerator.csproj -->
  <Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
      <TargetFramework>netstandard2.0</TargetFramework>
      <LangVersion>12</LangVersion>
      <ImplicitUsings>disable</ImplicitUsings>
      <Nullable>enable</Nullable>
      <GenerateDocumentationFile>true</GenerateDocumentationFile>
      <PackageId>I-Synergy.Framework.CQRS.SourceGenerator</PackageId>
      <Description>Roslyn source generator for I-Synergy CQRS AOT-compatible handler registration</Description>
      <PackageTags>I-Synergy, Framework, CQRS, SourceGenerator, AOT</PackageTags>
      <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
      <!-- Do not generate a library layout; this ships as an analyzer -->
      <GenerateLibraryLayout>false</GenerateLibraryLayout>
      <!-- Suppress signing for the generator (analyzers cannot be strong-named easily) -->
      <SignAssembly>false</SignAssembly>
    </PropertyGroup>

    <ItemGroup>
      <PackageReference Include="Microsoft.CodeAnalysis.CSharp" PrivateAssets="all" />
      <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" PrivateAssets="all" />
    </ItemGroup>
  </Project>
  ```

- [ ] **Step 2: Add package versions to `Directory.Packages.props`**

  Inside the `<ItemGroup>` in `Directory.Packages.props`, add:
  ```xml
  <PackageVersion Include="Microsoft.CodeAnalysis.CSharp" Version="4.14.0" />
  <PackageVersion Include="Microsoft.CodeAnalysis.Analyzers" Version="3.11.0" />
  <PackageVersion Include="Microsoft.CodeAnalysis.CSharp.SourceGenerators.Testing" Version="1.1.2" />
  ```

- [ ] **Step 3: Build the (empty) generator project**

  ```powershell
  dotnet build src/ISynergy.Framework.CQRS.SourceGenerator/ISynergy.Framework.CQRS.SourceGenerator.csproj
  ```

  Expected: **0 errors** (empty project).

---

### Task 7: Implement `HandlerInfo` — the data model

**Files:**
- Create: `src/ISynergy.Framework.CQRS.SourceGenerator/Parsers/HandlerInfo.cs`

> `HandlerInfo` must be a fully equatable value type (record struct or sealed record). Incremental generators cache pipeline steps by equality — a non-equatable model breaks caching and causes full regeneration on every keystroke.

- [ ] **Step 1: Create the file**

  ```csharp
  // src/ISynergy.Framework.CQRS.SourceGenerator/Parsers/HandlerInfo.cs
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
  internal sealed record HandlerInfo(
      /// <summary>Fully-qualified name of the handler implementation class.</summary>
      string HandlerTypeName,

      /// <summary>Fully-qualified name of the service interface to register against.</summary>
      string ServiceInterfaceName,

      /// <summary>Fully-qualified name of the first type argument (TCommand or TQuery).</summary>
      string FirstTypeArg,

      /// <summary>
      /// Fully-qualified name of the second type argument (TResult), or <c>null</c>
      /// for <see cref="HandlerKind.Command"/> (no return value).
      /// </summary>
      string? SecondTypeArg,

      /// <summary>The kind of handler.</summary>
      HandlerKind Kind);
  ```

- [ ] **Step 2: Build**

  ```powershell
  dotnet build src/ISynergy.Framework.CQRS.SourceGenerator/ISynergy.Framework.CQRS.SourceGenerator.csproj
  ```

  Expected: **0 errors**.

---

### Task 8: Implement `HandlerParser` — Roslyn symbol analysis

**Files:**
- Create: `src/ISynergy.Framework.CQRS.SourceGenerator/Parsers/HandlerParser.cs`

- [ ] **Step 1: Create the parser**

  ```csharp
  // src/ISynergy.Framework.CQRS.SourceGenerator/Parsers/HandlerParser.cs
  using System.Collections.Generic;
  using System.Linq;
  using Microsoft.CodeAnalysis;

  namespace ISynergy.Framework.CQRS.SourceGenerator.Parsers;

  internal static class HandlerParser
  {
      private const string CommandHandlerOpen     = "ISynergy.Framework.CQRS.Commands.ICommandHandler`1";
      private const string CommandHandlerWithResult = "ISynergy.Framework.CQRS.Abstractions.Commands.ICommandHandler`2";
      private const string QueryHandlerOpen       = "ISynergy.Framework.CQRS.Queries.IQueryHandler`2";

      /// <summary>
      /// Inspects <paramref name="typeSymbol"/> and returns all <see cref="HandlerInfo"/> records
      /// for each CQRS handler interface it implements. Returns an empty enumerable when the
      /// type implements no handler interfaces.
      /// </summary>
      public static IEnumerable<HandlerInfo> GetHandlerInfos(INamedTypeSymbol typeSymbol)
      {
          if (typeSymbol.IsAbstract || typeSymbol.TypeKind != TypeKind.Class)
              yield break;

          foreach (var iface in typeSymbol.AllInterfaces)
          {
              if (!iface.IsGenericType)
                  continue;

              var originalDef = iface.OriginalDefinition.ToDisplayString(
                  SymbolDisplayFormat.FullyQualifiedFormat);

              // Strip "global::" prefix that FullyQualifiedFormat adds
              originalDef = originalDef.Replace("global::", string.Empty);

              var typeArgs = iface.TypeArguments;

              if (originalDef == CommandHandlerOpen && typeArgs.Length == 1)
              {
                  var tCommand = typeArgs[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                  var serviceName = $"ISynergy.Framework.CQRS.Commands.ICommandHandler<{tCommand}>";
                  yield return new HandlerInfo(
                      HandlerTypeName: typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                      ServiceInterfaceName: serviceName,
                      FirstTypeArg: tCommand,
                      SecondTypeArg: null,
                      Kind: HandlerKind.Command);
              }
              else if (originalDef == CommandHandlerWithResult && typeArgs.Length == 2)
              {
                  var tCommand = typeArgs[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                  var tResult  = typeArgs[1].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                  var serviceName = $"ISynergy.Framework.CQRS.Abstractions.Commands.ICommandHandler<{tCommand}, {tResult}>";
                  yield return new HandlerInfo(
                      HandlerTypeName: typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                      ServiceInterfaceName: serviceName,
                      FirstTypeArg: tCommand,
                      SecondTypeArg: tResult,
                      Kind: HandlerKind.CommandWithResult);
              }
              else if (originalDef == QueryHandlerOpen && typeArgs.Length == 2)
              {
                  var tQuery  = typeArgs[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                  var tResult = typeArgs[1].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                  var serviceName = $"ISynergy.Framework.CQRS.Queries.IQueryHandler<{tQuery}, {tResult}>";
                  yield return new HandlerInfo(
                      HandlerTypeName: typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                      ServiceInterfaceName: serviceName,
                      FirstTypeArg: tQuery,
                      SecondTypeArg: tResult,
                      Kind: HandlerKind.Query);
              }
          }
      }
  }
  ```

- [ ] **Step 2: Build**

  ```powershell
  dotnet build src/ISynergy.Framework.CQRS.SourceGenerator/ISynergy.Framework.CQRS.SourceGenerator.csproj
  ```

  Expected: **0 errors**.

---

### Task 9: Implement `HandlerRegistrationEmitter`

**Files:**
- Create: `src/ISynergy.Framework.CQRS.SourceGenerator/Emitters/HandlerRegistrationEmitter.cs`

- [ ] **Step 1: Create the emitter**

  ```csharp
  // src/ISynergy.Framework.CQRS.SourceGenerator/Emitters/HandlerRegistrationEmitter.cs
  using System.Collections.Generic;
  using System.Text;
  using ISynergy.Framework.CQRS.SourceGenerator.Parsers;

  namespace ISynergy.Framework.CQRS.SourceGenerator.Emitters;

  internal static class HandlerRegistrationEmitter
  {
      /// <summary>
      /// Emits a static partial class with an <c>AddCQRSHandlers</c> extension method
      /// that registers all discovered handlers into <c>IServiceCollection</c>.
      /// </summary>
      /// <param name="handlers">All discovered handlers across the compilation.</param>
      /// <param name="rootNamespace">The assembly's root namespace (used as the generated class namespace).</param>
      /// <returns>C# source text ready to add to the compilation.</returns>
      public static string Emit(IReadOnlyList<HandlerInfo> handlers, string rootNamespace)
      {
          var sb = new StringBuilder();
          sb.AppendLine("// <auto-generated/>");
          sb.AppendLine("#nullable enable");
          sb.AppendLine();
          sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
          sb.AppendLine();
          sb.AppendLine($"namespace {rootNamespace};");
          sb.AppendLine();
          sb.AppendLine("[global::System.CodeDom.Compiler.GeneratedCode(\"ISynergy.Framework.CQRS.SourceGenerator\", \"1.0.0\")]");
          sb.AppendLine("public static partial class CqrsHandlerRegistrations");
          sb.AppendLine("{");
          sb.AppendLine("    /// <summary>");
          sb.AppendLine("    /// Registers all CQRS command and query handlers discovered at compile time.");
          sb.AppendLine("    /// AOT-safe: no reflection or assembly scanning.");
          sb.AppendLine("    /// </summary>");
          sb.AppendLine("    public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddCQRSHandlers(");
          sb.AppendLine("        this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)");
          sb.AppendLine("    {");

          foreach (var h in handlers)
          {
              sb.AppendLine($"        services.AddScoped<global::{h.ServiceInterfaceName}, global::{h.HandlerTypeName}>();");
          }

          sb.AppendLine("        return services;");
          sb.AppendLine("    }");
          sb.AppendLine("}");

          return sb.ToString();
      }
  }
  ```

- [ ] **Step 2: Build**

  ```powershell
  dotnet build src/ISynergy.Framework.CQRS.SourceGenerator/ISynergy.Framework.CQRS.SourceGenerator.csproj
  ```

  Expected: **0 errors**.

---

### Task 10: Implement `QueryDispatchTableEmitter`

**Files:**
- Create: `src/ISynergy.Framework.CQRS.SourceGenerator/Emitters/QueryDispatchTableEmitter.cs`

- [ ] **Step 1: Create the emitter**

  ```csharp
  // src/ISynergy.Framework.CQRS.SourceGenerator/Emitters/QueryDispatchTableEmitter.cs
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using ISynergy.Framework.CQRS.SourceGenerator.Parsers;

  namespace ISynergy.Framework.CQRS.SourceGenerator.Emitters;

  internal static class QueryDispatchTableEmitter
  {
      /// <summary>
      /// Emits a static partial class with an <c>AddQueryDispatchTable</c> extension method
      /// that registers a <see cref="ISynergy.Framework.CQRS.Dispatch.QueryDispatchTable"/> into DI,
      /// pre-populated with all discovered query handler mappings.
      /// </summary>
      /// <param name="queryHandlers">Only the query handlers from all discovered handlers.</param>
      /// <param name="rootNamespace">The assembly's root namespace.</param>
      /// <returns>C# source text ready to add to the compilation.</returns>
      public static string Emit(IReadOnlyList<HandlerInfo> queryHandlers, string rootNamespace)
      {
          var sb = new StringBuilder();
          sb.AppendLine("// <auto-generated/>");
          sb.AppendLine("#nullable enable");
          sb.AppendLine();
          sb.AppendLine("using ISynergy.Framework.CQRS.Dispatch;");
          sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
          sb.AppendLine("using Microsoft.Extensions.DependencyInjection.Extensions;");
          sb.AppendLine();
          sb.AppendLine($"namespace {rootNamespace};");
          sb.AppendLine();
          sb.AppendLine("[global::System.CodeDom.Compiler.GeneratedCode(\"ISynergy.Framework.CQRS.SourceGenerator\", \"1.0.0\")]");
          sb.AppendLine("public static partial class CqrsHandlerRegistrations");
          sb.AppendLine("{");
          sb.AppendLine("    /// <summary>");
          sb.AppendLine("    /// Registers an AOT-safe <see cref=\"global::ISynergy.Framework.CQRS.Dispatch.QueryDispatchTable\"/>");
          sb.AppendLine("    /// pre-populated with all compile-time-discovered query handlers.");
          sb.AppendLine("    /// Call this alongside <see cref=\"AddCQRSHandlers\"/> in application startup.");
          sb.AppendLine("    /// </summary>");
          sb.AppendLine("    public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddQueryDispatchTable(");
          sb.AppendLine("        this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)");
          sb.AppendLine("    {");
          sb.AppendLine("        var table = new global::ISynergy.Framework.CQRS.Dispatch.QueryDispatchTable();");

          foreach (var h in queryHandlers)
          {
              sb.AppendLine($"        table.Register<global::{h.FirstTypeArg}, global::{h.SecondTypeArg}>();");
          }

          sb.AppendLine("        services.TryAddSingleton(table);");
          sb.AppendLine("        return services;");
          sb.AppendLine("    }");
          sb.AppendLine("}");

          return sb.ToString();
      }
  }
  ```

- [ ] **Step 2: Build**

  ```powershell
  dotnet build src/ISynergy.Framework.CQRS.SourceGenerator/ISynergy.Framework.CQRS.SourceGenerator.csproj
  ```

  Expected: **0 errors**.

---

### Task 11: Implement `CqrsSourceGenerator` — the incremental pipeline

**Files:**
- Create: `src/ISynergy.Framework.CQRS.SourceGenerator/CqrsSourceGenerator.cs`

> **Key incremental generator rules:**
> - Use `ForAttributeWithMetadataName` or `SyntaxProvider.CreateSyntaxProvider` — never `RegisterForSyntaxNotification` (deprecated).
> - All transforms must be pure (no side effects, no `ISymbol` stored past the transform step — convert to `HandlerInfo` value types immediately).
> - Use `.Collect()` to aggregate and `.RegisterSourceOutput` to emit.

- [ ] **Step 1: Create the generator**

  ```csharp
  // src/ISynergy.Framework.CQRS.SourceGenerator/CqrsSourceGenerator.cs
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
  ```

- [ ] **Step 2: Build the generator project**

  ```powershell
  dotnet build src/ISynergy.Framework.CQRS.SourceGenerator/ISynergy.Framework.CQRS.SourceGenerator.csproj
  ```

  Expected: **0 errors, 0 warnings**.

- [ ] **Step 3: Commit Chunk 2 (generator project)**

  ```powershell
  git add src/ISynergy.Framework.CQRS.SourceGenerator/ Directory.Packages.props
  git commit -m "feat(cqrs): add Roslyn incremental source generator for AOT handler registration and query dispatch table"
  ```

---

## Chunk 3: Wire Generator into the CQRS Library NuGet

### Task 12: Bundle generator as an analyzer in the CQRS project

**Files:**
- Modify: `src/ISynergy.Framework.CQRS/ISynergy.Framework.CQRS.csproj`

> When a source generator is bundled inside a library NuGet, it must be placed in the `analyzers/dotnet/cs/` directory of the package. The `.csproj` reference uses `OutputItemType="Analyzer"` and `ReferenceOutputAssembly="false"` so the runtime library does not take a compile-time dependency on the generator DLL.

- [ ] **Step 1: Update the CQRS project file**

  Add to `src/ISynergy.Framework.CQRS/ISynergy.Framework.CQRS.csproj`:

  ```xml
  <ItemGroup>
    <!-- Bundle the source generator as a Roslyn analyzer inside this NuGet package.
         Consumers of ISynergy.Framework.CQRS automatically get the generator. -->
    <ProjectReference
      Include="..\ISynergy.Framework.CQRS.SourceGenerator\ISynergy.Framework.CQRS.SourceGenerator.csproj"
      OutputItemType="Analyzer"
      ReferenceOutputAssembly="false" />
  </ItemGroup>

  <!-- Ensure the generator DLL lands in the correct NuGet folder -->
  <Target Name="CopyGeneratorToAnalyzers" BeforeTargets="Pack">
    <ItemGroup>
      <None Include="$(OutputPath)ISynergy.Framework.CQRS.SourceGenerator.dll"
            Pack="true"
            PackagePath="analyzers/dotnet/cs/"
            Visible="false" />
    </ItemGroup>
  </Target>
  ```

- [ ] **Step 2: Build the full CQRS library (generator included)**

  ```powershell
  dotnet build src/ISynergy.Framework.CQRS/ISynergy.Framework.CQRS.csproj
  ```

  Expected: **0 errors**. Generator is compiled alongside the library.

- [ ] **Step 3: Verify generator triggers on the test project**

  ```powershell
  dotnet build tests/ISynergy.Framework.CQRS.Tests/ISynergy.Framework.CQRS.Tests.csproj
  ```

  Expected: **0 errors**. Inspect `tests/ISynergy.Framework.CQRS.Tests/obj/Debug/net10.0/generated/` — two files should appear:
  - `ISynergy.Framework.CQRS.SourceGenerator/ISynergy.Framework.CQRS.SourceGenerator.CqrsSourceGenerator/CqrsHandlerRegistrations.g.cs`
  - `ISynergy.Framework.CQRS.SourceGenerator/ISynergy.Framework.CQRS.SourceGenerator.CqrsSourceGenerator/CqrsQueryDispatchTable.g.cs`

- [ ] **Step 4: Inspect the generated files to confirm correctness**

  ```powershell
  Get-ChildItem "tests/ISynergy.Framework.CQRS.Tests/obj/Debug/net10.0/generated" -Recurse -Filter "*.g.cs" | Select-Object FullName
  ```

  Then read each file and verify:
  - `CqrsHandlerRegistrations.g.cs` contains `AddCQRSHandlers()` with registrations for `TestCommandHandler`, `TestCommandWithResultHandler`, `TestQueryHandler`
  - `CqrsQueryDispatchTable.g.cs` contains `table.Register<TestQuery, string>()`

- [ ] **Step 5: Commit**

  ```powershell
  git add src/ISynergy.Framework.CQRS/ISynergy.Framework.CQRS.csproj
  git commit -m "feat(cqrs): bundle source generator as Roslyn analyzer inside CQRS NuGet"
  ```

---

## Chunk 4: Source Generator Tests

### Task 13: Create the source generator test project

**Files:**
- Create: `tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/ISynergy.Framework.CQRS.SourceGenerator.Tests.csproj`
- Create: `tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/SourceGeneratorTestHelper.cs`

> Source generator tests use `Microsoft.CodeAnalysis.CSharp.SourceGenerators.Testing` which runs the generator in-process against a synthetic compilation — no full build required.

- [ ] **Step 1: Create the test project file**

  ```xml
  <!-- tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/ISynergy.Framework.CQRS.SourceGenerator.Tests.csproj -->
  <Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
      <TargetFramework>net10.0</TargetFramework>
      <IsPackable>false</IsPackable>
      <GeneratePackageOnBuild>false</GeneratePackageOnBuild>
      <Nullable>enable</Nullable>
      <ImplicitUsings>enable</ImplicitUsings>
    </PropertyGroup>

    <ItemGroup>
      <PackageReference Include="Microsoft.NET.Test.Sdk" />
      <PackageReference Include="MSTest.TestAdapter" />
      <PackageReference Include="MSTest.TestFramework" />
      <PackageReference Include="Microsoft.CodeAnalysis.CSharp" />
      <PackageReference Include="Microsoft.CodeAnalysis.CSharp.SourceGenerators.Testing" />
    </ItemGroup>

    <ItemGroup>
      <!-- Reference generator as a library for in-process test execution -->
      <ProjectReference Include="..\..\src\ISynergy.Framework.CQRS.SourceGenerator\ISynergy.Framework.CQRS.SourceGenerator.csproj" />
      <!-- Reference CQRS library to make its types available in test compilations -->
      <ProjectReference Include="..\..\src\ISynergy.Framework.CQRS\ISynergy.Framework.CQRS.csproj" />
    </ItemGroup>
  </Project>
  ```

- [ ] **Step 2: Create `SourceGeneratorTestHelper`**

  ```csharp
  // tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/SourceGeneratorTestHelper.cs
  using System.Collections.Immutable;
  using System.Reflection;
  using Microsoft.CodeAnalysis;
  using Microsoft.CodeAnalysis.CSharp;
  using ISynergy.Framework.CQRS.SourceGenerator;

  namespace ISynergy.Framework.CQRS.SourceGenerator.Tests;

  /// <summary>
  /// Runs <see cref="CqrsSourceGenerator"/> against a synthetic compilation built
  /// from the supplied C# source strings.
  /// </summary>
  internal static class SourceGeneratorTestHelper
  {
      /// <summary>
      /// Compiles <paramref name="sources"/> with references to ISynergy.Framework.CQRS
      /// and returns all files produced by the source generator.
      /// </summary>
      public static IReadOnlyDictionary<string, string> RunGenerator(params string[] sources)
      {
          var syntaxTrees = sources
              .Select(s => CSharpSyntaxTree.ParseText(s))
              .ToArray();

          // Collect references: mscorlib + CQRS library + DI abstractions
          var references = AppDomain.CurrentDomain.GetAssemblies()
              .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
              .Select(a => MetadataReference.CreateFromFile(a.Location))
              .Cast<MetadataReference>()
              .ToList();

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
  ```

- [ ] **Step 3: Build the test project**

  ```powershell
  dotnet build tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/ISynergy.Framework.CQRS.SourceGenerator.Tests.csproj
  ```

  Expected: **0 errors**.

---

### Task 14: Write handler registration generator tests

**Files:**
- Create: `tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/HandlerRegistrationGeneratorTests.cs`

- [ ] **Step 1: Write the failing tests first**

  ```csharp
  // tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/HandlerRegistrationGeneratorTests.cs
  namespace ISynergy.Framework.CQRS.SourceGenerator.Tests;

  [TestClass]
  public class HandlerRegistrationGeneratorTests
  {
      private const string CommandHandlerSource = """
          using ISynergy.Framework.CQRS.Commands;

          namespace MyApp.Handlers;

          public class DoThingCommand : ICommand { }

          public class DoThingCommandHandler : ICommandHandler<DoThingCommand>
          {
              public System.Threading.Tasks.Task HandleAsync(
                  DoThingCommand command,
                  System.Threading.CancellationToken cancellationToken = default)
                  => System.Threading.Tasks.Task.CompletedTask;
          }
          """;

      private const string QueryHandlerSource = """
          using ISynergy.Framework.CQRS.Queries;

          namespace MyApp.Handlers;

          public class GetItemQuery : IQuery<string> { }

          public class GetItemQueryHandler : IQueryHandler<GetItemQuery, string>
          {
              public System.Threading.Tasks.Task<string> HandleAsync(
                  GetItemQuery query,
                  System.Threading.CancellationToken cancellationToken = default)
                  => System.Threading.Tasks.Task.FromResult("item");
          }
          """;

      [TestMethod]
      public void Generator_WithCommandHandler_EmitsRegistrationFile()
      {
          var output = SourceGeneratorTestHelper.RunGenerator(CommandHandlerSource);

          Assert.IsTrue(output.ContainsKey("CqrsHandlerRegistrations.g.cs"),
              "Expected CqrsHandlerRegistrations.g.cs to be generated.");
      }

      [TestMethod]
      public void Generator_WithCommandHandler_RegistersHandlerAsScoped()
      {
          var output = SourceGeneratorTestHelper.RunGenerator(CommandHandlerSource);
          var source = output["CqrsHandlerRegistrations.g.cs"];

          StringAssert.Contains(source, "AddScoped");
          StringAssert.Contains(source, "DoThingCommandHandler");
          StringAssert.Contains(source, "ICommandHandler");
          StringAssert.Contains(source, "DoThingCommand");
      }

      [TestMethod]
      public void Generator_WithQueryHandler_EmitsRegistrationAndDispatchTableFiles()
      {
          var output = SourceGeneratorTestHelper.RunGenerator(QueryHandlerSource);

          Assert.IsTrue(output.ContainsKey("CqrsHandlerRegistrations.g.cs"),
              "Expected CqrsHandlerRegistrations.g.cs");
          Assert.IsTrue(output.ContainsKey("CqrsQueryDispatchTable.g.cs"),
              "Expected CqrsQueryDispatchTable.g.cs");
      }

      [TestMethod]
      public void Generator_WithQueryHandler_RegistersQueryHandlerAsScoped()
      {
          var output = SourceGeneratorTestHelper.RunGenerator(QueryHandlerSource);
          var source = output["CqrsHandlerRegistrations.g.cs"];

          StringAssert.Contains(source, "AddScoped");
          StringAssert.Contains(source, "GetItemQueryHandler");
          StringAssert.Contains(source, "IQueryHandler");
      }

      [TestMethod]
      public void Generator_WithNoHandlers_EmitsNoFiles()
      {
          const string noHandlerSource = """
              namespace MyApp;
              public class NotAHandler { }
              """;

          var output = SourceGeneratorTestHelper.RunGenerator(noHandlerSource);

          Assert.IsFalse(output.ContainsKey("CqrsHandlerRegistrations.g.cs"),
              "Should not generate when no handlers are present.");
      }

      [TestMethod]
      public void Generator_WithAbstractHandler_IgnoresAbstractClass()
      {
          const string abstractSource = """
              using ISynergy.Framework.CQRS.Commands;

              namespace MyApp;
              public class MyCmd : ICommand { }
              public abstract class AbstractHandler : ICommandHandler<MyCmd>
              {
                  public abstract System.Threading.Tasks.Task HandleAsync(
                      MyCmd command,
                      System.Threading.CancellationToken cancellationToken = default);
              }
              """;

          var output = SourceGeneratorTestHelper.RunGenerator(abstractSource);

          Assert.IsFalse(output.ContainsKey("CqrsHandlerRegistrations.g.cs"),
              "Abstract handlers should be ignored.");
      }
  }
  ```

- [ ] **Step 2: Run tests — they should fail (generator not yet connected to test compilation correctly)**

  ```powershell
  dotnet test tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/ --logger "console;verbosity=normal"
  ```

  Investigate failures carefully. If `SourceGeneratorTestHelper.RunGenerator` doesn't find CQRS types, add explicit metadata references for the CQRS assembly.

- [ ] **Step 3: Fix `SourceGeneratorTestHelper` if references are missing**

  If tests fail because `ICommandHandler` or `IQuery` are unresolved in the test compilation, add explicit references:

  ```csharp
  // Add inside RunGenerator, before creating the compilation:
  references.Add(MetadataReference.CreateFromFile(
      typeof(ISynergy.Framework.CQRS.Commands.ICommand).Assembly.Location));
  references.Add(MetadataReference.CreateFromFile(
      typeof(Microsoft.Extensions.DependencyInjection.IServiceCollection).Assembly.Location));
  ```

- [ ] **Step 4: Run tests until all pass**

  ```powershell
  dotnet test tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/ --logger "console;verbosity=normal"
  ```

  Expected: all 5 tests pass.

---

### Task 15: Write query dispatch table generator tests

**Files:**
- Create: `tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/QueryDispatchTableGeneratorTests.cs`

- [ ] **Step 1: Write the tests**

  ```csharp
  // tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/QueryDispatchTableGeneratorTests.cs
  namespace ISynergy.Framework.CQRS.SourceGenerator.Tests;

  [TestClass]
  public class QueryDispatchTableGeneratorTests
  {
      private const string QueryHandlerSource = """
          using ISynergy.Framework.CQRS.Queries;

          namespace MyApp.Handlers;

          public class FindUserQuery : IQuery<string> { }

          public class FindUserQueryHandler : IQueryHandler<FindUserQuery, string>
          {
              public System.Threading.Tasks.Task<string> HandleAsync(
                  FindUserQuery query,
                  System.Threading.CancellationToken cancellationToken = default)
                  => System.Threading.Tasks.Task.FromResult("user");
          }
          """;

      [TestMethod]
      public void Generator_EmitsDispatchTableFile_ForQueryHandlers()
      {
          var output = SourceGeneratorTestHelper.RunGenerator(QueryHandlerSource);

          Assert.IsTrue(output.ContainsKey("CqrsQueryDispatchTable.g.cs"));
      }

      [TestMethod]
      public void Generator_DispatchTable_ContainsRegisterCall()
      {
          var output = SourceGeneratorTestHelper.RunGenerator(QueryHandlerSource);
          var source = output["CqrsQueryDispatchTable.g.cs"];

          StringAssert.Contains(source, "table.Register<");
          StringAssert.Contains(source, "FindUserQuery");
          StringAssert.Contains(source, "string");
      }

      [TestMethod]
      public void Generator_DispatchTable_RegistersTryAddSingleton()
      {
          var output = SourceGeneratorTestHelper.RunGenerator(QueryHandlerSource);
          var source = output["CqrsQueryDispatchTable.g.cs"];

          StringAssert.Contains(source, "TryAddSingleton");
          StringAssert.Contains(source, "QueryDispatchTable");
      }

      [TestMethod]
      public void Generator_WithMultipleQueryHandlers_RegistersAll()
      {
          const string multiQuerySource = """
              using ISynergy.Framework.CQRS.Queries;
              using System.Threading;
              using System.Threading.Tasks;

              namespace MyApp.Handlers;

              public class QueryA : IQuery<int> { }
              public class QueryB : IQuery<string> { }

              public class HandlerA : IQueryHandler<QueryA, int>
              {
                  public Task<int> HandleAsync(QueryA q, CancellationToken ct = default) => Task.FromResult(1);
              }

              public class HandlerB : IQueryHandler<QueryB, string>
              {
                  public Task<string> HandleAsync(QueryB q, CancellationToken ct = default) => Task.FromResult("b");
              }
              """;

          var output = SourceGeneratorTestHelper.RunGenerator(multiQuerySource);
          var source = output["CqrsQueryDispatchTable.g.cs"];

          StringAssert.Contains(source, "QueryA");
          StringAssert.Contains(source, "QueryB");
      }

      [TestMethod]
      public void Generator_CommandOnlyHandlers_DoNotProduceDispatchTableFile()
      {
          const string commandOnlySource = """
              using ISynergy.Framework.CQRS.Commands;
              using System.Threading;
              using System.Threading.Tasks;

              namespace MyApp;
              public class Cmd : ICommand { }
              public class CmdHandler : ICommandHandler<Cmd>
              {
                  public Task HandleAsync(Cmd c, CancellationToken ct = default) => Task.CompletedTask;
              }
              """;

          var output = SourceGeneratorTestHelper.RunGenerator(commandOnlySource);

          Assert.IsFalse(output.ContainsKey("CqrsQueryDispatchTable.g.cs"),
              "Dispatch table should only be generated when query handlers are present.");
      }
  }
  ```

- [ ] **Step 2: Run all source generator tests**

  ```powershell
  dotnet test tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/ --logger "console;verbosity=normal"
  ```

  Expected: all 10 tests (5 from Task 14 + 5 from Task 15) pass.

- [ ] **Step 3: Commit**

  ```powershell
  git add tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/
  git commit -m "test(cqrs): add source generator unit tests for handler registration and query dispatch table"
  ```

---

## Chunk 5: Integration and Verification

### Task 16: Integration test — generated APIs used end-to-end

**Files:**
- Modify: `tests/ISynergy.Framework.CQRS.Tests/Extensions/ServiceCollectionExtensionsTests.cs`

The generated `CqrsHandlerRegistrations` class will be available in the test project because the CQRS project references the generator as an analyzer. Add integration tests that use `AddCQRSHandlers()` and `AddQueryDispatchTable()` instead of `AddHandlers()`.

- [ ] **Step 1: Add integration tests using generated methods**

  Add the following test methods to `ServiceCollectionExtensionsTests`:

  ```csharp
  [TestMethod]
  public async Task GeneratedCqrsHandlers_CommandDispatch_WorksEndToEnd()
  {
      // Arrange — use generated registration (no reflection)
      var services = new ServiceCollection();
      services.AddLogging();
      services.AddCQRS();
      services.AddCQRSHandlers();       // <-- generated
      services.AddQueryDispatchTable(); // <-- generated
      var provider = services.BuildServiceProvider();

      var commandDispatcher = provider.GetRequiredService<ICommandDispatcher>();
      var command = new TestCommandWithResult { Input = "AOT Integration Test" };

      // Act
      var result = await commandDispatcher.DispatchAsync<TestCommandWithResult, string>(command);

      // Assert
      Assert.AreEqual("Result: AOT Integration Test", result);
  }

  [TestMethod]
  public async Task GeneratedCqrsHandlers_QueryDispatch_UsesDispatchTable()
  {
      // Arrange — dispatch table must be registered for AOT-safe single-arg dispatch
      var services = new ServiceCollection();
      services.AddLogging();
      services.AddCQRS();
      services.AddCQRSHandlers();
      services.AddQueryDispatchTable();
      var provider = services.BuildServiceProvider();

      var queryDispatcher = provider.GetRequiredService<IQueryDispatcher>();
      var query = new TestQuery { Parameter = "AOT Query" };

      // Act — uses single-arg overload; QueryDispatchTable routes it without reflection
      var result = await queryDispatcher.DispatchAsync<string>(query);

      // Assert
      Assert.AreEqual("Query Result: AOT Query", result);
  }

  [TestMethod]
  public async Task GeneratedCqrsHandlers_QueryDispatch_DoublyGenericOverload_WorksWithoutTable()
  {
      // Arrange — doubly-generic overload is always AOT-safe even without dispatch table
      var services = new ServiceCollection();
      services.AddLogging();
      services.AddCQRS();
      services.AddCQRSHandlers(); // no AddQueryDispatchTable
      var provider = services.BuildServiceProvider();

      var queryDispatcher = provider.GetRequiredService<IQueryDispatcher>();
      var query = new TestQuery { Parameter = "Doubly Generic" };

      // Act — TQuery is statically known, no MakeGenericType needed
      var result = await queryDispatcher.DispatchAsync<TestQuery, string>(query);

      // Assert
      Assert.AreEqual("Query Result: Doubly Generic", result);
  }
  ```

- [ ] **Step 2: Run the full CQRS test suite**

  ```powershell
  dotnet test tests/ISynergy.Framework.CQRS.Tests/ --logger "console;verbosity=normal"
  ```

  Expected: all existing tests + 3 new integration tests pass.

---

### Task 17: Verify full solution build and run all tests

- [ ] **Step 1: Build the full solution**

  ```powershell
  dotnet build ISynergy.Framework.sln
  ```

  Expected: **0 errors**. Warnings about `[RequiresUnreferencedCode]` on `AddHandlers()` are acceptable and expected — they inform callers to migrate to the generated API.

- [ ] **Step 2: Run all CQRS-related tests**

  ```powershell
  dotnet test tests/ISynergy.Framework.CQRS.Tests/ tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/ --logger "console;verbosity=normal"
  ```

  Expected: all tests pass.

- [ ] **Step 3: Verify generated files exist on disk**

  ```powershell
  Get-ChildItem "tests/ISynergy.Framework.CQRS.Tests/obj/Debug/net10.0/generated" -Recurse -Filter "*.g.cs" | ForEach-Object { Write-Host $_.FullName }
  ```

  Expected output includes both `CqrsHandlerRegistrations.g.cs` and `CqrsQueryDispatchTable.g.cs`.

- [ ] **Step 4: Shut down build server**

  ```powershell
  dotnet build-server shutdown
  ```

- [ ] **Step 5: Final commit**

  ```powershell
  git add tests/ISynergy.Framework.CQRS.Tests/ tests/ISynergy.Framework.CQRS.SourceGenerator.Tests/
  git commit -m "test(cqrs): add AOT integration tests using generated handler registrations and dispatch table"
  ```

---

## AOT Compatibility Summary After Implementation

| Concern | Before | After |
|---|---|---|
| `QueryDispatcher` single-arg | `MakeGenericType` + `dynamic` — AOT broken | Uses `QueryDispatchTable` (dict lookup + pre-compiled `Func<>`) — AOT safe |
| `QueryDispatcher` doubly-generic | Not available | New overload `DispatchAsync<TQuery, TResult>` — always AOT safe |
| `AddHandlers()` | `Assembly.GetExportedTypes()` — trims silently | Annotated `[RequiresUnreferencedCode]`; replaced by generated `AddCQRSHandlers()` |
| Query dispatch table registration | N/A | Generated `AddQueryDispatchTable()` populates `Register<TQuery, TResult>()` at startup |
| `AddCommandNotifications()` / `AddCQRSLogging()` decorators | `MakeGenericType` + `ActivatorUtilities` | Still reflection-based — annotate with `[RequiresUnreferencedCode]` (future work) |

> **Note on decorators**: `AddCommandNotifications()` and `AddCQRSLogging()` remain reflection-based. They are optional features that most consumers won't use in AOT scenarios. Annotating them with `[RequiresUnreferencedCode]` is sufficient for this release; a follow-up can generate decorator wrapping too.

---

## Migration Guide for Consumers

After updating to the new version, consuming projects should change their startup code:

```csharp
// Before (reflection — not AOT-safe):
services.AddCQRS();
services.AddHandlers(typeof(MyHandler).Assembly);

// After (generated — AOT-safe):
services.AddCQRS();
services.AddCQRSHandlers();       // generated by source generator
services.AddQueryDispatchTable(); // generated by source generator
```

For query dispatch, the doubly-generic overload is always available without any generated code:

```csharp
// Always AOT-safe (no table needed):
var result = await queryDispatcher.DispatchAsync<GetOrderQuery, OrderDto>(query);

// AOT-safe when dispatch table is registered:
var result = await queryDispatcher.DispatchAsync<OrderDto>(query); // existing API, now table-backed
```
