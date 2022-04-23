using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.Core.Abstractions.Services
{
    public interface ILocalizationService
    {
        Task SetLocalizationLanguageAsync(string isoLanguage);
    }
}
