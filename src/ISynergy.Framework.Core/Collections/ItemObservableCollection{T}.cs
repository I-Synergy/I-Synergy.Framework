using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using System.Collections.ObjectModel;
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

    protected override void InsertItem(int index, T item)
    {
        base.InsertItem(index, item);
        item.PropertyChanged += item_PropertyChanged;
    }

    protected override void RemoveItem(int index)
    {
        var item = this[index];
        base.RemoveItem(index);
        item.PropertyChanged -= item_PropertyChanged;
    }

    protected override void ClearItems()
    {
        foreach (var item in this.EnsureNotNull())
            item.PropertyChanged -= item_PropertyChanged;

        base.ClearItems();
    }

    protected override void SetItem(int index, T item)
    {
        this[index].PropertyChanged -= item_PropertyChanged;
        base.SetItem(index, item);
        item.PropertyChanged += item_PropertyChanged;
    }

    private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        OnItemPropertyChanged((T)sender, e.PropertyName);
    }

    private void OnItemPropertyChanged(T item, string propertyName)
    {
        var handler = this.ItemPropertyChanged;

        if (handler != null)
            handler(this, new ItemPropertyChangedEventArgs<T>(item, propertyName));
    }
}
