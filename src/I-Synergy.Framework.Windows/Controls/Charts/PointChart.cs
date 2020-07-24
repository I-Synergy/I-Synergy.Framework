using System;
using System.Collections.Generic;
using System.Linq;
using ISynergy.Framework.Windows.Controls.Charts.Enumerations;
using ISynergy.Framework.Windows.Controls.Charts.Extensions;
using SkiaSharp;

namespace ISynergy.Framework.Windows.Controls.Charts
{
    /// <summary>
    /// Class PointChart.
    /// Implements the <see cref="Chart" />
    /// </summary>
    /// <seealso cref="Chart" />
    public class PointChart : Chart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointChart" /> class.
        /// </summary>
        public PointChart()
        {
            LabelOrientation = Orientation.Default;
            ValueLabelOrientation = Orientation.Default;
        }

        /// <summary>
        /// The label orientation
        /// </summary>
        private Orientation labelOrientation;
        /// <summary>
        /// The value label orientation
        /// </summary>
        private Orientation valueLabelOrientation;

        /// <summary>
        /// Gets or sets the size of the point.
        /// </summary>
        /// <value>The size of the point.</value>
        public float PointSize { get; set; } = 14;

        /// <summary>
        /// Gets or sets the point mode.
        /// </summary>
        /// <value>The point mode.</value>
        public PointMode PointMode { get; set; } = PointMode.Circle;

        /// <summary>
        /// Gets or sets the point area alpha.
        /// </summary>
        /// <value>The point area alpha.</value>
        public byte PointAreaAlpha { get; set; } = 100;

        /// <summary>
        /// Gets or sets the text orientation of labels.
        /// </summary>
        /// <value>The label orientation.</value>
        public Orientation LabelOrientation
        {
            get => labelOrientation;
            set => labelOrientation = (value == Orientation.Default) ? Orientation.Vertical : value;
        }

        /// <summary>
        /// Gets or sets the text orientation of value labels.
        /// </summary>
        /// <value>The label orientation.</value>
        public Orientation ValueLabelOrientation
        {
            get => valueLabelOrientation;
            set => valueLabelOrientation = (value == Orientation.Default) ? Orientation.Vertical : value;
        }

        /// <summary>
        /// Gets the value range.
        /// </summary>
        /// <value>The value range.</value>
        private float ValueRange => MaxValue - MinValue;

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

