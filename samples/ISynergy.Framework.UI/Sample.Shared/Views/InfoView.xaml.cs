using ISynergy.Framework.Mvvm.Abstractions;

namespace Sample.Shared.Views
{
    /// <summary>
    /// Class InfoView. This class cannot be inherited.
    /// </summary>
    public sealed partial class InfoView : ISynergy.Framework.UI.Controls.View, IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoView"/> class.
        /// </summary>
        public InfoView()
        {
            this.InitializeComponent();
        }
    }
}
