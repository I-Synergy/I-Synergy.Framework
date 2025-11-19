using ISynergy.Framework.AspNetCore.Monitoring.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;


namespace ISynergy.Framework.AspNetCore.Monitoring.Tests.Hubs;

[TestClass()]
public class MonitorHubTests
{
    private string connectionId = "1";
    private Guid userId = Guid.NewGuid();
    private string userName = "userName";
    private Guid groupName = Guid.NewGuid();

    private readonly Mock<IGroupManager> _mockGroupManager;
    private readonly Mock<HubCallerContext> _mockHubCallerContext;
    private readonly Mock<IHubCallerClients> _mockClients;
    private readonly Mock<IClientProxy> _mockClientProxy;
    private readonly Mock<ILogger<MonitorHub>> _mockLogger;
    private readonly MonitorHub _monitorHub;

    public MonitorHubTests()
    {
        _mockClientProxy = new Mock<IClientProxy>();
        _mockClients = new Mock<IHubCallerClients>();
        _mockGroupManager = new Mock<IGroupManager>();

        _mockLogger = new Mock<ILogger<MonitorHub>>();

        _mockHubCallerContext = new Mock<HubCallerContext>();
        _mockHubCallerContext.Setup(x => x.ConnectionId).Returns(connectionId);
        _mockHubCallerContext.Setup(x => x.User)
            .Returns(() =>
            {
                ClaimsIdentity identity = new(
                    "OAuth",
                    Claims.Username,
                    Claims.Role);

                identity.AddClaim(new Claim(Claims.KeyId, groupName.ToString()));
                identity.AddClaim(new Claim(Claims.Name, "Test"));
                identity.AddClaim(new Claim(Claims.Username, userName));
                identity.AddClaim(new Claim(Claims.Subject, userId.ToString()));

                return new ClaimsPrincipal(identity);
            });

        _mockClients.Setup(x => x.All)
            .Returns(_mockClientProxy.Object);

        _mockClients.Setup(x => x.OthersInGroup(groupName.ToString()))
            .Returns(_mockClientProxy.Object);

        _monitorHub = new MonitorHub(_mockLogger.Object)
        {
            Clients = _mockClients.Object,
            Context = _mockHubCallerContext.Object,
            Groups = _mockGroupManager.Object
        };
    }

    [TestMethod()]
    public async Task OnConnectedAsyncTest()
    {
        await _monitorHub.OnConnectedAsync();

        _mockClients.Verify(x => x.OthersInGroup(groupName.ToString()), Times.Once);
        _mockClients.Verify(x => x.All, Times.Never);
        _mockClientProxy.Verify(x => x.SendCoreAsync(
                "Connected",
                It.Is<object[]>(o => o != null),
                default), Times.Once);
    }

    [TestMethod()]
    public async Task OnDisconnectedAsyncTest()
    {
        await _monitorHub.OnDisconnectedAsync(null);

        _mockClients.Verify(x => x.OthersInGroup(groupName.ToString()), Times.Once);
        _mockClients.Verify(x => x.All, Times.Never);
        _mockClientProxy.Verify(x => x.SendCoreAsync(
                "Disconnected",
                It.Is<object[]>(o => o != null),
                default), Times.Once);
    }
}