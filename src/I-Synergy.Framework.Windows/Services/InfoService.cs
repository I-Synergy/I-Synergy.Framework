using ISynergy.Framework.Mvvm.Abstractions.Services;
using Windows.ApplicationModel;

namespace ISynergy.Framework.Windows
{
    /// <summary>
    /// Class InfoService.
    /// Implements the <see cref="IInfoService" />
    /// </summary>
    /// <seealso cref="IInfoService" />
    public class InfoService : IInfoService
    {
        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <value>The application path.</value>
        public string ApplicationPath
        {
            get
            {
                return Package.Current.InstalledLocation.Path;
            }
        }

        /// <summary>
        /// Gets the name of the company.
        /// </summary>
        /// <value>The name of the company.</value>
        public string CompanyName
        {
            get
            {
                return Package.Current.PublisherDisplayName;
            }
        }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>The product version.</value>
        public string ProductVersion
        {
            get
            {
                return $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";
            }
        }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>The name of the product.</value>
        public string ProductName
        {
            get
            {
                return Package.Current.DisplayName;
            }
        }

        /// <summary>
        /// Gets the copy rights detail.
        /// </summary>
        /// <value>The copy rights detail.</value>
        public string CopyRightsDetail
        {
            get
            {
                return Package.Current.Description;
            }
        }
    }
}
