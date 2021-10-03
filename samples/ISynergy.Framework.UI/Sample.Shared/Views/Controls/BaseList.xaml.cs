#if (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Sample.Views.Controls
{
    /// <summary>
    /// Class BaseList. This class cannot be inherited.
    /// </summary>
    public sealed partial class BaseList : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseList"/> class.
        /// </summary>
        public BaseList()
        {
            this.InitializeComponent();
        }
    }
}
