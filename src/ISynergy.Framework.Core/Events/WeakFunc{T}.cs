namespace ISynergy.Framework.Core.Events
{
    /// <summary>
    /// Class WeakFunc.
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    public class WeakFunc<TResult>
    {
        /// <summary>
        /// The static function
        /// </summary>
        private Func<TResult> _staticFunc;

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        protected MethodInfo Method { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is static.
        /// </summary>
        /// <value><c>true</c> if this instance is static; otherwise, <c>false</c>.</value>
        public bool IsStatic
        {
            get
            {
                return _staticFunc != null;
            }
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <value>The name of the method.</value>
        public virtual string MethodName
        {
            get
            {
                if (_staticFunc != null)
                {
                    return _staticFunc.Method.Name;
                }

                return Method.Name;
            }
        }

        /// <summary>
        /// Gets or sets the function reference.
        /// </summary>
        /// <value>The function reference.</value>
        protected WeakReference FuncReference { get; set; }

        /// <summary>
        /// Gets or sets the live reference.
        /// </summary>
        /// <value>The live reference.</value>
        protected object LiveReference { get; set; }

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>The reference.</value>
        protected WeakReference Reference { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakFunc{TResult}"/> class.
        /// </summary>
        protected WeakFunc()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakFunc{TResult}"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
        public WeakFunc(Func<TResult> func, bool keepTargetAlive = false)
            : this(func is null ? null : func.Target, func, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakFunc{TResult}"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="func">The function.</param>
        /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
        public WeakFunc(object target, Func<TResult> func, bool keepTargetAlive = false)
        {
            if (func.Method.IsStatic)
            {
                _staticFunc = func;

                if (target != null)
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
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        public virtual bool IsAlive
        {
            get
            {
                if (_staticFunc is null
                    && Reference is null
                    && LiveReference is null)
                {
                    return false;
                }

                if (_staticFunc != null)
                {
                    if (Reference != null)
                    {
                        return Reference.IsAlive;
                    }

                    return true;
                }

                // Non static action

                if (LiveReference != null)
                {
                    return true;
                }

                if (Reference != null)
                {
                    return Reference.IsAlive;
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
                if (Reference is null)
                {
                    return null;
                }

                return Reference.Target;
            }
        }

        /// <summary>
        /// Gets the function target.
        /// </summary>
        /// <value>The function target.</value>
        protected object FuncTarget
        {
            get
            {
                if (LiveReference != null)
                {
                    return LiveReference;
                }

                if (FuncReference is null)
                {
                    return null;
                }

                return FuncReference.Target;
            }
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>TResult.</returns>
        public TResult Execute()
        {
            if (_staticFunc != null)
            {
                return _staticFunc();
            }

            var funcTarget = FuncTarget;

            if (IsAlive)
            {
                if (Method != null
                    && (LiveReference != null
                        || FuncReference != null)
                    && funcTarget != null)
                {
                    return (TResult)Method.Invoke(funcTarget, null);
                }
            }

            return default(TResult);
        }

        /// <summary>
        /// Marks for deletion.
        /// </summary>
        public void MarkForDeletion()
        {
            Reference = null;
            FuncReference = null;
            LiveReference = null;
            Method = null;
            _staticFunc = null;
        }
    }
}
