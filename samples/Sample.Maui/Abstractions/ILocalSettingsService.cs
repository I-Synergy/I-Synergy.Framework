using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Synchronization.Options;
using Sample.Models;

namespace Sample.Abstractions;

public interface ILocalSettingsService : IApplicationSettingsService
{
    new LocalSettings Settings { get; }
    SynchronizationSettings SynchronizationSettings { get; }
}
