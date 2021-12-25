using ISynergy.Framework.Synchronization.Core.Abstractions;
using ISynergy.Framework.Synchronization.Core.Wrappers;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace ISynergy.Framework.Synchronization.Core
{
    public class Interceptors
    {
        // Internal table builder cache
        private readonly ConcurrentDictionary<Type, Lazy<ISyncInterceptor>> _dictionary = new();

        [DebuggerStepThrough]
        public InterceptorWrapper<T> GetInterceptor<T>() where T : ProgressArgs
        {
            var typeofT = typeof(T);

            // Get a lazy command instance
            var lazyInterceptor = _dictionary.GetOrAdd(typeofT,
                k => new Lazy<ISyncInterceptor>(() => new InterceptorWrapper<T>()));

            return lazyInterceptor.Value as InterceptorWrapper<T>;
        }

        /// <summary>
        /// Gets a boolean returning true if an interceptor of type T, exists
        /// </summary>
        public bool Contains<T>() where T : ProgressArgs => _dictionary.ContainsKey(typeof(T));

    }

}
