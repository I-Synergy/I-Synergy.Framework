# I-Synergy Framework EventSourcing Storage S3

AWS S3 (and S3-compatible) implementation of `IEventArchiveStorage` for I-Synergy Event Sourcing.

## Included types

- `S3EventArchiveStorage` — stores archived events as JSON objects in per-tenant S3 buckets

## Usage

Register an `AmazonS3Client` in DI first, then call `AddS3EventArchiveStorage`:

```csharp
// Register the shared AmazonS3Client (configure credentials and region once)
var credentials = new BasicAWSCredentials(accessKey, secretKey);
var config = new AmazonS3Config { RegionEndpoint = RegionEndpoint.EUWest1 };
services.AddSingleton(new AmazonS3Client(credentials, config));

// Register IEventArchiveStorage backed by S3
services.AddS3EventArchiveStorage("archive");

// Register the archiver and reader (from ISynergy.Framework.EventSourcing.Storage)
services.AddEventArchiving(configuration);
```

For MinIO or other S3-compatible providers, set `ServiceUrl` and `ForcePathStyle = true` on `AmazonS3Config`.

## Tenant isolation

Each tenant receives its own bucket: `{bucketNamePrefix}-{tenantId}` (e.g. `archive-a1b2c3d4-...`).
Buckets are created on demand. Within each bucket the object key is:

```
{streamType}/{streamId}/{versionFrom}-{versionTo}.json
```

## Bucket naming constraints

The prefix must be lowercase, contain only letters, digits and hyphens, and be at most
**26 characters** so that `{prefix}-{36-char-guid}` fits within the S3 63-character limit.
