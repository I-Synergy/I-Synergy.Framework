# Encryption Strategies & PKI
Encryption at rest and in transit, key rotation, and certificate/PKI lifecycle management. Load this when choosing encryption or managing certificates.

## Encryption Strategies

### Encryption at Rest

```csharp
/// <summary>
/// Encrypts sensitive data at rest using AES-256.
/// </summary>
public sealed class DataEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public DataEncryptionService(IConfiguration configuration)
    {
        _key = Convert.FromBase64String(configuration["Encryption:Key"]!);
        _iv = Convert.FromBase64String(configuration["Encryption:IV"]!);
    }

    public string Encrypt(string plaintext)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);

        sw.Write(plaintext);
        sw.Close();

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string ciphertext)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(Convert.FromBase64String(ciphertext));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
```

### Encryption in Transit

```csharp
// File: {ApplicationName}.Services.API/Program.cs

var builder = WebApplication.CreateBuilder(args);

// Enforce HTTPS
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 443;
});

// Configure Kestrel for HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
    });
});

// HSTS (HTTP Strict Transport Security)
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.Run();
```

---

## PKI and Certificate Management

### Certificate Management

```csharp
/// <summary>
/// Manages SSL/TLS certificates.
/// </summary>
public sealed class CertificateManagementService
{
    private readonly ILogger<CertificateManagementService> _logger;

    public async Task MonitorCertificateExpirationAsync(
        CancellationToken cancellationToken = default)
    {
        var certificates = await GetAllCertificatesAsync(cancellationToken);

        foreach (var cert in certificates)
        {
            var daysUntilExpiration = (cert.ExpirationDate - DateTimeOffset.UtcNow).Days;

            if (daysUntilExpiration <= 30)
            {
                _logger.LogWarning(
                    "Certificate expiring soon: {Subject}, Expires: {ExpirationDate}",
                    cert.Subject, cert.ExpirationDate);

                // Alert operations team
                await SendExpirationAlertAsync(cert, cancellationToken);
            }

            if (daysUntilExpiration <= 7)
            {
                _logger.LogCritical(
                    "Certificate expiring very soon: {Subject}, Expires: {ExpirationDate}",
                    cert.Subject, cert.ExpirationDate);
            }
        }
    }

    public async Task RenewCertificateAsync(
        string certificateName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Renewing certificate: {CertificateName}", certificateName);

        // Certificate renewal logic (e.g., Let's Encrypt, Azure Key Vault)

        _logger.LogInformation("Certificate renewed successfully: {CertificateName}", certificateName);
    }
}
```

---

