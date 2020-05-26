namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface IInfoService
    {
        string ApplicationPath { get; }
        string CompanyName { get; }
        string ProductVersion { get; }
        string ProductName { get; }
    }
}
