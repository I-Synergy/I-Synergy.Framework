using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface IExceptionHandlerService
    {
        Task HandleExceptionAsync(Exception exception);
    }
}
