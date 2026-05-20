using ISynergy.Framework.Core.Abstractions.Services;
using Moq;

namespace ISynergy.Framework.Core.Services;

[TestClass]
public class BusyServiceThreadSafetyTests
{
    [TestMethod]
    public async Task StartBusy_ConcurrentCalls_DoesNotThrow()
    {
        var languageService = new Mock<ILanguageService>();
        languageService.Setup(x => x.GetString(It.IsAny<string>())).Returns("Please wait...");

        var busyService = new BusyService(languageService.Object);
        var tasks = new List<Task>();

        for (int i = 0; i < 20; i++)
        {
            tasks.Add(Task.Run(() => busyService.StartBusy()));
            tasks.Add(Task.Run(() => busyService.StopBusy()));
        }

        await Task.WhenAll(tasks);
    }
}
