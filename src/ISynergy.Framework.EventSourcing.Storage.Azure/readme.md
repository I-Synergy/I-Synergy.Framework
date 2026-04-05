# I-Synergy Framework EventSourcing Storage Azure

Azure Blob Storage implementation of `IEventArchiveStorage` for I-Synergy Event Sourcing.

## Included types

- `AzureBlobEventArchiveStorage` — stores archived events as JSON blobs in Azure Blob Storage

## Usage

Requires `BlobServiceClient` to be registered in DI (via Aspire `AddAzureBlobServiceClient("blobs")` or directly).

```csharp
builder.AddAzureBlobServiceClient("blobs");
services.AddAzureEventArchiveStorage();   // registers IEventArchiveStorage
services.AddEventArchiving(configuration); // registers IEventArchiver + IEventArchiveReader
```
