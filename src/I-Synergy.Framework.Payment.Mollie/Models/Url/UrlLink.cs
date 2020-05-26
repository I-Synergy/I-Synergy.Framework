namespace ISynergy.Framework.Payment.Mollie.Models.Url
{
    /// <summary>
    /// Class UrlLink.
    /// </summary>
    public class UrlLink
    {
        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>The href.</value>
        public string Href { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{Type} - {Href}";
        }
    }
}
