using System.Reflection;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using System.Diagnostics;

#if NETFX_CORE
using Windows.ApplicationModel;
#else
using System.IO;
#endif

namespace ISynergy.Framework.UI
{
    /// <summary>
    /// Class InfoService.
    /// Implements the <see cref="IInfoService" />
    /// </summary>
    /// <seealso cref="IInfoService" />
    public class InfoService : IInfoService
    {
        /// <summary>
        /// The assembly
        /// </summary>
        private readonly Assembly _assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoService"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public InfoService(Assembly assembly)
        {
            _assembly = assembly;
        }

        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <value>The application path.</value>
        public string ApplicationPath
        {
            get
            {
#if NETFX_CORE
                return Package.Current.InstalledLocation.Path;
#else
                return Path.GetDirectoryName(_assembly.Location);
#endif
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
                return FileVersionInfo.GetVersionInfo(_assembly.Location).CompanyName;
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
                return FileVersionInfo.GetVersionInfo(_assembly.Location).FileVersion;
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
                return FileVersionInfo.GetVersionInfo(_assembly.Location).ProductName;
            }
        }

        /// <summary>
        /// Gets the copy rights detail.
        /// </summary>
        /// <value>The copy rights detail.</value>
        public string Copyrights
        {
            get
            {
                return FileVersionInfo.GetVersionInfo(_assembly.Location).LegalCopyright;
            }
        }
    }
}
