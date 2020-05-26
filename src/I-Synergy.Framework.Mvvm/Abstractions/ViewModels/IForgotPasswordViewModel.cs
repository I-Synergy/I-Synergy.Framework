using System.Threading.Tasks;

namespace ISynergy.Framework.Windows.Abstractions.ViewModels
{
    public interface IForgotPasswordViewModel
    {
        string EmailAddress { get; set; }
        string Title { get; }

        Task<bool> ResetPasswordAsync();
    }
}
