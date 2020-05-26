﻿namespace ISynergy.Framework.Core.Linq.Parsers.SupportedOperands
{
    internal interface IArithmeticSignatures
    {
        void F(int x, int y);
        void F(uint x, uint y);
        void F(long x, long y);
        void F(ulong x, ulong y);
        void F(float x, float y);
        void F(double x, double y);
        void F(decimal x, decimal y);
        void F(int? x, int? y);
        void F(uint? x, uint? y);
        void F(long? x, long? y);
        void F(ulong? x, ulong? y);
        void F(float? x, float? y);
        void F(double? x, double? y);
        void F(decimal? x, decimal? y);
    }
}
