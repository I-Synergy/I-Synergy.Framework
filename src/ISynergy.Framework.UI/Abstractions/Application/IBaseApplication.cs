namespace ISynergy.Framework.UI.Abstractions;

public interface IBaseApplication
{
    Task InitializeApplicationAsync();
    void InitializeApplication();
}
