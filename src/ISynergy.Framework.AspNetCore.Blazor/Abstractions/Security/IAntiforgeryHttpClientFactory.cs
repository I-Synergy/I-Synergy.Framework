namespace ISynergy.Framework.AspNetCore.Blazor.Abstractions.Security;
public interface IAntiforgeryHttpClientFactory
{
    Task<HttpClient> CreateClientAsync(string clientName = "authorizedClient");
}
