﻿using ISynergy.Framework.Core.Linq.Attributes;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers
{
    [DynamicLinqType]
    public enum TestEnum
    {
        Var1 = 0,
        Var2 = 1,
        Var3 = 2,
        Var4 = 4,
        Var5 = 8,
        Var6 = 16,
    }
}
