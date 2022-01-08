using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Abstractions
{
    public interface IFileSynchronizationService
    {
        Task SynchronizeAsync();
    }
}
