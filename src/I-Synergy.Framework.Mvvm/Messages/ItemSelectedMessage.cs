using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    public class ItemSelectedMessage<T> : EventMessage
    {
        public T Value { get; }

        public ItemSelectedMessage(object sender, T value)
            : base(sender)
        {
            Value = value;
        }
    }
}
