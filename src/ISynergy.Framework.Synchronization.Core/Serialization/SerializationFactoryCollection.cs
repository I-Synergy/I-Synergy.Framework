﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ISynergy.Framework.Synchronization.Core.Serialization
{
    public class SerializersCollection : ICollection<ISerializerFactory>
    {
        private Collection<ISerializerFactory> collection = new Collection<ISerializerFactory>();

        /// <summary>
        /// Get the default Json serializer
        /// </summary>
        public static ISerializerFactory JsonSerializerFactory { get; } = new JsonConverterFactory();

        /// <summary>
        /// Get the default Json serializer
        /// </summary>
        public static ISerializerFactory DataContractSerializerFactory { get; } = new ContractSerializerFactory();


        /// <summary>
        /// Create a default collection with 2 known serializers
        /// </summary>
        public SerializersCollection()
        {
            // add json serializer, as default
            Add(JsonSerializerFactory);
            // add utf8json serializer
            //Add(Utf8JsonSerializer);
            // add binary serializer;
            Add(DataContractSerializerFactory);
        }

        public ISerializerFactory this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");

                return collection.FirstOrDefault(c => c.Key.ToLowerInvariant() == key.ToLowerInvariant());
            }
        }

        public void Add(ISerializerFactory factory) => collection.Add(factory);
        public ISerializerFactory this[int index] => collection[index];
        public int Count => collection.Count;
        public bool IsReadOnly => false;
        public bool Remove(ISerializerFactory factory) => collection.Remove(factory);
        public void Clear() => collection.Clear();
        public bool Contains(ISerializerFactory item) => collection.Contains(item);
        public void CopyTo(ISerializerFactory[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);
        public int IndexOf(ISerializerFactory factory) => collection.IndexOf(factory);
        public void RemoveAt(int index) => collection.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => collection.GetEnumerator();
        public IEnumerator<ISerializerFactory> GetEnumerator() => collection.GetEnumerator();
        public override string ToString() => collection.Count.ToString();
    }
}