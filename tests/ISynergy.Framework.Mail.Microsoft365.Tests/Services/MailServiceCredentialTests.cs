using Azure.Core;
using Azure.Identity;
using ISynergy.Framework.Mail.Microsoft365.Options;
using ISynergy.Framework.Mail.Microsoft365.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptionsX = Microsoft.Extensions.Options.Options;

namespace ISynergy.Framework.Mail.Microsoft365.Services;

/// <summary>
/// Tests for the identity-first / client-secret-fallback credential selection.
/// </summary>
[TestClass]
public class MailServiceCredentialTests
{
    [TestMethod]
    public void GetOrCreateCredential_UsesClientSecret_WhenSecretIsConfiguredAndIdentityIsNotRequested()
    {
        var service = CreateService(CreateOptions(clientSecret: "s3cret"));

        Assert.IsInstanceOfType<ClientSecretCredential>(service.GetOrCreateCredential());
    }

    [TestMethod]
    public void GetOrCreateCredential_UsesManagedIdentity_WhenUseManagedIdentityIsSetWithClientId()
    {
        var options = CreateOptions(clientSecret: "s3cret");
        options.UseManagedIdentity = true;
        options.ManagedIdentityClientId = Guid.NewGuid().ToString();
        var service = CreateService(options);

        Assert.IsInstanceOfType<ManagedIdentityCredential>(service.GetOrCreateCredential());
    }

    [TestMethod]
    public void GetOrCreateCredential_UsesDefaultCredential_WhenUseManagedIdentityIsSetWithoutClientId()
    {
        var options = CreateOptions(clientSecret: "s3cret");
        options.UseManagedIdentity = true;
        var service = CreateService(options);

        Assert.IsInstanceOfType<DefaultAzureCredential>(service.GetOrCreateCredential());
    }

    [TestMethod]
    public void GetOrCreateCredential_UsesIdentity_WhenNoClientSecretIsConfigured()
    {
        var service = CreateService(CreateOptions(clientSecret: ""));

        Assert.IsInstanceOfType<DefaultAzureCredential>(service.GetOrCreateCredential());
    }

    [TestMethod]
    public void GetOrCreateCredential_PrefersInjectedCredential()
    {
        var injected = new FakeTokenCredential();

        var service = new Microsoft365MailService(OptionsX.Create(CreateOptions(clientSecret: "s3cret")), injected);

        Assert.AreSame(injected, service.GetOrCreateCredential());
    }

    [TestMethod]
    public void Constructor_Throws_WhenClientSecretPathIsIncomplete()
    {
        var options = CreateOptions(clientSecret: "s3cret");
        options.ClientId = string.Empty;

        Assert.ThrowsExactly<ArgumentNullException>(() => new Microsoft365MailService(OptionsX.Create(options)));
    }

    [TestMethod]
    public void Constructor_AllowsMissingClientIdAndTenant_WhenIdentityIsImplicit()
    {
        var options = CreateOptions(clientSecret: "");
        options.ClientId = string.Empty;
        options.TenantId = string.Empty;

        var service = new Microsoft365MailService(OptionsX.Create(options));

        Assert.IsNotNull(service);
    }

    private static Microsoft365MailService CreateService(Microsoft365MailOptions options)
        => new(OptionsX.Create(options));

    private static Microsoft365MailOptions CreateOptions(string clientSecret) => new()
    {
        EmailAddress = "support@example.com",
        Sender = "Support",
        ClientId = Guid.NewGuid().ToString(),
        TenantId = Guid.NewGuid().ToString(),
        ClientSecret = clientSecret,
        Scopes = ["https://graph.microsoft.com/.default"]
    };

    private sealed class FakeTokenCredential : TokenCredential
    {
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => new("fake-token", DateTimeOffset.MaxValue);

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => ValueTask.FromResult(new AccessToken("fake-token", DateTimeOffset.MaxValue));
    }
}
