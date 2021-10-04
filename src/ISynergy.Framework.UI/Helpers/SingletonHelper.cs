using System;
using System.Collections.Concurrent;

namespace ISynergy.Framework.UI.Helpers
{
    /// <summary>
    /// Class Singleton.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Singleton<T> where T : new()
    {
        /// <summary>
        /// The instances
        /// </summary>
        private static readonly ConcurrentDictionary<Type, T> _instances = new ConcurrentDictionary<Type, T>();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance => _instances.GetOrAdd(typeof(T), (_) => new T());
    }
}
