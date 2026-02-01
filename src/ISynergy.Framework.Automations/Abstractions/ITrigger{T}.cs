namespace ISynergy.Framework.Automations.Abstractions;

/// <summary>
/// Public interface of a generic trigger.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITrigger<T> : ITrigger
{
    /// <summary>
    /// Gets or sets the From property value.
    /// </summary>
    T From { get; set; }
    /// <summary>
    /// Gets or sets the To property value.
    /// </summary>
    T To { get; set; }
}
