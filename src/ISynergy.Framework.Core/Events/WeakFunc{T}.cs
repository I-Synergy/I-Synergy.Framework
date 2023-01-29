using System.Reflection;

namespace ISynergy.Framework.Core.Events
{
    /// <summary>
    /// Class WeakFunc.
    /// </summary>
    /// <typeparam name="T">The type of the t result.</typeparam>
    public class WeakFunc<T>
    {
        /// <summary>
        /// The static function
        /// </summary>
        private Func<T> _staticFunc;

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
        /// Initializes a new instance of the <see cref="WeakFunc{TResult}"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
        public WeakFunc(Func<T> func, bool keepTargetAlive = false)
            : this(func is null ? null : func.Target, func, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakFunc{TResult}"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="func">The function.</param>
        /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
        public WeakFunc(object target, Func<T> func, bool keepTargetAlive = false)
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
        public T Execute()
        {
            if (_staticFunc is not null)
            {
                return _staticFunc();
            }

            var funcTarget = FuncTarget;

            if (IsAlive)
            {
                if (_method is not null
                    && (_liveReference is not null
                        || _funcReference is not null)
                    && funcTarget is not null)
                {
                    return (T)_method.Invoke(funcTarget, null);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Marks for deletion.
        /// </summary>
        public void MarkForDeletion()
        {
            _reference = null;
            _funcReference = null;
            _liveReference = null;
            _method = null;
            _staticFunc = null;
        }
    }
}
