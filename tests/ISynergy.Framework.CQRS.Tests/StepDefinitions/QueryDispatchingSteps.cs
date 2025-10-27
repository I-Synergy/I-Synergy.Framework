using ISynergy.Framework.CQRS.Abstractions.Dispatchers;
using ISynergy.Framework.CQRS.Dispatchers;
using ISynergy.Framework.CQRS.Queries;
using ISynergy.Framework.CQRS.TestImplementations.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace ISynergy.Framework.CQRS.Tests.StepDefinitions;

/// <summary>
/// Step definitions for query dispatching scenarios.
/// Demonstrates proper CQRS query handling patterns.
/// </summary>
[Binding]
public class QueryDispatchingSteps
{
    private readonly ILogger<QueryDispatchingSteps> _logger;
    private IServiceProvider? _serviceProvider;
    private IQueryDispatcher? _queryDispatcher;
    private TestQuery? _query;
    private string? _result;
    private Exception? _caughtException;
    private CancellationTokenSource? _cancellationTokenSource;

    public QueryDispatchingSteps()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<QueryDispatchingSteps>();
    }

    [Given(@"the CQRS query system is initialized")]
    public void GivenTheCqrsQuerySystemIsInitialized()
    {
  _logger.LogInformation("Initializing CQRS query system");

        var services = new ServiceCollection();
 services.AddScoped<IQueryHandler<TestQuery, string>>(_ => new TestQueryHandler());
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        _serviceProvider = services.BuildServiceProvider();
      _queryDispatcher = new QueryDispatcher(_serviceProvider);

        ArgumentNullException.ThrowIfNull(_queryDispatcher);
 }

    [Given(@"I have a query with parameter ""(.*)""")]
    public void GivenIHaveAQueryWithParameter(string parameter)
    {
   _logger.LogInformation("Creating query with parameter: {Parameter}", parameter);
        _query = new TestQuery { Parameter = parameter };
      ArgumentNullException.ThrowIfNull(_query);
    }

    [Given(@"I have a query without a registered handler")]
    public void GivenIHaveAQueryWithoutARegisteredHandler()
    {
        _logger.LogInformation("Creating query without registered handler");

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
     _serviceProvider = services.BuildServiceProvider();
   _queryDispatcher = new QueryDispatcher(_serviceProvider);
    _query = new TestQuery { Parameter = "Unhandled" };
}

    [Given(@"I have a query with cancellation token")]
    public void GivenIHaveAQueryWithCancellationToken()
    {
        _logger.LogInformation("Creating query with cancellation token");

        var services = new ServiceCollection();
        services.AddScoped<IQueryHandler<TestQuery, string>>(_ => new TestQueryHandler());
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        _serviceProvider = services.BuildServiceProvider();
        _queryDispatcher = new QueryDispatcher(_serviceProvider);
        _query = new TestQuery { Parameter = "Cancellable Query" };
        _cancellationTokenSource = new CancellationTokenSource();
    }

    [Given(@"the cancellation token is cancelled")]
    public void GivenTheCancellationTokenIsCancelled()
    {
        _logger.LogInformation("Cancelling the cancellation token");
        ArgumentNullException.ThrowIfNull(_cancellationTokenSource);
        _cancellationTokenSource.Cancel();
    }

    [Given(@"I have a query that returns string data")]
    public void GivenIHaveAQueryThatReturnsStringData()
    {
        _logger.LogInformation("Creating query that returns string data");
        GivenTheCqrsQuerySystemIsInitialized();
 _query = new TestQuery { Parameter = "String Data Query" };
    }

    [When(@"I dispatch the query")]
    public async Task WhenIDispatchTheQuery()
    {
        _logger.LogInformation("Dispatching query");
     ArgumentNullException.ThrowIfNull(_queryDispatcher);
        ArgumentNullException.ThrowIfNull(_query);

        _result = await _queryDispatcher.DispatchAsync(_query);
    }

    [When(@"I attempt to dispatch the unhandled query")]
    public async Task WhenIAttemptToDispatchTheUnhandledQuery()
    {
        _logger.LogInformation("Attempting to dispatch unhandled query");

        try
        {
         ArgumentNullException.ThrowIfNull(_queryDispatcher);
    ArgumentNullException.ThrowIfNull(_query);
            await _queryDispatcher.DispatchAsync(_query);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Caught expected exception");
            _caughtException = ex;
        }
    }

    [When(@"I attempt to dispatch the query with cancellation")]
    public async Task WhenIAttemptToDispatchTheQueryWithCancellation()
 {
        _logger.LogInformation("Attempting to dispatch query with cancelled token");

  try
        {
  ArgumentNullException.ThrowIfNull(_queryDispatcher);
     ArgumentNullException.ThrowIfNull(_query);
    ArgumentNullException.ThrowIfNull(_cancellationTokenSource);

      // Simulate delay to allow cancellation to take effect
 await Task.Delay(100, _cancellationTokenSource.Token);
            await _queryDispatcher.DispatchAsync(_query, _cancellationTokenSource.Token);
        }
   catch (Exception ex)
        {
      _logger.LogWarning(ex, "Caught cancellation exception");
    _caughtException = ex;
        }
    }

    [Then(@"the query should return ""(.*)""")]
    public void ThenTheQueryShouldReturn(string expectedResult)
    {
        _logger.LogInformation("Verifying query result: {Expected}", expectedResult);
        ArgumentNullException.ThrowIfNull(_result, "Result should not be null");

        if (_result != expectedResult)
    {
        throw new InvalidOperationException($"Expected result '{expectedResult}' but got '{_result}'");
  }
    }

    [Then(@"an InvalidOperationException should be thrown for query")]
  public void ThenAnInvalidOperationExceptionShouldBeThrownForQuery()
    {
        _logger.LogInformation("Verifying InvalidOperationException was thrown");
        ArgumentNullException.ThrowIfNull(_caughtException, "Exception should be thrown");

     if (_caughtException is not InvalidOperationException)
        {
        throw new InvalidOperationException($"Expected InvalidOperationException but got {_caughtException.GetType().Name}");
        }
    }

    [Then(@"the operation should be cancelled")]
    public void ThenTheOperationShouldBeCancelled()
    {
        _logger.LogInformation("Verifying operation was cancelled");
        ArgumentNullException.ThrowIfNull(_caughtException, "Cancellation exception should be thrown");
    }

[Then(@"a cancellation exception should be raised")]
    public void ThenACancellationExceptionShouldBeRaised()
    {
        _logger.LogInformation("Verifying cancellation exception type");
        ArgumentNullException.ThrowIfNull(_caughtException, "Exception should exist");

if (_caughtException is not OperationCanceledException and not TaskCanceledException)
      {
    throw new InvalidOperationException($"Expected cancellation exception but got {_caughtException.GetType().Name}");
        }
  }

    [Then(@"the result should be of type string")]
    public void ThenTheResultShouldBeOfTypeString()
    {
        _logger.LogInformation("Verifying result type");
        ArgumentNullException.ThrowIfNull(_result, "Result should not be null");

      if (_result is not string)
        {
            throw new InvalidOperationException($"Expected string type but got {_result.GetType().Name}");
   }
    }

    [Then(@"the result should not be null or empty")]
    public void ThenTheResultShouldNotBeNullOrEmpty()
    {
        _logger.LogInformation("Verifying result is not null or empty");

        if (string.IsNullOrEmpty(_result))
        {
  throw new InvalidOperationException("Result should not be null or empty");
        }
 }
}
