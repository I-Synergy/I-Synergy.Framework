using System;
using System.Reflection;
using ISynergy.Framework.Core.Abstractions;

namespace ISynergy.Framework.Core.Linq.Helpers
{
    internal class AssemblyHelper : IAssemblyHelper
    {
        public Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
