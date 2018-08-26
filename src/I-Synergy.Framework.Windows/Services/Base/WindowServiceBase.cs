using System;
using ISynergy.ViewModels.Base;
using Windows.UI.Xaml;

namespace ISynergy.Services
{
    public abstract class WindowServiceBase : IWindowService
    {
        public object MainWindow => throw new NotImplementedException();

        public void ExitApplication()
        {
            Application.Current.Exit();
        }

        public IWindow GetWindow(IViewModel viewmodel)
        {
            return null;
            //throw new NotImplementedException();
        }

        public void InitializeScreen()
        {
            throw new NotImplementedException();
        }

        public void SetFullScreen()
        {
            throw new NotImplementedException();
        }

        public void SetMinimized()
        {
            throw new NotImplementedException();
        }

        public void SetNormalScreen()
        {
            throw new NotImplementedException();
        }

        public void ToggleScreen()
        {
            throw new NotImplementedException();
        }
    }
}
