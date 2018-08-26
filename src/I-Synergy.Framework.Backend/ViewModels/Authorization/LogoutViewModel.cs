using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ISynergy.ViewModels.Authorization
{
    public class LogoutViewModel
    {
        [BindNever]
        public string RequestId { get; set; }
    }
}