# Plan: AOT Compatibility for EventSourcing Libraries

## Context

The user wants `ISynergy.Framework.EventSourcing.EntityFramework` to be AOT (Ahead-of-Time compilation) compatible.
Currently it has three AOT-unsafe issues:
1. `DefaultEventTypeResolver` — assembly scanning via reflection (already annotated with `[RequiresUnreferencedCode]`)
2. `EventStore.AppendEventAsync` — `JsonSerializer.Serialize(object, Type, options)` without source-generated context (not annotated)
3. `AggregateRepository.DeserializeEvent` — `JsonSerializer.Deserialize(string, Type, options)` without source-generated context (not annotated)
4. `IEventStore.AppendEventAsync` has `object? metadata` parameter — requires reflection to serialize; no consumer currently passes metadata

The fix introduces an `IEventSerializer` abstraction so `EventStore` and `AggregateRepository` are agnostic to the JSON strategy, plus an AOT-safe `DictionaryEventTypeResolver` and a new AOT-safe registration overload.

---

## Approach

### Step 1 — Change `IEventStore.metadata` to pre-serialized `string?`

**File:** `src/ISynergy.Framework.EventSourcing/Abstractions/Events/IEventStore.cs`

Change the `AppendEventAsync` signature's `metadata` parameter from `object? metadata = null` to `string? metadataJson = null`.

Rationale: metadata is stored as a raw JSON string anyway. Pushing serialization to the caller eliminates the last untyped `object` reflection call from `EventStore`. No current caller passes metadata (checked via grep: all call sites omit the parameter).

---

### Step 2 — Introduce `IEventSerializer` abstraction

**New file:** `src/ISynergy.Framework.EventSourcing.EntityFramework/Abstractions/IEventSerializer.cs`

```csharp
/// <summary>Serializes and deserializes domain events to/from JSON.</summary>
public interface IEventSerializer
{
    /// <summary>Serializes <paramref name="event"/> to a JSON string.</summary>
    string Serialize(IDomainEvent @event);

    /// <summary>
    /// Deserializes a JSON string to the given <paramref name="type"/>.
    /// Returns <c>null</c> when the type cannot be resolved or the data is invalid.
    /// </summary>
    IDomainEvent? Deserialize(string data, Type type);
}
```

---

### Step 3 — Reflection-based implementation (existing behaviour, now explicit)

**New file:** `src/ISynergy.Framework.EventSourcing.EntityFramework/Services/JsonReflectionEventSerializer.cs`

```csharp
[RequiresUnreferencedCode("Uses reflection-based JSON serialization. Use JsonSourceGeneratedEventSerializer for AOT/trim-safe scenarios.")]
[RequiresDynamicCode("Uses reflection-based JSON serialization. Use JsonSourceGeneratedEventSerializer for AOT/trim-safe scenarios.")]
public sealed class JsonReflectionEventSerializer : IEventSerializer
{
    private static readonly JsonSerializerOptions s_options = new() { PropertyNameCaseInsensitive = true, WriteIndented = false };

    public string Serialize(IDomainEvent @event) =>
        JsonSerializer.Serialize((object)@event, @event.GetType(), s_options);

    public IDomainEvent? Deserialize(string data, Type type) =>
        JsonSerializer.Deserialize(data, type, s_options) as IDomainEvent;
}
```

---

### Step 4 — AOT-safe source-generated implementation

**New file:** `src/ISynergy.Framework.EventSourcing.EntityFramework/Services/JsonSourceGeneratedEventSerializer.cs`

```csharp
/// <summary>
/// AOT-safe <see cref="IEventSerializer"/> backed by a consumer-supplied
/// <see cref="JsonSerializerContext"/> that must have all event types registered via
/// <c>[JsonSerializable(typeof(MyEvent))]</c>.
/// </summary>
public sealed class JsonSourceGeneratedEventSerializer : IEventSerializer
{
    private readonly JsonSerializerContext _context;

    public JsonSourceGeneratedEventSerializer(JsonSerializerContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public string Serialize(IDomainEvent @event)
    {
        var typeInfo = _context.GetTypeInfo(@event.GetType())
            ?? throw new InvalidOperationException(
                $"Type '{@event.GetType().FullName}' is not registered in the provided JsonSerializerContext. " +
                "Add [JsonSerializable(typeof(YourEventType))] to your context.");
        return JsonSerializer.Serialize(@event, typeInfo);
    }

    public IDomainEvent? Deserialize(string data, Type type)
    {
        var typeInfo = _context.GetTypeInfo(type);
        return typeInfo is null ? null : JsonSerializer.Deserialize(data, typeInfo) as IDomainEvent;
    }
}
```

