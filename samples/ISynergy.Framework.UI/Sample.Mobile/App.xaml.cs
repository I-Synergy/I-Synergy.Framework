using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Abstractions.Views;

namespace Sample
{
    public partial class App : BaseApplication
    {
        public App()
            : base()
        {
            InitializeComponent();
        }

        public override async Task InitializeApplicationAsync()
        {
            await base.InitializeApplicationAsync();
            await Task.Delay(5000);

            if (ServiceLocator.Default.GetInstance<IShellView>() is Shell shell)
                MainPage = shell;
        }

        public override IList<ResourceDictionary> GetAdditionalResourceDictionaries() =>
            new List<ResourceDictionary>()
            {
                new Styles.Colors()
            };
    }
}