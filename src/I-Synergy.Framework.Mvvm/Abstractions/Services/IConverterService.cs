namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface IConverterService
    {
        int ConvertMediaColor2Integer(object mediacolor);
        string ConvertDecimalToCurrency(decimal value);
    }
}
