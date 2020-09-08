using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.UI.Sample.Models
{
    public class MediaItem : ModelBase
    {
        /// <summary>
        /// Gets or sets the Index property value.
        /// </summary>
        public int Index
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ImageUri property value.
        /// </summary>
        public string ImageUri
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
