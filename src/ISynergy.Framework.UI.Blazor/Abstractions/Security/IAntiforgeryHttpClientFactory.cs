namespace ISynergy.Framework.UI.Abstractions.Security;
public interface IAntiforgeryHttpClientFactory
{
    Task<HttpClient> CreateClientAsync(string clientName = "authorizedClient");
}
