using ISynergy.Framework.Core.Events;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface IExceptionHandlerService
    {
        Task HandleExceptionAsync(Exception exception);
    }
}
