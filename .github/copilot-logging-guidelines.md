# Structured Logging Implementation

This document provides an overview of the structured logging implementation.

## Overview

The structured logging system has been designed to provide comprehensive, consistent, and contextual logging throughout the application. It includes:

- **Event IDs**: Numeric identifiers with descriptive names for categorizing log events
- **Correlation IDs**: Unique identifiers for tracking related log entries across operations
- **Scopes**: Hierarchical context for grouping related log entries
- **Performance Metrics**: Automatic measurement and logging of operation durations
- **Structured Data**: Structured data format for easier parsing and analysis
- **Multiple Log Levels**: Different verbosity levels for different environments

### LoggingConstants.cs

Contains constants and EventIds used throughout the application:

- **Log Level Prefixes**: Visual indicators for log levels (TRACE, DEBUG, INFO, etc.)
- **Event IDs**: Categorized numeric identifiers for different types of events
- **Scope Properties**: Standard property names for structured logging scopes

### LoggingExtensions.cs

Helper methods for structured logging:

- **Scope Creation**: Methods for creating different types of scopes
- **Method Logging**: Automatic entry/exit logging with parameters and return values
- **Performance Logging**: Automatic duration measurement for operations
- **Operation Steps**: Tracking progress of complex operations
## Event ID Categories

The logging system uses categorized EventId constants that provide rich contextual information about log events:

| Category | ID Range | Description | Examples |
|----------|----------|-------------|----------|
| **Application Lifecycle** | 1000-1099 | Application startup/shutdown and overall lifecycle events | `ApplicationStarted`, `ConfigurationInitialized` |
| **Scaffolding Operations** | 2000-2999 | Project creation, code generation, and scaffolding | `ScaffoldingStarted`, `ProjectFileCreated` |
| **File Operations** | 3000-3099 | File system actions like creating or deleting files | `FileCreated`, `FileModified` |
| **Directory Operations** | 3100-3199 | Directory/folder creation and management | `DirectoryCreated`, `DirectoryPermissionsChanged` |
| **Market Data Operations** | 4000-4099 | Market data retrieval, storage, and processing | `MarketDataFetched`, `MarketDataStored` |
| **Configuration** | 5000-5099 | Loading and managing application settings | `ConfigurationLoaded`, `SettingsApplied` |
| **API and External Services** | 6000-6099 | External API calls and service communication | `ApiRequestSent`, `ApiResponseReceived` |
| **User Interaction** | 7000-7099 | User input, prompts, and interface actions | `UserPrompt`, `UserInputValidated` |
| **Performance Metrics** | 8000-8099 | Performance monitoring and resource usage | `PerformanceMetricRecorded`, `ResourceUsage` |
| **Security Events** | 9000-9099 | Authentication, authorization, and security concerns | `AuthenticationSucceeded`, `SecurityPolicyViolation` |
| **Database Operations** | 10000-10099 | Database connections, queries, and transactions | `DatabaseQueryExecuted`, `DatabaseTransactionCommitted` |
| **HTTP and API** | 11000-11099 | Web requests, APIs, and HTTP client/server events | `HttpRequest`, `ApiRequestFailed` |
| **Validation** | 12000-12099 | Data validation and business rule enforcement | `ValidationCompleted`, `BusinessRuleViolated` |
| **Caching** | 13000-13099 | Cache operations and management | `CacheHit`, `CacheItemUpdated` |
| **Messaging** | 14000-14099 | Message queues, events, and publish/subscribe | `MessagePublished`, `EventProcessed` |
| **Background Services** | 15000-15099 | Background services and scheduled tasks | `BackgroundServiceStarted`, `BackgroundServiceExecution` |
| **External Dependencies** | 16000-16099 | Integration with external services | `ServiceHealthy`, `RetryAttempted` |
| **Testing & Diagnostics** | 17000-17099 | Test execution and diagnostic operations | `TestCompleted`, `DiagnosticDataCollected` |
| **Domain Events** | 18000-18099 | Domain-driven design events and aggregates | `DomainEventPublished`, `AggregateCreated` |
| **Application Services** | 19000-19099 | CQRS commands/queries and use cases | `CommandHandled`, `UseCaseCompleted` |
| **ML Experiments** | 20000-20099 | Machine learning experiments and model training | `ExperimentStarted`, `ExperimentCompleted` |
## Usage Examples

### Basic Logging_logger.LogInformation(LogEventIds.ApplicationStarted, 
    "{Prefix} Application started", 
    LoggingConstants.InfoPrefix);
### Domain Event Logging_logger.LogInformation(LogEventIds.DomainEventPublished,
    "{Prefix} Domain event {EventName} published for aggregate {AggregateType} with ID {AggregateId}",
    LoggingConstants.InfoPrefix, 
    "OrderCreated", 
    "Order", 
    orderId);
