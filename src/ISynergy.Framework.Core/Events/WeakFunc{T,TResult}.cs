using ISynergy.Framework.Core.Abstractions.Events;
using System.Reflection;

namespace ISynergy.Framework.Core.Events
{
    /// <summary>
    /// Class WeakFunc.
    /// Implements the <see cref="WeakFunc{TResult}" />
    /// Implements the <see cref="IExecuteWithObjectAndResult" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <seealso cref="WeakFunc{TResult}" />
    /// <seealso cref="IExecuteWithObjectAndResult" />
    public class WeakFunc<T, TResult> : IExecuteWithObjectAndResult
    {
        /// <summary>
        /// The static function
        /// </summary>
        private Func<T, TResult> _staticFunc;

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        private MethodInfo _method;

        /// <summary>
        /// Gets or sets the function reference.
        /// </summary>
        /// <value>The function reference.</value>
        private WeakReference _funcReference;

        /// <summary>
        /// Gets or sets the live reference.
        /// </summary>
        /// <value>The live reference.</value>
        private object _liveReference;

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>The reference.</value>
        private WeakReference _reference;

        /// <summary>
        /// Gets a value indicating whether this instance is static.
        /// </summary>
        /// <value><c>true</c> if this instance is static; otherwise, <c>false</c>.</value>
        public bool IsStatic
        {
            get
            {
                return _staticFunc is not null;
            }
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <value>The name of the method.</value>
        public string MethodName
        {
            get
            {
                if (_staticFunc is not null)
                    return _staticFunc.Method.Name;

                return _method.Name;
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
                    _reference = new WeakReference(target);
                }

                return;
            }

            _method = func.Method;
            _funcReference = new WeakReference(func.Target);
            _liveReference = keepTargetAlive ? func.Target : null;
            _reference = new WeakReference(target);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive
        {
            get
            {
                if (_staticFunc is null
                    && _reference is null
                    && _liveReference is null)
                {
                    return false;
                }

                if (_staticFunc is not null)
                {
                    if (_reference is not null)
                    {
                        return _reference.IsAlive;
                    }

                    return true;
                }

                // Non static action

                if (_liveReference is not null)
                {
                    return true;
                }

                if (_reference is not null)
                {
                    return _reference.IsAlive;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public object Target
        {
            get
            {
                if (_reference is null)
                {
                    return null;
                }

                return _reference.Target;
            }
        }

        /// <summary>
        /// Gets the function target.
        /// </summary>
        /// <value>The function target.</value>
        private object FuncTarget
        {
            get
            {
                if (_liveReference is not null)
                {
                    return _liveReference;
                }

                if (_funcReference is null)
                {
                    return null;
                }

                return _funcReference.Target;
            }
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>T.</returns>
        public TResult Execute()
        {
            return Execute(default(T));
        }

        /// <summary>
        /// Executes the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>T.</returns>
        public TResult Execute(T parameter)
        {
            if (_staticFunc is not null)
            {
                return _staticFunc(parameter);
            }

            var funcTarget = FuncTarget;

            if (IsAlive)
            {
                if (_method is not null
                    && (_liveReference is not null
                        || _funcReference is not null)
                    && funcTarget is not null)
                {
                    return (TResult)_method.Invoke(
                        funcTarget,
                        new object[]
                        {
                            parameter
                        });
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
        public void MarkForDeletion()
        {
            _staticFunc = null;
            _reference = null;
            _funcReference = null;
            _liveReference = null;
            _method = null;
            _staticFunc = null;
        }
    }
}
