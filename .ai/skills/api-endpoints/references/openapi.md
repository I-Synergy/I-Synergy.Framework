# OpenAPI Configuration

## API Project .csproj

Every API project must generate the OpenAPI spec at build time and regenerate the Kiota client:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <OpenApiDocumentsDirectory>../../openapi</OpenApiDocumentsDirectory>
    <OpenApiGenerateDocuments>true</OpenApiGenerateDocuments>
    <OpenApiGenerateDocumentsOnBuild>true</OpenApiGenerateDocumentsOnBuild>
    <OpenApiGenerateEnvironment>Development</OpenApiGenerateEnvironment>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.ApiDescription.Server">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <!-- Generate Kiota client from OpenAPI spec on every build -->
  <Target Name="OpenAPI" AfterTargets="Build">
    <Exec Command="dotnet kiota generate -l CSharp --output ../{ApplicationName}.Clients.Api --namespace-name {ApplicationName}.Clients.Api --class-name ApiClient --exclude-backward-compatible --openapi ../../openapi/{ApplicationName}.Api.json" WorkingDirectory="$(ProjectDir)" />
  </Target>

</Project>
```

Key points:
- `OpenApiDocumentsDirectory` — relative to project file, typically `../../openapi`
- `OpenApiGenerateDocumentsOnBuild` — `true` ensures the spec is always current
- `OpenAPI` target — runs `dotnet kiota generate` after **every** build (no `Condition` restricting to Debug)
- Generated OpenAPI file: `../../openapi/{ProjectName}.json`

## Document + Schema Transformers

Both transformers MUST be registered in a single `AddOpenApi()` call. Do NOT pass a document name parameter — it would change the output file from `{ProjectName}.json` to `{name}.json`, breaking the Kiota generation path.

```csharp
// File: {ApplicationName}.Service.Api/Program.cs

builder.Services.AddOpenApi(options =>
{
    // Document transformer: sets servers URL for Kiota clients
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Servers = [new OpenApiServer { Url = "https://api" }];
        return Task.CompletedTask;
    });

    // Schema transformer: maps .NET types to JSON schema types
    options.AddSchemaTransformer((schema, context, ct) =>
    {
        var propertyType = context.JsonPropertyInfo?.PropertyType;
        if (propertyType == null)
            return Task.CompletedTask;

        var actualType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        // Date/Time
        if (actualType == typeof(DateTimeOffset) || actualType == typeof(DateTime))
        { schema.Type = JsonSchemaType.String; schema.Format = "date-time"; }
        else if (actualType == typeof(DateOnly))
        { schema.Type = JsonSchemaType.String; schema.Format = "date"; }
        else if (actualType == typeof(TimeOnly))
        { schema.Type = JsonSchemaType.String; schema.Format = "time"; }
        else if (actualType == typeof(TimeSpan))
        { schema.Type = JsonSchemaType.String; schema.Format = "duration"; }

        // Numeric
        else if (actualType == typeof(decimal) || actualType == typeof(double))
        { schema.Type = JsonSchemaType.Number; schema.Format = "double"; }
        else if (actualType == typeof(float))
        { schema.Type = JsonSchemaType.Number; schema.Format = "float"; }

        // Integer
        else if (actualType == typeof(int))
        { schema.Type = JsonSchemaType.Integer; schema.Format = "int32"; }
        else if (actualType == typeof(long))
        { schema.Type = JsonSchemaType.Integer; schema.Format = "int64"; }
        else if (actualType == typeof(short))
        { schema.Type = JsonSchemaType.Integer; schema.Format = "int16"; }
        else if (actualType == typeof(byte))
        { schema.Type = JsonSchemaType.Integer; schema.Format = "int8"; schema.Minimum = "0"; schema.Maximum = "255"; }
        else if (actualType == typeof(sbyte))
        { schema.Type = JsonSchemaType.Integer; schema.Format = "int8"; schema.Minimum = "-128"; schema.Maximum = "127"; }
        else if (actualType == typeof(uint))
        { schema.Type = JsonSchemaType.Integer; schema.Format = "int32"; schema.Minimum = "0"; }
        else if (actualType == typeof(ulong))
        { schema.Type = JsonSchemaType.Integer; schema.Format = "int64"; schema.Minimum = "0"; }
        else if (actualType == typeof(ushort))
        { schema.Type = JsonSchemaType.Integer; schema.Format = "int16"; schema.Minimum = "0"; }

        // String/Identifier
        else if (actualType == typeof(Guid))
        { schema.Type = JsonSchemaType.String; schema.Format = "uuid"; }
        else if (actualType == typeof(string))
        { schema.Type = JsonSchemaType.String; }
        else if (actualType == typeof(char))
        { schema.Type = JsonSchemaType.String; schema.MinLength = 1; schema.MaxLength = 1; }

        // Other
        else if (actualType == typeof(bool))
        { schema.Type = JsonSchemaType.Boolean; }
        else if (actualType == typeof(byte[]))
        { schema.Type = JsonSchemaType.String; schema.Format = "byte"; }
        else if (actualType == typeof(Uri))
        { schema.Type = JsonSchemaType.String; schema.Format = "uri"; }

        // Enums
        else if (actualType.IsEnum)
        {
            schema.Type = JsonSchemaType.String;
            schema.Enum = Enum.GetNames(actualType)
                .Select(name => System.Text.Json.Nodes.JsonValue.Create(name)!)
                .Cast<System.Text.Json.Nodes.JsonNode>()
                .ToList();
        }

        return Task.CompletedTask;
    });
});
```

Without these transformers:
- **No document transformer** → Kiota client has no servers URL, doesn't know where to send requests
- **No schema transformer** → Kiota generates all model properties as `UntypedNode` instead of `int`, `string`, `Guid`, etc.

## Do NOT Use

```csharp
// WRONG — Old Swashbuckle patterns
.WithName("CreateEntity")
.WithOpenApi()

