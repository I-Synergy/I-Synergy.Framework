using System.Reflection;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using System.IO;

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
                return Path.GetDirectoryName(_assembly.Location);
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
                return _assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
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
                return _assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
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
                return _assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
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
                return _assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
            }
        }
    }
}
