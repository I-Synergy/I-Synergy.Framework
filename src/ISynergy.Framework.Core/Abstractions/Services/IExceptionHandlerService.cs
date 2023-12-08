namespace ISynergy.Framework.Core.Abstractions.Services;

public interface IExceptionHandlerService
{
    Task HandleExceptionAsync(Exception exception);
}
