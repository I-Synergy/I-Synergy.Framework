using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Collections
{
    /// <summary>
    ///   Vanilla key-based comparer for <see cref="KeyValuePair{TKey, TValue}"/>.
    /// </summary>
    /// 
    /// <typeparam name="TKey">The key type in the key-value pair.</typeparam>
    /// <typeparam name="TValue">The value type in the key-value pair.</typeparam>
    /// 
    [Serializable]
    public class KeyValuePairComparer<TKey, TValue>
        : Comparer<KeyValuePair<TKey, TValue>>, IComparer<TKey>
    {

        private readonly IComparer<TKey> _keyComparer;


        /// <summary>
        ///   Initializes a new instance of the <see cref="KeyValuePairComparer&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="keyComparer">The comparer to be used to compare keys.</param>
        /// 
        public KeyValuePairComparer(IComparer<TKey> keyComparer)
        {
            Argument.IsNotNull(keyComparer);

            _keyComparer = keyComparer;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KeyValuePairComparer&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// 
        public KeyValuePairComparer()
        {
            _keyComparer = Comparer<TKey>.Default;
        }

        /// <summary>
        ///   Compares two objects and returns a value indicating whether 
        ///   one is less than, equal to, or greater than the other.
        /// </summary>
        /// 
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// 
        public override int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return _keyComparer.Compare(x.Key, y.Key);
        }

        /// <summary>
        ///   Compares two objects and returns a value indicating whether 
        ///   one is less than, equal to, or greater than the other.
        /// </summary>
        /// 
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// 
        public int Compare(TKey x, TKey y)
        {
            return _keyComparer.Compare(x, y);
        }

        /// <summary>
        ///    Returns a default sort order comparer for the
        ///    key-value pair specified by the generic argument.
        /// </summary>
        /// 
        public new static KeyValuePairComparer<TKey, TValue> Default
        {
            get { return new KeyValuePairComparer<TKey, TValue>(); }
        }
    }
}
