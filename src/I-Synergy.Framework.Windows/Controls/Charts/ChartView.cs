using ISynergy.Framework.Windows.Controls.Charts.Events;
using SkiaSharp;
using SkiaSharp.Views.UWP;
using Windows.UI.Xaml;

namespace ISynergy.Framework.Windows.Controls.Charts
{
    /// <summary>
    /// Class ChartView.
    /// Implements the <see cref="SKXamlCanvas" />
    /// </summary>
    /// <seealso cref="SKXamlCanvas" />
    public class ChartView : SKXamlCanvas
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartView"/> class.
        /// </summary>
        public ChartView()
        {
            PaintSurface += OnPaintCanvas;
        }

        /// <summary>
        /// The chart property
        /// </summary>
        public static readonly DependencyProperty ChartProperty = DependencyProperty.Register(nameof(Chart), typeof(Chart), typeof(ChartView), new PropertyMetadata(null, new PropertyChangedCallback(OnChartChanged)));

        /// <summary>
        /// The handler
        /// </summary>
        private InvalidatedWeakEventHandler<ChartView> handler;

        /// <summary>
        /// The chart
        /// </summary>
        private Chart chart;

        /// <summary>
        /// Gets or sets the chart.
        /// </summary>
        /// <value>The chart.</value>
        public Chart Chart
        {
            get { return (Chart)GetValue(ChartProperty); }
            set { SetValue(ChartProperty, value); }
        }

        /// <summary>
        /// Handles the <see cref="E:ChartChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnChartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as ChartView;

            if (view.chart != null)
            {
                view.handler.Dispose();
                view.handler = null;
            }

            view.chart = e.NewValue as Chart;
            view.Invalidate();

            if (view.chart != null)
            {
                view.handler = view.chart.ObserveInvalidate(view, (v) => v.Invalidate());
            }
        }

        /// <summary>
        /// Handles the <see cref="E:PaintCanvas" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SKPaintSurfaceEventArgs"/> instance containing the event data.</param>
        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            if (chart != null)
            {
                chart.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            }
            else
            {
                e.Surface.Canvas.Clear(SKColors.Transparent);
            }
        }
    }
}
