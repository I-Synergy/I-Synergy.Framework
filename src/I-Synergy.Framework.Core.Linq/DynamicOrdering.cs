using System.Linq.Expressions;

namespace ISynergy.Framework.Core.Linq
{
    internal class DynamicOrdering
    {
        public Expression Selector;
        public bool Ascending;
        public string MethodName;
    }
}
