using System;

namespace ISynergy.Events
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