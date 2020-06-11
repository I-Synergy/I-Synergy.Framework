using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Windows.Samples.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Windows.Samples.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : IView
    {
        public MainPage()
        {
            InitializeComponent();

            var contextMock = new Mock<IContext>();
            var commonMock = new Mock<IBaseCommonServices>();
            var loggerMock = new Mock<ILoggerFactory>();

            DataContext = new MainViewModel(contextMock.Object, commonMock.Object, loggerMock.Object);
        }
    }
}
