using System;
using Sample.Abstractions.Services;

namespace Sample.Services
{
    public class SettingsService : ISettingsService
    {
        public bool IsFirstRun => throw new NotImplementedException();

        public int DefaultCurrencyId => throw new NotImplementedException();
    }
}
