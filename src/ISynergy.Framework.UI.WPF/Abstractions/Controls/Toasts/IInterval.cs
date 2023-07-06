using System.Windows.Threading;

namespace ISynergy.Framework.UI.Abstractions.Controls.Toasts
{
    public interface IInterval
    {
        bool IsRunning { get; }
        void Invoke(TimeSpan frequency, Action action, Dispatcher dispatcher);
        void Stop();
    }
}
