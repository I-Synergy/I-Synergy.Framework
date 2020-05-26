using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Generic;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Props.Live
{
    /// <summary>
    /// The <c>getlastmodified</c> property
    /// </summary>
    public class LastModifiedProperty : GenericDateTimeRfc1123Property, ILiveProperty
    {
        /// <summary>
        /// The XML name of the property
        /// </summary>
        public static readonly XName PropertyName = WebDavXml.Dav + "getlastmodified";

        /// <summary>
        /// Initializes a new instance of the <see cref="LastModifiedProperty"/> class.
        /// </summary>
        /// <param name="propValue">The initial property value</param>
        /// <param name="setValueAsyncFunc">The delegate to set the value asynchronously</param>
        public LastModifiedProperty(DateTime propValue, SetPropertyValueAsyncDelegate<DateTime> setValueAsyncFunc)
            : base(PropertyName, 0, ct => Task.FromResult(propValue), setValueAsyncFunc, WebDavXml.Dav + "lastmodified")
        {
        }

        /// <inheritdoc />
        public async Task<bool> IsValidAsync(CancellationToken cancellationToken)
        {
            return Converter.IsValidValue(await GetValueAsync(cancellationToken));
        }
    }
}
