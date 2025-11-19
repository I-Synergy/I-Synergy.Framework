using System.ComponentModel;

namespace ISynergy.Framework.Core.Abstractions.Base;

public interface IGlobalSettings
{
    event PropertyChangedEventHandler? PropertyChanged;

    bool IsFirstRun { get; set; }
    int Decimals { get; set; }
    int CurrencyId { get; set; }
}
