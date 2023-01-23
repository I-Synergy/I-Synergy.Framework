namespace ISynergy.Framework.Core.Abstractions.Services
{
    public interface ILocalizationService
    {
        void SetLocalizationLanguage(string language);
        List<string> GetTimeZoneIds(string iso2country);
    }
}
