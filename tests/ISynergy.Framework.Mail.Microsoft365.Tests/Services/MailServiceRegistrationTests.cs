using Azure.Core;
using ISynergy.Framework.Mail.Abstractions.Services;
using ISynergy.Framework.Mail.Microsoft365.Extensions;
using ISynergy.Framework.Mail.Microsoft365.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mail.Microsoft365.Services;

/// <summary>
/// Tests for <see cref="ServiceCollectionExtensions.AddMicrosoft365MailIntegration"/>.
/// </summary>
[TestClass]
public class MailServiceRegistrationTests
{
    [TestMethod]
    public void AddMicrosoft365MailIntegration_RegistersMailServiceFromConfiguration()
    {
        var services = new ServiceCollection();
        services.AddMicrosoft365MailIntegration(BuildConfiguration());

        using var provider = services.BuildServiceProvider();
        var mail = provider.GetRequiredService<IMailService>();

        Assert.IsInstanceOfType<Microsoft365MailService>(mail);
    }

    [TestMethod]
    public void AddMicrosoft365MailIntegration_RegistersSuppliedCredential()
    {
        var credential = new FakeTokenCredential();
        var services = new ServiceCollection();
        services.AddMicrosoft365MailIntegration(BuildConfiguration(), credential: credential);

        using var provider = services.BuildServiceProvider();

        Assert.AreSame(credential, provider.GetRequiredService<TokenCredential>());
    }

    private static IConfiguration BuildConfiguration()
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MailOptions:EmailAddress"] = "support@example.com",
                ["MailOptions:Sender"] = "Support",
                ["MailOptions:ClientId"] = Guid.NewGuid().ToString(),
                ["MailOptions:TenantId"] = Guid.NewGuid().ToString(),
                ["MailOptions:ClientSecret"] = "s3cret"
            })
            .Build();

    private sealed class FakeTokenCredential : TokenCredential
    {
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => new("fake-token", DateTimeOffset.MaxValue);

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => ValueTask.FromResult(new AccessToken("fake-token", DateTimeOffset.MaxValue));
    }
}
