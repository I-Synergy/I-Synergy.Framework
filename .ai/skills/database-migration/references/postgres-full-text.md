# PostgreSQL Full-Text Search

Custom dictionaries without stopwords, index-friendly `to_tsvector` usage, and pinning the migrations-history schema. Load this when implementing search over translated text or debugging a full-scan / "relation already exists" failure.

## PostgreSQL Full-Text Search

### Custom Config Without Stopwords

Built-in `*_stem` dictionaries (e.g. `english_stem`) have stopwords baked into their Snowball template. So `plainto_tsquery('english','will')` reduces to an empty query — common words silently match nothing ("will" is a stopword). Create custom dictionaries without `StopWords` and a custom config:

```sql
CREATE TEXT SEARCH DICTIONARY x_english_dict (TEMPLATE = snowball, Language = english);

CREATE TEXT SEARCH CONFIGURATION x_english (COPY = pg_catalog.english);

ALTER TEXT SEARCH CONFIGURATION x_english
    ALTER MAPPING FOR asciiword, asciihword, hword_asciipart, word, hword, hword_part
    WITH x_english_dict;
```

### Reference Each Translation Column Directly in the WHERE Clause

Reference each translation column directly in the `WHERE` clause via `EF.Functions.ToTsVector("config", x.Column).Matches(EF.Functions.PlainToTsQuery("config", term))`:

```csharp
// ✅ CORRECT — WHERE references each column directly, so the per-column GIN index is used
var rows = await dataContext.Documents
    .Where(x =>
        EF.Functions.ToTsVector("x_english", x.TitleEn)
            .Matches(EF.Functions.PlainToTsQuery("x_english", term))
        || EF.Functions.ToTsVector("x_english", x.TitleAr)
            .Matches(EF.Functions.PlainToTsQuery("x_english", term)))
    .Select(x => new SearchResult(
        x.Id,
        // ✅ CASE belongs here, in the final output projection only
        x.Language == "en" ? x.TitleEn : x.TitleAr))
    .ToListAsync(ct);
```

A `CASE`-wrapped `to_tsvector()` projection in the `WHERE` clause defeats the per-column GIN index — Postgres can't match an expression index against a `CASE`, so it falls back to a full scan. Keep the `CASE` only in the final output projection.

### Pin the Migrations History Table Schema

Pin the migrations history table schema in the provider options — in **both** dev and prod registrations:

```csharp
// ✅ CORRECT — both dev and prod registrations pin the same schema
options.UseNpgsql(npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "system"));
```

`IDbContextOptionsConfiguration` does not reliably apply at runtime, so `MigrateAsync` reads the default `public.__EFMigrationsHistory`, finds no applied migrations, and re-runs every migration (failing with "relation already exists").
