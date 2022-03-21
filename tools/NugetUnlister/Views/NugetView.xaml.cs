using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.UI.Controls;

namespace NugetUnlister
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NugetView : View, IView
    {
        public NugetView()
        {
            this.InitializeComponent();
        }
    }
}
