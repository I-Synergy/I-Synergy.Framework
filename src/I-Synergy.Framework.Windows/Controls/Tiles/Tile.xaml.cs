using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ISynergy.Framework.Windows.Controls
{
    public sealed partial class Tile : Button
    {
        public enum Modes
        {
            Default,
            Wide,
            Small
        }

        public Modes Mode
        {
            get { return (Modes)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(Modes), typeof(Tile), new PropertyMetadata(Modes.Default));

        public string Count
        {
            get { return (string)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Count.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(nameof(Count), typeof(string), typeof(Tile), new PropertyMetadata(string.Empty));

        public double CountFontSize
        {
            get { return (double)GetValue(CountFontSizeProperty); }
            set { SetValue(CountFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CountFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountFontSizeProperty = DependencyProperty.Register(nameof(CountFontSize), typeof(double), typeof(Tile), new PropertyMetadata(18));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(Tile), new PropertyMetadata(string.Empty));

        public int TitleFontSize
        {
            get { return (int)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register(nameof(TitleFontSize), typeof(int), typeof(Tile), new PropertyMetadata(10));

        public VerticalAlignment TitleVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(TitleVerticalAlignmentProperty); }
            set { SetValue(TitleVerticalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderVerticalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleVerticalAlignmentProperty = DependencyProperty.Register(nameof(TitleVerticalAlignment), typeof(VerticalAlignment), typeof(Tile), new PropertyMetadata(VerticalAlignment.Top));

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Details.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object), typeof(Tile), new PropertyMetadata(string.Empty));

        public int HeaderFontSize
        {
            get { return (int)GetValue(HeaderFontSizeProperty); }
            set { SetValue(HeaderFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DetailsFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.Register(nameof(HeaderFontSize), typeof(int), typeof(Tile), new PropertyMetadata(14));

        public Tile()
        {
            InitializeComponent();
            ResizeTile();
        }

        private void ResizeTile()
        {
            switch (Mode)
            {
                case Modes.Wide:
                    break;

                case Modes.Small:
                    break;
            }
        }
    }
}