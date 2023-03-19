using System.Collections;
using System.Data;
using System.IO.Compression;

namespace ISynergy.Framework.Mathematics.IO.NumPy
{
    /// <summary>
    ///     Lazily-loaded collection of arrays from a compressed .npz archive.
    /// </summary>
    /// <typeparam name="T">The type of the arrays to be loaded.</typeparam>
    /// <seealso cref="NpyFormat" />
    /// <seealso cref="NpzFormat" />
    public class NpzDictionary<T> : IDisposable, IReadOnlyDictionary<string, T>, ICollection<T>
        where T : class, ICloneable, IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
    {
        private ZipArchive archive;
        private Dictionary<string, T> arrays;

        private bool disposedValue;

        private Dictionary<string, ZipArchiveEntry> entries;
        private Stream stream;
        /// <summary>
        ///     Initializes a new instance of the <see cref="NpzDictionary{T}" /> class.
        /// </summary>
        /// <param name="stream">The stream from where the arrays should be loaded from.</param>
        public NpzDictionary(Stream stream)
        {
            this.stream = stream;
            archive = new ZipArchive(stream, ZipArchiveMode.Read, true);

            entries = new Dictionary<string, ZipArchiveEntry>();
            foreach (var entry in archive.Entries)
                entries[entry.Name] = entry;

            arrays = new Dictionary<string, T>();
        }

        /// <summary>
        ///     Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        public object SyncRoot => ((ICollection)entries).SyncRoot;

        /// <summary>
        ///     Gets a value indicating whether the access to collection is synchronized (thread-safe).
        /// </summary>
        public bool IsSynchronized => ((ICollection)entries).IsSynchronized;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly => true;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (var entry in archive.Entries)
                yield return OpenEntry(entry);
        }

        /// <summary>
        ///     Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an
        ///     <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied
        ///     from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have
        ///     zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var v in this)
                array.SetValue(v, arrayIndex++);
        }

        /// <summary>
        ///     Throws a <see cref="ReadOnlyException" />.
        /// </summary>
        public void Add(T item)
        {
            throw new ReadOnlyException();
        }

        /// <summary>
        ///     Throws a <see cref="ReadOnlyException" />.
        /// </summary>
        public void Clear()
        {
            throw new ReadOnlyException();
        }

        /// <summary>
        ///     Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        ///     true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />;
        ///     otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            foreach (var v in this)
                if (Equals(v.Value, item))
                    return true;
            return false;
        }

        /// <summary>
        ///     Throws a <see cref="ReadOnlyException" />.
        /// </summary>
        public bool Remove(T item)
        {
            throw new ReadOnlyException();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        /// <summary>
        ///     Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        /// <value>The keys.</value>
        public IEnumerable<string> Keys => entries.Keys;

        /// <summary>
        ///     Gets an enumerable collection that contains the values in the read-only dictionary.
        /// </summary>
        /// <value>The values.</value>
        public IEnumerable<T> Values => entries.Values.Select(OpenEntry);

        /// <summary>
        ///     Gets the number of elements in the collection.
        /// </summary>
        /// <value>The count.</value>
        public int Count => entries.Count;

        /// <summary>
        ///     Gets the array stored under the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        public T this[string key] => OpenEntry(entries[key]);

        /// <summary>
        ///     Determines whether the read-only dictionary contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>true if the read-only dictionary contains an element that has the specified key; otherwise, false.</returns>
        public bool ContainsKey(string key)
        {
            return entries.ContainsKey(key);
        }

        /// <summary>
        ///     Gets the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">
        ///     When this method returns, the value associated with the specified key, if the key is found;
        ///     otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed
        ///     uninitialized.
        /// </param>
        /// <returns>
        ///     true if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2" />
        ///     interface contains an element that has the specified key; otherwise, false.
        /// </returns>
        public bool TryGetValue(string key, out T value)
        {
            value = default;
            ZipArchiveEntry entry;
            if (!entries.TryGetValue(key, out entry))
                return false;
            value = OpenEntry(entry);
            return true;
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            foreach (var entry in archive.Entries)
                yield return new KeyValuePair<string, T>(entry.Name, OpenEntry(entry));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var entry in archive.Entries)
                yield return new KeyValuePair<string, T>(entry.Name, OpenEntry(entry));
        }
        private T OpenEntry(ZipArchiveEntry entry)
        {
            T array;
            if (arrays.TryGetValue(entry.Name, out array))
                return array;

            var s = entry.Open();
            array = Load(s);
            arrays[entry.Name] = array;
            return array;
        }

        /// <summary>
        ///     Loads the array from the specified stream.
        /// </summary>
        protected virtual T Load(Stream s)
        {
            return NpyFormat.Load<T>(s);
        }

        /// <summary>
        ///     Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an
        ///     <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied
        ///     from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have
        ///     zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(Array array, int arrayIndex)
        {
            foreach (var v in this)
                array.SetValue(v, arrayIndex++);
        }
        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    archive.Dispose();
                    stream.Dispose();
                }

                archive = null;
                stream = null;
                entries = null;
                arrays = null;

                disposedValue = true;
            }
        }
    }
}