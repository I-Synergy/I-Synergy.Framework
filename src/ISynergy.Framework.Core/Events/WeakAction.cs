using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using System.Reflection;

namespace ISynergy.Framework.Core.Events
{
    /// <summary>
    /// Class WeakAction.
    /// </summary>
    public class WeakAction
    {
        /// <summary>
        /// The static action
        /// </summary>
        private Action _staticAction;

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        private MethodInfo _method;

        /// <summary>
        /// Gets or sets the action reference.
        /// </summary>
        /// <value>The action reference.</value>
        private WeakReference _actionReference;
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
        /// Gets the name of the method.
        /// </summary>
        /// <value>The name of the method.</value>
        public string MethodName
        {
            get
            {
                if (_staticAction is not null)
                    return _staticAction.Method.Name;

                return _method.Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is static.
        /// </summary>
        /// <value><c>true</c> if this instance is static; otherwise, <c>false</c>.</value>
        public bool IsStatic
        {
            get
            {
                return _staticAction is not null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
        public WeakAction(Action action, bool keepTargetAlive = false)
            : this(action?.Target, action, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="action">The action.</param>
        /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
        public WeakAction(object target, Action action, bool keepTargetAlive = false)
        {
            if (action.Method.IsStatic)
            {
                _staticAction = action;

                if (target is not null)
                {
                    // Keep a reference to the target to control the
                    // WeakAction's lifetime.
                    _reference = new WeakReference(target);
                }

                return;
            }

            _method = action.Method;
            _actionReference = new WeakReference(action.Target);
            _liveReference = keepTargetAlive ? action.Target : null;
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
                if (_staticAction is null
                    && _reference is null
                    && _liveReference is null)
                return false;

                if (_staticAction is not null)
                {
                    if (_reference is not null)
                        return _reference.IsAlive;

                    return true;
                }

                // Non static action

                if (_liveReference is not null)
                    return true;

                if (_reference is not null)
                    return _reference.IsAlive;

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
                    return null;

                return _reference.Target;
            }
        }

        /// <summary>
        /// Gets the action target.
        /// </summary>
        /// <value>The action target.</value>
        protected object ActionTarget
        {
            get
            {
                if (_liveReference is not null)
                    return _liveReference;

                if (_actionReference is null)
                    return null;

                return _actionReference.Target;
            }
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public void Execute()
        {
            try
            {
                if (_staticAction is not null)
                {
                    _staticAction();
                    return;
                }

                if (IsAlive && _method is not null && (_liveReference is not null || _actionReference is not null) && ActionTarget is not null)
                {
                    _method.Invoke(ActionTarget, null);
                    return;
                }
            }
            catch (Exception ex)
            {
                var exceptionService = ServiceLocator.Default.GetInstance<IExceptionHandlerService>();

                if (ex.InnerException != null)
                    exceptionService.HandleExceptionAsync(ex.InnerException).Await();
                else
                    exceptionService.HandleExceptionAsync(ex).Await();
            }
        }

        /// <summary>
        /// Marks for deletion.
        /// </summary>
        public void MarkForDeletion()
        {
            _reference = null;
            _actionReference = null;
            _liveReference = null;
            _method = null;
            _staticAction = null;
        }
    }
}
