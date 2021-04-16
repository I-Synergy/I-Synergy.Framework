﻿using System;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

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
