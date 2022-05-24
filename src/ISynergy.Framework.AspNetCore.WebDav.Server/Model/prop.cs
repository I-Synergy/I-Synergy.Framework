using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Model
{
    /// <summary>
    /// The WebDAV prop element
    /// </summary>
    public partial class prop
    {
        /// <summary>
        /// Gets or sets the language code
        /// </summary>
        [XmlAttribute("xml:lang", DataType = "language")]
        public string Language { get; set; }
    }
}
