namespace ISynergy.Framework.Mvvm.Models
{
    /// <summary>
    /// Theme colors.
    /// </summary>
    public class ThemeColors
    {
        private readonly List<string> _colors;

        /// <summary>
        /// Get list of all available colors.
        /// </summary>
        public List<string> Colors { get => _colors; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ThemeColors()
        {
            _colors = new List<string>();
            _colors.Add("#ffb900");
            _colors.Add("#ff8c00");
            _colors.Add("#f7630c");
            _colors.Add("#ca5010");
            _colors.Add("#da3b01");
            _colors.Add("#ef6950");
            _colors.Add("#d13438");
            _colors.Add("#ff4343");
            _colors.Add("#e74856");
            _colors.Add("#e81123");
            _colors.Add("#ea005e");
            _colors.Add("#c30052");
            _colors.Add("#e3008c");
            _colors.Add("#bf0077");
            _colors.Add("#c239b3");
            _colors.Add("#9a0089");
            _colors.Add("#0078d7");
            _colors.Add("#0063b1");
            _colors.Add("#8e8cd8");
            _colors.Add("#6b69d6");
            _colors.Add("#8764b8");
            _colors.Add("#744da9");
            _colors.Add("#b146c2");
            _colors.Add("#881798");
            _colors.Add("#0099bc");
            _colors.Add("#2d7d9a");
            _colors.Add("#00b7c3");
            _colors.Add("#038387");
            _colors.Add("#00b294");
            _colors.Add("#018574");
            _colors.Add("#00cc6a");
            _colors.Add("#10893e");
            _colors.Add("#7a7574");
            _colors.Add("#5d5a58");
            _colors.Add("#68768a");
            _colors.Add("#515c6b");
            _colors.Add("#567c73");
            _colors.Add("#486860");
            _colors.Add("#498205");
            _colors.Add("#107c10");
            _colors.Add("#767676");
            _colors.Add("#4c4a48");
            _colors.Add("#69797e");
            _colors.Add("#4a5459");
            _colors.Add("#647c64");
            _colors.Add("#525e54");
            _colors.Add("#847545");
            _colors.Add("#7e735f");
        }

        /// <summary>
        /// Gets or sets the Default property value.
        /// </summary>
        public static string Default
        {
            get => "#0078d7";
        }
    }
}
