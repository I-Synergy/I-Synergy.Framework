using System;
using System.Reflection;
using ISynergy.Framework.Core.Abstractions;

namespace ISynergy.Framework.Core.Linq.Helpers
{
    /// <summary>
    /// Class AssemblyHelper.
    /// Implements the <see cref="IAssemblyHelper" />
    /// </summary>
    /// <seealso cref="IAssemblyHelper" />
    internal class AssemblyHelper : IAssemblyHelper
    {
        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <returns>Assembly[].</returns>
        public Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
