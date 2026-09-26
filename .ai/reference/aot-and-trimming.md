# AOT & Trimming

Hard-won lessons from shipping a .NET MAUI + API + PostgreSQL app to iOS/macCatalyst Release builds. Release builds on iOS/macCatalyst are fully ahead-of-time compiled — there is no JIT — so anything that relies on runtime code generation, reflection, or `Assembly.GetTypes()` breaks in ways that surface far from the actual cause.

## 1. EF Core Is Incompatible with iOS/macCatalyst Release AOT

**Root cause:** Release builds on iOS/macCatalyst are fully ahead-of-time compiled (no JIT). EF Core's query compilation and `Database.EnsureCreated()` need runtime dynamic code generation, which AOT cannot provide. The failure surfaces as `EXC_BAD_ACCESS` deep inside the app — not a clear message near the EF Core call site.

**Fix:** Do not ship EF Core in a MAUI mobile app. Replace the consumer (e.g. a Favorites store) with a JSON-file or SQLite-lite store behind the same interface, so callers do not change:

```csharp
// BEFORE — EF Core in the mobile app (crashes under Release AOT)
public sealed class EfCoreFavoriteStore(DbContext db) : IFavoriteStore
{
    public async Task<IReadOnlyList<Favorite>> GetAllAsync(CancellationToken ct)
        => await db.Favorites.ToListAsync(ct);
}

// AFTER — JSON-file store behind the same interface; callers unchanged
public sealed class JsonFileFavoriteStore(string path) : IFavoriteStore
{
    public async Task<IReadOnlyList<Favorite>> GetAllAsync(CancellationToken ct)
    {
        var json = await File.ReadAllTextAsync(path, ct);
        return JsonSerializer.Deserialize<List<Favorite>>(json) ?? [];
    }
}
```

**Do not pursue these dead ends:**
- `UseInterpreter=true` / `--interpreter` only masks the failure; the interpreter path does not reliably cover EF Core's query compilation.
- A hand-written compiled model is a dead end — EF Core still requires runtime code generation for query compilation regardless of a precompiled model.

## 2. IConfiguration.Bind(object) Silently Returns Defaults Under Trimming

**Root cause:** The trimmer strips properties it cannot prove are used, without throwing. `IConfiguration.Bind<T>(object)` sets properties via reflection, so under trimming the properties are removed and binding silently returns default values. A missing `BaseAddress` then throws `UriFormatException` on every launch with no log pointing at the config.

**Fix:** Read each value explicitly:

```csharp
// BEFORE — reflection-based bind; silently returns defaults when trimmed
var settings = new AppSettings();
configuration.GetSection("App").Bind(settings); // settings.BaseAddress == "" → UriFormatException

// AFTER — explicit per-value reads survive trimming
var section = configuration.GetSection("App");
var settings = new AppSettings(
    BaseAddress: section.GetValue<string>(nameof(AppSettings.BaseAddress))
        ?? throw new InvalidOperationException("App:BaseAddress is required"),
    TimeoutSeconds: section.GetValue<int>(nameof(AppSettings.TimeoutSeconds)));
```

## 3. Reflection-Resolved DI Registrations Get Stripped; Root Them

**Root cause:** Convention-based registration via `Assembly.GetTypes()` has no static reference to each type, so the trimmer removes the types (it cannot see they are instantiated through reflection). The registration loop then registers nothing or throws at runtime.

**Fix:** Add `<TrimmerRootAssembly Include="AssemblyName" />` in the `.csproj` for every assembly registered by convention:

```xml
<ItemGroup>
  <TrimmerRootAssembly Include="Contoso.Features" />
  <TrimmerRootAssembly Include="Contoso.Common" />
</ItemGroup>
```

**Related:** prefer compiled bindings (`x:DataType`) over `{Binding}` string paths. Compiled bindings are resolved at build time and survive trimming; string-path `{Binding}` relies on reflection.

## 4. macCatalyst Hardened Runtime

**Root cause:** MAUI signs macCatalyst bundles without Hardened Runtime. On macOS 26 the Store applies `CS_RESTRICT`, which blocks `@` path expansion, so the app fails to load.

**Fix:** Re-sign with Hardened Runtime and grant only the two entitlements the runtime needs:

```bash
codesign --force --sign "$KEY" \
  --options=runtime \
  --preserve-metadata=identifier,entitlements \
  "$APP"
```

```xml
<!-- entitlements.plist — grant only these two -->
<dict>
  <key>com.apple.security.cs.allow-jit</key><true/>
  <key>com.apple.security.cs.allow-unsigned-executable-memory</key><true/>
</dict>
```

**NEVER add `com.apple.security.cs.disable-library-validation`** — under Hardened Runtime it makes macOS enforce a stricter `@rpath` policy and the load fails.

## 5. Strip Dangling XCFramework Symlinks Before Packaging; Use NUL-Delimited Loops

**Root cause:** A framework can ship a `Frameworks -> Versions/Current/Frameworks` symlink whose target doesn't exist in that slice. Apple rejects the upload (`ITMS-90332`). Naive `find | xargs rm` word-splits on spaces (e.g. the app name "I-Synergy Quran.app") and silently no-ops, leaving the dangling symlink in place.

**Fix:** Use a NUL-delimited loop:

```bash
while IFS= read -r -d '' L; do
  rm -f "$L"
done < <(find "$FRAMEWORK" -type l ! -exec test -e {} \; -print0)
```

**NEVER use `find | xargs rm`** — `xargs` word-splits on spaces and silently no-ops on paths like `I-Synergy Quran.app`, so the dangling symlink survives and the upload still fails.
