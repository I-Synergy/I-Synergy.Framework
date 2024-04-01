using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
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
public sealed class ItemObservableCollection<T> : ObservableCollection<T>
    where T : INotifyPropertyChanged
{
    public event EventHandler<ItemPropertyChangedEventArgs<T>> ItemPropertyChanged;

    public ItemObservableCollection() 
        : base()
    {
        CollectionChanged += item_CollectionChanged;
    }

    private void item_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (T item in e.NewItems.EnsureNotNull())
        {
            item.PropertyChanged += item_PropertyChanged;
        }
    }

    protected override void SetItem(int index, T item)
    {
        base.SetItem(index, item);
        item.PropertyChanged += item_PropertyChanged;
    }

    private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        T item = (T)sender;
        OnItemPropertyChanged(item, e.PropertyName);
    }

    private void OnItemPropertyChanged(T item, string propertyName)
    {
        ItemPropertyChanged?.Invoke(this, new ItemPropertyChangedEventArgs<T>(item, propertyName));
    }
}
