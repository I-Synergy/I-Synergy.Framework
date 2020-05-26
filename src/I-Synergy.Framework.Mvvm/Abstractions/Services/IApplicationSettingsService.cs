namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface IApplicationSettingsService
    {
        string DefaultUser { get; set; }
        string Users { get; set; }
        string RefreshToken { get; set; }
        bool IsAutoLogin { get; set; }
        byte[] Wallpaper { get; set; }
        string Color { get; set; }
        bool IsFullscreen { get; set; }
        string Culture { get; set; }
        bool IsUpdate { get; set; }
        bool IsAdvanced { get; set; }
    }
}
