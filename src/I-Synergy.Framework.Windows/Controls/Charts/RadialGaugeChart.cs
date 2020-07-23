using System;
using System.Linq;
using SkiaSharp;

namespace ISynergy.Framework.Windows.Controls.Charts
{
    /// <summary>
    /// Class RadialGaugeChart.
    /// Implements the <see cref="ISynergy.Framework.Windows.Controls.Charts.Chart" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Windows.Controls.Charts.Chart" />
    public class RadialGaugeChart : Chart
    {
        /// <summary>
        /// Gets or sets the size of each gauge. If negative, then its will be calculated from the available space.
        /// </summary>
        /// <value>The size of the line.</value>
        public float LineSize { get; set; } = -1;

        /// <summary>
        /// Gets or sets the gauge background area alpha.
        /// </summary>
        /// <value>The line area alpha.</value>
        public byte LineAreaAlpha { get; set; } = 52;

        /// <summary>
        /// Gets or sets the start angle.
        /// </summary>
        /// <value>The start angle.</value>
        public float StartAngle { get; set; } = -90;

        /// <summary>
        /// Gets the absolute minimum.
        /// </summary>
        /// <value>The absolute minimum.</value>
        private float AbsoluteMinimum => Entries?.Select(x => x.Value).Concat(new[] { MaxValue, MinValue, InternalMinValue ?? 0 }).Min(x => Math.Abs(x)) ?? 0;

        /// <summary>
        /// Gets the absolute maximum.
        /// </summary>
        /// <value>The absolute maximum.</value>
        private float AbsoluteMaximum => Entries?.Select(x => x.Value).Concat(new[] { MaxValue, MinValue, InternalMinValue ?? 0 }).Max(x => Math.Abs(x)) ?? 0;

        /// <summary>
        /// Gets the value range.
        /// </summary>
        /// <value>The value range.</value>
        private float ValueRange => AbsoluteMaximum - AbsoluteMinimum;

        /// <summary>
        /// Draws the gauge area.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="entry">The entry.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="cx">The cx.</param>
        /// <param name="cy">The cy.</param>
        /// <param name="strokeWidth">Width of the stroke.</param>
        public void DrawGaugeArea(SKCanvas canvas, ChartEntry entry, float radius, int cx, int cy, float strokeWidth)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                Color = entry.Color.WithAlpha(LineAreaAlpha),
                IsAntialias = true,
            };
            canvas.DrawCircle(cx, cy, radius, paint);
        }

        /// <summary>
        /// Draws the gauge.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="entry">The entry.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="cx">The cx.</param>
        /// <param name="cy">The cy.</param>
        /// <param name="strokeWidth">Width of the stroke.</param>
        public void DrawGauge(SKCanvas canvas, ChartEntry entry, float radius, int cx, int cy, float strokeWidth)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                StrokeCap = SKStrokeCap.Round,
                Color = entry.Color,
                IsAntialias = true,
            };
            using SKPath path = new SKPath();
            var sweepAngle = AnimationProgress * 360 * (Math.Abs(entry.Value) - AbsoluteMinimum) / ValueRange;
            path.AddArc(SKRect.Create(cx - radius, cy - radius, 2 * radius, 2 * radius), StartAngle, sweepAngle);
            canvas.DrawPath(path, paint);
        }

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
                DrawCaption(canvas, width, height);

                var sumValue = Entries.Sum(x => Math.Abs(x.Value));
                var radius = (Math.Min(width, height) - (2 * Margin)) / 2;
                var cx = width / 2;
                var cy = height / 2;
                var lineWidth = (LineSize < 0) ? (radius / ((Entries.Count() + 1) * 2)) : LineSize;
                var radiusSpace = lineWidth * 2;

                for (int i = 0; i < Entries.Count(); i++)
                {
                    var entry = Entries.ElementAt(i);
                    var entryRadius = (i + 1) * radiusSpace;
                    DrawGaugeArea(canvas, entry, entryRadius, cx, cy, lineWidth);
                    DrawGauge(canvas, entry, entryRadius, cx, cy, lineWidth);
                }
            }
        }

        /// <summary>
        /// Draws the caption.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private void DrawCaption(SKCanvas canvas, int width, int height)
        {
            var rightValues = Entries.Take(Entries.Count() / 2).ToList();
            var leftValues = Entries.Skip(rightValues.Count()).ToList();

            leftValues.Reverse();

            DrawCaptionElements(canvas, width, height, rightValues, false, false);
            DrawCaptionElements(canvas, width, height, leftValues, true, false);
        }
    }
}
