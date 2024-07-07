﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
public sealed class WindowsAppSdkHostBuilder<TApp> : IHostBuilder
    where TApp : Application, new()
{
    private readonly List<Action<IConfigurationBuilder>> _configureHostConfigActions = new();
    private readonly List<Action<HostBuilderContext, IConfigurationBuilder>> _configureAppConfigActions = new();
    private readonly List<Action<HostBuilderContext, IServiceCollection>> _configureServicesActions = new();
    private readonly List<IConfigureContainerAdapter> _configureContainerActions = new();
    private IServiceProviderFactory<IServiceCollection> _serviceProviderFactory = new DefaultServiceProviderFactory();
    private bool _hostBuilt;
    private IConfiguration _hostConfiguration;
    private IConfiguration _appConfiguration;
    private HostBuilderContext _hostBuilderContext;
    private HostingEnvironment _hostingEnvironment;
    private IServiceProvider _appServices;
    private PhysicalFileProvider _defaultProvider;

    /// <summary>
    /// A central location for sharing state between components during the host building process.
    /// </summary>
    public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

    /// <summary>
    /// Set up the configuration for the builder itself. This will be used to initialize the <see cref="IHostEnvironment"/>
    /// for use later in the build process. This can be called multiple times and the results will be additive.
    /// </summary>
    /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
    /// to construct the <see cref="IConfiguration"/> for the host.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        _configureHostConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
        return this;
    }

    /// <summary>
    /// Sets up the configuration for the remainder of the build process and application. This can be called multiple times and
    /// the results will be additive. The results will be available at <see cref="HostBuilderContext.Configuration"/> for
    /// subsequent operations, as well as in <see cref="IHost.Services"/>.
    /// </summary>
    /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
    /// to construct the <see cref="IConfiguration"/> for the host.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        _configureAppConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
        return this;
    }

    /// <summary>
    /// Adds services to the container. This can be called multiple times and the results will be additive.
    /// </summary>
    /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
    /// to construct the <see cref="IConfiguration"/> for the host.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        _configureServicesActions.Add(
            (_, collection) =>
            {
                collection.AddSingleton<TApp>();
            });
        _configureServicesActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
        return this;
    }

    /// <inheritdoc />
    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(
        IServiceProviderFactory<TContainerBuilder> factory
    )
    {
        if (factory is IServiceProviderFactory<IServiceCollection> spf)
        {
            _serviceProviderFactory = spf;
        }
        return this;
    }

    /// <inheritdoc />
    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(
        Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory
    )
    {
        if (factory(_hostBuilderContext) is IServiceProviderFactory<IServiceCollection> spf)
        {
            _serviceProviderFactory = spf;
        }

        return this;
    }

    /// <inheritdoc />
    public IHostBuilder ConfigureContainer<TContainerBuilder>(
        Action<HostBuilderContext, TContainerBuilder> configureDelegate
    )
    {
        if (_serviceProviderFactory is TContainerBuilder builder)
        {
            configureDelegate(_hostBuilderContext, builder);
        }

        return this;
    }

    /// <summary>
    /// Run the given actions to initialize the host. This can only be called once.
    /// </summary>
    /// <returns>An initialized <see cref="IHost"/></returns>
    public IHost Build()
    {
        if (_hostBuilt)
        {
            throw new InvalidOperationException(ServiceLocator.Default.GetInstance<ILanguageService>().GetString("BuildCalled"));
        }
        _hostBuilt = true;

        // REVIEW: If we want to raise more events outside of these calls then we will need to
        // stash this in a field.
        using var diagnosticListener = new DiagnosticListener("Microsoft.Extensions.Hosting");
        const string hostBuildingEventName = "HostBuilding";
        const string hostBuiltEventName = "HostBuilt";

        if (diagnosticListener.IsEnabled() && diagnosticListener.IsEnabled(hostBuildingEventName))
        {
            Write(diagnosticListener, hostBuildingEventName, this);
        }

        BuildHostConfiguration();
        CreateHostingEnvironment();
        CreateHostBuilderContext();
        BuildAppConfiguration();
        CreateServiceProvider();

        var host = _appServices.GetRequiredService<IHost>();
        if (diagnosticListener.IsEnabled() && diagnosticListener.IsEnabled(hostBuiltEventName))
        {
            Write(diagnosticListener, hostBuiltEventName, host);
        }

        return host;
    }

    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
        Justification = "The values being passed into Write are being consumed by the application already.")]
    private static void Write<T>(
        DiagnosticSource diagnosticSource,
        string name,
        T value)
    {
        diagnosticSource.Write(name, value);
    }

    private void BuildHostConfiguration()
    {
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(); // Make sure there's some default storage since there are no default providers

        foreach (var buildAction in _configureHostConfigActions.EnsureNotNull())
        {
            buildAction(configBuilder);
        }
        _hostConfiguration = configBuilder.Build();
    }

    private void CreateHostingEnvironment()
    {
        _hostingEnvironment = new HostingEnvironment
        {
            ApplicationName = _hostConfiguration[HostDefaults.ApplicationKey],
            EnvironmentName = _hostConfiguration[HostDefaults.EnvironmentKey] ?? Environments.Production,
            ContentRootPath = ResolveContentRootPath(_hostConfiguration[HostDefaults.ContentRootKey], AppContext.BaseDirectory),
        };

        if (string.IsNullOrEmpty(_hostingEnvironment.ApplicationName))
        {
            // Note GetEntryAssembly returns null for the net4x console test runner.
            _hostingEnvironment.ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name;
        }

        _hostingEnvironment.ContentRootFileProvider = _defaultProvider = new PhysicalFileProvider(_hostingEnvironment.ContentRootPath);
    }

    private string ResolveContentRootPath(string contentRootPath, string basePath)
    {
        if (string.IsNullOrEmpty(contentRootPath))
        {
            return basePath;
        }
        if (Path.IsPathRooted(contentRootPath))
        {
            return contentRootPath;
        }
        return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
    }

    private void CreateHostBuilderContext()
    {
        _hostBuilderContext = new HostBuilderContext(Properties)
        {
            HostingEnvironment = _hostingEnvironment,
            Configuration = _hostConfiguration
        };
    }

    private void BuildAppConfiguration()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(_hostingEnvironment.ContentRootPath)
            .AddConfiguration(_hostConfiguration, shouldDisposeConfiguration: true);

        foreach (var buildAction in _configureAppConfigActions.EnsureNotNull())
        {
            buildAction(_hostBuilderContext, configBuilder);
        }
        _appConfiguration = configBuilder.Build();
        _hostBuilderContext.Configuration = _appConfiguration;
    }

    private void CreateServiceProvider()
    {
        var services = new ServiceCollection();
#pragma warning disable CS0618 // Type or member is obsolete
        services.AddSingleton<IHostingEnvironment>(_hostingEnvironment);
#pragma warning restore CS0618 // Type or member is obsolete
        services.AddSingleton<IHostEnvironment>(_hostingEnvironment);
        services.AddSingleton(_hostBuilderContext);
        // register configuration as factory to make it dispose with the service provider
        services.AddSingleton(_ => _appConfiguration);
#pragma warning disable CS0618 // Type or member is obsolete
        services.AddSingleton(s => (IApplicationLifetime)s.GetService<IHostApplicationLifetime>());
#pragma warning restore CS0618 // Type or member is obsolete
        services.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();

        AddLifetime(services);

        services.AddSingleton<IHost>(_ => new WindowsAppSdkHost<TApp>(_appServices,
            _hostingEnvironment,
            _defaultProvider,
            _appServices.GetRequiredService<IHostApplicationLifetime>(),
            _appServices.GetRequiredService<ILogger<WindowsAppSdkHost<TApp>>>(),
            _appServices.GetRequiredService<IHostLifetime>(),
            _appServices.GetRequiredService<IOptions<HostOptions>>())
        );
        services.AddOptions().Configure<HostOptions>(options => { options.Initialize(_hostConfiguration); });
        services.AddLogging();

        foreach (var configureServicesAction in _configureServicesActions.EnsureNotNull())
        {
            configureServicesAction(_hostBuilderContext, services);
        }

        var containerBuilder = _serviceProviderFactory.CreateBuilder(services);

        foreach (var containerAction in _configureContainerActions.EnsureNotNull())
        {
            containerAction.ConfigureContainer(_hostBuilderContext, containerBuilder);
        }

        _appServices = _serviceProviderFactory.CreateServiceProvider(containerBuilder);

        if (_appServices == null)
        {
            throw new InvalidOperationException(ServiceLocator.Default.GetInstance<ILanguageService>().GetString("NullIServiceProvider"));
        }

        // resolve configuration explicitly once to mark it as resolved within the
        // service provider, ensuring it will be properly disposed with the provider
        _ = _appServices.GetService<IConfiguration>();
    }
    private static void AddLifetime(ServiceCollection services)
    {
        services.AddSingleton<IHostLifetime, ConsoleLifetime>();
    }
}

internal interface IConfigureContainerAdapter
{
    void ConfigureContainer(HostBuilderContext hostContext, object containerBuilder);
}
