using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Enumerations;
using ISynergy.Framework.Core.Base;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Automations;

/// <summary>
/// This class is responsible for managing and executing automations. Automations are a set of conditions and actions that should happen automatically under certain circumstances.
/// The AutomationService takes in dependencies for:
/// - IAutomationManager: This manages getting a list of defined automations
/// - IOptions: This provides configuration options
/// - ILogger: This is used for logging
/// It has a method called RefreshAutomationsAsync() which will call the IAutomationManager to get an updated list of all automations that are defined.
/// The ValidateConditionsAsync method takes an automation and a value, and checks if all the conditions defined in that automation evaluate to true for the provided value.This determines if the automation should execute based on the conditions.
/// The ExecuteAsync method takes an automation, a value, and a cancellation token. It will execute the series of actions defined in the automation one by one until complete or cancelled.The actions transform the provided value in some way to accomplish the purpose of the automation.
/// Overall, this class handles getting the defined automations, evaluating if an automation should execute based on conditions, and then executing the actions of an automation if triggered.It provides the core logic to enable automations in an application.
/// </summary>
public class Automation : ObservableClass
{
    /// <summary>
    /// Gets or sets the AutomationId property value.
    /// </summary>
    public Guid AutomationId
    {
        get { return GetValue<Guid>(); }
        private set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Name property value.
    /// </summary>
    public string Name
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Description property value.
    /// </summary>
    public string Description
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Mode property value.
    /// </summary>
    public RunModes Mode
    {
        get { return GetValue<RunModes>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the IsActive property value.
    /// </summary>
    public bool IsActive
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Excecution Timeout property value.
    /// </summary>
    public TimeSpan ExecutionTimeout
    {
        get { return GetValue<TimeSpan>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Triggers property value.
    /// </summary>
    public ObservableCollection<object> Triggers
    {
        get { return GetValue<ObservableCollection<object>>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Conditions property value.
    /// </summary>
    public ObservableCollection<ICondition> Conditions
    {
        get { return GetValue<ObservableCollection<ICondition>>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Actions property value.
    /// </summary>
    public ObservableCollection<IAction> Actions
    {
        get { return GetValue<ObservableCollection<IAction>>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Default constructor for Automation.
    /// </summary>
    public Automation()
    {
        AutomationId = Guid.NewGuid();
        Mode = RunModes.Single;
        IsActive = false;
        ExecutionTimeout = TimeSpan.FromSeconds(30);
        Triggers = new ObservableCollection<object>();
        Conditions = new ObservableCollection<ICondition>();
        Actions = new ObservableCollection<IAction>();
    }
}
