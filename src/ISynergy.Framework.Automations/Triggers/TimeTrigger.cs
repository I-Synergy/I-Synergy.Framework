using ISynergy.Framework.Automations.Triggers.Base;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Automations.Triggers;

/// <summary>
/// Time trigger.
/// </summary>
public class TimeTrigger : BaseTrigger
{
    /// <summary>
    /// Gets or sets the At property value.
    /// </summary>
    public TimeSpan At
    {
        get { return GetValue<TimeSpan>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the IsFixedTime property value.
    /// </summary>
    public bool IsFixedTime
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="automationId"></param>
    /// <param name="at"></param>
    /// <param name="isFixedTime"></param>
    public TimeTrigger(Guid automationId, TimeSpan at, bool isFixedTime)
        : base(automationId)
    {
        Argument.IsNotNull(at);

        At = at;
        IsFixedTime = isFixedTime;
    }
}
