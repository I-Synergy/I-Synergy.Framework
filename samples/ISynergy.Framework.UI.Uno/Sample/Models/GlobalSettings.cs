using ISynergy.Framework.Core.Base;

namespace Sample.Models;

public class GlobalSettings : ModelBase
{
    public bool IsFirstRun
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    public int Decimals
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    public int CurrencyId
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    public int CountryId
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }
}
