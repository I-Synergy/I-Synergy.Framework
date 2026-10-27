using System.Diagnostics;
using System.Text;

namespace Sample.Blazor.UI.Tests.Infrastructure;

/// <summary>
/// Assembly level fixture that owns the lifecycle of the <c>Sample.Blazor</c> application under test.
/// <para>
/// The suite is self-contained: unless <c>SAMPLE_BLAZOR_BASE_URL</c> is set, <c>dotnet run</c> starts
/// the sample app on a fixed HTTP port before the first test and the whole process tree is killed
/// after the last one. No manually started server is required.
/// </para>
/// <para>
/// The app host log is written next to the test binaries (<c>app-host.log</c>) so that a failed start
/// can be diagnosed after the run.
/// </para>
/// </summary>
[TestClass]
public static class SampleAppHost
{
    /// <summary>Default address the fixture starts the sample app on.</summary>
    public const string DefaultBaseUrl = "http://localhost:5210";

    private const string EnvironmentVariable = "SAMPLE_BLAZOR_BASE_URL";
    private const string SampleProject = "samples/Sample.Blazor/Sample.Blazor.csproj";

    private static readonly TimeSpan StartupTimeout = TimeSpan.FromMinutes(4);

    private static Process? _appProcess;
    private static string _logPath = string.Empty;

    /// <summary>Base URL every test navigates to. Trailing slash is removed.</summary>
    public static string BaseUrl { get; private set; } = DefaultBaseUrl;

    /// <summary>True when the fixture launched the app itself (false when an external instance is used).</summary>
    public static bool OwnsProcess => _appProcess is not null;

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext context)
    {
        var configured = Environment.GetEnvironmentVariable(EnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(configured))
        {
            // Escape hatch used when a developer already has the app running (or wants it started
            // manually under a debugger). The fixture then only verifies the address responds.
            BaseUrl = configured.TrimEnd('/');
            Log($"Using externally hosted app at {BaseUrl} ({EnvironmentVariable} is set); not starting a process.");
            WaitUntilResponding(BaseUrl, TimeSpan.FromSeconds(60), failWhenProcessExits: false);
            return;
        }

        BaseUrl = DefaultBaseUrl;
        _logPath = Path.Combine(AppContext.BaseDirectory, "app-host.log");

        var repoRoot = FindRepositoryRoot();
        var projectPath = Path.Combine(repoRoot, SampleProject.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(projectPath))
            throw new FileNotFoundException($"Sample.Blazor project not found at '{projectPath}'.", projectPath);

        Log($"Repository root: {repoRoot}");
        Log($"Starting: dotnet run --project {projectPath} --no-launch-profile --urls {BaseUrl}");

        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            // The sample app has no HTTPS endpoint in this configuration, so UseHttpsRedirection()
            // finds no port to redirect to and passes the HTTP request through unchanged.
            ArgumentList =
            {
                "run", "--project", projectPath, "--no-launch-profile", "--urls", BaseUrl
            }
        };
        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
        startInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";

        _appProcess = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
        _appProcess.OutputDataReceived += (_, e) => Log(e.Data);
        _appProcess.ErrorDataReceived += (_, e) => Log(e.Data);

        if (!_appProcess.Start())
            throw new InvalidOperationException("Failed to start the 'dotnet run' process for Sample.Blazor.");

        _appProcess.BeginOutputReadLine();
        _appProcess.BeginErrorReadLine();

        try
        {
            WaitUntilResponding(BaseUrl, StartupTimeout, failWhenProcessExits: true);
            Log($"Sample.Blazor is responding at {BaseUrl} (pid {_appProcess.Id}).");
        }
        catch
        {
            TryKillTree();
            throw;
        }
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        if (_appProcess is null)
            return;

        Log("Stopping the Sample.Blazor host process tree.");
        TryKillTree();
        _appProcess.Dispose();
        _appProcess = null;
    }

    private static void TryKillTree()
    {
        if (_appProcess is null)
            return;

        try
        {
            if (!_appProcess.HasExited)
                _appProcess.Kill(entireProcessTree: true);
        }
        catch (Exception ex)
        {
            Log($"Failed to kill the app process: {ex.Message}");
        }
    }

    /// <summary>
    /// Polls the address until a request completes with a non error status code. `dotnet run` builds
    /// the project before hosting, so the first poll can take a while on a cold build.
    /// </summary>
    private static void WaitUntilResponding(string baseUrl, TimeSpan timeout, bool failWhenProcessExits)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        var deadline = DateTime.UtcNow + timeout;
        var lastError = "no response yet";

        while (DateTime.UtcNow < deadline)
        {
            if (failWhenProcessExits && _appProcess is { HasExited: true })
            {
                throw new InvalidOperationException(
                    $"The Sample.Blazor process exited with code {_appProcess.ExitCode} before it started listening " +
                    $"on {baseUrl}.{Environment.NewLine}{TailOfLog()}");
            }

            try
            {
                using var response = client.GetAsync(baseUrl + "/").GetAwaiter().GetResult();
                if ((int)response.StatusCode < 400)
                    return;

                lastError = $"HTTP {(int)response.StatusCode}";
            }
            catch (Exception ex)
            {
                lastError = ex.InnerException?.Message ?? ex.Message;
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        throw new TimeoutException(
            $"The sample app did not respond at {baseUrl} within {timeout.TotalSeconds:F0}s (last error: {lastError})." +
            $"{Environment.NewLine}{TailOfLog()}");
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "I-Synergy.Framework.slnx")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the repository root (a directory containing I-Synergy.Framework.slnx) " +
            $"above '{AppContext.BaseDirectory}'.");
    }

    private static readonly object LogLock = new();

    private static void Log(string? message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        Console.WriteLine($"[Sample.Blazor host] {message}");

        if (string.IsNullOrEmpty(_logPath))
            return;

        try
        {
            lock (LogLock)
            {
                File.AppendAllText(_logPath, message + Environment.NewLine);
            }
        }
        catch
        {
            // Diagnostics must never fail the run.
        }
    }

    private static string TailOfLog()
    {
        const int tailLength = 4000;

        try
        {
            if (string.IsNullOrEmpty(_logPath) || !File.Exists(_logPath))
                return "(no host log captured)";

            var content = new StringBuilder(File.ReadAllText(_logPath));
            return content.Length <= tailLength
                ? content.ToString()
                : "..." + content.ToString(content.Length - tailLength, tailLength);
        }
        catch (Exception ex)
        {
            return $"(host log unavailable: {ex.Message})";
        }
    }
}
