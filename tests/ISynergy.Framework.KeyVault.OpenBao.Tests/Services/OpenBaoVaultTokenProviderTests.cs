using ISynergy.Framework.KeyVault.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.KeyVault.OpenBao.Tests.Services;

[TestClass]
public class OpenBaoVaultTokenProviderTests
{
    [TestMethod]
    public async Task GetTokenAsync_ReturnsTokenFromConfig_WhenConfigured()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["KeyVaultOptions:Token"] = "my-token"
            })
            .Build();

        var provider = new OpenBaoVaultTokenProvider(config);
        var token = provider.GetToken();

        Assert.AreEqual("my-token", token);
    }

    [TestMethod]
    public async Task GetTokenAsync_ThrowsIfStateFileNotFound_WhenTokenNotConfigured()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["VaultInitializer:StateDirectory"] = "/nonexistent/path/that/does/not/exist"
            })
            .Build();

        var provider = new OpenBaoVaultTokenProvider(config);

        try
        {
            provider.GetToken();
            Assert.Fail("Expected InvalidOperationException was not thrown.");
        }
        catch (InvalidOperationException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task GetTokenAsync_FallsBackToStateFile_WhenTokenNotConfigured()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var stateFile = Path.Combine(tempDir, "vault-state.json");
        await File.WriteAllTextAsync(stateFile, """{"RootToken":"file-token","Keys":["key1"]}""");

        try
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["VaultInitializer:StateDirectory"] = tempDir
                })
                .Build();

            var provider = new OpenBaoVaultTokenProvider(config);
            var token = provider.GetToken();

            Assert.AreEqual("file-token", token);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [TestMethod]
    public async Task GetTokenAsync_ExpandsTildePrefix_InStateDirectory()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var relPath = Path.Combine(".test-vault-state-" + Guid.NewGuid().ToString("N"));
        var fullDir = Path.Combine(home, relPath);
        Directory.CreateDirectory(fullDir);
        var stateFile = Path.Combine(fullDir, "vault-state.json");
        await File.WriteAllTextAsync(stateFile, """{"RootToken":"home-token","Keys":[]}""");

        try
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["VaultInitializer:StateDirectory"] = $"~/{relPath}"
                })
                .Build();

            var provider = new OpenBaoVaultTokenProvider(config);
            var token = provider.GetToken();

            Assert.AreEqual("home-token", token);
        }
        finally
        {
            Directory.Delete(fullDir, recursive: true);
        }
    }
}
