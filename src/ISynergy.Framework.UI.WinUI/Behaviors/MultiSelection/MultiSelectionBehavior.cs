using ISynergy.Framework.Core.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System.Collections.Specialized;
using ListView = Microsoft.UI.Xaml.Controls.ListView;

namespace ISynergy.Framework.UI.Behaviors;

/// <summary>
/// Class MultiSelectionBehavior.
/// Implements the <see cref="Behavior{ListView}" />
/// </summary>
/// <seealso cref="Behavior{ListView}" />
public class MultiSelectionBehavior : Behavior<ListView>
{
    /// <summary>
    /// Called after the behavior is attached to the <see cref="P:Behavior.AssociatedObject" />.
    /// </summary>
    /// <remarks>Override this to hook up functionality to the <see cref="P:Behavior.AssociatedObject" /></remarks>
    protected override void OnAttached()
    {
        base.OnAttached();
        if (SelectedItems is not null)
        {
            AssociatedObject.SelectedItems.Clear();
            foreach (var item in SelectedItems.EnsureNotNull())
            {
                AssociatedObject.SelectedItems.Add(item);
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected items.
    /// </summary>
    /// <value>The selected items.</value>
    public IList<object> SelectedItems
    {
        get => (IList<object>)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    /// <summary>
    /// The selected items property
    /// </summary>
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register(nameof(SelectedItems), typeof(IList<object>), typeof(MultiSelectionBehavior), new PropertyMetadata(null, SelectedItemsChanged));

    /// <summary>
    /// Selecteds the items changed.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void SelectedItemsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (!(o is MultiSelectionBehavior behavior))
            return;


        if (e.OldValue is INotifyCollectionChanged oldValue)
        {
            oldValue.CollectionChanged -= behavior.SourceCollectionChanged;
            behavior.AssociatedObject.SelectionChanged -= behavior.ListViewSelectionChanged;
        }
        if (e.NewValue is INotifyCollectionChanged newValue)
        {
            behavior.AssociatedObject.SelectedItems.Clear();
            foreach (var item in ((IEnumerable<object>)newValue).EnsureNotNull())
            {
                behavior.AssociatedObject.SelectedItems.Add(item);
            }

            behavior.AssociatedObject.SelectionChanged += behavior.ListViewSelectionChanged;
            newValue.CollectionChanged += behavior.SourceCollectionChanged;
        }
    }

    /// <summary>
    /// The is updating target
    /// </summary>
    private bool _isUpdatingTarget;
    /// <summary>
    /// The is updating source
    /// </summary>
    private bool _isUpdatingSource;

    /// <summary>
    /// Sources the collection changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
    void SourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_isUpdatingSource)
            return;

        try
        {
            _isUpdatingTarget = true;

            if (e.OldItems is not null)
            {
                foreach (var item in e.OldItems.EnsureNotNull())
                {
                    AssociatedObject.SelectedItems.Remove(item);
                }
            }

            if (e.NewItems is not null)
            {
                foreach (var item in e.NewItems.EnsureNotNull())
                {
                    AssociatedObject.SelectedItems.Add(item);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                AssociatedObject.SelectedItems.Clear();
            }
        }
        finally
        {
            _isUpdatingTarget = false;
        }
    }

    /// <summary>
    /// ListViews the selection changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
    private void ListViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isUpdatingTarget)
            return;

        var selectedItems = SelectedItems;
        if (selectedItems is null)
            return;

        try
        {
            _isUpdatingSource = true;

            foreach (var item in e.RemovedItems.EnsureNotNull())
            {
                selectedItems.Remove(item);
            }

            foreach (var item in e.AddedItems.EnsureNotNull())
            {
                selectedItems.Add(item);
            }
        }
        finally
        {
            _isUpdatingSource = false;
        }
    }
}