// WRONG — Document name changes output file
builder.Services.AddOpenApi("v1", ...)  // Produces v1.json, not {ProjectName}.json

// WRONG — Transformers not registered
builder.Services.AddOpenApi();
```

## Union Types: `[FromQuery] int` Emits `["integer","string"]`

.NET 10 emits `[FromQuery] int` as the union `["integer","string"]` (a query value can be a number or its string form). Kiota has no `(type,format)` entry for a multi-type schema, so it silently falls back to `string` and emits a build warning.

**Fix 1 — operation transformer that collapses the union.** Register it in the same `AddOpenApi()` call:

```csharp
options.AddOperationTransformer((operation, context, ct) =>
{
    foreach (var parameter in operation.Parameters)
    {
        var type = parameter.Schema.Type;
        if (type.HasFlag(JsonSchemaType.String) &&
            (type.HasFlag(JsonSchemaType.Integer) || type.HasFlag(JsonSchemaType.Number)))
        {
            parameter.Schema.Type = type & ~JsonSchemaType.String;
        }
    }
    return Task.CompletedTask;
});
```

**Fix 2 — schema transformer must read `JsonTypeInfo` too.** Use `context.JsonPropertyInfo?.PropertyType ?? context.JsonTypeInfo?.Type` (not just `JsonPropertyInfo`), otherwise parameter/component schemas come through as `{}` and get no type mapping:

```csharp
options.AddSchemaTransformer((schema, context, ct) =>
{
    var propertyType = context.JsonPropertyInfo?.PropertyType ?? context.JsonTypeInfo?.Type;
    if (propertyType == null)
        return Task.CompletedTask;
    // ... existing type mapping ...
    return Task.CompletedTask;
});
```

Without both fixes: Kiota generates `[FromQuery] int` parameters as `string`, and component/parameter schemas lose their concrete types.
