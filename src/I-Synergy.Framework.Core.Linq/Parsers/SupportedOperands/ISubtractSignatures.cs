using System;

namespace ISynergy.Framework.Core.Linq.Parsers.SupportedOperands
{
    internal interface ISubtractSignatures : IAddSignatures
    {
        void F(DateTime x, DateTime y);
        void F(DateTime? x, DateTime? y);
    }
}
