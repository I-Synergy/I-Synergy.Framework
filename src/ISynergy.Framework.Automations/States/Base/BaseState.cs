using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Automations.States.Base;

/// <summary>
/// Base State
/// </summary>
public abstract class BaseState : ObservableClass
{
    /// <summary>
    /// Gets or sets the StateId property value.
    /// </summary>
    public Guid StateId
    {
        get { return GetValue<Guid>(); }
        private set { SetValue(value); }
    }

    /// <summary>
    /// You can use For to have the trigger only fire if the state holds for some time.
    /// </summary>
    public TimeSpan For
    {
        get { return GetValue<TimeSpan>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Entity property value.
    /// </summary>
    public object Entity
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Attribute property value.
    /// </summary>
    public object Attribute
    {
        get { return GetValue<object>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="for"></param>
    protected BaseState(TimeSpan @for)
    {
        Argument.IsNotNull(@for);

        StateId = Guid.NewGuid();
        For = @for;
    }
}
