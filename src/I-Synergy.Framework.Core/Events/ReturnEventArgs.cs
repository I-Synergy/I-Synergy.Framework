using System;

namespace ISynergy.Common.Types
{
    public class ReturnEventArgs<T> : EventArgs
    {
        public T Result { get; set; }

        public ReturnEventArgs()
        {
        }
    }
}