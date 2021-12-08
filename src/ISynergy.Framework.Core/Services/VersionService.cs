using ISynergy.Framework.Core.Abstractions.Services;
using System;
using System.Reflection;

namespace ISynergy.Framework.Core.Services
{
    /// <summary>
    /// Version service.
    /// </summary>
    public class VersionService : IVersionService
    {
        /// <summary>
        /// The assembly
        /// </summary>
        protected readonly Assembly _assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionService" /> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public VersionService(Assembly assembly)
        {
            _assembly = assembly;
        }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>The product version.</value>
        public Version ProductVersion
        {
            get
            {
                if (_assembly.IsDefined(typeof(AssemblyInformationalVersionAttribute), false))
                {
                    return new Version(_assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
                }
                else if (_assembly.IsDefined(typeof(AssemblyVersionAttribute), false))
                {
                    return new Version(_assembly.GetCustomAttribute<AssemblyVersionAttribute>().Version);
                }
                else if (_assembly.IsDefined(typeof(AssemblyFileVersionAttribute), false))
                {
                    return new Version(_assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
                }

                return new Version("0.0.0");
            }
        }
    }
}
