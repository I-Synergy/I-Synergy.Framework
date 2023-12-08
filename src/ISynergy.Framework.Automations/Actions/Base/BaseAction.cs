using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Base;

namespace ISynergy.Framework.Automations.Actions.Base;

/// <summary>
/// Base action.
/// </summary>
public abstract class BaseAction : AutomationModel, IAction
{
    /// <summary>
    /// Gets or sets the ActionId property value.
    /// </summary>
    public Guid ActionId
    {
        get { return GetValue<Guid>(); }
        private set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Data property value.
    /// </summary>
    public object Data
    {
        get { return GetValue<object>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets the Executed property value.
    /// </summary>
    public bool Executed
    {
        get { return GetValue<bool>(); }
        private set { SetValue(value); }
    }

    /// <summary>
    /// Gets the ExecutedDateTime property value.
    /// </summary>
    public DateTimeOffset ExecutedDateTime
    {
        get { return GetValue<DateTimeOffset>(); }
        private set { SetValue(value); }
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="automationId"></param>
    protected BaseAction(Guid automationId)
        : base(automationId)
    {
        ActionId = Guid.NewGuid();
    }
}
