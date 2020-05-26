using System.Collections.Generic;
using System.Linq;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Props.Filters
{
    /// <summary>
    /// Implementation of a property filter that allows only readable properties
    /// </summary>
    public class ReadableFilter : IPropertyFilter
    {
        /// <inheritdoc />
        public void Reset()
        {
        }

        /// <inheritdoc />
        public bool IsAllowed(IProperty property)
        {
            return property is IUntypedReadableProperty;
        }

        /// <inheritdoc />
        public void NotifyOfSelection(IProperty property)
        {
        }

        /// <inheritdoc />
        public IEnumerable<MissingProperty> GetMissingProperties()
        {
            return Enumerable.Empty<MissingProperty>();
        }
    }
}
