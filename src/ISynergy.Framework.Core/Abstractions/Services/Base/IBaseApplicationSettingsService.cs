using ISynergy.Framework.Core.Abstractions.Base;
using System.Threading.Tasks;

namespace ISynergy.Framework.Core.Abstractions.Services.Base
{
    public interface IBaseApplicationSettingsService
    {
        IBaseApplicationSettings Settings { get; }
        Task LoadSettingsAsync();
        Task SaveSettingsAsync();
    }
}
