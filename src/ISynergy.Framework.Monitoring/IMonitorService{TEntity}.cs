namespace ISynergy.Services
{
    public interface IMonitorService<TEntity> where TEntity : class
    {
        Task PublishAsync(string channel, string eventname, TEntity data, CancellationToken cancellationToken = default);
    }
}
