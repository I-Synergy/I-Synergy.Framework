using Flurl.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface IBaseRestService
    {
        IFlurlClient Client { get; }

        Task<T> GetJsonAsync<T>(object[] segments, object queryparameters, CancellationToken cancellationToken);
        Task<string> GetStringAsync(object[] segments, object queryparameters, CancellationToken cancellationToken);
        Task<int> PostJsonAsync(object[] segments, object data);
        Task<T> PostAsync<T>(string baseUrl, object[] segments, object data, bool IsAnonymous = false);
        Task<int> PutJsonAsync(object[] segments, object data);
        Task<T> PutAsync<T>(string baseUrl, object[] segments, object data);
        Task<int> DeleteJsonAsync(object[] segments, object queryparameters = null);
        Task<int> DeleteAsync(string baseUrl, object[] segments, object queryparameters = null);
    }
}