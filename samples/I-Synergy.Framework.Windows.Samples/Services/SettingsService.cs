using System;
using ISynergy.Framework.Windows.Samples.Abstractions.Services;

namespace ISynergy.Framework.Windows.Samples.Services
{
    public class SettingsService : ISettingsService
    {
        public bool IsFirstRun => throw new NotImplementedException();

        public int DefaultCurrencyId => throw new NotImplementedException();
    }
}
