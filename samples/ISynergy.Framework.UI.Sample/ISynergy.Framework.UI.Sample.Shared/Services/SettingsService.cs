using System;
using ISynergy.Framework.UI.Sample.Abstractions.Services;

namespace ISynergy.Framework.UI.Sample.Services
{
    public class SettingsService : ISettingsService
    {
        public bool IsFirstRun => throw new NotImplementedException();

        public int DefaultCurrencyId => throw new NotImplementedException();
    }
}
