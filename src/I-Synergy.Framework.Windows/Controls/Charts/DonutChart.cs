using System;
using System.Collections.Generic;
using System.Linq;
using ISynergy.Framework.Windows.Controls.Charts.Enumerations;
using ISynergy.Framework.Windows.Controls.Charts.Helpers;
using SkiaSharp;

namespace ISynergy.Framework.Windows.Controls.Charts
{
    /// <summary>
    /// Class DonutChart.
    /// Implements the <see cref="Chart" />
    /// </summary>
    /// <seealso cref="Chart" />
    public class DonutChart : Chart
    {
        /// <summary>
        /// Gets or sets the radius of the hole in the center of the chart.
        /// </summary>
        /// <value>The hole radius.</value>
        public float HoleRadius { get; set; } = 0.5f;

        /// <summary>
        /// Gets or sets a value whether the caption elements should all reside on the right side.
        /// </summary>
        /// <value>The label mode.</value>
        public LabelMode LabelMode { get; set; } = LabelMode.LeftAndRight;

        /// <summary>
        /// Gets or sets whether the graph should be drawn in the center or automatically fill the space.
        /// </summary>
        /// <value>The graph position.</value>
        public GraphPosition GraphPosition { get; set; } = GraphPosition.AutoFill;

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
                using (new SKAutoCanvasRestore(canvas))
                {
                    if (DrawDebugRectangles)
                    {
                        using var paint = new SKPaint
                        {
                            Color = SKColors.Red,
                            IsStroke = true,
                        };
                        canvas.DrawRect(DrawableChartArea, paint);
                    }

                    canvas.Translate(DrawableChartArea.Left + DrawableChartArea.Width / 2, height / 2);

                    var sumValue = Entries.Sum(x => Math.Abs(x.Value));
                    var radius = (Math.Min(DrawableChartArea.Width, DrawableChartArea.Height) - (2 * Margin)) / 2;
                    var start = 0.0f;

                    for (int i = 0; i < Entries.Count(); i++)
                    {
                        var entry = Entries.ElementAt(i);
                        var end = start + ((Math.Abs(entry.Value) / sumValue) * AnimationProgress);

                        // Sector
                        var path = RadialHelpers.CreateSectorPath(start, end, radius, radius * HoleRadius);
                        using (var paint = new SKPaint
                        {
                            Style = SKPaintStyle.Fill,
                            Color = entry.Color,
                            IsAntialias = true,
                        })
                        {
                            canvas.DrawPath(path, paint);
                        }

                        start = end;
                    }
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
            var isGraphCentered = GraphPosition == GraphPosition.Center;
            var sumValue = Entries.Sum(x => Math.Abs(x.Value));

            switch (LabelMode)
            {
                case LabelMode.None:
                    return;

                case LabelMode.RightOnly:
                    DrawCaptionElements(canvas, width, height, Entries.ToList(), false, isGraphCentered);
                    return;

                case LabelMode.LeftAndRight:
                    DrawCaptionLeftAndRight(canvas, width, height, isGraphCentered);
                    return;
            }
        }

        /// <summary>
        /// Draws the caption left and right.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="isGraphCentered">if set to <c>true</c> [is graph centered].</param>
        private void DrawCaptionLeftAndRight(SKCanvas canvas, int width, int height, bool isGraphCentered)
        {
            var sumValue = Entries.Sum(x => Math.Abs(x.Value));
            var rightValues = new List<ChartEntry>();
            var leftValues = new List<ChartEntry>();
            int i = 0;
            var current = 0.0f;

            while (i < Entries.Count() && (current < sumValue / 2))
            {
                var entry = Entries.ElementAt(i);
                rightValues.Add(entry);
                current += Math.Abs(entry.Value);
                i++;
            }

            while (i < Entries.Count())
            {
                var entry = Entries.ElementAt(i);
                leftValues.Add(entry);
                i++;
            }

            leftValues.Reverse();

            DrawCaptionElements(canvas, width, height, rightValues, false, isGraphCentered);
            DrawCaptionElements(canvas, width, height, leftValues, true, isGraphCentered);
        }
    }
}
