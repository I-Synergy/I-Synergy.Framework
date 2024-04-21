using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services.Base;

namespace ISynergy.Framework.Synchronization.Abstractions;
public interface ISynchronizationSettingsService : IBaseApplicationSettingsService
{
    new ISynchronizationApplicationSettings Settings { get; }
}
