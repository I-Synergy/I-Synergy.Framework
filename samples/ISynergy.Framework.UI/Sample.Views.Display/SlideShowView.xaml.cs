using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.UI.Controls;

namespace Sample.Views
{
    /// <summary>
    /// Class SlideShowView. This class cannot be inherited.
    /// </summary>
    public sealed partial class SlideShowView : View, IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlideShowView"/> class.
        /// </summary>
        public SlideShowView()
        {
            this.InitializeComponent();
        }
    }
}
