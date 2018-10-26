using System;
using System.Collections.ObjectModel;

namespace ISynergy
{
    public interface IContext
    {
        ObservableCollection<Profile> Profiles { get; set; }
        Profile CurrentProfile { get; set; }
        TimeZoneInfo CurrentTimeZone { get; set; }
        string Title { get; set; }
        string ApiUrl { get; set; }
        string AccountUrl { get; set; }
        string WebUrl { get; set; }
        string MonitorUrl { get; set; }
        string Client_Id { get; set; }
        string Client_Secret { get; set; }
        string Environment { get; set; }
        bool Application_NormalScreen { get; set; }
        string TokenUrl { get; set; }
        string CurrencySymbol { get; set; }
        string CurrencyCode { get; set; }
        bool IsAuthenticated { get; set; }
        bool IsUserAdmin { get; set; }
    }
}