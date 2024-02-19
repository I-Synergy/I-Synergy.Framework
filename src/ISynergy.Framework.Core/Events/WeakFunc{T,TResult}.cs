using ISynergy.Framework.Core.Abstractions.Events;

namespace ISynergy.Framework.Core.Events;

/// <summary>
/// Class WeakFunc.
/// Implements the <see cref="WeakFunc{TResult}" />
/// Implements the <see cref="IExecuteWithObjectAndResult" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TResult">The type of the t result.</typeparam>
/// <seealso cref="WeakFunc{TResult}" />
/// <seealso cref="IExecuteWithObjectAndResult" />
public class WeakFunc<T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult
{
    /// <summary>
    /// The static function
    /// </summary>
    private Func<T, TResult> _staticFunc;

    /// <summary>
    /// Gets the name of the method.
    /// </summary>
    /// <value>The name of the method.</value>
    public override string MethodName
    {
        get
        {
            if (_staticFunc is not null)
            {
                return _staticFunc.Method.Name;
            }

            return Method.Name;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is alive.
    /// </summary>
    /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
    public override bool IsAlive
    {
        get
        {
            if (_staticFunc is null
                && Reference is null)
            {
                return false;
            }

            if (_staticFunc is not null)
            {
                if (Reference is not null)
                {
                    return Reference.IsAlive;
                }

                return true;
            }

            return Reference.IsAlive;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakFunc{T, TResult}"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
    public WeakFunc(Func<T, TResult> func, bool keepTargetAlive = false)
        : this(func is null ? null : func.Target, func, keepTargetAlive)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakFunc{T, TResult}"/> class.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="func">The function.</param>
    /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
    public WeakFunc(object target, Func<T, TResult> func, bool keepTargetAlive = false)
    {
        if (func.Method.IsStatic)
        {
            _staticFunc = func;

            if (target is not null)
            {
                // Keep a reference to the target to control the
                // WeakAction's lifetime.
                Reference = new WeakReference(target);
            }

            return;
        }

        Method = func.Method;
        FuncReference = new WeakReference(func.Target);
        LiveReference = keepTargetAlive ? func.Target : null;
        Reference = new WeakReference(target);
    }

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <returns>TResult.</returns>
    public new TResult Execute()
    {
        return Execute(default(T));
    }

    /// <summary>
    /// Executes the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns>TResult.</returns>
    public TResult Execute(T parameter)
    {
        if (_staticFunc is not null)
        {
            return _staticFunc(parameter);
        }

        var funcTarget = FuncTarget;

        if (IsAlive)
        {
            if (Method is not null
                && (LiveReference is not null
                    || FuncReference is not null)
                && funcTarget is not null)
            {
                return (TResult)Method.Invoke(
                    funcTarget,
                    [
                        parameter
                    ]);
            }
        }

        return default(TResult);
    }

    /// <summary>
    /// Executes a Func and returns the result.
    /// </summary>
    /// <param name="parameter">A parameter passed as an object,
    /// to be casted to the appropriate type.</param>
    /// <returns>The result of the operation.</returns>
    public object ExecuteWithObject(object parameter)
    {
        var parameterCasted = (T)parameter;
        return Execute(parameterCasted);
    }

    /// <summary>
    /// Marks for deletion.
    /// </summary>
    public new void MarkForDeletion()
    {
        _staticFunc = null;
        base.MarkForDeletion();
    }
}
