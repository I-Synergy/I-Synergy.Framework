using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Props
{
    /// <summary>
    /// A simple writeable untyped property
    /// </summary>
    public abstract class SimpleUntypedProperty : IUntypedWriteableProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleUntypedProperty"/> class.
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="language">The language for the property value</param>
        /// <param name="cost">The cost to get the properties value</param>
        /// <param name="alternativeNames">The alternative names</param>
        protected SimpleUntypedProperty(XName name, string language, int cost, params XName[] alternativeNames)
        {
            Name = name;
            Cost = cost;
            AlternativeNames = alternativeNames;
            Language = language;
        }

        /// <inheritdoc />
        public XName Name { get; }

        /// <inheritdoc />
        public string Language { get; protected set; }

        /// <inheritdoc />
        public IReadOnlyCollection<XName> AlternativeNames { get; }

        /// <inheritdoc />
        public int Cost { get; }

        /// <inheritdoc />
        public abstract Task SetXmlValueAsync(XElement element, CancellationToken ct);

        /// <inheritdoc />
        public abstract Task<XElement> GetXmlValueAsync(CancellationToken ct);
    }
}
