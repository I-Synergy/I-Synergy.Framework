using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.Core.Abstractions.Services
{
    public interface ILocalizationService
    {
        void SetLocalizationLanguage(Languages language);
        List<string> GetTimeZoneIds(string iso2country);
    }
}
