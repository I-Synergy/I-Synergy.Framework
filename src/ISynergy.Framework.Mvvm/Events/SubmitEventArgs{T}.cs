namespace ISynergy.Framework.Mvvm.Events;

/// <summary>
/// Class SubmitEventArgs.
/// Implements the <see cref="EventArgs" />
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
/// <seealso cref="EventArgs" />
public class SubmitEventArgs<TEntity> : EventArgs
{
    /// <summary>
    /// Gets the result.
    /// </summary>
    /// <value>The result.</value>
    public TEntity Result { get; }
    /// <summary>
    /// Gets the owner.
    /// </summary>
    /// <value>The owner.</value>
    public object? Owner { get; }
    /// <summary>
    /// Gets the target property.
    /// </summary>
    /// <value>The target property.</value>
    public string? TargetProperty { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitEventArgs{TEntity}"/> class.
    /// </summary>
    /// <param name="result">The result.</param>
    public SubmitEventArgs(TEntity result)
    {
        Result = result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitEventArgs{TEntity}"/> class.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="result">The result.</param>
    public SubmitEventArgs(object owner, TEntity result)
        : this(result)
    {
        Owner = owner;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitEventArgs{TEntity}"/> class.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="result">The result.</param>
    /// <param name="targetProperty">The target property.</param>
    public SubmitEventArgs(object owner, TEntity result, string targetProperty)
        : this(owner, result)
    {
        TargetProperty = targetProperty;
    }
}
