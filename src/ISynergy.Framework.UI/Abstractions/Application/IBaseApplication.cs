using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Messages;
using System.Runtime.ExceptionServices;

namespace ISynergy.Framework.UI.Abstractions;

public interface IBaseApplication
{
    Task InitializeApplicationAsync();
    void StyleChanged(StyleChangedMessage m);
    void AuthenticationChanged(object sender, ReturnEventArgs<bool> e);
    void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e);
    void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e);
}
