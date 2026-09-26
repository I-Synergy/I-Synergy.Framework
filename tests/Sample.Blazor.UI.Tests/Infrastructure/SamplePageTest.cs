using System.Runtime.ExceptionServices;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sample.Blazor.UI.Tests.Infrastructure;

/// <summary>
/// Common base for every Playwright test in this suite.
/// <para>
/// This class deliberately does <b>not</b> derive from <c>Microsoft.Playwright.MSTest.PageTest</c>.
/// That type was compiled against MSTest v2 (<c>Microsoft.VisualStudio.TestPlatform.TestFramework</c>)
/// while this project uses MSTest 4.x (<c>MSTest.TestFramework.dll</c>); MSTest matches attributes by
/// exact type identity, so the inherited <c>[TestInitialize]</c>/<c>[TestCleanup]</c> would be silently
/// ignored at run time (build warning MSTEST0082) and <c>Page</c> would never be created. The lifecycle
/// is therefore owned here explicitly.
/// </para>
/// <para>
/// The class provides: the base URL of the app started by <see cref="SampleAppHost"/>, a desktop
/// viewport, collected console errors / page errors / failed requests, a deterministic wait for the
/// Blazor Server circuit to be live before a test interacts with the page, and a circuit-health
/// assertion that runs from teardown for every test (see <see cref="AssertCircuitHealthyAsync"/>).
/// </para>
/// </summary>
public abstract class SamplePageTest
{
    protected const int DesktopWidth = 1280;
    protected const int DesktopHeight = 900;

    /// <summary>
    /// Console fingerprints that prove the Blazor circuit threw or died. The defect this guards against
    /// was an unhandled exception raised from an <c>async void</c> dialog close handler
    /// ("Failed to remove dialog from DialogProvider: the ID '...' doesn't exist in the
    /// DialogServiceProvider.") after which Blazor reported "Connection disconnected." and the page
    /// silently ignored every click while still looking intact.
    /// </summary>
    protected static readonly string[] CircuitFatalFingerprints =
    [
        "InvalidOperationException",
        "Failed to remove dialog",
        "Connection disconnected",
        "Unhandled exception",
        "cannot be reached"
    ];

    /// <summary>
    /// Every console message of every type, not just the errors/warnings split out below. The fatal
    /// exception above is not guaranteed to arrive as <c>console.error</c>, so the circuit scan must
    /// see the whole capture.
    /// </summary>
    private readonly List<string> _consoleAll = [];

    private readonly List<string> _consoleErrors = [];
    private readonly List<string> _consoleWarnings = [];
    private readonly List<string> _pageErrors = [];
    private readonly List<string> _httpErrors = [];
    private readonly List<string> _localRequestFailures = [];
    private readonly List<string> _externalRequestFailures = [];

    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private TaskCompletionSource _circuitFrame = NewSignal();

    protected IBrowserContext Context { get; private set; } = null!;

    protected IPage Page { get; private set; } = null!;

    /// <summary>Injected by MSTest; used for test output.</summary>
    public TestContext TestContext { get; set; } = null!;

    /// <summary>Base URL of the running sample app.</summary>
    protected static string BaseUrl => SampleAppHost.BaseUrl;

    /// <summary>Viewport used unless a test changes it (the responsive tests resize after setup).</summary>
    protected virtual ViewportSize Viewport => new() { Width = DesktopWidth, Height = DesktopHeight };

