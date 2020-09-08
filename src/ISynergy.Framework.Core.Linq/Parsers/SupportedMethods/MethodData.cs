using System.Linq.Expressions;
using System.Reflection;

namespace ISynergy.Framework.Core.Linq.Parsers.SupportedMethods
{
    /// <summary>
    /// Class MethodData.
    /// </summary>
    internal class MethodData
    {
        /// <summary>
        /// Gets or sets the method base.
        /// </summary>
        /// <value>The method base.</value>
        public MethodBase MethodBase { get; set; }
        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public ParameterInfo[] Parameters { get; set; }
        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public Expression[] Args { get; set; }
    }
}
