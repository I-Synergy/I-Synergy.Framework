using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HostOptions = ISynergy.Framework.UI.Options.HostOptions;

namespace ISynergy.Framework.UI.Hosting;

/// <summary>
/// A program initialization utility.
/// </summary>
public sealed class WindowsAppSdkHostBuilder<TApp> : IHostApplicationBuilder
    where TApp : Application, new()
{
    private readonly ConfigurationManager _configuration = new();
    private readonly ServiceCollection _services = new();
    private readonly HostingEnvironment _hostingEnvironment;
    private IServiceProviderFactory<IServiceCollection> _serviceProviderFactory = new DefaultServiceProviderFactory();
    private bool _hostBuilt;
    private IServiceProvider _appServices = null!;
    private readonly PhysicalFileProvider _defaultProvider;

    /// <inheritdoc />
    public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

    /// <inheritdoc />
    public IConfigurationManager Configuration => _configuration;

    /// <inheritdoc />
    public IHostEnvironment Environment => _hostingEnvironment;

    /// <inheritdoc />
    public ILoggingBuilder Logging { get; }

    /// <inheritdoc />
    public IMetricsBuilder Metrics { get; }

    /// <inheritdoc />
    public IServiceCollection Services => _services;

    /// <summary>
    /// Initializes a new instance of <see cref="WindowsAppSdkHostBuilder{TApp}"/>.
    /// </summary>
    public WindowsAppSdkHostBuilder()
    {
        _hostingEnvironment = new HostingEnvironment
        {
            ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name,
            EnvironmentName = System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environments.Production,
            ContentRootPath = AppContext.BaseDirectory,
        };
        _hostingEnvironment.ContentRootFileProvider = _defaultProvider = new PhysicalFileProvider(_hostingEnvironment.ContentRootPath);

        _services.AddSingleton<TApp>();
#pragma warning disable CS0618
        _services.AddSingleton<IHostingEnvironment>(_hostingEnvironment);
#pragma warning restore CS0618
        _services.AddSingleton<IHostEnvironment>(_hostingEnvironment);
        _services.AddSingleton<IConfiguration>(_ => _configuration);
#pragma warning disable CS0618
        _services.AddSingleton(s => (IApplicationLifetime)s.GetService<IHostApplicationLifetime>()!);
#pragma warning restore CS0618
        _services.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();
        _services.AddSingleton<IHostLifetime, ConsoleLifetime>();
        _services.AddOptions().Configure<HostOptions>(options => options.Initialize(_configuration));
        _services.AddLogging();

        Logging = new LoggingBuilder(_services);
        Metrics = new MetricsBuilder(_services);
    }

    /// <inheritdoc />
    public void ConfigureContainer<TContainerBuilder>(
        IServiceProviderFactory<TContainerBuilder> factory,
        Action<TContainerBuilder>? configure = null)
    {
        if (factory is IServiceProviderFactory<IServiceCollection> spf)
        {
            _serviceProviderFactory = spf;
        }
    }

    /// <summary>
    /// Builds and returns an initialized <see cref="IHost"/>. This can only be called once.
    /// </summary>
    public IHost Build()
    {
        if (_hostBuilt)
            throw new InvalidOperationException("Build can only be called once.");
        _hostBuilt = true;

        using var diagnosticListener = new DiagnosticListener("Microsoft.Extensions.Hosting");
        const string hostBuildingEventName = "HostBuilding";
        const string hostBuiltEventName = "HostBuilt";

        if (diagnosticListener.IsEnabled() && diagnosticListener.IsEnabled(hostBuildingEventName))
            Write(diagnosticListener, hostBuildingEventName, this);

        _services.AddSingleton<IHost>(_ => new WindowsAppSdkHost<TApp>(
            _appServices,
            _hostingEnvironment,
            _defaultProvider,
            _appServices.GetRequiredService<IHostApplicationLifetime>(),
            _appServices.GetRequiredService<ILogger<WindowsAppSdkHost<TApp>>>(),
            _appServices.GetRequiredService<IHostLifetime>(),
            _appServices.GetRequiredService<IOptions<HostOptions>>()));

        var containerBuilder = _serviceProviderFactory.CreateBuilder(_services);
        _appServices = _serviceProviderFactory.CreateServiceProvider(containerBuilder)
            ?? throw new InvalidOperationException("Service provider factory returned null.");

        _ = _appServices.GetService<IConfiguration>();

        var host = _appServices.GetRequiredService<IHost>();

        if (diagnosticListener.IsEnabled() && diagnosticListener.IsEnabled(hostBuiltEventName))
            Write(diagnosticListener, hostBuiltEventName, host);

        return host;
    }

    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
        Justification = "The values being passed into Write are being consumed by the application already.")]
    private static void Write<T>(DiagnosticSource diagnosticSource, string name, T value)
        => diagnosticSource.Write(name, value);

    private sealed class LoggingBuilder(IServiceCollection services) : ILoggingBuilder
    {
        public IServiceCollection Services { get; } = services;
    }

    private sealed class MetricsBuilder(IServiceCollection services) : IMetricsBuilder
    {
        public IServiceCollection Services { get; } = services;
    }
}
