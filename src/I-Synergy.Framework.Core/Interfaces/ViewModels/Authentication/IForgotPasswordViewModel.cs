using ISynergy.Services;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Authentication
{
    public interface IForgotPasswordViewModel
    {
        IBaseService BaseService { get; }
        string EmailAddress { get; set; }
        bool Mail_Valid { get; set; }
        string Title { get; }

        Task<bool> ResetPasswordAsync();
        Task SubmitAsync(object e);
    }
}