    [TestInitialize]
    public async Task StartBrowserAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        Context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = BaseUrl,
            IgnoreHTTPSErrors = true,
            Locale = "en-US",
            ViewportSize = Viewport
        });

        Page = await Context.NewPageAsync();
        AttachDiagnostics();
    }

    /// <summary>
    /// Runs <see cref="AssertCircuitHealthyAsync"/> for <b>every</b> test, so a circuit that threw or
    /// disconnected fails the test that broke it even when the test never asserted the global baseline.
    /// The blind spot this closes: every health assertion used to sit on a "happy path" (the dialog's
    /// *open* path), so the state after a dialog *closed* - the path that killed the circuit - was
    /// never observed.
    /// </summary>
    [TestCleanup]
    public async Task StopBrowserAsync()
    {
        Exception? failure = null;

        try
        {
            if (Page is not null)
                await AssertCircuitHealthyAsync("teardown");
        }
        catch (Exception exception)
        {
            failure = exception;
        }

        try
        {
            if (Context is not null && Context.IsClosed == false)
                await Context.CloseAsync();

            if (_browser is not null)
                await _browser.CloseAsync();
        }
        catch (Exception exception)
        {
            failure ??= exception;
        }
        finally
        {
            _playwright?.Dispose();
        }

        // Rethrown after the browser is disposed: the health failure must never leak a browser process.
        // ExceptionDispatchInfo keeps the original stack trace of the assertion that failed.
        if (failure is not null)
            ExceptionDispatchInfo.Capture(failure).Throw();
    }

    private void AttachDiagnostics()
    {
        Page.Console += (_, message) =>
        {
            var line = $"{message.Type}: {message.Text} ({message.Location})";

            // Captured for every message type: the circuit-fatal exception does not have to be
            // reported as console.error, so AssertCircuitHealthyAsync scans this list, not _consoleErrors.
            _consoleAll.Add(line);

            if (string.Equals(message.Type, "error", StringComparison.OrdinalIgnoreCase))
                _consoleErrors.Add(line);
            else if (string.Equals(message.Type, "warning", StringComparison.OrdinalIgnoreCase))
                _consoleWarnings.Add(line);
        };

        Page.PageError += (_, error) => _pageErrors.Add(error);

        Page.Response += (_, response) =>
        {
            if (response.Status >= 400)
                _httpErrors.Add($"HTTP {response.Status} {response.Url}");
        };

        Page.RequestFailed += (_, request) =>
        {
            var line = $"{request.Method} {request.Url} -> {request.Failure}";
            if (IsAppUrl(request.Url))
                _localRequestFailures.Add(line);
            else
                _externalRequestFailures.Add(line);
        };

        Page.WebSocket += (_, socket) =>
        {
            if (!socket.Url.Contains("/_blazor", StringComparison.OrdinalIgnoreCase))
                return;

            // The first frame from the server proves the SignalR circuit completed its handshake,
            // which is the point at which Blazor event handlers are actually wired up.
            socket.FrameReceived += (_, _) => _circuitFrame.TrySetResult();
        };
    }

    protected static ILocatorAssertions Expect(ILocator locator) => Microsoft.Playwright.Assertions.Expect(locator);

    protected static IPageAssertions Expect(IPage page) => Microsoft.Playwright.Assertions.Expect(page);

    /// <summary>
    /// Navigates to <paramref name="path"/> with a full page load and waits until the Blazor Server
    /// circuit has delivered its first frame, so the page is interactive when the test proceeds.
    /// </summary>
    protected async Task<IResponse?> NavigateAsync(string path)
    {
        _circuitFrame = NewSignal();
        var response = await Page.GotoAsync(path);
        await WaitForCircuitAsync();
        return response;
    }

    /// <summary>Waits for the SignalR circuit of the current page load to become live.</summary>
    protected async Task WaitForCircuitAsync()
    {
        await Page.WaitForFunctionAsync("() => !!window.Blazor", null, new() { Timeout = 30_000 });

        var completed = await Task.WhenAny(_circuitFrame.Task, Task.Delay(TimeSpan.FromSeconds(30)));
        if (completed != _circuitFrame.Task)
            throw new TimeoutException(
                $"No frame was received on the /_blazor WebSocket within 30s for {Page.Url}; " +
                "the Blazor Server circuit did not start.");
    }

    /// <summary>Waits for the layout JS to finish its first ResizeObserver pass (desktop/mobile decision).</summary>
    protected async Task WaitForLayoutInitialisedAsync()
    {
        await Page.WaitForFunctionAsync(
            "() => document.querySelector('.fluent-layout')?.fluentLayoutInitialized === true",
            null,
            new() { Timeout = 15_000 });
    }

    /// <summary>True when <paramref name="url"/> belongs to the application under test.</summary>
    protected static bool IsAppUrl(string url) =>
        url.StartsWith(BaseUrl, StringComparison.OrdinalIgnoreCase);

    /// <summary>Renders everything the diagnostics collected; used both for failures and for evidence.</summary>
    protected void ReportDiagnostics(string label)
    {
        Write($"[diagnostics:{label}] url={Page.Url}");
        Write($"[diagnostics:{label}] console errors={_consoleErrors.Count} warnings={_consoleWarnings.Count} " +
              $"page errors={_pageErrors.Count} http>=400={_httpErrors.Count} " +
              $"app request failures={_localRequestFailures.Count} external request failures={_externalRequestFailures.Count}");

        // Evidence that the circuit scan below actually had something to scan (a guard over an empty
        // capture would be vacuous), plus the fingerprints it found.
        Write($"[diagnostics:{label}] full console capture={_consoleAll.Count} message(s), " +
              $"circuit-fatal fingerprints matched={FindCircuitFatalConsoleLines().Count}");

        foreach (var line in _consoleErrors)
            Write($"[diagnostics:{label}] CONSOLE-ERROR {line}");
        foreach (var line in _pageErrors)
            Write($"[diagnostics:{label}] PAGE-ERROR {line}");
        foreach (var line in _httpErrors)
            Write($"[diagnostics:{label}] HTTP-ERROR {line}");
        foreach (var line in _localRequestFailures)
            Write($"[diagnostics:{label}] APP-REQUEST-FAILED {line}");
        foreach (var line in _externalRequestFailures)
            Write($"[diagnostics:{label}] EXTERNAL-REQUEST-FAILED {line}");
        foreach (var line in _consoleWarnings)
            Write($"[diagnostics:{label}] CONSOLE-WARNING {line}");
    }

    /// <summary>
    /// Asserts the global baseline: no uncaught JavaScript error, no console error, no 4xx/5xx for an
    /// application URL, and no Blazor error UI. This is the inline (mid-test) entry point; the same
    /// checks run from teardown for every test through <see cref="AssertCircuitHealthyAsync"/>.
    /// </summary>
    protected async Task AssertBaselineAsync(string label)
    {
        await AssertCircuitHealthyAsync(label);
    }

    /// <summary>
    /// Asserts that the Blazor circuit is still alive and clean, and that the page did not die quietly.
    /// <para>
    /// This is the guard for the *close* side of every interaction. The suite used to assert health only
    /// on happy paths (a dialog that had just opened), so a defect on a dialog's close path - an
    /// unhandled exception in an <c>async void</c> handler - passed green while it tore down the circuit;
    /// the DOM of a dead circuit looks identical to a live one, so nothing in the test output hinted at
    /// it.
    /// </para>
    /// <para>
    /// It therefore checks, in order: no circuit-fatal console output anywhere in the full console
    /// capture (all message types), the Blazor error bar is hidden, and no uncaught JavaScript error,
    /// console error, 4xx/5xx or failed request for the application.
    /// </para>
    /// </summary>
    protected async Task AssertCircuitHealthyAsync(string label)
    {
        ReportDiagnostics(label);

        await AssertBlazorErrorUiHiddenAsync();

        var fatal = FindCircuitFatalConsoleLines();
        Assert.AreEqual(
            0,
            fatal.Count,
            $"{label}: the Blazor circuit threw or disconnected - the page can keep rendering its DOM " +
            $"while silently ignoring every click. Circuit-fatal console line(s):{Environment.NewLine}{Join(fatal)}" +
            $"{Environment.NewLine}Complete console capture ({_consoleAll.Count} line(s)):{Environment.NewLine}{Join(_consoleAll)}");

        Assert.AreEqual(0, _pageErrors.Count, $"{label}: uncaught JavaScript error(s):{Environment.NewLine}{Join(_pageErrors)}");
        Assert.AreEqual(0, _consoleErrors.Count, $"{label}: browser console error(s):{Environment.NewLine}{Join(_consoleErrors)}");
        Assert.AreEqual(0, _httpErrors.Count, $"{label}: 4xx/5xx response(s):{Environment.NewLine}{Join(_httpErrors)}");
        Assert.AreEqual(0, _localRequestFailures.Count, $"{label}: failed request(s) for the app:{Environment.NewLine}{Join(_localRequestFailures)}");
    }

    /// <summary>
    /// Every captured console line (all message types); used by tests that need to dump the console in a
    /// failure diagnostic of their own.
    /// </summary>
    protected IReadOnlyList<string> ConsoleCapture => _consoleAll;

    /// <summary>
    /// The captured console lines that contain a <see cref="CircuitFatalFingerprints"/> entry
    /// (case-insensitive); the lines themselves are reported verbatim by the caller.
    /// </summary>
    protected List<string> FindCircuitFatalConsoleLines()
    {
        return _consoleAll
            .Where(line => CircuitFatalFingerprints.Any(
                fingerprint => line.Contains(fingerprint, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    /// <summary>The Blazor error bar must stay hidden on a healthy page.</summary>
    protected async Task AssertBlazorErrorUiHiddenAsync()
    {
        var errorUi = Page.Locator(SampleDom.BlazorErrorUi);
        await Expect(errorUi).ToBeHiddenAsync();
    }

    /// <summary>Counts elements matching <paramref name="selector"/>.</summary>
    protected Task<int> CountAsync(string selector) => Page.Locator(selector).CountAsync();

    /// <summary>Human readable dump of the navigation structure; used as evidence in the test output.</summary>
    protected async Task<string> DescribeNavigationAsync()
    {
        return await Page.EvaluateAsync<string>(
            """
            () => {
                const nav = document.querySelector('nav.fluent-nav');
                if (!nav) return 'no nav.fluent-nav in the DOM';
                const items = [...nav.querySelectorAll('a.fluent-navitem')];
                return JSON.stringify({
                    navCount: document.querySelectorAll('nav.fluent-nav').length,
                    menuIdCount: document.querySelectorAll('#main_menu').length,
                    itemCount: items.length,
                    activeCount: items.filter(i => i.classList.contains('active')).length,
                    items: items.map(i => ({ text: i.textContent.trim(), href: i.getAttribute('href'), cls: i.className })),
                    navWidth: getComputedStyle(nav).width,
                    navHeight: Math.round(nav.getBoundingClientRect().height)
                });
            }
            """);
    }

    protected void Log(string message) => Write($"[evidence] {message}");

    /// <summary>
    /// Writes a line both to the test output and to <c>evidence.log</c> next to the test binaries, so the
    /// measurements of *passing* tests are still available after the run (dotnet test only prints the
    /// standard output of failing tests).
    /// </summary>
    private void Write(string message)
    {
        Console.WriteLine(message);

        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "evidence.log");
            lock (EvidenceLock)
            {
                File.AppendAllText(path, $"{DateTime.Now:HH:mm:ss.fff} [{TestContext?.TestName ?? "?"}] {message}{Environment.NewLine}");
            }
        }
        catch
        {
            // Diagnostics must never fail a test.
        }
    }

    private static readonly object EvidenceLock = new();

    private static string Join(IEnumerable<string> lines) => string.Join(Environment.NewLine, lines);

    private static TaskCompletionSource NewSignal() => new(TaskCreationOptions.RunContinuationsAsynchronously);
}
