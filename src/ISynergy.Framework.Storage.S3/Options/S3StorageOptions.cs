namespace ISynergy.Framework.Storage.S3.Options;

/// <summary>
/// Configuration options for S3 and S3-compatible storage providers (AWS S3, MinIO, etc.).
/// </summary>
public class S3StorageOptions
{
    /// <summary>
    /// Gets or sets the service URL for S3-compatible providers (e.g. http://minio:9000).
    /// Leave empty or null to use the default AWS S3 endpoint resolved from <see cref="Region"/>.
    /// </summary>
    public string? ServiceUrl { get; set; }

    /// <summary>
    /// Gets or sets the AWS access key or S3-compatible provider access key.
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the AWS secret key or S3-compatible provider secret key.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the AWS region (e.g. us-east-1).
    /// Required for AWS S3; may be arbitrary for MinIO and other S3-compatible providers.
    /// </summary>
    public string Region { get; set; } = "us-east-1";

    /// <summary>
    /// Gets or sets a value indicating whether to use path-style addressing.
    /// Set to <c>true</c> for MinIO and other S3-compatible providers that require path-style URLs.
    /// AWS S3 uses virtual-hosted style by default (<c>false</c>).
    /// </summary>
    public bool ForcePathStyle { get; set; } = false;
}
