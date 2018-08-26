using ISynergy.Core.Classes;
using System;
using System.Collections.ObjectModel;

namespace ISynergy
{
    public abstract class ContextBase : ObservableClass
    {
        public ContextBase()
        {
            Profiles = new ObservableCollection<Profile>();
        }

        public ObservableCollection<Profile> Profiles
        {
            get { return GetValue<ObservableCollection<Profile>>(); }
            set { SetValue(value); }
        }

        public Profile CurrentProfile
        {
            get { return GetValue<Profile>(); }
            set { SetValue(value); }
        }

        public TimeZoneInfo CurrentTimeZone
        {
            get { return GetValue<TimeZoneInfo>(); }
            set { SetValue(value); }
        }

        public string Title
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Environment { get; set; } = "";

        public string CurrencySymbol
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string CurrencyCode
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public bool Application_NormalScreen
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool Authenticated
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool IsUserAdmin
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
    }
}