using ISynergy.Framework.Storage.Azure.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Storage.Azure;

/// <summary>
/// Tests for the identity-first / connection-string-fallback resolution rules.
/// </summary>
[TestClass]
public class AzureStorageOptionsTests
{
    [TestMethod]
    public void TryGetServiceUri_PrefersConfiguredServiceEndpoint_OverConnectionString()
    {
        var options = new AzureStorageOptions
        {
            ServiceEndpoint = "https://myaccount.blob.core.windows.net/",
            ConnectionString = "UseDevelopmentStorage=true"
        };

        Assert.IsTrue(options.TryGetServiceUri(out var serviceUri));
        Assert.IsNotNull(serviceUri);
        Assert.AreEqual("https://myaccount.blob.core.windows.net/", serviceUri.AbsoluteUri);
    }

    [TestMethod]
    public void TryGetServiceUri_DerivesEndpoint_FromAccountNameWithoutAccountKey()
    {
        var options = new AzureStorageOptions
        {
            AccountName = "myaccount",
            ConnectionString = "UseDevelopmentStorage=true"
        };

        Assert.IsTrue(options.TryGetServiceUri(out var serviceUri));
        Assert.IsNotNull(serviceUri);
        Assert.AreEqual("https://myaccount.blob.core.windows.net/", serviceUri.AbsoluteUri);
    }

    [TestMethod]
    public void TryGetServiceUri_ReturnsFalse_WhenAccountKeyIsConfigured()
    {
        var options = new AzureStorageOptions
        {
            AccountName = "myaccount",
            AccountKey = "a2V5"
        };

        Assert.IsFalse(options.TryGetServiceUri(out var serviceUri));
        Assert.IsNull(serviceUri);
    }

    [TestMethod]
    public void TryGetServiceUri_ReturnsFalse_WhenOnlyConnectionStringIsConfigured()
    {
        var options = new AzureStorageOptions
        {
            ConnectionString = "UseDevelopmentStorage=true"
        };

        Assert.IsFalse(options.TryGetServiceUri(out var serviceUri));
        Assert.IsNull(serviceUri);
    }

    [TestMethod]
    public void GetEffectiveConnectionString_PrefersExplicitConnectionString()
    {
        var options = new AzureStorageOptions
        {
            ConnectionString = "UseDevelopmentStorage=true",
            AccountName = "myaccount",
            AccountKey = "a2V5"
        };

        Assert.AreEqual("UseDevelopmentStorage=true", options.GetEffectiveConnectionString());
    }

    [TestMethod]
    public void GetEffectiveConnectionString_BuildsFromAccountNameAndAccountKey()
    {
        var options = new AzureStorageOptions
        {
            AccountName = "myaccount",
            AccountKey = "a2V5"
        };

        var connectionString = options.GetEffectiveConnectionString();

        StringAssert.Contains(connectionString, "AccountName=myaccount");
        StringAssert.Contains(connectionString, "AccountKey=a2V5");
        StringAssert.Contains(connectionString, "BlobEndpoint=https://myaccount.blob.core.windows.net");
    }

    [TestMethod]
    public void GetEffectiveConnectionString_ReturnsEmpty_WhenNothingIsConfigured()
    {
        var options = new AzureStorageOptions();

        Assert.AreEqual(string.Empty, options.GetEffectiveConnectionString());
    }
}
