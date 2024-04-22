using ISynergy.Framework.AspNetCore.Monitoring.Hubs;
using ISynergy.Framework.AspNetCore.Monitoring.Services;
using ISynergy.Framework.Monitoring.Abstractions.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.AspNetCore.Monitoring.Tests.Services;

[TestClass]
public class MonitorServiceTests
{
    private const string _channel = "channel";

    private readonly IMonitorService<object> _monitorService;

    public MonitorServiceTests()
    {
        Mock<IHubContext<MonitorHub>> contextMock = new();
        contextMock.Setup(x => x.Clients)
            .Returns(() => new Mock<IHubClients>().Object);
        contextMock.Setup(x => x.Groups)
            .Returns(() => new Mock<IGroupManager>().Object);
        contextMock.Setup(x => x.Clients.Group(_channel))
            .Returns(() => new Mock<IClientProxy>().Object);

        IHubContext<MonitorHub> context = contextMock.Object;
        context.Groups.AddToGroupAsync("0", _channel);

        _monitorService = new MonitorService<object>(context);
    }

    [TestMethod]
    public async Task PublishTestAsync()
    {
        await _monitorService.PublishAsync(_channel, "eventName", null);
    }

    [TestMethod]
    public async Task PublishInvalidChannelTestAsync()
    {
        await _monitorService.PublishAsync("invalidchannel", "eventName", null);
    }
}
