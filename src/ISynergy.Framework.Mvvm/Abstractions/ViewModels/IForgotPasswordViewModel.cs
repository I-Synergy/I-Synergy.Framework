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
    }
}
