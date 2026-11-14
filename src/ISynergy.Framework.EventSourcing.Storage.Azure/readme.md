# I-Synergy Framework EventSourcing Storage Azure

Azure Blob Storage implementation of `IEventArchiveStorage` for I-Synergy Event Sourcing.

## Included types

- `AzureBlobEventArchiveStorage` — stores archived events as JSON blobs in Azure Blob Storage

## Authentication

This library does not create the storage client; it consumes a `BlobServiceClient` from DI.
Identity-based authentication is therefore preferred and is selected where the client is
registered:

- **Aspire (recommended):** `AddAzureBlobServiceClient("blobs")` uses identity
  (`DefaultAzureCredential`/managed identity) in published environments and the emulator
  connection string locally — identity first, connection string as the fallback.
- **Manual:** `new BlobServiceClient(new Uri(serviceUri), credential)` for identity, or
  `new BlobServiceClient(connectionString)` as the fallback.

## Usage

Requires `BlobServiceClient` to be registered in DI (via Aspire `AddAzureBlobServiceClient("blobs")`
or directly).

```csharp
builder.AddAzureBlobServiceClient("blobs");
services.AddAzureEventArchiveStorage();   // registers IEventArchiveStorage
services.AddEventArchiving(configuration); // registers IEventArchiver + IEventArchiveReader
```
