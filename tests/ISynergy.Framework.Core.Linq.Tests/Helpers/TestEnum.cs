using ISynergy.Framework.Core.Linq.Attributes;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers
{
    /// <summary>
    /// Enum TestEnum
    /// </summary>
    [DynamicLinqType]
    public enum TestEnum
    {
        /// <summary>
        /// The var1
        /// </summary>
        Var1 = 0,
        /// <summary>
        /// The var2
        /// </summary>
        Var2 = 1,
        /// <summary>
        /// The var3
        /// </summary>
        Var3 = 2,
        /// <summary>
        /// The var4
        /// </summary>
        Var4 = 4,
        /// <summary>
        /// The var5
        /// </summary>
        Var5 = 8,
        /// <summary>
        /// The var6
        /// </summary>
        Var6 = 16,
    }
}
