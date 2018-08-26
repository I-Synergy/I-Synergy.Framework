using System;

namespace ISynergy.Common.Types
{
    public class ViewModelEventArgs<T> : EventArgs
    {
        public T Result { get; set; }

        public ViewModelEventArgs()
        {
        }
    }
}