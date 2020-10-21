using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Framework.UI.Extensions
{
    /// <summary>
    /// Extension methods and attached properties for the ListView class.
    /// </summary>
    public static class ListViewExtensions
    {
        /// <summary>
        /// BindableSelection Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty BindableSelectionProperty =
            DependencyProperty.RegisterAttached(
                "BindableSelection",
                typeof(object),
                typeof(ListViewExtensions),
                new PropertyMetadata(null, OnBindableSelectionChanged));

        /// <summary>
        /// Gets the BindableSelection property. This dependency property
        /// indicates the list of selected items that is synchronized
        /// with the items selected in the ListView.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>ObservableCollection&lt;System.Object&gt;.</returns>
        public static ObservableCollection<object> GetBindableSelection(DependencyObject d) =>
            (ObservableCollection<object>)d.GetValue(BindableSelectionProperty);

        /// <summary>
        /// Sets the BindableSelection property. This dependency property
        /// indicates the list of selected items that is synchronized
        /// with the items selected in the ListView.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        public static void SetBindableSelection(DependencyObject d, ObservableCollection<object> value) =>
            d.SetValue(BindableSelectionProperty, value);

        /// <summary>
        /// Handles changes to the BindableSelection property.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject" /> on which
        /// the property has changed value.</param>
        /// <param name="e">Event data that is issued by any event that
        /// tracks changes to the effective value of this property.</param>
        private static void OnBindableSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is ListViewBase listViewBase)
            {
                var oldBindableSelection = e.OldValue;
                var newBindableSelection = d.GetValue(BindableSelectionProperty);

                if (oldBindableSelection != null)
                {
                    var handler = GetBindableSelectionHandler(d);
                    SetBindableSelectionHandler(d, null);
                    handler.Detach();
                }

                if (newBindableSelection is ObservableCollection<object> newSelection)
                {
                    var handler = new ListViewBindableSelectionHandler(listViewBase, newSelection);
                    SetBindableSelectionHandler(d, handler);
                }
            }
        }

        /// <summary>
        /// BindableSelectionHandler Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty BindableSelectionHandlerProperty =
            DependencyProperty.RegisterAttached(
                "BindableSelectionHandler",
                typeof(ListViewBindableSelectionHandler),
                typeof(ListViewExtensions),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the BindableSelectionHandler property. This dependency property
        /// indicates BindableSelectionHandler for a ListView - used
        /// to manage synchronization of BindableSelection and SelectedItems.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>ListViewBindableSelectionHandler.</returns>
        public static ListViewBindableSelectionHandler GetBindableSelectionHandler(DependencyObject d) =>
            (ListViewBindableSelectionHandler)d.GetValue(BindableSelectionHandlerProperty);

        /// <summary>
        /// Sets the BindableSelectionHandler property. This dependency property
        /// indicates BindableSelectionHandler for a ListView - used to manage synchronization of BindableSelection and SelectedItems.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        public static void SetBindableSelectionHandler(DependencyObject d, ListViewBindableSelectionHandler value) =>
            d.SetValue(BindableSelectionHandlerProperty, value);

        /// <summary>
        /// ItemToBringIntoView Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty ItemToBringIntoViewProperty =
            DependencyProperty.RegisterAttached(
                "ItemToBringIntoView",
                typeof(object),
                typeof(ListViewExtensions),
                new PropertyMetadata(null, OnItemToBringIntoViewChanged));

        /// <summary>
        /// Gets the ItemToBringIntoView property. This dependency property
        /// indicates the item that should be brought into view.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Object.</returns>
        public static object GetItemToBringIntoView(DependencyObject d) =>
            d.GetValue(ItemToBringIntoViewProperty);

        /// <summary>
        /// Sets the ItemToBringIntoView property. This dependency property
        /// indicates the item that should be brought into view when first set.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        public static void SetItemToBringIntoView(DependencyObject d, object value) =>
            d.SetValue(ItemToBringIntoViewProperty, value);

        /// <summary>
        /// Handles changes to the ItemToBringIntoView property.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject" /> on which
        /// the property has changed value.</param>
        /// <param name="e">Event data that is issued by any event that
        /// tracks changes to the effective value of this property.</param>
        private static void OnItemToBringIntoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newItemToBringIntoView = d.GetValue(ItemToBringIntoViewProperty);

            if (newItemToBringIntoView != null)
            {
                var listView = (ListView)d;

#if NETFX_CORE || __ANDROID__
                listView.ScrollIntoView(newItemToBringIntoView);
#endif
            }
        }

        /// <summary>
        /// Scrolls a vertical ListView to the bottom.
        /// </summary>
        /// <param name="listView">The list view.</param>
        public static void ScrollToBottom(this ListView listView)
        {
            var scrollViewer = listView.FindParent<ScrollViewer>();
            scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
        }
    }

    /// <summary>
    /// Handles synchronization of ListViewExtensions.BindableSelection to a ListView.
    /// </summary>
    public class ListViewBindableSelectionHandler
    {
        /// <summary>
        /// The list view
        /// </summary>
        private ListViewBase _listView;
        /// <summary>
        /// The selection
        /// </summary>
        private ObservableCollection<object> _selection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewBindableSelectionHandler" /> class.
        /// </summary>
        /// <param name="listView">The ListView.</param>
        /// <param name="selection">The bound selection.</param>
        public ListViewBindableSelectionHandler(ListViewBase listView, ObservableCollection<object> selection)
        {
            Attach(listView, selection);
        }

        /// <summary>
        /// Attaches the specified list view.
        /// </summary>
        /// <param name="listView">The list view.</param>
        /// <param name="selection">The selection.</param>
        private void Attach(ListViewBase listView, ObservableCollection<object> selection)
        {
            _listView = listView;
            _listView.SelectionChanged += OnListViewSelectionChanged;
            _selection = selection;
            _listView.SelectedItems.Clear();

            foreach (var item in _selection)
            {
                if (!_listView.SelectedItems.Contains(item))
                    _listView.SelectedItems.Add(item);
            }

            selection.CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// Handles the <see cref="E:ListViewSelectionChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                if (_selection.Contains(item))
                    _selection.Remove(item);
            }

            foreach (var item in e.AddedItems)
            {
                if (!_selection.Contains(item))
                    _selection.Add(item);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:CollectionChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _listView.SelectedItems.Clear();

                foreach (var item in _selection)
                {
                    if (!_listView.SelectedItems.Contains(item))
                        _listView.SelectedItems.Add(item);
                }

                return;
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (_listView.SelectedItems.Contains(item))
                        _listView.SelectedItems.Remove(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (!_listView.SelectedItems.Contains(item))
                        _listView.SelectedItems.Add(item);
                }
            }
        }

        /// <summary>
        /// Detaches this instance.
        /// </summary>
        internal void Detach()
        {
            if(_listView != null)
                _listView.SelectionChanged -= OnListViewSelectionChanged;

            _listView = null;

            if (_selection != null)
                _selection.CollectionChanged -= OnCollectionChanged;

            _selection = null;
        }
    }
}
