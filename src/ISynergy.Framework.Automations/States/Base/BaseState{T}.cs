using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Automations.States.Base;

/// <summary>
/// Base Generic State
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseState<T> : BaseState, IState<T>
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
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="for"></param>
    protected BaseState(T from, T to, TimeSpan @for)
        : base(@for)
    {
        Argument.IsNotNull(from);
        Argument.IsNotNull(to);

        From = from;
        To = to;
    }
}
