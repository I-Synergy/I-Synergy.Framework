using ISynergy.Framework.Core.Abstractions.Services.Base;
using Sample.Models;

namespace Sample.Abstractions.Services;

public interface ISettingsService : IBaseSettingsService
{
    new LocalSettings LocalSettings { get; }
    new RoamingSettings RoamingSettings { get; }
    new GlobalSettings GlobalSettings { get; }
}
