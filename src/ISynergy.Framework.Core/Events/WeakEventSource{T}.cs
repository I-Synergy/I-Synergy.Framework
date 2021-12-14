using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ISynergy.Framework.Core.Events
{
    /// <summary>
    /// Class WeakEventSource.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the t event arguments.</typeparam>
    public class WeakEventSource<TEventArgs>
    {
        /// <summary>
        /// The handlers
        /// </summary>
        private readonly List<WeakDelegate> _handlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventSource{TEventArgs}"/> class.
        /// </summary>
        public WeakEventSource()
        {
            _handlers = new List<WeakDelegate>();
        }

        /// <summary>
        /// Raises the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        public void Raise(object sender, TEventArgs e)
        {
            lock (_handlers)
            {
                _handlers.RemoveAll(h => !h.Invoke(sender, e));
            }
        }

        /// <summary>
        /// Subscribes the specified handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void Subscribe(EventHandler<TEventArgs> handler)
        {
            var weakHandlers = handler
                .GetInvocationList()
                .Select(d => new WeakDelegate(d))
                .ToList();

            lock (_handlers)
            {
                _handlers.AddRange(weakHandlers);
            }
        }

        /// <summary>
        /// Unsubscribes the specified handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void Unsubscribe(EventHandler<TEventArgs> handler)
        {
            lock (_handlers)
            {
                var index = _handlers.FindIndex(h => h.IsMatch(handler));
                if (index >= 0)
                    _handlers.RemoveAt(index);
            }
        }

        /// <summary>
        /// Class WeakDelegate.
        /// </summary>
        private class WeakDelegate
        {
            /// <summary>
            /// Delegate OpenEventHandler
            /// </summary>
            /// <param name="target">The target.</param>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The instance containing the event data.</param>
            private delegate void OpenEventHandler(object target, object sender, TEventArgs e);

            /// <summary>
            /// The open handler cache
            /// </summary>
            private static readonly ConcurrentDictionary<MethodInfo, OpenEventHandler> _openHandlerCache =
                new ConcurrentDictionary<MethodInfo, OpenEventHandler>();

            /// <summary>
            /// Creates the open handler.
            /// </summary>
            /// <param name="method">The method.</param>
            /// <returns>OpenEventHandler.</returns>
            private static OpenEventHandler CreateOpenHandler(MethodInfo method)
            {
                var target = Expression.Parameter(typeof(object), "target");
                var sender = Expression.Parameter(typeof(object), "sender");
                var e = Expression.Parameter(typeof(TEventArgs), "e");

                if (method.IsStatic)
                {
                    var expr = Expression.Lambda<OpenEventHandler>(
                        Expression.Call(
                            method,
                            sender, e),
                        target, sender, e);
                    return expr.Compile();
                }
                else
                {
                    var expr = Expression.Lambda<OpenEventHandler>(
                        Expression.Call(
                            Expression.Convert(target, method.DeclaringType),
                            method,
                            sender, e),
                        target, sender, e);
                    return expr.Compile();
                }
            }

            /// <summary>
            /// The weak target
            /// </summary>
            private readonly WeakReference _weakTarget;

            /// <summary>
            /// The method
            /// </summary>
            private readonly MethodInfo _method;

            /// <summary>
            /// The open handler
            /// </summary>
            private readonly OpenEventHandler _openHandler;

            /// <summary>
            /// Initializes a new instance of the <see cref="WeakDelegate"/> class.
            /// </summary>
            /// <param name="handler">The handler.</param>
            public WeakDelegate(Delegate handler)
            {
                _weakTarget = handler.Target is not null ? new WeakReference(handler.Target) : null;
                _method = handler.GetMethodInfo();
                _openHandler = _openHandlerCache.GetOrAdd(_method, CreateOpenHandler);
            }

            /// <summary>
            /// Invokes the specified sender.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The instance containing the event data.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public bool Invoke(object sender, TEventArgs e)
            {
                object target = null;
                if (_weakTarget is not null)
                {
                    target = _weakTarget.Target;
                    if (target is null)
                        return false;
                }
                _openHandler(target, sender, e);
                return true;
            }

            /// <summary>
            /// Determines whether the specified handler is match.
            /// </summary>
            /// <param name="handler">The handler.</param>
            /// <returns><c>true</c> if the specified handler is match; otherwise, <c>false</c>.</returns>
            public bool IsMatch(EventHandler<TEventArgs> handler)
            {
                return ReferenceEquals(handler.Target, _weakTarget?.Target)
                    && handler.GetMethodInfo().Equals(_method);
            }
        }
    }
}
