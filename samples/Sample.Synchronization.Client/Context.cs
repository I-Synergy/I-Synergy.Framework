using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Enumerations;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Sample.Synchronization.Client
{
    internal class Context : IContext
    {
        private static IContext _context;

        public ObservableCollection<IProfile> Profiles { get; set; }
        public IProfile CurrentProfile { get; set; }
        public TimeZoneInfo CurrentTimeZone => throw new NotImplementedException();
        public NumberFormatInfo NumberFormat { get; set; }
        public SoftwareEnvironments Environment { get; set; }
        public bool NormalScreen { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsAuthenticated => throw new NotImplementedException();
        public bool IsUserAdministrator => throw new NotImplementedException();
        public bool IsOffline { get; set; }
        public List<Type> ViewModels { get; set; }

        public Context()
        {
            IsOffline = false;
        }

        public static IContext GetInstance()
        {
            if (_context == null)
                _context = new Context();

            return _context;
        }
    }
}
