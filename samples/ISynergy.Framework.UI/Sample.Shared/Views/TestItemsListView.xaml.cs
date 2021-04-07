using ISynergy.Framework.Mvvm.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Sample.Views
{
    /// <summary>
    /// Class TestItemsListView. This class cannot be inherited.
    /// </summary>
    public sealed partial class TestItemsListView : IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestItemsListView"/> class.
        /// </summary>
        public TestItemsListView()
        {
            this.InitializeComponent();
        }
    }
}
