using System.Linq;
using ISynergy.Framework.Windows.Controls.Charts.Enumerations;
using SkiaSharp;

namespace ISynergy.Framework.Windows.Controls.Charts
{
    /// <summary>
    /// Class LineChart.
    /// Implements the <see cref="PointChart" />
    /// </summary>
    /// <seealso cref="PointChart" />
    public class LineChart : PointChart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineChart" /> class.
        /// </summary>
        public LineChart()
        {
            PointSize = 10;
        }

        /// <summary>
        /// Gets or sets the size of the line.
        /// </summary>
        /// <value>The size of the line.</value>
        public float LineSize { get; set; } = 3;

        /// <summary>
        /// Gets or sets the line mode.
        /// </summary>
        /// <value>The line mode.</value>
        public LineMode LineMode { get; set; } = LineMode.Spline;

        /// <summary>
        /// Gets or sets the alpha of the line area.
        /// </summary>
        /// <value>The line area alpha.</value>
        public byte LineAreaAlpha { get; set; } = 32;

        /// <summary>
        /// Enables or disables a fade out gradient for the line area in the Y direction
        /// </summary>
        /// <value>The state of the fadeout gradient.</value>
        public bool EnableYFadeOutGradient { get; set; } = false;

        /// <summary>
        /// Draws the chart content.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public override void DrawContent(SKCanvas canvas, int width, int height)
        {
            if (Entries != null)
            {
                var labels = Entries.Select(x => x.Label).ToArray();
                var labelSizes = MeasureLabels(labels);
                var footerHeight = CalculateFooterHeaderHeight(labelSizes, LabelOrientation);

                var valueLabels = Entries.Select(x => x.ValueLabel).ToArray();
                var valueLabelSizes = MeasureLabels(valueLabels);
                var headerHeight = CalculateFooterHeaderHeight(valueLabelSizes, ValueLabelOrientation);

                var itemSize = CalculateItemSize(width, height, footerHeight, headerHeight);
                var origin = CalculateYOrigin(itemSize.Height, headerHeight);
                var points = CalculatePoints(itemSize, origin, headerHeight);

                DrawArea(canvas, points, itemSize, origin);
                DrawLine(canvas, points, itemSize);
                DrawPoints(canvas, points);
                DrawHeader(canvas, valueLabels, valueLabelSizes, points, itemSize, height, headerHeight);
                DrawFooter(canvas, labels, labelSizes, points, itemSize, height, footerHeight);
            }
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="points">The points.</param>
        /// <param name="itemSize">Size of the item.</param>
        protected void DrawLine(SKCanvas canvas, SKPoint[] points, SKSize itemSize)
        {
            if (points.Length > 1 && LineMode != LineMode.None)
            {
                using var paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.White,
                    StrokeWidth = LineSize,
                    IsAntialias = true,
                };
                using var shader = CreateXGradient(points);
                paint.Shader = shader;

                var path = new SKPath();

                path.MoveTo(points.First());

                var last = (LineMode == LineMode.Spline) ? points.Length - 1 : points.Length;
                for (int i = 0; i < last; i++)
                {
                    if (LineMode == LineMode.Spline)
                    {
                        var entry = Entries.ElementAt(i);
                        var nextEntry = Entries.ElementAt(i + 1);
                        var (point, control, nextPoint, nextControl) = CalculateCubicInfo(points, i, itemSize);
                        path.CubicTo(control, nextControl, nextPoint);
                    }
                    else if (LineMode == LineMode.Straight)
                    {
                        path.LineTo(points[i]);
                    }
                }

                canvas.DrawPath(path, paint);
            }
        }

        /// <summary>
        /// Draws the area.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="points">The points.</param>
        /// <param name="itemSize">Size of the item.</param>
        /// <param name="origin">The origin.</param>
        protected void DrawArea(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float origin)
        {
            if (LineAreaAlpha > 0 && points.Length > 1)
            {
                using var paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.White,
                    IsAntialias = true,
                };
                using var shaderX = CreateXGradient(points, (byte)(LineAreaAlpha * AnimationProgress));
                using var shaderY = CreateYGradient(points, (byte)(LineAreaAlpha * AnimationProgress));
                paint.Shader = EnableYFadeOutGradient ? SKShader.CreateCompose(shaderY, shaderX, SKBlendMode.SrcOut) : shaderX;

                var path = new SKPath();

                path.MoveTo(points.First().X, origin);
                path.LineTo(points.First());

                var last = (LineMode == LineMode.Spline) ? points.Length - 1 : points.Length;
                for (int i = 0; i < last; i++)
                {
                    if (LineMode == LineMode.Spline)
                    {
                        var entry = Entries.ElementAt(i);
                        var nextEntry = Entries.ElementAt(i + 1);
                        var (point, control, nextPoint, nextControl) = CalculateCubicInfo(points, i, itemSize);
                        path.CubicTo(control, nextControl, nextPoint);
                    }
                    else if (LineMode == LineMode.Straight)
                    {
                        path.LineTo(points[i]);
                    }
                }

                path.LineTo(points.Last().X, origin);

                path.Close();

                canvas.DrawPath(path, paint);
            }
        }

        /// <summary>
        /// Calculates the cubic information.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="i">The i.</param>
        /// <param name="itemSize">Size of the item.</param>
        /// <returns>System.ValueTuple&lt;SKPoint, SKPoint, SKPoint, SKPoint&gt;.</returns>
        private (SKPoint point, SKPoint control, SKPoint nextPoint, SKPoint nextControl) CalculateCubicInfo(SKPoint[] points, int i, SKSize itemSize)
        {
            var point = points[i];
            var nextPoint = points[i + 1];
            var controlOffset = new SKPoint(itemSize.Width * 0.8f, 0);
            var currentControl = point + controlOffset;
            var nextControl = nextPoint - controlOffset;
            return (point, currentControl, nextPoint, nextControl);
        }

        /// <summary>
        /// Creates the x gradient.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="alpha">The alpha.</param>
        /// <returns>SKShader.</returns>
        private SKShader CreateXGradient(SKPoint[] points, byte alpha = 255)
        {
            var startX = points.First().X;
            var endX = points.Last().X;
            var rangeX = endX - startX;

            return SKShader.CreateLinearGradient(
                new SKPoint(startX, 0),
                new SKPoint(endX, 0),
                Entries.Select(x => x.Color.WithAlpha(alpha)).ToArray(),
                null,
                SKShaderTileMode.Clamp);
        }

        /// <summary>
        /// Creates the y gradient.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="alpha">The alpha.</param>
        /// <returns>SKShader.</returns>
        private SKShader CreateYGradient(SKPoint[] points, byte alpha = 255)
        {
            var startY = points.Max(i => i.Y);
            var endY = 0;

            return SKShader.CreateLinearGradient(
                new SKPoint(0, startY),
                new SKPoint(0, endY),
                new SKColor[] { SKColors.White.WithAlpha(alpha), SKColors.White.WithAlpha(0) },
                null,
                SKShaderTileMode.Clamp);
        }
    }
}
