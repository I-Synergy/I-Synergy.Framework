namespace ISynergy.Framework.Core.Collections
{
    /// <summary>
    ///   Sorted dictionary based on a <see cref="RedBlackTree{T}">red-black tree</see>.
    /// </summary>
    /// 
    /// <typeparam name="TKey">The type of keys in the collection.</typeparam>
    /// <typeparam name="TValue">The type of the values in the collection</typeparam>
    /// 
    [Serializable]
    public class RedBlackTreeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private RedBlackTree<TKey, TValue> _tree;

        // proxies for tree iteration
        private ValueCollection _values;
        private KeyCollection _keys;


        /// <summary>
        ///   Creates a new <see cref="RedBlackTreeDictionary{TKey, TValue}"/> 
        ///   using the default comparer for the <typeparamref name="TKey">key
        ///   type</typeparamref>.
        /// </summary>
        /// 
        public RedBlackTreeDictionary()
        {
            init(Comparer<TKey>.Default);
        }

        /// <summary>
        ///   Creates a new <see cref="RedBlackTreeDictionary{TKey, TValue}"/>.
        /// </summary>
        /// 
        public RedBlackTreeDictionary(IComparer<TKey> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            init(comparer);
        }

        private void init(IComparer<TKey> comparer)
        {
            var keyComparer = new KeyValuePairComparer<TKey, TValue>(comparer);
            _tree = new RedBlackTree<TKey, TValue>(keyComparer, false);
            _values = new ValueCollection(_tree);
            _keys = new KeyCollection(_tree);
        }



        /// <summary>
        ///   Adds an element with the provided key and value to the <see cref="T:IDictionary`2" />.
        /// </summary>
        /// 
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// 
        public void Add(TKey key, TValue value)
        {
            _tree.Add(new RedBlackTreeNode<TKey, TValue>(key, value));
        }

        /// <summary>
        ///   Adds an element with the provided key and value to the dictionary.
        /// </summary>
        /// 
        /// <param name="item">
        ///   The <see cref="KeyValuePair{TKey, TValue}">key-value pair</see> 
        ///   containing the desired key and the value to be added.
        /// </param>
        /// 
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _tree.Add(new RedBlackTreeNode<TKey, TValue>(item));
        }

        /// <summary>
        ///   Removes the element with the specified key from the dictionary.
        /// </summary>
        /// 
        /// <param name="key">The key of the element to remove.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the element is successfully removed; otherwise, false. 
        ///   This method also returns false if <paramref name="key" /> was not found 
        ///   in the original dictionary.
        /// </returns>
        /// 
        public bool Remove(TKey key)
        {
            return Remove(new KeyValuePair<TKey, TValue>(key, default(TValue)));
        }

        /// <summary>
        ///   Removes the first occurrence of a specific object from the dictionary.
        /// </summary>
        /// 
        /// <param name="item">The object to remove from the dictionary.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if <paramref name="item" /> was successfully removed from 
        ///   the dictionary; otherwise, false. This method also returns false if 
        ///   <paramref name="item" /> is not found in the original dictionary.
        /// </returns>
        /// 
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var node = _tree.Find(item);

            if (node == null)
                return false;

            var result = _tree.Remove(node);

            return result != null;
        }


        /// <summary>
        ///   Determines whether the dictionary contains an element with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The key to locate in the dictionary.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the dictionary contains an element with the key; otherwise, false.
        /// </returns>
        /// 
        public bool ContainsKey(TKey key)
        {
            return _tree.Find(new KeyValuePair<TKey, TValue>(key, default(TValue))) != null;
        }

        /// <summary>
        ///   Determines whether the dictionary contains a specific value.
        /// </summary>
        /// 
        /// <param name="item">The object to locate in the dictionary.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if <paramref name="item" /> is found in the dictionary; otherwise, false.
        /// </returns>
        /// 
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var result = _tree.Find(item);

            return result != null && item.Value.Equals(result.Value);
        }

        /// <summary>
        ///   Gets an <see cref="T:ICollection{T}" /> 
        ///   containing the keys of the <see cref="RedBlackTreeDictionary{TKey, TValue}"/>.
        /// </summary>
        /// 
        public ICollection<TKey> Keys
        {
            get { return _keys; }
        }

        /// <summary>
        ///   Gets an <see cref="T:ICollection{T}" /> 
        ///   containing the values of the <see cref="RedBlackTreeDictionary{TKey, TValue}"/>.
        /// </summary>
        /// 
        public ICollection<TValue> Values
        {
            get { return _values; }
        }


        /// <summary>
        ///   Gets the value associated with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">
        ///   When this method returns, the value associated with the specified key, 
        ///   if the key is found; otherwise, the default value for the type of the 
        ///   <paramref name="value" /> parameter. This parameter is passed 
        ///   uninitialized.
        /// </param>
        /// 
        /// <returns>
        ///   <c>true</c> if the dictionary contains an element with the specified key; otherwise, false.
        /// </returns>
        /// 
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);

            var result = _tree.Find(new KeyValuePair<TKey, TValue>(key, value));

            if (result == null)
                return false;

            value = result.Value.Value;
            return true;
        }



        /// <summary>
        ///   Gets or sets the element with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The key.</param>
        /// 
        /// <exception cref="KeyNotFoundException">The requested key was not found in the present tree.</exception>
        /// 
        public TValue this[TKey key]
        {
            get
            {
                var result = _tree.Find(new KeyValuePair<TKey, TValue>(key, default(TValue)));

                if (result == null)
                    throw new KeyNotFoundException("The requested key was not found in the present tree.");

                return result.Value.Value;
            }
            set
            {
                var pair = new KeyValuePair<TKey, TValue>(key, value);

                var result = _tree.Find(pair);

                if (result != null)
                {
                    result.Value = pair;
                }
                else
                {
                    Add(pair);
                }
            }
        }


        /// <summary>
        ///   Removes all elements from the dictionary.
        /// </summary>
        /// 
        public void Clear()
        {
            _tree.Clear();
        }


        /// <summary>
        ///   Copies the elements of this dictionary to an array, starting at a particular array index.
        /// </summary>
        /// 
        /// <param name="array">
        ///   The one-dimensional Array that is the destination of the elements
        ///   copied from ICollection. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// 
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var node in _tree)
                array[arrayIndex++] = node.Value;
        }

        /// <summary>
        ///   Gets the number of elements on this dictionary.
        /// </summary>
        /// 
        public int Count
        {
            get { return _tree.Count; }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is read only.
        /// </summary>
        /// 
        /// <value>
        ///   Returns <c>false</c>.
        /// </value>
        /// 
        public bool IsReadOnly
        {
            get { return false; }
        }


        /// <summary>
        ///   Returns an enumerator that iterates through the dictionary.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:IEnumerator{T}"/>
        ///   object that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var node in _tree)
                yield return node.Value;
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the dictionary.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///   Gets the pair with the minimum <c>key</c> stored in the dictionary.
        /// </summary>
        /// 
        /// <returns>
        ///   The <see cref="KeyValuePair{TKey, TValue}"/> with
        ///   the minimum key present in the dictionary.
        /// </returns>
        /// 
        public KeyValuePair<TKey, TValue> Min()
        {
            if (_tree.Count == 0)
                throw new InvalidOperationException("The dictionary is empty.");

            return _tree.Min().Value;
        }

        /// <summary>
        ///   Gets the pair with the maximum <c>key</c> stored in the dictionary.
        /// </summary>
        /// 
        /// <returns>
        ///   The <see cref="KeyValuePair{TKey, TValue}"/> with
        ///   the minimum key present in the dictionary.
        /// </returns>
        /// 
        public KeyValuePair<TKey, TValue> Max()
        {
            if (_tree.Count == 0)
                throw new InvalidOperationException("The dictionary is empty.");

            return _tree.Max().Value;
        }


        /// <summary>
        ///   Gets the next key-value pair in the dictionary whose key is
        ///   the immediate ancestor of the given <paramref name="key"/>.
        /// </summary>
        /// 
        /// <param name="key">The key whose ancestor must be found.</param>
        /// 
        /// <returns>
        ///   The key-value pair whose key is the immediate ancestor of <paramref name="key"/>.
        /// </returns>
        /// 
        public KeyValuePair<TKey, TValue> GetPrevious(TKey key)
        {
            var node = _tree.Find(new KeyValuePair<TKey, TValue>(key, default(TValue)));
            var prevNode = _tree.GetPreviousNode(node);

            if (prevNode != null)
                return prevNode.Value;

            throw new KeyNotFoundException("There are no ancestor keys in the dictionary.");
        }

        /// <summary>
        ///   Gets the next key-value pair in the dictionary whose key is
        ///   the immediate ancestor of the given <paramref name="key"/>.
        /// </summary>
        /// 
        /// <param name="key">The key whose ancestor must be found.</param>
        /// <param name="prev">
        ///   The key-value pair whose key is the immediate ancestor of
        ///   <paramref name="key"/>, returned as an out parameter.
        /// </param>
        /// 
        /// <returns>
        ///   True if there was an ancestor in the dictionary; false otherwise.
        /// </returns>
        /// 
        public bool TryGetPrevious(TKey key, out KeyValuePair<TKey, TValue> prev)
        {
            prev = default(KeyValuePair<TKey, TValue>);

            var node = _tree.Find(new KeyValuePair<TKey, TValue>(key, default(TValue)));
            var prevNode = _tree.GetPreviousNode(node);

            if (prevNode != null)
            {
                prev = prevNode.Value;
                return true;
            }

            return false;
        }

        /// <summary>
        ///   Gets the next key-value pair in the dictionary whose key is
        ///   the immediate successor to the given <paramref name="key"/>.
        /// </summary>
        /// 
        /// <param name="key">The key whose successor must be found.</param>
        /// 
        /// <returns>
        ///   The key-value pair whose key is the immediate successor of <paramref name="key"/>.
        /// </returns>
        /// 
        public KeyValuePair<TKey, TValue> GetNext(TKey key)
        {
            var node = _tree.Find(new KeyValuePair<TKey, TValue>(key, default(TValue)));
            var nextNode = _tree.GetNextNode(node);

            if (nextNode != null)
                return nextNode.Value;

            throw new KeyNotFoundException("There are no successor keys in the dictionary.");
        }

        /// <summary>
        ///   Gets the next key-value pair in the dictionary whose key is
        ///   the immediate successor to the given <paramref name="key"/>.
        /// </summary>
        /// 
        /// <param name="key">The key whose successor must be found.</param>
        /// <param name="next">
        ///   The key-value pair whose key is the immediate sucessor of
        ///   <paramref name="key"/>, returned as an out parameter.
        /// </param>
        /// 
        /// <returns>
        ///   True if there was a successor in the dictionary; false otherwise.
        /// </returns>
        /// 
        public bool TryGetNext(TKey key, out KeyValuePair<TKey, TValue> next)
        {
            next = default(KeyValuePair<TKey, TValue>);

            var node = _tree.Find(new KeyValuePair<TKey, TValue>(key, default(TValue)));
            var nextNode = _tree.GetNextNode(node);

            if (nextNode != null)
            {
                next = nextNode.Value;
                return true;
            }

            return false;
        }



        [Serializable]
        internal class ValueCollection : ICollection<TValue>
        {
            RedBlackTree<KeyValuePair<TKey, TValue>> owner;

            public ValueCollection(RedBlackTree<KeyValuePair<TKey, TValue>> owner)
            {
                this.owner = owner;
            }

            public bool Contains(TValue item)
            {
                foreach (var node in owner)
                    if (item.Equals(node.Value.Value))
                        return true;
                return false;
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                foreach (var node in owner)
                    array[arrayIndex++] = node.Value.Value;
            }

            public int Count
            {
                get { return owner.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public void Add(TValue item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                foreach (var node in owner)
                    yield return node.Value.Value;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Serializable]
        internal class KeyCollection : ICollection<TKey>
        {
            RedBlackTree<KeyValuePair<TKey, TValue>> owner;

            public KeyCollection(RedBlackTree<KeyValuePair<TKey, TValue>> owner)
            {
                this.owner = owner;
            }

            public bool Contains(TKey item)
            {
                foreach (var node in owner)
                    if (item.Equals(node.Value.Key))
                        return true;
                return false;
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                foreach (var node in owner)
                    array[arrayIndex++] = node.Value.Key;
            }

            public int Count
            {
                get { return owner.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public void Add(TKey item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Remove(TKey item)
            {
                throw new NotSupportedException();
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                foreach (var node in owner)
                    yield return node.Value.Key;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
