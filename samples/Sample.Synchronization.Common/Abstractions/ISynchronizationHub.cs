namespace Sample.Synchronization.Common.Abstractions
{
    public interface ISynchronizationHub
    {
        /// <summary>
        /// Sends message to other group members asynchronously.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessageAsync(string message);
    }
}
