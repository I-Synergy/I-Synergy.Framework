using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Core.Abstractions.Base
{
    /// <summary>
    /// Interface IBindable
    /// Implements the <see cref="INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public interface IBindable : INotifyPropertyChanged
    {
        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        void OnPropertyChanged([CallerMemberName] string propertyName = null);
    }
}
