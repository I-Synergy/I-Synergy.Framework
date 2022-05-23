using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.Core.Models
{
    /// <summary>
    /// Application style model.
    /// </summary>
    public class Style : ObservableClass
    {

        /// <summary>
        /// Gets or sets the Color property value.
        /// </summary>
        public string Color
        {
            get => GetValue<string>();
            set => SetValue(value);
        }


        /// <summary>
        /// Gets or sets the Theme property value.
        /// </summary>
        public Themes Theme
        {
            get => GetValue<Themes>();
            set => SetValue(value);
        }

    }
}
