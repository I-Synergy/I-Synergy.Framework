using CommonServiceLocator;
using ISynergy.Services;
using ISynergy.ViewModels.Authentication;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ISynergy.Views.Authentication.Controls
{
    public sealed partial class SignUpControl : UserControl
    {
        public SignUpControl()
        {
            this.InitializeComponent();
        }
    }
}
