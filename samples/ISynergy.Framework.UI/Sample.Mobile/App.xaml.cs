using ISynergy.Framework.UI;

namespace Sample
{
    public partial class App : BaseApplication
    {
        public App()
            : base()
        {
            InitializeComponent();
        }

        public override IList<ResourceDictionary> GetAdditionalResourceDictionaries() =>
            new List<ResourceDictionary>()
            {
                //new Styles.Colors()
                //new Styles.Style()
            };
    }
}