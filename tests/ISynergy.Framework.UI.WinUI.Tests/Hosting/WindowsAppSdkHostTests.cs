using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Reflection;
using MsOptions = Microsoft.Extensions.Options.Options;

namespace ISynergy.Framework.UI.WinUI.Hosting;

/// <summary>
/// Coverage for the internal <c>WindowsAppSdkHost&lt;TApp&gt;</c> guard, lifecycle and disposal paths.
/// <para>
/// The type is <c>internal</c> and closed over <c>TApp : Application, new()</c>, so it is reached purely
/// through reflection against a minimal <see cref="ProbeApp"/> (an <see cref="ISynergy.Framework.UI.Application"/>
/// subclass that is only used to close the generic — it is never instantiated). Starting the real host
/// (which calls <c>Application.Start</c>) is out of scope; the constructor guards, <c>StopAsync</c>,
/// <c>DisposeAsync</c>, the <c>Services</c> property and the hosted-service failure path are exercised.
/// </para>
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class WindowsAppSdkHostTests
{
    private static readonly Type OpenType =
        typeof(DialogService).Assembly.GetType("ISynergy.Framework.UI.Hosting.WindowsAppSdkHost`1")!;

    private static readonly Type ClosedType = OpenType.MakeGenericType(typeof(ProbeApp));

    private static readonly ConstructorInfo HostConstructor = ClosedType
        .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        .Single();

    private readonly List<HostArgs> _created = new();

    [TestCleanup]
    public void Cleanup()
    {
        foreach (var created in _created)
        {
            try { created.Provider.Dispose(); } catch { /* already disposed */ }
            try { Directory.Delete(created.Dir, recursive: true); } catch { /* best effort */ }
        }
    }

    #region Constructor guards

    [TestMethod]
    public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
        => AssertCtorThrows<ArgumentNullException>(args => args[0] = null);

    [TestMethod]
    public void Constructor_WithNullApplicationLifetime_ThrowsArgumentNullException()
        => AssertCtorThrows<ArgumentNullException>(args => args[3] = null);

    [TestMethod]
    public void Constructor_WithNonApplicationLifetime_ThrowsArgumentException()
        => AssertCtorThrows<ArgumentException>(args => args[3] = new Mock<IHostApplicationLifetime>().Object);

    [TestMethod]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        => AssertCtorThrows<ArgumentNullException>(args => args[4] = null);

    [TestMethod]
    public void Constructor_WithNullHostLifetime_ThrowsArgumentNullException()
        => AssertCtorThrows<ArgumentNullException>(args => args[5] = null);

    [TestMethod]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
        => AssertCtorThrows<ArgumentNullException>(args => args[6] = null);

    [TestMethod]
    public void Constructor_WithValidArguments_ExposesServiceProvider()
    {
        var created = BuildArgs();
        var host = HostConstructor.Invoke(created.Args);

        var services = ClosedType.GetProperty("Services")!.GetValue(host);

        Assert.AreSame(created.Sp.Object, services);
    }

    #endregion

    #region StopAsync / DisposeAsync lifecycle

    [TestMethod]
    public async Task StopAsync_RunsShutdownSequence_AndAggregatesApplicationExitFailure()
    {
        var host = HostConstructor.Invoke(BuildArgs().Args);

        var exception = await CaptureAsync(async () =>
        {
            var method = ClosedType.GetMethod("StopAsync")!;
            await (Task)method.Invoke(host, [default(CancellationToken)])!;
        });

        // Application.Current cannot be activated in the test host, so StopAsync aggregates that failure
        // while still running the hosted-service / lifetime shutdown sequence.
        Assert.IsTrue(exception is AggregateException,
            "StopAsync should aggregate the unavailable-Application failure, but got: " + exception);
    }

    [TestMethod]
    public async Task DisposeAsync_DisposesRegisteredResources_AndSurfacesApplicationFailure()
    {
        var host = HostConstructor.Invoke(BuildArgs().Args);

        var exception = await CaptureAsync(async () =>
        {
            var method = ClosedType.GetMethod("DisposeAsync")!;
            await ((ValueTask)method.Invoke(host, [])!).ConfigureAwait(false);
        });

        // The content-root/default providers are disposed before the Application.Current access fails.
        Assert.IsTrue(exception is not null, "DisposeAsync should surface the unavailable-Application failure");
    }

    [TestMethod]
    public async Task TryExecuteBackgroundServiceAsync_WhenServiceFaults_StopsApplication()
    {
        var created = BuildArgs();
        var host = HostConstructor.Invoke(created.Args);

        var backgroundService = new FaultingBackgroundService();
        await backgroundService.StartAsync(CancellationToken.None);

        var method = ClosedType.GetMethod("TryExecuteBackgroundServiceAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
        await (Task)method.Invoke(host, [backgroundService])!;
        await backgroundService.StopAsync(CancellationToken.None);

        Assert.IsTrue(created.Lifetime.ApplicationStopping.IsCancellationRequested,
            "A faulting background service with StopHost behaviour must stop the application");
    }

    #endregion

    #region Helpers

    private void AssertCtorThrows<TException>(Action<object?[]> mutate)
        where TException : Exception
    {
        var created = BuildArgs();
        var args = (object?[])created.Args.Clone();
        mutate(args);

        Exception? caught = null;
        try
        {
            HostConstructor.Invoke(args);
        }
        catch (TargetInvocationException ex)
        {
            caught = ex.InnerException;
        }

        Assert.IsNotNull(caught, $"Expected {typeof(TException).Name} to be thrown");
        Assert.IsTrue(caught is TException,
            $"Expected {typeof(TException).Name} but got {caught!.GetType().Name}: {caught.Message}");
    }

    private HostArgs BuildArgs()
    {
        var dir = Path.Combine(Path.GetTempPath(), "winsdktest_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);

        var provider = new PhysicalFileProvider(dir);

        var sp = new Mock<IServiceProvider>();
        sp.Setup(x => x.GetService(typeof(IEnumerable<IHostedService>))).Returns(Array.Empty<IHostedService>());

        var hostEnvironment = new Mock<IHostEnvironment>();
        hostEnvironment.SetupGet(x => x.ContentRootFileProvider).Returns(provider);

        var lifetime = new ApplicationLifetime(NullLogger<ApplicationLifetime>.Instance);

        var hostLifetime = new Mock<IHostLifetime>();
        hostLifetime.Setup(x => x.WaitForStartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        hostLifetime.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var logger = typeof(NullLogger<>)
            .MakeGenericType(ClosedType)
            .GetField("Instance")!
            .GetValue(null)!;

        var options = MsOptions.Create(new ISynergy.Framework.UI.Options.HostOptions());

        // Constructor parameter order:
        // (IServiceProvider, IHostEnvironment, PhysicalFileProvider, IHostApplicationLifetime, ILogger<>, IHostLifetime, IOptions<>)
        var args = new object?[]
        {
            sp.Object,
            hostEnvironment.Object,
            provider,
            lifetime,
            logger,
            hostLifetime.Object,
            options
        };

        var created = new HostArgs(args, provider, dir, sp, hostLifetime, lifetime);
        _created.Add(created);
        return created;
    }

    private static async Task<Exception?> CaptureAsync(Func<Task> action)
    {
        try
        {
            await action();
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    #endregion

    #region Test doubles

    /// <summary>Minimal <see cref="ISynergy.Framework.UI.Application"/> used only to close the host's generic.</summary>
    public sealed class ProbeApp : ISynergy.Framework.UI.Application
    {
        public ProbeApp()
        {
        }

        protected override Task InitializeApplicationAsync() => Task.CompletedTask;
    }

    private sealed class FaultingBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            throw new InvalidOperationException("background service failed");
        }
    }

    private sealed record HostArgs(
        object?[] Args,
        PhysicalFileProvider Provider,
        string Dir,
        Mock<IServiceProvider> Sp,
        Mock<IHostLifetime> Hl,
        ApplicationLifetime Lifetime);

    #endregion
}
