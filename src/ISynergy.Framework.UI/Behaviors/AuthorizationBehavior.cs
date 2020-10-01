using ISynergy.Framework.UI.Behaviors.Base;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel;
using ISynergy.Framework.UI.Abstractions.Providers;
using Microsoft.Xaml.Interactivity;
using ISynergy.Framework.Core.Locators;

namespace ISynergy.Framework.UI.Behaviors
{
    /// <summary>
    /// The available actions to perform when a user is not able to view a specific UI element.
    /// </summary>
    public enum AuthenticationAction
    {
        /// <summary>
        /// Collapses the associated control.
        /// </summary>
        Collapse,

        /// <summary>
        /// Disables the associated control.
        /// </summary>
        Disable
    }

    /// <summary>
    /// Class Authorization.
    /// Implements the <see cref="BehaviorBase{Control}" />
    /// </summary>
    /// <seealso cref="BehaviorBase{Control}" />
    public class Authorization : BehaviorBase<Control>
    {
        /// <summary>
        /// The authentication provider.
        /// </summary>
        private static IAuthenticationProvider AuthenticationProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Authorization" /> class.
        /// </summary>
        /// <exception cref="NotSupportedException">No IAuthenticationProvider is registered, cannot use the Authentication behavior without an IAuthenticationProvider</exception>
        public Authorization()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                if (AuthenticationProvider is null)
                    AuthenticationProvider = ServiceLocator.Default.GetInstance<IAuthenticationProvider>();

                if (AuthenticationProvider is null)
                    throw new NotSupportedException("No IAuthenticationProvider is registered, cannot use the Authentication behavior without an IAuthenticationProvider");
            }
        }

        /// <summary>
        /// Gets or sets the action to execute when the user has no access to the specified UI element.
        /// </summary>
        /// <value>The action.</value>
        public AuthenticationAction Action
        {
            get { return (AuthenticationAction)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Action.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ActionProperty = DependencyProperty.Register(nameof(Action), typeof(AuthenticationAction),
            typeof(Authorization), new PropertyMetadata(AuthenticationAction.Disable));

        /// <summary>
        /// Gets or sets the authentication tag which can be used to provide additional information to the <see cref="IAuthenticationProvider" />.
        /// </summary>
        /// <value>The authentication tag.</value>
        public string AuthenticationTag
        {
            get { return (string)GetValue(AuthenticationTagProperty); }
            set { SetValue(AuthenticationTagProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AuthenticationTag.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AuthenticationTagProperty =
            DependencyProperty.Register(nameof(AuthenticationTag), typeof(string), typeof(Authorization), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject" /> has been loaded.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException">No instance of <see cref="IAuthenticationProvider" /> is registered in the <see cref="IServiceLocator" />.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="Action" /> is set to <see cref="AuthenticationAction.Disable" /> and the <see cref="Behavior{T}.AssociatedObject" /> is not a <see cref="Control" />.</exception>
        protected override void OnAssociatedObjectLoaded()
        {
            if (!AuthenticationProvider.HasAccessToUIElement(AssociatedObject, AssociatedObject.Tag, AuthenticationTag))
            {
                switch (Action)
                {
                    case AuthenticationAction.Collapse:
                        AssociatedObject.Visibility = Visibility.Collapsed;
                        break;

                    case AuthenticationAction.Disable:
                        AssociatedObject.IsEnabled = false;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