                DrawPointAreas(canvas, points, origin);
                DrawPoints(canvas, points);
                DrawHeader(canvas, valueLabels, valueLabelSizes, points, itemSize, height, headerHeight);
                DrawFooter(canvas, labels, labelSizes, points, itemSize, height, footerHeight);
            }
        }

        /// <summary>
        /// Calculates the y origin.
        /// </summary>
        /// <param name="itemHeight">Height of the item.</param>
        /// <param name="headerHeight">Height of the header.</param>
        /// <returns>System.Single.</returns>
        protected float CalculateYOrigin(float itemHeight, float headerHeight)
        {
            if (MaxValue <= 0)
            {
                return headerHeight;
            }

            if (MinValue > 0)
            {
                return headerHeight + itemHeight;
            }

            return headerHeight + ((MaxValue / ValueRange) * itemHeight);
        }

        /// <summary>
        /// Calculates the size of the item.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="footerHeight">Height of the footer.</param>
        /// <param name="headerHeight">Height of the header.</param>
        /// <returns>SKSize.</returns>
        protected SKSize CalculateItemSize(int width, int height, float footerHeight, float headerHeight)
        {
            var total = Entries.Count();
            var w = (width - ((total + 1) * Margin)) / total;
            var h = height - Margin - footerHeight - headerHeight;
            return new SKSize(w, h);
        }

        /// <summary>
        /// Calculates the points.
        /// </summary>
        /// <param name="itemSize">Size of the item.</param>
        /// <param name="origin">The origin.</param>
        /// <param name="headerHeight">Height of the header.</param>
        /// <returns>SKPoint[].</returns>
        protected SKPoint[] CalculatePoints(SKSize itemSize, float origin, float headerHeight)
        {
            var result = new List<SKPoint>();

            for (int i = 0; i < Entries.Count(); i++)
            {
                var entry = Entries.ElementAt(i);
                var value = entry.Value;

                var x = Margin + (itemSize.Width / 2) + (i * (itemSize.Width + Margin));
                var y = headerHeight + ((1 - AnimationProgress) * (origin - headerHeight) + (((MaxValue - value) / ValueRange) * itemSize.Height) * AnimationProgress);
                var point = new SKPoint(x, y);
                result.Add(point);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Draws the header.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="labels">The labels.</param>
        /// <param name="labelSizes">The label sizes.</param>
        /// <param name="points">The points.</param>
        /// <param name="itemSize">Size of the item.</param>
        /// <param name="height">The height.</param>
        /// <param name="headerHeight">Height of the header.</param>
        protected void DrawHeader(SKCanvas canvas, string[] labels, SKRect[] labelSizes, SKPoint[] points, SKSize itemSize, int height, float headerHeight)
        {
            DrawLabels(canvas,
                            labels,
                            points.Select(p => new SKPoint(p.X, headerHeight - Margin)).ToArray(),
                            labelSizes,
                            Entries.Select(x => x.Color.WithAlpha((byte)(255 * AnimationProgress))).ToArray(),
                            ValueLabelOrientation,
                            true,
                            itemSize,
                            height);
        }

        /// <summary>
        /// Draws the footer.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="labels">The labels.</param>
        /// <param name="labelSizes">The label sizes.</param>
        /// <param name="points">The points.</param>
        /// <param name="itemSize">Size of the item.</param>
        /// <param name="height">The height.</param>
        /// <param name="footerHeight">Height of the footer.</param>
        protected void DrawFooter(SKCanvas canvas, string[] labels, SKRect[] labelSizes, SKPoint[] points, SKSize itemSize, int height, float footerHeight)
        {
            DrawLabels(canvas,
                            labels,
                            points.Select(p => new SKPoint(p.X, height - footerHeight + Margin)).ToArray(),
                            labelSizes,
                            Entries.Select(x => LabelColor).ToArray(),
                            LabelOrientation,
                            false,
                            itemSize,
                            height);
        }

        /// <summary>
        /// Draws the points.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="points">The points.</param>
        protected void DrawPoints(SKCanvas canvas, SKPoint[] points)
        {
            if (points.Length > 0 && PointMode != PointMode.None)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var entry = Entries.ElementAt(i);
                    var point = points[i];
                    canvas.DrawPoint(point, entry.Color, PointSize, PointMode);
                }
            }
        }

        /// <summary>
        /// Draws the point areas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="points">The points.</param>
        /// <param name="origin">The origin.</param>
        protected void DrawPointAreas(SKCanvas canvas, SKPoint[] points, float origin)
        {
            if (points.Length > 0 && PointAreaAlpha > 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var entry = Entries.ElementAt(i);
                    var point = points[i];
                    var y = Math.Min(origin, point.Y);

                    using var shader = SKShader.CreateLinearGradient(new SKPoint(0, origin), new SKPoint(0, point.Y), new[] { entry.Color.WithAlpha(PointAreaAlpha), entry.Color.WithAlpha((byte)(PointAreaAlpha / 3)) }, null, SKShaderTileMode.Clamp);
                    using var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color.WithAlpha(PointAreaAlpha),
                    };
                    paint.Shader = shader;
                    var height = Math.Max(2, Math.Abs(origin - point.Y));
                    canvas.DrawRect(SKRect.Create(point.X - (PointSize / 2), y, PointSize, height), paint);
                }
            }
        }

        /// <summary>
        /// Draws the labels.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="texts">The texts.</param>
        /// <param name="points">The points.</param>
        /// <param name="sizes">The sizes.</param>
        /// <param name="colors">The colors.</param>
        /// <param name="orientation">The orientation.</param>
        /// <param name="isTop">if set to <c>true</c> [is top].</param>
        /// <param name="itemSize">Size of the item.</param>
        /// <param name="height">The height.</param>
        protected void DrawLabels(SKCanvas canvas, string[] texts, SKPoint[] points, SKRect[] sizes, SKColor[] colors, Orientation orientation, bool isTop, SKSize itemSize, float height)
        {
            if (points.Length > 0)
            {
                var maxWidth = sizes.Max(x => x.Width);

                for (int i = 0; i < points.Length; i++)
                {
                    var entry = Entries.ElementAt(i);
                    var point = points[i];

                    if (!string.IsNullOrEmpty(entry.ValueLabel))
                    {
                        using (new SKAutoCanvasRestore(canvas))
                        {
                            using (var paint = new SKPaint())
                            {
                                paint.TextSize = LabelTextSize;
                                paint.IsAntialias = true;
                                paint.Color = colors[i];
                                paint.IsStroke = false;
                                paint.Typeface = base.Typeface;
                                var bounds = sizes[i];
                                var text = texts[i];

                                if (orientation == Orientation.Vertical)
                                {
                                    var y = point.Y;

                                    if (isTop)
                                    {
                                        y -= bounds.Width;
                                    }

                                    canvas.RotateDegrees(90);
                                    canvas.Translate(y, -point.X + (bounds.Height / 2));
                                }
                                else
                                {
                                    if (bounds.Width > itemSize.Width)
                                    {
                                        text = text.Substring(0, Math.Min(3, text.Length));
                                        paint.MeasureText(text, ref bounds);
                                    }

                                    if (bounds.Width > itemSize.Width)
                                    {
                                        text = text.Substring(0, Math.Min(1, text.Length));
                                        paint.MeasureText(text, ref bounds);
                                    }

                                    var y = point.Y;

                                    if (isTop)
                                    {
                                        y -= bounds.Height;
                                    }

                                    canvas.Translate(point.X - (bounds.Width / 2), y);
                                }

                                canvas.DrawText(text, 0, 0, paint);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the height of the footer.
        /// </summary>
        /// <param name="valueLabelSizes">Value label sizes.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns>The footer height.</returns>
        protected float CalculateFooterHeaderHeight(SKRect[] valueLabelSizes, Orientation orientation)
        {
            var result = Margin;

            if (Entries.Any(e => !string.IsNullOrEmpty(e.Label)))
            {
                if (orientation == Orientation.Vertical)
                {
                    var maxValueWidth = valueLabelSizes.Max(x => x.Width);
                    if (maxValueWidth > 0)
                    {
                        result += maxValueWidth + Margin;
                    }
                }
                else
                {
                    result += LabelTextSize + Margin;
                }
            }

            return result;
        }

        /// <summary>
        /// Measures the value labels.
        /// </summary>
        /// <param name="labels">The labels.</param>
        /// <returns>The value labels.</returns>
        protected SKRect[] MeasureLabels(string[] labels)
        {
            using (var paint = new SKPaint())
            {
                paint.TextSize = LabelTextSize;
                return labels.Select(text =>
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        return SKRect.Empty;
                    }

                    var bounds = new SKRect();
                    paint.MeasureText(text, ref bounds);
                    return bounds;
                }).ToArray();
            }
        }
    }
}
