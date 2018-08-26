namespace ISynergy.Services
{
    public interface IInfoService
    {
        string Application_Path { get; }
        string CompanyName { get; }
        string ProductVersion { get; }
        string ProductName { get; }
    }
}