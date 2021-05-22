using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.UI.Abstractions;
using System;
using System.Diagnostics;
using System.Globalization;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
#endif

namespace ISynergy.Framework.UI.Behaviors.Base
{
    /// <summary>
    /// A base class for behaviors, implementing the basic plumbing of IBehavior
    /// </summary>
    public abstract partial class Behavior : DependencyObject, IBehavior
    {
        /// <summary>
        /// Gets the <see cref="DependencyObject"/> to which the behavior is attached.
        /// </summary>
        public DependencyObject AssociatedObject { get; private set; }

        /// <summary>
        /// Attaches the behavior to the specified <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="associatedObject">The <see cref="DependencyObject"/> to which to attach.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="associatedObject"/> is null.</exception>
        public void Attach(DependencyObject associatedObject)
        {
            if (associatedObject == this.AssociatedObject || Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }

            if (this.AssociatedObject != null)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    ServiceLocator.Default.GetInstance<ILanguageService>().GetString("CannotAttachBehaviorMultipleTimesExceptionMessage"),
                    associatedObject,
                    this.AssociatedObject));
            }

            Debug.Assert(associatedObject != null, "Cannot attach the behavior to a null object.");

            if (associatedObject == null) throw new ArgumentNullException(nameof(associatedObject));

            AssociatedObject = associatedObject;
            OnAttached();
        }

        /// <summary>
        /// Detaches the behaviors from the <see cref="AssociatedObject"/>.
        /// </summary>
        public void Detach()
        {
            OnDetaching();
            AssociatedObject = null;
        }

        /// <summary>
        /// Called after the behavior is attached to the <see cref="AssociatedObject"/>.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the <see cref="AssociatedObject"/>
        /// </remarks>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// Called when the behavior is being detached from its <see cref="AssociatedObject"/>.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the <see cref="AssociatedObject"/>
        /// </remarks>
        protected virtual void OnDetaching()
        {
        }
    }
}
