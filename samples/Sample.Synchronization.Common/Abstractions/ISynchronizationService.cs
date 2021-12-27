namespace Sample.Synchronization.Common.Abstractions
{
    public interface ISynchronizationService
    {
        Task SynchronizeAsync();
        Task DisconnectAsync();
    }
}