---

### Step 5 — AOT-safe `DictionaryEventTypeResolver`

**New file:** `src/ISynergy.Framework.EventSourcing.EntityFramework/Services/DictionaryEventTypeResolver.cs`

```csharp
/// <summary>
/// AOT/trim-safe <see cref="IEventTypeResolver"/> that resolves event types from an
/// explicit compile-time dictionary. Use instead of <see cref="DefaultEventTypeResolver"/>
/// when publishing with <c>PublishAot</c> or <c>PublishTrimmed</c>.
/// </summary>
public sealed class DictionaryEventTypeResolver : IEventTypeResolver
{
    private readonly IReadOnlyDictionary<string, Type> _typeMap;

    public DictionaryEventTypeResolver(IReadOnlyDictionary<string, Type> typeMap) =>
        _typeMap = typeMap ?? throw new ArgumentNullException(nameof(typeMap));

    public Type? Resolve(string eventTypeName) =>
        _typeMap.TryGetValue(eventTypeName, out var type) ? type : null;

    public string GetTypeName(Type eventType) => eventType.Name;
}
```

---

### Step 6 — Refactor `EventStore` to use `IEventSerializer`

**File:** `src/ISynergy.Framework.EventSourcing.EntityFramework/Services/EventStore.cs`

- Add `IEventSerializer _serializer` constructor parameter
- Remove `static readonly JsonSerializerOptions s_jsonOptions`
- Remove `using System.Text.Json`
- In `AppendEventAsync`:
  - Replace `JsonSerializer.Serialize((object)@event, @event.GetType(), s_jsonOptions)` → `_serializer.Serialize(@event)`
  - Replace metadata serialization block → accept `string? metadataJson` directly (matching updated interface)
- Constructor: `EventStore(EventSourcingDbContext, ITenantService, IEventTypeResolver, IEventSerializer)`

---

### Step 7 — Refactor `AggregateRepository` to use `IEventSerializer`

**File:** `src/ISynergy.Framework.EventSourcing.EntityFramework/Services/AggregateRepository.cs`

- Add `IEventSerializer _serializer` constructor parameter
- Remove `static readonly JsonSerializerOptions s_jsonOptions`
- Remove `using System.Text.Json`
- In `DeserializeEvent`: replace `JsonSerializer.Deserialize(record.Data, type, s_jsonOptions) as IDomainEvent` → `_serializer.Deserialize(record.Data, type)`
- Constructor: `AggregateRepository(IEventStore, ITenantService, IEventTypeResolver, IEventSerializer)`

---

### Step 8 — Update `ServiceCollectionExtensions`

**File:** `src/ISynergy.Framework.EventSourcing.EntityFramework/Extensions/ServiceCollectionExtensions.cs`

Keep the **existing** reflection-based overload (already `[RequiresUnreferencedCode]`); update it to also register `JsonReflectionEventSerializer`:

```csharp
[RequiresUnreferencedCode("...")]
[RequiresDynamicCode("...")]
public static IServiceCollection AddEventSourcingEntityFramework(
    this IServiceCollection services,
    Action<DbContextOptionsBuilder> optionsAction)
{
    services.AddDbContext<EventSourcingDbContext>(optionsAction);
    services.AddScoped<IEventStore, EventStore>();
    services.AddSingleton<IEventTypeResolver, DefaultEventTypeResolver>();
    services.AddSingleton<IEventSerializer, JsonReflectionEventSerializer>();
    return services;
}
```

Add a **new AOT-safe overload**:

```csharp
/// <summary>
/// AOT/trim-safe registration. The caller must provide:
/// <list type="bullet">
/// <item>A source-generated <see cref="JsonSerializerContext"/> with all event types declared via <c>[JsonSerializable]</c>.</item>
/// <item>An explicit type map used by <see cref="DictionaryEventTypeResolver"/> to resolve stored event type names back to CLR types.</item>
/// </list>
/// </summary>
public static IServiceCollection AddEventSourcingEntityFramework(
    this IServiceCollection services,
    Action<DbContextOptionsBuilder> optionsAction,
    JsonSerializerContext jsonSerializerContext,
    IReadOnlyDictionary<string, Type> eventTypeMap)
{
    services.AddDbContext<EventSourcingDbContext>(optionsAction);
    services.AddScoped<IEventStore, EventStore>();
    services.AddSingleton<IEventTypeResolver>(new DictionaryEventTypeResolver(eventTypeMap));
    services.AddSingleton<IEventSerializer>(new JsonSourceGeneratedEventSerializer(jsonSerializerContext));
    return services;
}
```

