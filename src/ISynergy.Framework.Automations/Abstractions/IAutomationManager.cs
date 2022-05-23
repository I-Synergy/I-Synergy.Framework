using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISynergy.Framework.Automations.Abstractions
{
    public interface IAutomationManager
    {
        Task<Automation> GetItemAsync(Guid automationId);
        Task<bool> AddAsync(Automation automation);
        Task<bool> RemoveAsync(Guid automationId);
        Task<bool> UpdateAsync(Automation automation);
        Task<List<Automation>> GetItemsAsync();
    }
}
