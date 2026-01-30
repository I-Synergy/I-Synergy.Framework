namespace ISynergy.Framework.Core.Events;

public sealed class ItemPropertyChangedEventArgs<T> : EventArgs
{
    public ItemPropertyChangedEventArgs(T item, string? propertyName)
    {
        Item = item;
        PropertyName = propertyName;
    }

    public T Item { get; private set; }

    public string? PropertyName { get; private set; }
}
