using ISynergy.Framework.KeyVault.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VaultSharp;
using VaultSharp.V1;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines;
using VaultSharp.V1.SecretsEngines.KeyValue;
using VaultSharp.V1.SecretsEngines.KeyValue.V2;

namespace ISynergy.Framework.KeyVault.OpenBao.Tests.Services;

[TestClass]
public class OpenBaoKeyVaultServiceTests
{
    private Mock<IVaultClient> _mockClient = null!;
    private Mock<IKeyValueSecretsEngineV2> _mockKvV2 = null!;
    private OpenBaoKeyVaultService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        _mockKvV2 = new Mock<IKeyValueSecretsEngineV2>();

        var mockKvEngine = new Mock<IKeyValueSecretsEngine>();
        mockKvEngine.SetupGet(e => e.V2).Returns(_mockKvV2.Object);

        var mockSecrets = new Mock<ISecretsEngine>();
        mockSecrets.SetupGet(s => s.KeyValue).Returns(mockKvEngine.Object);

        var mockV1 = new Mock<IVaultClientV1>();
        mockV1.SetupGet(v => v.Secrets).Returns(mockSecrets.Object);

        _mockClient = new Mock<IVaultClient>();
        _mockClient.SetupGet(c => c.V1).Returns(mockV1.Object);

        _service = new OpenBaoKeyVaultService(_mockClient.Object);
    }

    [TestMethod]
    public async Task GetSecretAsync_ReturnsExpectedData()
    {
        var secretData = new Dictionary<string, string> { ["key"] = "value" };
        var secretDataWrapper = new SecretData<Dictionary<string, string>> { Data = secretData };
        var response = new Secret<SecretData<Dictionary<string, string>>>
        {
            Data = secretDataWrapper
        };

        _mockKvV2
            .Setup(kv => kv.ReadSecretAsync<Dictionary<string, string>>(
                "test/path", It.IsAny<int?>(), "mymount", It.IsAny<string>()))
            .ReturnsAsync(response);

        var result = await _service.GetSecretAsync("test/path", "mymount");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("value", result["key"]);
    }

    [TestMethod]
    public async Task SetSecretAsync_CallsWriteSecretAsync()
    {
        IDictionary<string, object> data = new Dictionary<string, object> { ["key"] = "value" };

        _mockKvV2
            .Setup(kv => kv.WriteSecretAsync(
                "test/path", data, It.IsAny<int?>(), "mymount"))
            .ReturnsAsync(new Secret<CurrentSecretMetadata> { Data = new CurrentSecretMetadata() });

        await _service.SetSecretAsync("test/path", data, "mymount");

        _mockKvV2.Verify(
            kv => kv.WriteSecretAsync(
                "test/path", data, It.IsAny<int?>(), "mymount"),
            Times.Once);
    }

    [TestMethod]
    public async Task ListPathsAsync_ReturnsKeys()
    {
        var listResult = new Secret<ListInfo>
        {
            Data = new ListInfo { Keys = ["key1", "key2"] }
        };

        _mockKvV2
            .Setup(kv => kv.ReadSecretPathsAsync("base/path", "mymount", It.IsAny<string>()))
            .ReturnsAsync(listResult);

        var paths = await _service.ListPathsAsync("base/path", "mymount");

        Assert.AreEqual(2, paths.Count);
        CollectionAssert.Contains(paths.ToList(), "key1");
        CollectionAssert.Contains(paths.ToList(), "key2");
    }
}
