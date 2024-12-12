namespace ISynergy.Framework.Core.Abstractions.Services;
public interface IScopedContextService : IDisposable
{
    void CreateNewScope();
    object GetService(Type serviceType);
    TService GetService<TService>();
    IServiceProvider ServiceProvider { get; }
}
