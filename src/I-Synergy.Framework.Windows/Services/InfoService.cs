using ISynergy.Services;
using Windows.ApplicationModel;

namespace ISynergy
{
    public class InfoService : IInfoService
    {
        public string Application_Path
        {
            get
            {
                return Package.Current.InstalledLocation.Path;
            }
        }

        public string CompanyName
        {
            get
            {
                return Package.Current.PublisherDisplayName;
            }
        }

        public string ProductVersion
        {
            get
            {
                return $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";
            }
        }

        public string ProductName
        {
            get
            {
                return Package.Current.DisplayName;
            }
        }

        public string CopyRightsDetail
        {
            get
            {
                return Package.Current.Description;
            }
        }
    }
}