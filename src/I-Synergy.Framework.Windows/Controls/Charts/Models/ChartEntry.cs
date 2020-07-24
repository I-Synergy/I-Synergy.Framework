using SkiaSharp;

namespace ISynergy.Framework.Windows.Controls.Charts
{
    /// <summary>
    /// Class ChartEntry.
    /// </summary>
    public class ChartEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartEntry"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ChartEntry(float value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public float Value { get; }

        /// <summary>
        /// Gets or sets the caption label.
        /// </summary>
        /// <value>The label.</value>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the label associated to the value.
        /// </summary>
        /// <value>The value label.</value>
        public string ValueLabel { get; set; }

        /// <summary>
        /// Gets or sets the color of the fill.
        /// </summary>
        /// <value>The color of the fill.</value>
        public SKColor Color { get; set; } = SKColors.Black;

        /// <summary>
        /// Gets or sets the color of the text (for the caption label).
        /// </summary>
        /// <value>The color of the text.</value>
        public SKColor TextColor { get; set; } = SKColors.Gray;
    }
}
