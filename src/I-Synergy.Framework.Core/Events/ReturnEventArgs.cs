using System;

namespace ISynergy.Framework.Core.Events
{
    public class ReturnEventArgs<T> : EventArgs
    {
        public T Value { get; }

        public ReturnEventArgs(T value)
        {
            Value = value;
        }
    }
}