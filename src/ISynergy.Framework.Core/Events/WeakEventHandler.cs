using System.Diagnostics;
using System.Reflection;

namespace ISynergy.Framework.Core.Events;

[DebuggerNonUserCode]
public sealed class WeakEventHandler<TEventArgs> where TEventArgs : EventArgs
{
    private readonly WeakReference _targetReference;
    private readonly MethodInfo _method;

    public WeakEventHandler(EventHandler<TEventArgs> callback)
    {
        _method = callback.Method;
        _targetReference = new WeakReference(callback.Target, true);
    }

    [DebuggerNonUserCode]
    public void Handler(object sender, TEventArgs e)
    {
        var target = _targetReference.Target;
        if (target is not null)
        {
            var callback = Delegate.CreateDelegate(typeof(Action<object, TEventArgs>), target, _method, true) as Action<object, TEventArgs>;

            if (callback is not null)
                callback(sender, e);
        }
    }
}
