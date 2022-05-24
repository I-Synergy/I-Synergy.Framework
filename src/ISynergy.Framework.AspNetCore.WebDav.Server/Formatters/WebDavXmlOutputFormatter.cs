using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Formatters
{
    /// <summary>
    /// The default implementation of the <see cref="IWebDavOutputFormatter"/> interface
    /// </summary>
    public class WebDavXmlOutputFormatter : IWebDavOutputFormatter
    {
        private static readonly Encoding _defaultEncoding = new UTF8Encoding(false);

        private readonly ILogger<WebDavXmlOutputFormatter> _logger;

        private readonly string _namespacePrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavXmlOutputFormatter"/> class.
        /// </summary>
        /// <param name="options">The formatter options</param>
        /// <param name="logger">The logger</param>
        public WebDavXmlOutputFormatter(IOptions<WebDavFormatterOptions> options, ILogger<WebDavXmlOutputFormatter> logger)
        {
            _logger = logger;
            Encoding = _defaultEncoding;

            var contentType = options.Value.ContentType ?? "text/xml";
            ContentType = $"{contentType}; charset=\"{Encoding.WebName}\"";

            _namespacePrefix = options.Value.NamespacePrefix;
        }

        /// <inheritdoc />
        public string ContentType { get; }

        /// <inheritdoc />
        public Encoding Encoding { get; }

        /// <inheritdoc />
        public void Serialize<T>(Stream output, T data)
        {
            var writerSettings = new XmlWriterSettings { Encoding = Encoding };

            var ns = new XmlSerializerNamespaces();
            if (!string.IsNullOrEmpty(_namespacePrefix))
            {
                ns.Add(_namespacePrefix, WebDavXml.Dav.NamespaceName);

                var xelem = data as XElement;
                if (xelem != null && xelem.GetPrefixOfNamespace(WebDavXml.Dav) != _namespacePrefix)
                {
                    xelem.SetAttributeValue(XNamespace.Xmlns + _namespacePrefix, WebDavXml.Dav.NamespaceName);
                }
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var debugOutput = new StringWriter();
                SerializerInstance<T>.Serializer.Serialize(debugOutput, data, ns);
                _logger.LogDebug(debugOutput.ToString());
            }

            using (var writer = XmlWriter.Create(output, writerSettings))
            {
                SerializerInstance<T>.Serializer.Serialize(writer, data, ns);
            }
        }

        private static class SerializerInstance<T>
        {
            public static readonly XmlSerializer Serializer = new XmlSerializer(typeof(T));
        }
    }
}
