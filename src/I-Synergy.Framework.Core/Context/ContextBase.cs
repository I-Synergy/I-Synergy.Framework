using ISynergy.Data;
using ISynergy.Enumerations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ISynergy
{
    public abstract class ContextBase : ObservableClass
    {
        protected ContextBase()
        {
            Profiles = new ObservableCollection<Profile>();
            ViewModels = new List<Type>();
        }

        public List<Type> ViewModels
        {
            get { return GetValue<List<Type>>(); }
            set { SetValue(value); }
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

        public SoftwareEnvironments Environment
        {
            get { return GetValue<SoftwareEnvironments>(); }
            set { SetValue(value); }
        }

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

        public bool IsAuthenticated
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