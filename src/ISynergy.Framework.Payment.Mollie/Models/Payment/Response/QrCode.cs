﻿namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Response
{
    /// <summary>
    /// Class QrCode.
    /// </summary>
    public class QrCode
    {
        /// <summary>
        /// Height of the image in pixels.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; set; }

        /// <summary>
        /// Width of the image in pixels.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; set; }

        /// <summary>
        /// The URI you can use to display the QR code. Note that we can send both data URIs as well as links to HTTPS images.
        /// You should support both.
        /// </summary>
        /// <value>The source.</value>
        public string Src { get; set; }
    }
}
