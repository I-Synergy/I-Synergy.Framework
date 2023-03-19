namespace ISynergy.Framework.Automations.Abstractions
{
    /// <summary>
    /// Repository manager for Scheduled and Delayed actions.
    /// </summary>
    public interface IActionManager
    {
        Task<IAction> GetItemAsync(Guid actionId);
        Task<bool> AddAsync(IAction automationQueue);
        Task<bool> RemoveAsync(Guid actionId);
        Task<bool> UpdateAsync(IAction automationQueue);
        Task<IAction> GetFirstUpcomingTaskAsync();

        /// <summary>
        /// Get all scheduled and delayes actions.
        /// Default: only non-executed ones.
        /// </summary>
        /// <param name="onlyActive"></param>
        /// <returns></returns>
        Task<List<IAction>> GetItemsAsync(bool onlyActive = true);

        /// <summary>
        /// Sets the action as executed and saves the time it finished execution.
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns></returns>
        Task SetActionExcecutedAsync(Guid actionId);

        /// <summary>
        /// Gets the time of the previous completed task in this Automation.
        /// </summary>
        /// <param name="automationId"></param>
        /// <returns></returns>
        Task<DateTimeOffset?> GetTimePreviousCompletedTaskAsync(Guid automationId);
    }
}
