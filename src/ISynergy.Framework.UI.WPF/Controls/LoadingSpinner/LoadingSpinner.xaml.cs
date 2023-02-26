using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Interaction logic for LoadingSpinner.xaml
    /// </summary>
    public partial class LoadingSpinner : UserControl
    {
        private Storyboard _storyboard;

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(LoadingSpinner), new PropertyMetadata(string.Empty));

        public LoadingSpinner()
        {
            InitializeComponent();
            StartStoryBoard();
            IsVisibleChanged += LoadingSpinner_IsVisibleChanged;
        }

        private void LoadingSpinner_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_storyboard == null)
                return;

            if (e.NewValue is bool visible && visible)
            {
                _storyboard.Begin();
                _storyboard.Resume();
            }
            else
            {
                _storyboard.Stop();
            }
        }

        private void StartStoryBoard()
        {
            try
            {
                _storyboard = (Storyboard)TryFindResource("spinner");
                _storyboard?.Begin();
            }
            catch
            { }
        }
    }
}
