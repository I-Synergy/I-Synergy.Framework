using ISynergy.Framework.Core.Abstractions.Services.Base;
using Sample.Models;

namespace Sample.Abstractions.Services;

public interface ILocalSettingsService : IApplicationSettingsService
{
    new LocalSettings Settings { get; }
}
