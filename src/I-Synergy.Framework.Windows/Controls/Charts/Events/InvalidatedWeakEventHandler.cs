using System;

namespace ISynergy.Framework.Windows.Controls.Charts.Events
{
    /// <summary>
    /// A lightweight proxy instance that will subscribe to a given event with a weak reference to the subscribed target.
    /// If the subscriber is garbage collected, then only this WeakEventHandler will remain subscribed and keeped
    /// in memory instead of the actual subscriber.
    /// This could be considered as an acceptable solution in most cases.
    /// </summary>
    /// <typeparam name="TTarget">The type of the t target.</typeparam>
    public class InvalidatedWeakEventHandler<TTarget> : IDisposable where TTarget : class
    {
        /// <summary>
        /// The source reference
        /// </summary>
        private readonly WeakReference<Chart> sourceReference;
        /// <summary>
        /// The target reference
        /// </summary>
        private readonly WeakReference<TTarget> targetReference;
        /// <summary>
        /// The target method
        /// </summary>
        private readonly Action<TTarget> targetMethod;

        /// <summary>
        /// The is subscribed
        /// </summary>
        private bool isSubscribed;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidatedWeakEventHandler{TTarget}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="targetMethod">The target method.</param>
        public InvalidatedWeakEventHandler(Chart source, TTarget target, Action<TTarget> targetMethod)
        {
            sourceReference = new WeakReference<Chart>(source);
            targetReference = new WeakReference<TTarget>(target);
            this.targetMethod = targetMethod;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="InvalidateWeakEventHandler`1" /> is alive.
        /// </summary>
        /// <value><c>true</c> if is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive => sourceReference.TryGetTarget(out Chart s) && targetReference.TryGetTarget(out TTarget t);

        /// <summary>
        /// Subsribe this handler to the source.
        /// </summary>
        public void Subsribe()
        {
            if (!isSubscribed && sourceReference.TryGetTarget(out Chart source))
            {
                source.Invalidated += OnEvent;
                isSubscribed = true;
            }
        }

        /// <summary>
        /// Unsubscribe this handler from the source.
        /// </summary>
        public void Unsubscribe()
        {
            if (isSubscribed)
            {
                if (sourceReference.TryGetTarget(out Chart source))
                {
                    source.Invalidated -= OnEvent;
                }

                isSubscribed = false;
            }
        }

        /// <summary>
        /// Releases all resource used by the <see cref="InvalidatedWeakEventHandler{TTarget}" /> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose" /> when you are finished using the
        /// <see cref="InvalidatedWeakEventHandler{TTarget}" />. The <see cref="Dispose" /> method leaves the
        /// <see cref="InvalidatedWeakEventHandler{TTarget}" /> in an unusable state. After calling
        /// <see cref="Dispose" />, you must release all references to the
        /// <see cref="InvalidatedWeakEventHandler{TTarget}" /> so the garbage collector can reclaim the memory
        /// that the <see cref="InvalidatedWeakEventHandler{TTarget}" /> was occupying.</remarks>
        public void Dispose() => Unsubscribe();

        /// <summary>
        /// Handles the <see cref="E:Event" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnEvent(object sender, EventArgs args)
        {
            if (targetReference.TryGetTarget(out TTarget t))
            {
                targetMethod(t);
            }
            else
            {
                Unsubscribe();
            }
        }
    }
}
