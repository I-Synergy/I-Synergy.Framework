﻿using System;
using System.Collections.Concurrent;

namespace ISynergy.Helpers
{
    public static class Singleton<T> where T : new()
    {
        private static readonly ConcurrentDictionary<Type, T> _instances = new ConcurrentDictionary<Type, T>();

        public static T Instance
        {
            get
            {
                return _instances.GetOrAdd(typeof(T), (_) => new T());
            }
        }
    }
}
