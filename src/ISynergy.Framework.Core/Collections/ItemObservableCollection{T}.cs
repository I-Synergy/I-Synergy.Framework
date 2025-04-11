using ISynergy.Framework.Core.Events;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ISynergy.Framework.Core.Collections;

/// <summary>
/// ObservableCollection where also item propertychanged is observed.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <example>
/// var invoices = new ItemObservableCollection{Invoice}();
/// invoices.CollectionChanged   += OnInvoicesChanged;
/// invoices.ItemPropertyChanged += OnInvoiceChanged;
/// </example>
public sealed class ItemObservableCollection<T> : ObservableCollection<T>, IDisposable
    where T : INotifyPropertyChanged
{
    public event EventHandler<ItemPropertyChangedEventArgs<T>>? ItemPropertyChanged;

    public ItemObservableCollection()
        : base()
    {
        CollectionChanged += item_CollectionChanged;
    }

    private void item_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (T item in e.NewItems)
            {
                item.PropertyChanged += item_PropertyChanged;
            }
        }

        if (e.OldItems is not null)
        {
            foreach (T item in e.OldItems)
            {
                item.PropertyChanged -= item_PropertyChanged;
            }
        }
    }

    protected override void SetItem(int index, T item)
    {
        T oldItem = this[index];
        oldItem.PropertyChanged -= item_PropertyChanged;
        base.SetItem(index, item);
    }

    private void item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not null && sender is T item)
            OnItemPropertyChanged(item, e.PropertyName);
    }

    private void OnItemPropertyChanged(T item, string? propertyName)
    {
        ItemPropertyChanged?.Invoke(this, new ItemPropertyChangedEventArgs<T>(item, propertyName));
    }

    protected override void RemoveItem(int index)
    {
        var item = this[index];
        item.PropertyChanged -= item_PropertyChanged;

        base.RemoveItem(index);
    }

    protected override void ClearItems()
    {
        foreach (var item in this)
        {
            item.PropertyChanged -= item_PropertyChanged;
        }

        base.ClearItems();
    }

    public void Dispose()
    {
        CollectionChanged -= item_CollectionChanged;

        foreach (var item in this)
        {
            item.PropertyChanged -= item_PropertyChanged;
        }
    }
}
