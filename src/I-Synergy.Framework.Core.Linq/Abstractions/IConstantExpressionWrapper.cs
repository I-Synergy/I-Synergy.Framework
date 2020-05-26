using System.Linq.Expressions;

namespace ISynergy.Framework.Core.Linq.Abstractions
{
    internal interface IConstantExpressionWrapper
    {
        void Wrap(ref Expression expression);
    }
}
