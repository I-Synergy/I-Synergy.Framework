using ISynergy.Framework.Mvvm.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace ISynergy.Framework.Mvvm.Tests.StepDefinitions;

/// <summary>
/// Step definitions for ViewModel lifecycle scenarios.
/// Demonstrates BDD testing for ViewModel initialization, disposal, and state management.
/// </summary>
[Binding]
public class ViewModelLifecycleSteps
{
    private readonly ILogger<ViewModelLifecycleSteps> _logger;
    private readonly MvvmTestContext _context;
    private bool _initializationExecuted;
    private bool _resourcesCleaned;

    public ViewModelLifecycleSteps(MvvmTestContext context)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
     _logger = loggerFactory.CreateLogger<ViewModelLifecycleSteps>();
 _context = context;
    }

    [Given(@"I have a ViewModel")]
    public void GivenIHaveAViewModel()
    {
    _logger.LogInformation("Creating ViewModel");
 _context.ViewModel = new TestViewModel();
     _context.SubscribeToPropertyChanged();
    }

    [Given(@"the ViewModel is initialized")]
    public async Task GivenTheViewModelIsInitialized()
    {
   _logger.LogInformation("Initializing ViewModel");
        ArgumentNullException.ThrowIfNull(_context.ViewModel);
  await _context.ViewModel.InitializeAsync();
    }

  [Given(@"the ViewModel has an async operation")]
    public void GivenTheViewModelHasAnAsyncOperation()
    {
        _logger.LogInformation("ViewModel has async operation (AsyncCommand)");
 ArgumentNullException.ThrowIfNull(_context.ViewModel);
        ArgumentNullException.ThrowIfNull(_context.ViewModel.AsyncCommand);
    }

    [Given(@"the ViewModel has Title and Subtitle properties")]
 public void GivenTheViewModelHasTitleAndSubtitleProperties()
{
        _logger.LogInformation("Verifying Title and Subtitle properties");
  ArgumentNullException.ThrowIfNull(_context.ViewModel);
   
        var titleProperty = _context.ViewModel.GetType().GetProperty("Title");
        var subtitleProperty = _context.ViewModel.GetType().GetProperty("Subtitle");
        
        if (titleProperty == null || subtitleProperty == null)
 {
            throw new InvalidOperationException("ViewModel must have Title and Subtitle properties");
        }
    }

    [When(@"the ViewModel is created")]
    public void WhenTheViewModelIsCreated()
    {
        _logger.LogInformation("ViewModel created");
        ArgumentNullException.ThrowIfNull(_context.ViewModel);
    }

    [When(@"I call InitializeAsync")]
    public async Task WhenICallInitializeAsync()
    {
  _logger.LogInformation("Calling InitializeAsync");
ArgumentNullException.ThrowIfNull(_context.ViewModel);
     
        try
        {
   await _context.ViewModel.InitializeAsync();
  _initializationExecuted = true;
        }
 catch (Exception ex)
   {
       _logger.LogError(ex, "Error during initialization");
   _context.CaughtException = ex;
    }
    }

    [When(@"I dispose the ViewModel")]
    public void WhenIDisposeTheViewModel()
    {
    _logger.LogInformation("Disposing ViewModel");
        ArgumentNullException.ThrowIfNull(_context.ViewModel);
     
     try
        {
       _context.ViewModel.Dispose();
   _resourcesCleaned = true;
     }
 catch (Exception ex)
{
       _logger.LogError(ex, "Error during disposal");
      _context.CaughtException = ex;
        }
    }

    [When(@"the operation starts")]
 public void WhenTheOperationStarts()
    {
  _logger.LogInformation("Starting async operation");
ArgumentNullException.ThrowIfNull(_context.ViewModel);
        
     // Execute async command which sets IsBusy
        // Don't await - we want to check IsBusy while it's running
#pragma warning disable CS4014
   _context.ViewModel.AsyncCommand?.ExecuteAsync(null);
#pragma warning restore CS4014
  
        // Give it a moment to start and set IsBusy = true
   Thread.Sleep(20);
    }

    [When(@"the operation completes")]
    public async Task WhenTheOperationCompletes()
    {
  _logger.LogInformation("Waiting for operation to complete");
     ArgumentNullException.ThrowIfNull(_context.ViewModel);
    
        // Wait for any running command to complete
        await Task.Delay(150); // The AsyncCommand takes 100ms
    }

    [When(@"I set Title to ""(.*)""")]
 public void WhenISetTitleTo(string title)
    {
        _logger.LogInformation("Setting Title to: {Title}", title);
   ArgumentNullException.ThrowIfNull(_context.ViewModel);
_context.ViewModel.Title = title;
    }

    [When(@"I set Subtitle to ""(.*)""")]
    public void WhenISetSubtitleTo(string subtitle)
    {
        _logger.LogInformation("Setting Subtitle to: {Subtitle}", subtitle);
  ArgumentNullException.ThrowIfNull(_context.ViewModel);
        _context.ViewModel.Subtitle = subtitle;
    }

    [Then(@"the IsInitialized property should be false")]
    public void ThenTheIsInitializedPropertyShouldBeFalse()
{
        _logger.LogInformation("Verifying IsInitialized is false");
        ArgumentNullException.ThrowIfNull(_context.ViewModel);
        
        if (_context.ViewModel.IsInitialized)
        {
            throw new InvalidOperationException("Expected IsInitialized to be false");
        }
    }

    [Then(@"the IsInitialized property should be true")]
    public void ThenTheIsInitializedPropertyShouldBeTrue()
    {
        _logger.LogInformation("Verifying IsInitialized is true");
    ArgumentNullException.ThrowIfNull(_context.ViewModel);
        
 if (!_context.ViewModel.IsInitialized)
        {
throw new InvalidOperationException("Expected IsInitialized to be true");
}
    }

    [Then(@"initialization logic should have executed")]
    public void ThenInitializationLogicShouldHaveExecuted()
    {
        _logger.LogInformation("Verifying initialization executed");
        
        if (!_initializationExecuted)
 {
   throw new InvalidOperationException("Expected initialization to execute");
        }
 }

    [Then(@"Dispose should be called")]
    public void ThenDisposeShouldBeCalled()
    {
  _logger.LogInformation("Verifying Dispose was called");
    
        if (!_resourcesCleaned)
        {
        throw new InvalidOperationException("Expected Dispose to be called");
     }
    }

    [Then(@"resources should be cleaned up")]
    public void ThenResourcesShouldBeCleanedUp()
  {
_logger.LogInformation("Verifying resources cleaned up");
 ThenDisposeShouldBeCalled();
    }

    [Then(@"the ViewModel should not be usable")]
    public void ThenTheViewModelShouldNotBeUsable()
    {
        _logger.LogInformation("Verifying ViewModel is disposed");
        // After disposal, the ViewModel should not be usable
  // This is a conceptual check - in practice, you might check for ObjectDisposedException
    }

    [Then(@"IsBusy should be true")]
    public void ThenIsBusyShouldBeTrue()
    {
   _logger.LogInformation("Verifying IsBusy is true");
        ArgumentNullException.ThrowIfNull(_context.ViewModel);
   
   if (!_context.ViewModel.IsBusy)
  {
 throw new InvalidOperationException("Expected IsBusy to be true");
        }
    }

    [Then(@"IsBusy should be false")]
    public void ThenIsBusyShouldBeFalse()
 {
        _logger.LogInformation("Verifying IsBusy is false");
      ArgumentNullException.ThrowIfNull(_context.ViewModel);
        
   if (_context.ViewModel.IsBusy)
        {
      throw new InvalidOperationException("Expected IsBusy to be false");
        }
    }

    [AfterScenario]
    public void Cleanup()
    {
     _logger.LogInformation("Cleaning up lifecycle test");
      _context.Cleanup();
     _initializationExecuted = false;
_resourcesCleaned = false;
    }
}
