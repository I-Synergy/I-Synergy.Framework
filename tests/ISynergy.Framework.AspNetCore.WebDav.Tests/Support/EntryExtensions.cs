using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Dead;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Support
{
    public static class EntryExtensions
    {
        public static Task<IReadOnlyCollection<XElement>> GetPropertyElementsAsync(
            this IEntry entry,
            IWebDavDispatcher dispatcher,
            CancellationToken ct)
        {
            return GetPropertyElementsAsync(entry, dispatcher, false, ct);
        }

        public static async Task<IReadOnlyCollection<XElement>> GetPropertyElementsAsync(
            this IEntry entry,
            IWebDavDispatcher dispatcher,
            bool skipEtag,
            CancellationToken ct)
        {
            var result = new List<XElement>();
            using (var propEnum = entry.GetProperties(dispatcher).GetEnumerator())
            {
                while (await propEnum.MoveNext(ct))
                {
                    var prop = propEnum.Current;
                    if (skipEtag && prop.Name == GetETagProperty.PropertyName)
                        continue;
                    var element = await prop.GetXmlValueAsync(ct);
                    result.Add(element);
                }
            }

            return result;
        }
    }
}
