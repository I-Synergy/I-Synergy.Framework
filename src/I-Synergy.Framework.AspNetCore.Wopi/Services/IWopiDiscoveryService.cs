using System.Collections.Generic;
using System.Threading.Tasks;
using ISynergy.Framework.Wopi.Models;

namespace ISynergy.Framework.Wopi.Services
{
    public interface IWopiDiscoveryService
    {
        Task<List<WopiAction>> GetActionsAsync();
        string GetActionUrl(WopiAction action, string fileId, string authority);
        Task RefreshAsync();
    }
}