# OpenTelemetry Integration Guide

## Overview

The I-Synergy.Framework provides a flexible OpenTelemetry integration that separates instrumentation configuration from exporter configuration. This separation allows for cleaner code organization and better maintainability.

## Key Concepts

- **Instrumentation**: Defines what to collect (which libraries, frameworks, or custom sources to monitor)
- **Exporters**: Defines where to send the collected telemetry data (console, OTLP endpoint, Azure Monitor, etc.)

## Architecture

The OpenTelemetry integration in I-Synergy.Framework consists of:

- `ITelemetryProvider` interface that defines the contract
- `OpenTelemetryProvider` implementation
- Extension methods for different provider builders (Tracer, Meter, Logger)

## Usage Guide

### Basic Setup

```csharp
// In Program.cs or Startup.cs
builder.Logging.AddOpenTelemetry(
    builder.Configuration,
    builder.Environment,
    infoService,
    "Telemetry",
    tracerInstrumentationAction: ConfigureTracingInstrumentation,
    tracerExportersAction: ConfigureTracingExporters,
    meterInstrumentationAction: ConfigureMetricsInstrumentation,
    meterExportersAction: ConfigureMetricsExporters,
    loggerInstrumentationAction: ConfigureLoggingInstrumentation,
    loggerExportersAction: ConfigureLoggingExporters);
```

### Instrumentation Actions
Instrumentation actions should configure what telemetry data to collect.\
These actions should add sources, configure sampling, and set up instrumentation for specific libraries or frameworks.

```csharp
private static void ConfigureTracingInstrumentation(TracerProviderBuilder builder)
{
    // Add sources to collect data from
    builder.AddSource("MyApplicationName");
    
    // Add instrumentation for specific libraries
    builder.AddHttpClientInstrumentation(opts => 
    {
        opts.RecordException = true;
        opts.EnrichWithException = (activity, exception) =>
        {
            activity.SetTag("error.type", exception.GetType().Name);
            activity.SetTag("error.message", exception.Message);
        };
    });
    
    // Add ASP.NET Core instrumentation
    builder.AddAspNetCoreInstrumentation();
}
```

### Exporter Actions
Exporter actions should configure where to send the collected telemetry data.\
These actions should add exporters to different backends or services.

```csharp
private static void ConfigureTracingExporters(TracerProviderBuilder builder)
{
    // Add Azure Monitor exporter
    builder.AddAzureMonitorTraceExporter(options =>
    {
        options.ConnectionString = "your-connection-string";
    });
    
    // Add Jaeger exporter
    builder.AddJaegerExporter(options =>
    {
        options.AgentHost = "localhost";
        options.AgentPort = 6831;
    });
}
```

### Integration Examples
#### Azure Monitor Integration

```csharp
builder.Logging.AddOpenTelemetry(
    builder.Configuration,
    builder.Environment,
    infoService,
    "Telemetry",
    tracerInstrumentationAction: builder =>
    {
        builder.AddSource(infoService.ProductName);
        builder.AddHttpClientInstrumentation();
    },
    tracerExportersAction: builder =>
    {
        var connectionString = builder.Configuration["Telemetry:ConnectionString"];
        if (!string.IsNullOrEmpty(connectionString))
        {
            builder.AddAzureMonitorTraceExporter(options =>
            {
                options.ConnectionString = connectionString;
            });
        }
    },
    meterExportersAction: builder =>
    {
        var connectionString = builder.Configuration["Telemetry:ConnectionString"];
        if (!string.IsNullOrEmpty(connectionString))
        {
            builder.AddAzureMonitorMetricExporter(options =>
            {
                options.ConnectionString = connectionString;
            });
        }
    },
    loggerExportersAction: builder =>
    {
        var connectionString = builder.Configuration["Telemetry:ConnectionString"];
        if (!string.IsNullOrEmpty(connectionString))
        {
            builder.AddOpenTelemetry(options =>
            {
                options.AddAzureMonitorLogExporter(o => 
                    o.ConnectionString = connectionString);
            });
        }
    });
```

#### Sentry Integration

```csharp
builder.Logging.AddOpenTelemetry(
    builder.Configuration,
    builder.Environment,
    infoService,
    "Telemetry",
    tracerInstrumentationAction: builder =>
    {
        builder.AddSource(infoService.ProductName);
        builder.AddSentry();

        SentrySdk.Init(options =>
        {
            builder.Configuration.GetSection("Telemetry").Bind(options);
            options.Environment = builder.Environment.EnvironmentName;
            options.Debug = builder.Environment.IsDevelopment();
            options.ServerName = infoService.ProductName;
            options.Release = infoService.ProductVersion.ToString();
            options.UseOpenTelemetry();
        });
    });

```

### Advanced Configuration
#### Manual Instrumentation
For manual instrumentation, you can inject and use the ActivitySource that's registered by the framework:

```csharp
public class MyService
{
    private readonly ActivitySource _activitySource;

    public MyService(ActivitySource activitySource)
    {
        _activitySource = activitySource;
    }

    public void DoSomething()
    {
        using var activity = _activitySource.StartActivity("DoSomething");
        activity?.SetTag("custom.tag", "value");
        
        // Your code here
    }
}
```

### Custom Resource Attributes
You can add custom resource attributes through the OpenTelemetryOptions:

```json
{
  "Telemetry": {
    "CustomAttributes": {
      "deployment.region": "WestEurope",
      "service.team": "MyTeam"
    }
  }
}
```

These attributes will be added to all telemetry data sent from your application.