### Database Operation Loggingusing (_logger.BeginPerformanceScope("DatabaseQuery"))
{
    try
    {
        // Execute database query
        _logger.LogInformation(LogEventIds.DatabaseQueryExecuted,
            "{Prefix} Database query executed: {QueryName}",
            LoggingConstants.InfoPrefix,
            "GetCustomerOrders");
    }
    catch (Exception ex)
    {
        _logger.LogError(LogEventIds.DatabaseQueryFailed, ex,
            "{Prefix} Database query failed: {QueryName}, Error: {ErrorMessage}",
            LoggingConstants.ErrorPrefix,
            "GetCustomerOrders",
            ex.Message);
        throw;
    }
}
### Using Scopesusing (_logger.BeginScope(new Dictionary<string, object>
{
    [LoggingConstants.ScopeProperties.CorrelationId] = Guid.NewGuid().ToString("N"),
    [LoggingConstants.ScopeProperties.OperationName] = "ProcessMarketData"
}))
{
    _logger.LogInformation("Working with market data");
    // All log entries in this scope will include the correlation ID and operation name
}
### Market-Specific Scopesusing (_logger.BeginMarketScope("BTC-EUR", "DownloadData"))
{
    _logger.LogInformation(LogEventIds.MarketDataFetched, 
        "{Prefix} Starting download for {Market}", 
        LoggingConstants.InfoPrefix, "BTC-EUR");
    // All log entries will be tagged with the market information
}
### Performance Loggingusing (_logger.BeginPerformanceScope("ExpensiveOperation"))
{
    // Do work
    // Duration is automatically logged when the scope is disposed
}
### Method Entry/Exit Logging_logger.LogMethodEntry(new { param1, param2 });
// Method implementation
_logger.LogMethodExit(result);
### Operation Steps_logger.LogOperationStep("Starting file processing");
// Process files
_logger.LogOperationStep("File processing completed", new { FileCount = 10 });
### Background Service Logging_logger.LogInformation(LogEventIds.BackgroundServiceStarted,
    "{Prefix} Market data background service started. Processing markets: {Markets}",
    LoggingConstants.InfoPrefix, string.Join(", ", markets));

using (_logger.BeginScope(new Dictionary<string, object>
{
    [LoggingConstants.ScopeProperties.CorrelationId] = Guid.NewGuid().ToString("N"),
    [LoggingConstants.ScopeProperties.OperationName] = "ProcessMarketData"
}))
{
    using (_logger.BeginPerformanceScope("ProcessAllMarkets"))
    {
        // Process market data...
    }
}
## Best Practices

1. **Use Event IDs**: Always use an appropriate EventId from LogEventIds to categorize log entries
2. **Use Log Level Prefixes**: Include the appropriate prefix (e.g., LoggingConstants.InfoPrefix) for visual clarity
3. **Use Scopes**: Create scopes for related operations to group log entries
4. **Include Context**: Provide relevant context in log messages (e.g., operation name, entity ID)
5. **Structured Data**: Use structured data format (e.g., `{Name}`) instead of string concatenation
6. **Performance Metrics**: Use performance scopes for measuring operation durations
7. **Error Handling**: Log exceptions with appropriate context and don't swallow exceptions
8. **Choose Appropriate Category**: Select the most specific EventId category for your logging needs
9. **Use Correlation IDs**: Always include correlation IDs for tracking related operations
10. **Balance Detail and Performance**: Log enough detail to be useful without impacting performance
11. **Track Method Entry/Exit**: For complex methods, log entry and exit with parameters/return values
12. **Hierarchical Operations**: Use operation steps to track progress within complex operations
13. **Consistent Naming**: Use consistent naming for operations, metrics, and other identifiers
## Extending the System

To add new event types:

1. Add new EventId constants to LogEventIds class
2. Use consistent naming (e.g., FunctionalArea + Operation + Result)
3. Use sequential numbering within each category

To add new scope types:

1. Add new extension methods to LoggingExtensions class
2. Follow the existing pattern for scope creation
3. Add new scope property names to LoggingConstants.ScopeProperties
## Troubleshooting

- **Missing Logs**: Check log level configuration in appsettings.json
- **Missing Context**: Ensure scopes are properly created and disposed
- **Performance Issues**: Check for excessive logging in tight loops
- **File Size Issues**: Configure rolling interval and max file size
- **Too Much Data**: Use appropriate log levels (Trace for debugging, Info for normal operations)
- **Missing Event IDs**: Add specific event IDs for your domain when needed
- **Tracking Operations**: Use correlation IDs and nested scopes for complex operations
- **Background Service Issues**: Check for proper scope nesting and iteration tracking
## Analyzing Log Data

The structured logging format enables powerful analysis:

1. **Filtering by Event ID**: Filter logs by specific event types
2. **Correlation Tracking**: Follow operations across components using correlation IDs
3. **Performance Analysis**: Analyze operation durations using performance metrics
4. **Error Analysis**: Identify patterns in exceptions and failures
5. **Service Health**: Monitor background service execution patterns
6. **Resource Usage**: Track resource consumption patterns
7. **User Behavior**: Analyze user interaction patterns