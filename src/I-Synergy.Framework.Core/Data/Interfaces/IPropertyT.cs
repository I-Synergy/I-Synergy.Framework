using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy
{
    public interface IProperty<T> : IProperty
    {
        T OriginalValue { get; set; }
        T Value { get; set; }
    }
}
