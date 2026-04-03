# I-Synergy Framework EventSourcing Storage

Provider-agnostic cold-tier archiving abstractions and services for I-Synergy Event Sourcing.

## Included types

- `IEventArchiver` — runs the archive job (snapshot + upload + delete hot events)
- `IEventArchiveReader` — reads full history from cold + hot storage
- `IEventArchiveStorage` — provider-agnostic storage back-end interface
- `EventArchiveSettings` — configuration bound from `EventArchive` section
- `EventArchiver` — default `IEventArchiver` implementation
- `EventArchiveReader` — default `IEventArchiveReader` implementation

## Usage

Register a storage back-end (e.g. `AddAzureEventArchiveStorage()` from the Azure package), then:

```csharp
services.AddEventArchiving(configuration);
```

Inject `IEventArchiver` to run the job, `IEventArchiveReader` for audit queries.
