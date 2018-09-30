using System;

namespace ISynergy.Events
{
    public class ReturnEventArgs<T> : EventArgs
    {
        public T Result { get; set; }

        public ReturnEventArgs()
        {
        }
    }
}