using ISynergy.Framework.Core.Abstractions.Events;
using System;

namespace ISynergy.Framework.Core.Events
{
    /// <summary>
    /// Class WeakAction.
    /// Implements the <see cref="WeakAction" />
    /// Implements the <see cref="IExecuteWithObject" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="WeakAction" />
    /// <seealso cref="IExecuteWithObject" />
    public class WeakAction<T> : WeakAction, IExecuteWithObject
    {
        /// <summary>
        /// The static action
        /// </summary>
        private Action<T> _staticAction;

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <value>The name of the method.</value>
        public override string MethodName
        {
            get
            {
                if (_staticAction is not null)
                {
                    return _staticAction.Method.Name;
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
                if (_staticAction is null
                    && Reference is null)
                {
                    return false;
                }

                if (_staticAction is not null)
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
        /// Initializes a new instance of the <see cref="WeakAction{T}"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
        public WeakAction(Action<T> action, bool keepTargetAlive = false)
            : this(action is null ? null : action.Target, action, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction{T}"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="action">The action.</param>
        /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
        public WeakAction(object target, Action<T> action, bool keepTargetAlive = false)
        {
            if (action.Method.IsStatic)
            {
                _staticAction = action;

                if (target is not null)
                {
                    // Keep a reference to the target to control the
                    // WeakAction's lifetime.
                    Reference = new WeakReference(target);
                }

                return;
            }

            Method = action.Method;
            ActionReference = new WeakReference(action.Target);
            LiveReference = keepTargetAlive ? action.Target : null;
            Reference = new WeakReference(target);
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public new void Execute()
        {
            Execute(default);
        }

        /// <summary>
        /// Executes the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public void Execute(T parameter)
        {
            if (_staticAction is not null)
            {
                _staticAction(parameter);
                return;
            }

            var actionTarget = ActionTarget;

            if (IsAlive)
            {
                if (Method is not null
                    && (LiveReference is not null
                        || ActionReference is not null)
                    && actionTarget is not null)
                {
                    Method.Invoke(
                        actionTarget,
                        new object[]
                        {
                            parameter
                        });
                }
            }
        }

        /// <summary>
        /// Executes an action.
        /// </summary>
        /// <param name="parameter">A parameter passed as an object,
        /// to be casted to the appropriate type.</param>
        public void ExecuteWithObject(object parameter)
        {
            var parameterCasted = (T)parameter;
            Execute(parameterCasted);
        }

        /// <summary>
        /// Marks for deletion.
        /// </summary>
        public new void MarkForDeletion()
        {
            _staticAction = null;
            base.MarkForDeletion();
        }
    }
}
