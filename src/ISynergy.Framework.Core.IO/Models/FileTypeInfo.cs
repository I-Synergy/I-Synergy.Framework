using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Core.IO.Models
{
    /// <summary>
    /// Information regarding the file type, including name, extension, mime type and signature.
    /// </summary>
    public class FileTypeInfo
    {
        /// <summary>
        /// Gets the name of the file type.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the extension which is related to this type
        /// </summary>
        /// <value>The type of the file.</value>
        public string Extension { get; set; }

        /// <summary>
        /// Gets the MimeType of this type
        /// </summary>
        /// <value>The type of the MIME.</value>
        public string MimeType { get; set; }

        /// <summary>
        /// Other names for this file type
        /// </summary>
        /// <value>The alias.</value>
        public string Alias { get; set; }

        /// <summary>
        /// Gets the offset location of the Header details
        /// </summary>
        /// <value>The offset.</value>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets the additional.
        /// </summary>
        /// <value>The additional.</value>
        public string Additional { get; set; }

        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        /// <value>The signature.</value>
        public string Signature { get; set; }

        /// <summary>
        /// Gets the additional identifier to guarantee uniqueness of the file type
        /// </summary>
        /// <value>The additional identifier.</value>
        public byte[] SubHeader
        {
            get => string.IsNullOrWhiteSpace(Additional) ? null : HexStringToByteArray(Additional);
        }

        /// <summary>
        /// Gets unique header 'Magic Numbers' to identifiy this file type
        /// </summary>
        /// <value>The header.</value>
        public byte[] Header
        {
            get => HexStringToByteArray(Signature);
        }

        /// <summary>
        /// Gets the aliases.
        /// </summary>
        /// <value>The aliases.</value>
        public string[] Aliases
        {
            get => string.IsNullOrWhiteSpace(Alias) ? null : Alias.Split('|');
        }

        /// <summary>
        /// Hexadecimals the string to byte array.
        /// </summary>
        /// <param name="hexString">The hexadecimal string.</param>
        /// <returns>System.Byte[].</returns>
        private static byte[] HexStringToByteArray(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
                return Array.Empty<byte>();

            int numberOfCharacters = hexString.Length;
            byte[] byteArray = new byte[numberOfCharacters / 2];
            for (int i = 0; i < numberOfCharacters; i += 2)
                byteArray[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            return byteArray;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $@"{Name} ({Extension})
{MimeType}
{string.Join("|", Aliases ?? Array.Empty<string>())}";
        }
    }
}
