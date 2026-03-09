using ISynergy.Framework.KeyVault.DataProtection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Xml.Linq;
using VaultSharp;
using VaultSharp.V1;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines;
using VaultSharp.V1.SecretsEngines.KeyValue;
using VaultSharp.V1.SecretsEngines.KeyValue.V2;

namespace ISynergy.Framework.KeyVault.OpenBao.Tests.DataProtection;

[TestClass]
public class OpenBaoDataProtectionRepositoryTests
{
    private Mock<IVaultClient> _mockClient = null!;
    private Mock<IKeyValueSecretsEngineV2> _mockKvV2 = null!;
    private OpenBaoDataProtectionRepository _repository = null!;

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

        _repository = new OpenBaoDataProtectionRepository(
            _mockClient.Object,
            "isynergy/dataprotection",
            "secret",
            NullLogger<OpenBaoDataProtectionRepository>.Instance);
    }

    [TestMethod]
    public void GetAllElements_ListsAndReadsEachKey()
    {
        var listResult = new Secret<ListInfo>
        {
            Data = new ListInfo { Keys = ["key-1"] }
        };
        var xml = "<key id=\"1\" version=\"1\"><creationDate>2024-01-01T00:00:00Z</creationDate></key>";
        var secretDataWrapper = new SecretData<Dictionary<string, string>>
        {
            Data = new Dictionary<string, string> { ["xml"] = xml }
        };
        var secretResult = new Secret<SecretData<Dictionary<string, string>>>
        {
            Data = secretDataWrapper
        };

        _mockKvV2
            .Setup(kv => kv.ReadSecretPathsAsync("isynergy/dataprotection", "secret", It.IsAny<string>()))
            .ReturnsAsync(listResult);
        _mockKvV2
            .Setup(kv => kv.ReadSecretAsync<Dictionary<string, string>>(
                "isynergy/dataprotection/key-1", It.IsAny<int?>(), "secret", It.IsAny<string>()))
            .ReturnsAsync(secretResult);

        var elements = _repository.GetAllElements();

        Assert.AreEqual(1, elements.Count);
        Assert.AreEqual("key", elements.First().Name.LocalName);
    }

    [TestMethod]
    public void GetAllElements_SkipsInvalidXml_Gracefully()
    {
        var listResult = new Secret<ListInfo>
        {
            Data = new ListInfo { Keys = ["bad-key"] }
        };
        var secretDataWrapper = new SecretData<Dictionary<string, string>>
        {
            Data = new Dictionary<string, string> { ["xml"] = "NOT VALID XML <<<<" }
        };
        var secretResult = new Secret<SecretData<Dictionary<string, string>>>
        {
            Data = secretDataWrapper
        };

        _mockKvV2
            .Setup(kv => kv.ReadSecretPathsAsync("isynergy/dataprotection", "secret", It.IsAny<string>()))
            .ReturnsAsync(listResult);
        _mockKvV2
            .Setup(kv => kv.ReadSecretAsync<Dictionary<string, string>>(
                "isynergy/dataprotection/bad-key", It.IsAny<int?>(), "secret", It.IsAny<string>()))
            .ReturnsAsync(secretResult);

        // Should not throw — invalid XML is logged and skipped
        var elements = _repository.GetAllElements();
        Assert.AreEqual(0, elements.Count);
    }

    [TestMethod]
    public void StoreElement_WritesSerializedXmlToVault()
    {
        var element = new XElement("key", new XAttribute("id", "test"));

        _mockKvV2
            .Setup(kv => kv.WriteSecretAsync(
                "isynergy/dataprotection/friendly",
                It.Is<IDictionary<string, object>>(d => d.ContainsKey("xml")),
                It.IsAny<int?>(),
                "secret"))
            .ReturnsAsync(new Secret<CurrentSecretMetadata> { Data = new CurrentSecretMetadata() });

        _repository.StoreElement(element, "friendly");

        _mockKvV2.Verify(
            kv => kv.WriteSecretAsync(
                "isynergy/dataprotection/friendly",
                It.Is<IDictionary<string, object>>(d => d.ContainsKey("xml")),
                It.IsAny<int?>(),
                "secret"),
            Times.Once);
    }
}
