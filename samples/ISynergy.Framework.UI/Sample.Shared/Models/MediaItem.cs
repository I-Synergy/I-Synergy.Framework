using ISynergy.Framework.Core.Data;

namespace Sample.Models
{
    /// <summary>
    /// Class MediaItem.
    /// </summary>
    public class MediaItem : ModelBase
    {
        /// <summary>
        /// Gets or sets the ImageUri property value.
        /// </summary>
        /// <value>The image URI.</value>
        public string ImageUri
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
