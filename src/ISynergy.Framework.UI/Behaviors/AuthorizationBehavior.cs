using System;
using Windows.ApplicationModel;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.Core.Locators;
using Microsoft.Xaml.Interactivity;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

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
    /// Implements the <see cref="Behavior{Control}" />
    /// </summary>
    /// <seealso cref="Behavior{Control}" />
    public class Authorization : Behavior<Control>
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
            get => (AuthenticationAction)GetValue(ActionProperty);
            set => SetValue(ActionProperty, value);
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
            get => (string)GetValue(AuthenticationTagProperty);
            set => SetValue(AuthenticationTagProperty, value);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AuthenticationTag.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AuthenticationTagProperty =
            DependencyProperty.Register(nameof(AuthenticationTag), typeof(string), typeof(Authorization), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject" /> has been loaded.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

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
