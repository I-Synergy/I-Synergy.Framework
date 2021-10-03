using ISynergy.Framework.Mvvm.Abstractions;

namespace Sample.Views
{
    /// <summary>
    /// Class SelectionTestView. This class cannot be inherited.
    /// </summary>
    public sealed partial class SelectionTestView : ISynergy.Framework.UI.Controls.View, IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionTestView"/> class.
        /// </summary>
        public SelectionTestView()
        {
            this.InitializeComponent();
        }
    }
}
