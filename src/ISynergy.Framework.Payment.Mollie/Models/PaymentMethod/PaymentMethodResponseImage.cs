﻿namespace ISynergy.Framework.Payment.Mollie.Models.PaymentMethod
{
    /// <summary>
    /// URLs of images representing the payment method.
    /// </summary>
    public class PaymentMethodResponseImage
    {
        /// <summary>
        /// The URL for a payment method icon of 55x37 pixels.
        /// </summary>
        /// <value>The size1x.</value>
        public string Size1x { get; set; }

        /// <summary>
        /// The URL for a payment method icon of 110x74 pixels. Use this for high resolution screens.
        /// </summary>
        /// <value>The size2x.</value>
        public string Size2x { get; set; }

        /// <summary>
        /// The URL for a payment method icon in vector format. Usage of this format is preferred since it can scale to any desired size.
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
