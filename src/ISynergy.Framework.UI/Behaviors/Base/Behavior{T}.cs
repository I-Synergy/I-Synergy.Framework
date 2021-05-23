﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using System;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
#endif

namespace ISynergy.Framework.UI.Behaviors.Base
{
    /// <summary>
    /// A base class for behaviors making them code compatible with older frameworks,
    /// and allow for typed associtated objects.
    /// </summary>
    /// <typeparam name="T">The object type to attach to</typeparam>
    public abstract class Behavior<T> : Behavior where T : DependencyObject
    {
        /// <summary>
        /// Gets the object to which this behavior is attached.
        /// </summary>
        public new T AssociatedObject
        {
            get { return (T)base.AssociatedObject; }
        }

        /// <summary>
        /// Called after the behavior is attached to the <see cref="AssociatedObject"/>.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the <see cref="AssociatedObject"/>
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (this.AssociatedObject == null)
            {
                string actualType = base.AssociatedObject.GetType().FullName;
                string expectedType = typeof(T).FullName;
                string message = string.Format(ServiceLocator.Default.GetInstance<ILanguageService>().GetString("InvalidAssociatedObjectExceptionMessage"), actualType, expectedType);
                throw new InvalidOperationException(message);
            }
        }
    }
}
