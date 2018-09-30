using System;

namespace ISynergy.Events
{
    public class ViewModelEventArgs<T> : EventArgs
    {
        public T Result { get; set; }

        public ViewModelEventArgs()
        {
        }
    }
}