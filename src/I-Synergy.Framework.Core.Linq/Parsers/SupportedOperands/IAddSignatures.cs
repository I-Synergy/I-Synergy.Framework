using System;

namespace ISynergy.Framework.Core.Linq.Parsers.SupportedOperands
{
    internal interface IAddSignatures : IArithmeticSignatures
    {
        void F(DateTime x, TimeSpan y);
        void F(TimeSpan x, TimeSpan y);
        void F(DateTime? x, TimeSpan? y);
        void F(TimeSpan? x, TimeSpan? y);
    }
}
