using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.Core.Abstractions
{
    public interface IContext
    {
        ObservableCollection<IProfile> Profiles { get; set; }
        IProfile CurrentProfile { get; set; }
        TimeZoneInfo CurrentTimeZone { get; }
        NumberFormatInfo NumberFormat { get; set; }
        string Title { get; set; }
        SoftwareEnvironments Environment { get; set; }
        bool NormalScreen { get; set; }
        string CurrencySymbol { get; set; }
        string CurrencyCode { get; set; }
        bool IsAuthenticated { get; }
        bool IsUserAdministrator { get; }
        bool IsOffline { get; set; }
        List<Type> ViewModels { get; set; }
    }
}