---

### Step 9 — Add `<IsAotCompatible>true</IsAotCompatible>` to the EF project

**File:** `src/ISynergy.Framework.EventSourcing.EntityFramework/ISynergy.Framework.EventSourcing.EntityFramework.csproj`

Add inside `<PropertyGroup>`:
```xml
<IsAotCompatible>true</IsAotCompatible>
```

This activates the trim analyzer which will surface any remaining issues and verify the AOT-safe path is clean.

---

### Step 10 — Update tests

Both test classes directly instantiate `EventStore` and `AggregateRepository`. After the constructor changes, tests must pass a serializer.

**File:** `tests/ISynergy.Framework.EventSourcing.EntityFramework.Tests/Services/EventStoreTests.cs`
- Add field: `private JsonReflectionEventSerializer _serializer = null!;`
- In `Setup()`: `_serializer = new JsonReflectionEventSerializer(); _store = new EventStore(_context, _tenantService, _resolver, _serializer);`

**File:** `tests/ISynergy.Framework.EventSourcing.EntityFramework.Tests/Services/AggregateRepositoryTests.cs`
- Add field: `private JsonReflectionEventSerializer _serializer = null!;`
- In `Setup()`: `_serializer = new JsonReflectionEventSerializer(); _store = new EventStore(_context, _tenantService, _resolver, _serializer); _repository = new AggregateRepository<OrderAggregate, Guid>(_store, _tenantService, _resolver, _serializer);`

---

## Files Modified Summary

| File | Action |
|------|--------|
| `src/ISynergy.Framework.EventSourcing/Abstractions/Events/IEventStore.cs` | Change `object? metadata` → `string? metadataJson` |
| `src/ISynergy.Framework.EventSourcing.EntityFramework/Abstractions/IEventSerializer.cs` | **NEW** |
| `src/ISynergy.Framework.EventSourcing.EntityFramework/Services/JsonReflectionEventSerializer.cs` | **NEW** |
| `src/ISynergy.Framework.EventSourcing.EntityFramework/Services/JsonSourceGeneratedEventSerializer.cs` | **NEW** |
| `src/ISynergy.Framework.EventSourcing.EntityFramework/Services/DictionaryEventTypeResolver.cs` | **NEW** |
| `src/ISynergy.Framework.EventSourcing.EntityFramework/Services/EventStore.cs` | Add `IEventSerializer` param, remove inline JSON |
| `src/ISynergy.Framework.EventSourcing.EntityFramework/Services/AggregateRepository.cs` | Add `IEventSerializer` param, remove inline JSON |
| `src/ISynergy.Framework.EventSourcing.EntityFramework/Extensions/ServiceCollectionExtensions.cs` | Register serializer; add AOT-safe overload |
| `src/ISynergy.Framework.EventSourcing.EntityFramework/ISynergy.Framework.EventSourcing.EntityFramework.csproj` | Add `<IsAotCompatible>true</IsAotCompatible>` |
| `tests/.../Services/EventStoreTests.cs` | Pass `JsonReflectionEventSerializer` to constructors |
| `tests/.../Services/AggregateRepositoryTests.cs` | Pass `JsonReflectionEventSerializer` to constructors |

---

## Consumer Migration (Sample)

The sample `Sample.EventSourcing.Api` uses `AddEventSourcingEntityFramework(o => o.UseNpgsql(...))`. This continues to work unchanged (reflection path). For AOT, consumers would switch to:

```csharp
[JsonSerializable(typeof(OrderPlaced))]
[JsonSerializable(typeof(OrderShipped))]
[JsonSerializable(typeof(OrderCancelled))]
public partial class OrderEventJsonContext : JsonSerializerContext { }

services.AddEventSourcingEntityFramework(
    o => o.UseNpgsql(connectionString),
    jsonSerializerContext: OrderEventJsonContext.Default,
    eventTypeMap: new Dictionary<string, Type>
    {
        [nameof(OrderPlaced)] = typeof(OrderPlaced),
        [nameof(OrderShipped)] = typeof(OrderShipped),
        [nameof(OrderCancelled)] = typeof(OrderCancelled),
    });
```

---

## Verification

1. `dotnet build src/ISynergy.Framework.EventSourcing.EntityFramework` — should produce zero trim/AOT warnings on the AOT-safe code paths
2. `dotnet test tests/ISynergy.Framework.EventSourcing.EntityFramework.Tests` — all existing tests pass unchanged
3. Build the sample: `dotnet build samples/Sample.EventSourcing.Api` — no errors
