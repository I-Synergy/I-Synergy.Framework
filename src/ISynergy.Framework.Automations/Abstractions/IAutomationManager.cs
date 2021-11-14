namespace ISynergy.Framework.Automations.Abstractions
{
    /// <summary>
    /// Automation manager interface.
    /// </summary>
    public interface IAutomationManager
    {
        /// <summary>
        /// Get automation asynchronous.
        /// </summary>
        /// <param name="automationId"></param>
        /// <returns></returns>
        Task<Automation> GetItemAsync(Guid automationId);

        /// <summary>
        /// Add automation asynchronous.
        /// </summary>
        /// <param name="automation"></param>
        /// <returns></returns>
        Task<bool> AddAsync(Automation automation);

        /// <summary>
        /// Remove automation asynchronous.
        /// </summary>
        /// <param name="automationId"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(Guid automationId);

        /// <summary>
        /// Update automation asynchronous.
        /// </summary>
        /// <param name="automation"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(Automation automation);

        /// <summary>
        /// Get list of automations asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<List<Automation>> GetItemsAsync();
    }
}
