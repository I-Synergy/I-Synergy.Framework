using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Sample
{
    /// <summary>
    /// Context to share application specific domain between viewmodels and other classes.
    /// </summary>
    public class Context : ObservableClass, IContext
    {
        public Context()
        {
            ViewModels = new List<Type>();
            Title = "Sample";
        }

        /// <summary>
        /// Gets the current time zone.
        /// </summary>
        /// <value>The current time zone.</value>
        public TimeZoneInfo CurrentTimeZone
        {
            get
            {
                return TimeZoneInfo.Local;
            }
        }

        /// <summary>
        /// Gets or sets the NumberFormat property value.
        /// </summary>
        /// <value>The number format.</value>
        public NumberFormatInfo NumberFormat
        {
            get { return GetValue<NumberFormatInfo>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>The environment.</value>
        public SoftwareEnvironments Environment
        {
            get { return GetValue<SoftwareEnvironments>(); }
            set
            {
                SetValue(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [normal screen].
        /// </summary>
        /// <value><c>true</c> if [normal screen]; otherwise, <c>false</c>.</value>
        public bool NormalScreen
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is user administrator.
        /// </summary>
        /// <value><c>true</c> if this instance is user administrator; otherwise, <c>false</c>.</value>
        public bool IsUserAdministrator
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is offline.
        /// </summary>
        /// <value><c>true</c> if this instance is offline; otherwise, <c>false</c>.</value>
        public bool IsOffline
        {
            get { return GetValue<bool>(); }
            set
            {
                SetValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the view models.
        /// </summary>
        /// <value>The view models.</value>
        public List<Type> ViewModels
        {
            get { return GetValue<List<Type>>(); }
            set { SetValue(value); }
        }

        public ObservableCollection<IProfile> Profiles
        {
            get { return GetValue<ObservableCollection<IProfile>>(); }
            set { SetValue(value); }
        }

        public IProfile CurrentProfile
        {
            get { return GetValue<IProfile>(); }
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
    }
}
