namespace ISynergy.Framework.Core.Abstractions.Base;

public interface IGlobalSettings
{
    bool IsFirstRun { get; set; }
    int Decimals { get; set; }
    int CurrencyId { get; set; }
}
