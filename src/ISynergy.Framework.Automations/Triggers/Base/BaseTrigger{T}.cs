using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Automations.Triggers.Base;

/// <summary>
/// Base trigger.
/// </summary>
public abstract class BaseTrigger<T> : BaseTrigger, ITrigger<T>
    where T : IEquatable<T>, IComparable
{
    /// <summary>
    /// Gets or sets the From property value.
    /// </summary>
    public T From
    {
        get { return GetValue<T>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the To property value.
    /// </summary>
    public T To
    {
        get { return GetValue<T>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="automationId"></param>
    /// <param name="function"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="callbackAsync"></param>
    /// <param name="for"></param>
    protected BaseTrigger(
        Guid automationId,
        Func<(IObservableValidatedClass Entity, IProperty<T> Property)> function,
        T from,
        T to,
        Func<T, Task> callbackAsync,
        TimeSpan @for)
        : base(automationId)

    {
        Argument.IsNotNull(from);
        Argument.IsNotNull(to);
        Argument.Equals(from, to);
        Argument.IsNotNull(@for);

        From = from;
        To = to;
        For = @for;
    }
}
