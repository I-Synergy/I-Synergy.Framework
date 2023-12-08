using System.Windows.Threading;

namespace ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;

public interface IInterval
{
    bool IsRunning { get; }
    void Invoke(TimeSpan frequency, Action action, Dispatcher dispatcher);
    void Stop();
}
