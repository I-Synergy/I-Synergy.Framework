using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    /// <summary>
    /// Interface ILoginViewModel
    /// Implements the <see cref="IViewModelNavigation{Boolean}" />
    /// </summary>
    /// <seealso cref="IViewModelNavigation{Boolean}" />
    public interface ILoginViewModel : IViewModelNavigation<bool>
    {
        /// <summary>
        /// Gets or sets a value indicating whether [automatic login].
        /// </summary>
        /// <value><c>true</c> if [automatic login]; otherwise, <c>false</c>.</value>
        bool AutoLogin { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [login visible].
        /// </summary>
        /// <value><c>true</c> if [login visible]; otherwise, <c>false</c>.</value>
        bool LoginVisible { get; set; }
        /// <summary>
        /// Checks the automatic login asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task CheckAutoLoginAsync();
    }
}
