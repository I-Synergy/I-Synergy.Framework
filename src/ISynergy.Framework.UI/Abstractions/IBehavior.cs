#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
#endif

namespace ISynergy.Framework.UI.Abstractions
{
    /// <summary>
    /// Interface implemented by all custom behaviors.
    /// </summary>
    public interface IBehavior
    {
        /// <summary>
        /// Gets the <see cref="DependencyObject"/> to which the <seealso cref="IBehavior"/> is attached.
        /// </summary>
        DependencyObject AssociatedObject
        {
            get;
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The <see cref="DependencyObject"/> to which the <seealso cref="IBehavior"/> will be attached.</param>
        void Attach(DependencyObject associatedObject);

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        void Detach();
    }
}
