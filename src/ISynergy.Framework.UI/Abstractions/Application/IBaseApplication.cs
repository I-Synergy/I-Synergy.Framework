using ISynergy.Framework.Core.Events;
using System.Runtime.ExceptionServices;

namespace ISynergy.Framework.UI.Abstractions;

public interface IBaseApplication
{
    event EventHandler<ReturnEventArgs<bool>> ApplicationInitialized;
    event EventHandler<ReturnEventArgs<bool>> ApplicationLoaded;
    void RaiseApplicationInitialized();
    void RaiseApplicationLoaded();
    Task InitializeApplicationAsync();
    void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e);
    void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e);
}
