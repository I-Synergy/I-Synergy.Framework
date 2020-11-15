using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class ImageView. This class cannot be inherited.
    /// Implements the <see cref="UserControl" />
    /// </summary>
    /// <seealso cref="UserControl" />
    public sealed partial class ImageView : UserControl
    {
        /// <summary>
        /// The source property
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(ImageView), new PropertyMetadata(default(ImageSource), SourceChanged));

        /// <summary>
        /// The place holder property
        /// </summary>
        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register(nameof(PlaceHolder), typeof(ImageSource), typeof(ImageView), new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the place holder.
        /// </summary>
        /// <value>The place holder.</value>
        public ImageSource PlaceHolder
        {
            get { return (ImageSource)GetValue(PlaceHolderProperty); }
            set { SetValue(PlaceHolderProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageView"/> class.
        /// </summary>
        public ImageView()
        {
            InitializeComponent();
        }

#if NETFX_CORE
        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="source">The source.</param>
        private void LoadImage(ImageSource source)
        {
            void handler(object sender, object e)
            {
                ImageFadeOut.Completed -= handler;
                Image.Source = source;
                ImageFadeIn.Begin();
            }

            ImageFadeOut.Completed += handler;
            ImageFadeOut.Begin();
        }
#endif

        /// <summary>
        /// Sources the changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void SourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (ImageView)dependencyObject;

            if ((ImageSource)dependencyPropertyChangedEventArgs.NewValue is ImageSource newSource)
            {
                var image = (BitmapImage)newSource;

#if NETFX_CORE
                // If the image is not a local resource or it was not cached
                if (image.UriSource.Scheme != "ms-appx" && image.UriSource.Scheme != "ms-resource" && (image.PixelHeight * image.PixelWidth == 0))
                {
                    image.ImageOpened += (sender, args) => control.LoadImage(image);
                    control.Staging.Source = image;
                }
                else
                {
                    control.LoadImage(newSource);
                }
#else
                control.Image.Source = image;
#endif
            }
        }
    }
}
