using ISynergy.Framework.KeyVault.OpenBao.Services;
using ISynergy.Framework.KeyVault.Options;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.KeyVault.OpenBao.Tests.Services;

[TestClass]
public class OpenBaoVaultTokenProviderTests
{
    private static OpenBaoVaultTokenProvider CreateProvider(string? token = null, string? stateDirectory = null)
    {
        var keyVaultOptions = Microsoft.Extensions.Options.Options.Create(new KeyVaultOptions { Token = token ?? string.Empty });
        var vaultStateOptions = Microsoft.Extensions.Options.Options.Create(new VaultStateOptions { StateDirectory = stateDirectory ?? ".secrets/vault-state" });
        return new OpenBaoVaultTokenProvider(keyVaultOptions, vaultStateOptions);
    }

    [TestMethod]
    public void GetToken_ReturnsTokenFromOptions_WhenConfigured()
    {
        var provider = CreateProvider(token: "my-token");
        var token = provider.GetToken();

        Assert.AreEqual("my-token", token);
    }

    [TestMethod]
    public void GetToken_ThrowsIfStateFileNotFound_WhenTokenNotConfigured()
    {
        var provider = CreateProvider(stateDirectory: "/nonexistent/path/that/does/not/exist");

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
    public async Task GetToken_FallsBackToStateFile_WhenTokenNotConfigured()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var stateFile = Path.Combine(tempDir, "vault-state.json");
        await File.WriteAllTextAsync(stateFile, """{"RootToken":"file-token","Keys":["key1"]}""");

        try
        {
            var provider = CreateProvider(stateDirectory: tempDir);
            var token = provider.GetToken();

            Assert.AreEqual("file-token", token);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [TestMethod]
    public async Task GetToken_ExpandsTildePrefix_InStateDirectory()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var relPath = ".test-vault-state-" + Guid.NewGuid().ToString("N");
        var fullDir = Path.Combine(home, relPath);
        Directory.CreateDirectory(fullDir);
        var stateFile = Path.Combine(fullDir, "vault-state.json");
        await File.WriteAllTextAsync(stateFile, """{"RootToken":"home-token","Keys":[]}""");

        try
        {
            var provider = CreateProvider(stateDirectory: $"~/{relPath}");
            var token = provider.GetToken();

            Assert.AreEqual("home-token", token);
        }
        finally
        {
            Directory.Delete(fullDir, recursive: true);
        }
    }
}
