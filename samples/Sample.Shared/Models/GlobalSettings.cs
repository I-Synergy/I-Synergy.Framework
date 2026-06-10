using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Base;

namespace Sample.Models;

public class GlobalSettings : BaseModel, IGlobalSettings
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

    public string CurrencyName
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public string CurrencySymbol
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public GlobalSettings()
    {
        IsFirstRun = true;
        Decimals = 2;
        CurrencyId = 1;
        CurrencyName = "Euro";
        CurrencySymbol = "€";
    }
}

