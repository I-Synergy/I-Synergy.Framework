using System;

namespace ISynergy.Framework.Mvvm.Events
{
    public class SubmitEventArgs<TEntity> : EventArgs
    {
        public TEntity Result { get; }
        public object Owner { get; }
        public string TargetProperty { get; }

        public SubmitEventArgs(TEntity result)
        {
            Result = result;
        }

        public SubmitEventArgs(object owner, TEntity result)
            : this(result)
        {
            Owner = owner;
        }

        public SubmitEventArgs(object owner, TEntity result, string targetProperty)
            : this(owner, result)
        {
            TargetProperty = targetProperty;
        }
    }
}
