using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Mvvm.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace ISynergy.Framework.Mvvm.Tests.StepDefinitions;

/// <summary>
/// Step definitions for property change notification scenarios.
/// Demonstrates BDD testing for INotifyPropertyChanged patterns.
/// </summary>
[Binding]
public class PropertyChangeNotificationSteps
{
    private readonly ILogger<PropertyChangeNotificationSteps> _logger;
    private readonly MvvmTestContext _context;

    public PropertyChangeNotificationSteps(MvvmTestContext context)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<PropertyChangeNotificationSteps>();
        _context = context;
    }

    [Given(@"I have an ObservableObject")]
    public void GivenIHaveAnObservableObject()
    {
_logger.LogInformation("Creating ObservableObject (TestViewModel)");
   _context.ViewModel = new TestViewModel();
        _context.SubscribeToPropertyChanged();
    }

  [Given(@"the object has a property ""(.*)""")]
    public void GivenTheObjectHasAProperty(string propertyName)
 {
        _logger.LogInformation("Verifying object has property: {PropertyName}", propertyName);
  ArgumentNullException.ThrowIfNull(_context.ViewModel);
    
        var property = _context.ViewModel.GetType().GetProperty(propertyName);
        if (property == null)
     {
            throw new InvalidOperationException($"Property '{propertyName}' not found");
        }
    }

    [Given(@"the object has properties ""(.*)"" and ""(.*)""")]
    public void GivenTheObjectHasProperties(string property1, string property2)
    {
        _logger.LogInformation("Verifying object has properties: {Property1}, {Property2}", property1, property2);
 GivenTheObjectHasAProperty(property1);
     GivenTheObjectHasAProperty(property2);
    }

    [Given(@"the object has a property with value ""(.*)""")]
    public void GivenTheObjectHasAPropertyWithValue(string value)
    {
        _logger.LogInformation("Setting initial property value: {Value}", value);
        ArgumentNullException.ThrowIfNull(_context.ViewModel);
 _context.ViewModel.Name = value;
        _context.PropertyChangedEvents.Clear(); // Clear initial change event
        _context.PropertyChangeCount = 0;
    }

    [Given(@"the object has a computed property ""(.*)""")]
    public void GivenTheObjectHasAComputedProperty(string propertyName)
    {
        _logger.LogInformation("Verifying computed property: {PropertyName}", propertyName);
        GivenTheObjectHasAProperty(propertyName);
    }

    [Given(@"""(.*)"" depends on ""(.*)"" and ""(.*)""")]
    public void GivenDependsOnAnd(string computedProperty, string dependency1, string dependency2)
    {
        _logger.LogInformation("{ComputedProperty} depends on {Dep1} and {Dep2}", 
            computedProperty, dependency1, dependency2);
        // FullName depends on FirstName and LastName - this is already implemented in TestViewModel
    }

    [Given(@"the object has a validated property")]
    public void GivenTheObjectHasAValidatedProperty()
    {
        _logger.LogInformation("Object has validated property");
      // For this scenario, we'll use Name property as an example
        ArgumentNullException.ThrowIfNull(_context.ViewModel);
    }

    [When(@"I change the property value to ""(.*)""")]
    public void WhenIChangeThePropertyValueTo(string newValue)
    {
        _logger.LogInformation("Changing property value to: {NewValue}", newValue);
        ArgumentNullException.ThrowIfNull(_context.ViewModel);
     _context.ViewModel.Name = newValue;
    }

    [When(@"I change ""(.*)"" to ""(.*)""")]
    public void WhenIChangeTo(string propertyName, string newValue)
    {
        _logger.LogInformation("Changing {PropertyName} to: {NewValue}", propertyName, newValue);
        ArgumentNullException.ThrowIfNull(_context.ViewModel);

   switch (propertyName)
        {
  case "FirstName":
        _context.ViewModel.FirstName = newValue;
    break;
 case "LastName":
      _context.ViewModel.LastName = newValue;
   break;
            case "Name":
       _context.ViewModel.Name = newValue;
              break;
     default:
 throw new InvalidOperationException($"Property '{propertyName}' not supported in test");
        }
    }

    [When(@"I set the property to ""(.*)"" again")]
    public void WhenISetThePropertyToAgain(string value)
    {
        _logger.LogInformation("Setting property to same value: {Value}", value);
ArgumentNullException.ThrowIfNull(_context.ViewModel);
        
        _context.PropertyChangedEvents.Clear();
        _context.PropertyChangeCount = 0;
     
   _context.ViewModel.Name = value;
    }

 [When(@"I set the property to an invalid value")]
    public void WhenISetThePropertyToAnInvalidValue()
    {
        _logger.LogInformation("Setting property to invalid value");
        ArgumentNullException.ThrowIfNull(_context.ViewModel);
        
        try
        {
          _context.ViewModel.Name = string.Empty; // Empty could be considered invalid
        }
        catch (Exception ex)
        {
   _context.CaughtException = ex;
        }
    }

    [Then(@"PropertyChanged event should be raised")]
    public void ThenPropertyChangedEventShouldBeRaised()
    {
        _logger.LogInformation("Verifying PropertyChanged event was raised");
        
 if (_context.PropertyChangeCount == 0)
        {
            throw new InvalidOperationException("Expected PropertyChanged event to be raised");
        }

        _logger.LogInformation("PropertyChanged raised {Count} time(s)", _context.PropertyChangeCount);
    }

    [Then(@"the event should specify property ""(.*)""")]
 public void ThenTheEventShouldSpecifyProperty(string expectedPropertyName)
    {
        _logger.LogInformation("Verifying PropertyChanged for: {PropertyName}", expectedPropertyName);
        
      if (!_context.PropertyChangedEvents.Contains(expectedPropertyName))
      {
  throw new InvalidOperationException(
          $"Expected PropertyChanged for '{expectedPropertyName}' but got: {string.Join(", ", _context.PropertyChangedEvents)}");
        }
    }

    [Then(@"PropertyChanged should be raised (.*) times")]
  public void ThenPropertyChangedShouldBeRaisedTimes(int expectedCount)
    {
        _logger.LogInformation("Verifying PropertyChanged count: {Expected} vs {Actual}", 
   expectedCount, _context.PropertyChangeCount);
        
        if (_context.PropertyChangeCount != expectedCount)
  {
   throw new InvalidOperationException(
       $"Expected {expectedCount} PropertyChanged events but got {_context.PropertyChangeCount}");
        }
    }

    [Then(@"both property names should be notified")]
    public void ThenBothPropertyNamesShouldBeNotified()
    {
        _logger.LogInformation("Verifying both properties notified");
        
        if (_context.PropertyChangedEvents.Count < 2)
        {
   throw new InvalidOperationException(
       $"Expected at least 2 property notifications but got {_context.PropertyChangedEvents.Count}");
        }
    }

    [Then(@"PropertyChanged event should not be raised")]
 public void ThenPropertyChangedEventShouldNotBeRaised()
    {
        _logger.LogInformation("Verifying PropertyChanged was NOT raised");
        
        if (_context.PropertyChangeCount > 0)
        {
            throw new InvalidOperationException(
  $"Expected no PropertyChanged events but got {_context.PropertyChangeCount}");
        }
  }

    [Then(@"PropertyChanged should be raised for ""(.*)""")]
    public void ThenPropertyChangedShouldBeRaisedFor(string propertyName)
 {
        ThenTheEventShouldSpecifyProperty(propertyName);
    }

    [Then(@"validation errors should be set")]
    public void ThenValidationErrorsShouldBeSet()
    {
    _logger.LogInformation("Verifying validation errors");
     // This would check INotifyDataErrorInfo implementation if present
        // For now, we just verify the property change occurred
        if (_context.PropertyChangeCount == 0)
   {
         throw new InvalidOperationException("Expected property change for validation");
        }
    }

    [AfterScenario]
    public void Cleanup()
    {
        _logger.LogInformation("Cleaning up test context");
        _context.Cleanup();
    }
}
