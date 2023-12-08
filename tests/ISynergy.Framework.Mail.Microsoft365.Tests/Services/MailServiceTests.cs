using ISynergy.Framework.Mail.Models;
using ISynergy.Framework.Mail.Options;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using OptionsX = Microsoft.Extensions.Options.Options;

namespace ISynergy.Framework.Mail.Services.Tests;

public class MailServiceTests
{
    private readonly MailOptions _mailOptions;

    public MailServiceTests()
    {
        Mock<MailOptions> configMock = new();
        _mailOptions = configMock.Object;
        _mailOptions.ClientId = "";
        _mailOptions.ClientSecret = "";
        _mailOptions.TenantId = "";
        _mailOptions.Sender = "Support Test";
        _mailOptions.EmailAddress = "support@i-synergy.nl";
        _mailOptions.Scopes = new string[] { "https://graph.microsoft.com/.default" };
    }

    [TestMethod()]
    public async Task SendEmailAsyncTest()
    {
        MailService service = new(OptionsX.Create(_mailOptions), new LoggerFactory().CreateLogger<MailServiceTests>());
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
        MailService service = new(OptionsX.Create(_mailOptions), new LoggerFactory().CreateLogger<MailServiceTests>());
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
        MailService service = new(OptionsX.Create(_mailOptions), new LoggerFactory().CreateLogger<MailServiceTests>());
        MailMessage message = new(
            "info@i-synergy.nl",
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
        MailService service = new(OptionsX.Create(_mailOptions), new LoggerFactory().CreateLogger<MailServiceTests>());
        MailMessage message = new(
            "info@i-synergy.nl",
            ["support@i-synergy.nl"],
            "Test subject",
            "Test body",
            true);
        ;

        Assert.IsTrue(await service.SendEmailAsync(message));
    }
}