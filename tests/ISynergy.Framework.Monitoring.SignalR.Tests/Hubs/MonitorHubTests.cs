using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ClaimTypes = ISynergy.Framework.Core.Constants.ClaimTypes;


namespace ISynergy.Framework.Monitoring.Hubs.Tests;

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
    private readonly MonitorHub _monitorHub;

    public MonitorHubTests()
    {
        _mockClientProxy = new Mock<IClientProxy>();
        _mockClients = new Mock<IHubCallerClients>();
        _mockGroupManager = new Mock<IGroupManager>();

        _mockHubCallerContext = new Mock<HubCallerContext>();
        _mockHubCallerContext.Setup(x => x.ConnectionId).Returns(connectionId);
        _mockHubCallerContext.Setup(x => x.User)
            .Returns(() =>
            {
                ClaimsIdentity identity = new(
                    "OAuth",
                    ClaimTypes.UserNameType,
                    ClaimTypes.RoleType);

                identity.AddClaim(new Claim(ClaimTypes.AccountIdType, groupName.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.AccountDescriptionType, "Test"));
                identity.AddClaim(new Claim(ClaimTypes.UserNameType, userName));
                identity.AddClaim(new Claim(ClaimTypes.UserIdType, userId.ToString()));

                return new ClaimsPrincipal(identity);
            });

        _mockClients.Setup(x => x.All)
            .Returns(_mockClientProxy.Object);

        _mockClients.Setup(x => x.OthersInGroup(groupName.ToString()))
            .Returns(_mockClientProxy.Object);

        _monitorHub = new MonitorHub(
            new Mock<ILogger<MonitorHub>>().Object)
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