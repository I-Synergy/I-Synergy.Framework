using ISynergy.Framework.Synchronization.Abstractions;
using Sample.Models;

namespace Sample.Abstractions;
public interface ILocalSettingsService : ISynchronizationSettingsService
{
    new LocalSettings Settings { get; }
}
