using ISynergy.ViewModels.Base;

namespace ISynergy.Services
{
    public interface IWindowService
    {
        void InitializeScreen();

        void ToggleScreen();

        void SetNormalScreen();

        void SetFullScreen();

        void SetMinimized();

        void ExitApplication();

        object MainWindow { get; }
        IWindow GetWindow(IViewModel viewmodel);
    }
}