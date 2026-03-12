using ISynergy.Framework.Mail.Microsoft365.Options;
using ISynergy.Framework.Mail.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OptionsX = Microsoft.Extensions.Options.Options;

namespace ISynergy.Framework.Mail.Microsoft365.Services;

public class MailServiceTests
{
    private readonly Microsoft365MailOptions _mailOptions;

    private Mock<ILogger<Microsoft365MailService>> _mockLogger;

    public MailServiceTests()
    {
        _mockLogger = new Mock<ILogger<Microsoft365MailService>>();

        Mock<Microsoft365MailOptions> configMock = new();
        _mailOptions = configMock.Object;
        _mailOptions.ClientId = "";
        _mailOptions.ClientSecret = "";
        _mailOptions.TenantId = "";
        _mailOptions.Sender = "Support Test";
        _mailOptions.EmailAddress = "support@i-synergy.nl";
        _mailOptions.Scopes = ["https://graph.microsoft.com/.default"];
    }

    [TestMethod()]
    public async Task SendEmailAsyncTest()
    {
        Microsoft365MailService service = new(OptionsX.Create(_mailOptions), _mockLogger.Object);
        MailMessage message = new(
            ["ismail.hassani@i-synergy.nl"],
            "Test subject",
            "Test body",
            false);
        ;

        Assert.IsTrue(await service.SendEmailAsync(message));
    }

    [TestMethod()]
    public async Task SendEmailWithCopyAsyncTest()
    {
        Microsoft365MailService service = new(OptionsX.Create(_mailOptions), _mockLogger.Object);
        MailMessage message = new(
            ["ismail.hassani@i-synergy.nl"],
            "Test subject",
            "Test body",
            true);
        ;

        Assert.IsTrue(await service.SendEmailAsync(message));
    }

    [TestMethod()]
    public async Task SendEmailWithFromAsyncTest()
    {
        Microsoft365MailService service = new(OptionsX.Create(_mailOptions), _mockLogger.Object);
        MailMessage message = new(
            ["support@i-synergy.nl"],
            "Test subject",
            "Test body",
            false);
        ;

        Assert.IsTrue(await service.SendEmailAsync(message));
    }

    [TestMethod()]
    public async Task SendEmailWithFromAndCopyAsyncTest()
    {
        Microsoft365MailService service = new(OptionsX.Create(_mailOptions), _mockLogger.Object);
        MailMessage message = new(
            ["support@i-synergy.nl"],
            "Test subject",
            "Test body",
            true);
        ;

        Assert.IsTrue(await service.SendEmailAsync(message));
    }
}