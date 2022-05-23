using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    /// <summary>
    /// Interface IForgotPasswordViewModel
    /// </summary>
    public interface IForgotPasswordViewModel
    {
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>The email address.</value>
        string EmailAddress { get; set; }
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; }

        /// <summary>
        /// Resets the password asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ResetPasswordAsync();
    }
}
