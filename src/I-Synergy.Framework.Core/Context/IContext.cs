using ISynergy.Enumerations;
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
        string ApiUrl { get; }
        string AccountUrl { get; }
        string WebUrl { get; }
        string MonitorUrl { get; }
        string Client_Id { get; }
        string Client_Secret { get; }
        SoftwareEnvironments Environment { get; set; }
        bool Application_NormalScreen { get; set; }
        string TokenUrl { get; }
        string CurrencySymbol { get; set; }
        string CurrencyCode { get; set; }
        bool IsAuthenticated { get; set; }
        bool IsUserAdmin { get; set; }
    }
}