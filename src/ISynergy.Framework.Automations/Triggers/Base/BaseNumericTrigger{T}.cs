using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Automations.Triggers.Base;

/// <summary>
/// Base generic numeric trigger. 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseNumericTrigger<T> : BaseTrigger, ITrigger
    where T : struct
{
    /// <summary>
    /// Gets or sets the Below property value.
    /// </summary>
    public T Below
    {
        get { return GetValue<T>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Above property value.
    /// </summary>
    public T Above
    {
        get { return GetValue<T>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="automationId"></param>
    /// <param name="below"></param>
    /// <param name="above"></param>
    /// <param name="for"></param>
    protected BaseNumericTrigger(Guid automationId, T below, T above, TimeSpan @for)
        : base(automationId)
    {
        Argument.IsNotNull(below);
        Argument.IsNotNull(above);
        Argument.IsNotNull(@for);

        Below = below;
        Above = above;
        For = @for;
    }

    /// <summary>
    /// Trigger for T type properties.
    /// </summary>
    /// <param name="automationId"></param>
    /// <param name="function"></param>
    /// <param name="below"></param>
    /// <param name="above"></param>
    /// <param name="callbackAsync"></param>
    protected BaseNumericTrigger(
        Guid automationId,
        Func<(IObservableValidatedClass Entity, IProperty<T> Property)> function,
        T below,
        T above,
        Func<T, Task> callbackAsync)
        : this(automationId, below, above, TimeSpan.Zero)
    {
    }
}
