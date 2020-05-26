namespace ISynergy.Framework.Payment.Mollie.Models.Issuer
{
    /// <summary>
    /// URLs of images representing the issuer.
    /// </summary>
    public class IssuerResponseImage
    {
        /// <summary>
        /// Gets or sets the size1x.
        /// </summary>
        /// <value>The size1x.</value>
        public string Size1x { get; set; }
        /// <summary>
        /// Gets or sets the size2x.
        /// </summary>
        /// <value>The size2x.</value>
        public string Size2x { get; set; }
        /// <summary>
        /// Gets or sets the SVG.
        /// </summary>
        /// <value>The SVG.</value>
        public string Svg { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Size1x;
        }
    }
}
