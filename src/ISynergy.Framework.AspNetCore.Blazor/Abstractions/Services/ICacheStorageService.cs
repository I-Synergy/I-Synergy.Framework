namespace ISynergy.Framework.AspNetCore.Blazor.Abstractions.Services;

public interface ICacheStorageService
{
    ValueTask<string> GetAsync(HttpRequestMessage requestMessage);
    ValueTask<string> PutAndGetAsync(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage);
    ValueTask PutAsync(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage);
    ValueTask RemoveAllAsync();
    ValueTask RemoveAsync(HttpRequestMessage requestMessage);
}