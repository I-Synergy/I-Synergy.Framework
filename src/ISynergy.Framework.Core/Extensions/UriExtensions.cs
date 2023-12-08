using System.Text;
using System.Web;

namespace ISynergy.Framework.Core.Extensions;

public static class UriExtensions
{
    public static Uri AddQueryParameter(this Uri uri, string name, string value)
    {
        var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

        if (httpValueCollection.AllKeys.Contains(name))
            httpValueCollection.Remove(name);

        httpValueCollection.Add(name, value);

        var uriBuilder = new UriBuilder(uri);

        if (httpValueCollection.Count == 0)
        {
            uriBuilder.Query = string.Empty;
        }
        else
        {
            var sb = new StringBuilder();

            for (int i = 0; i < httpValueCollection.Count; i++)
            {
                var key = httpValueCollection.GetKey(i);
                key = HttpUtility.UrlEncode(key);

                var val = (key != null) ? (key + "=") : string.Empty;
                var values = httpValueCollection.GetValues(i);

                if (sb.Length > 0)
                    sb.Append('&');

                if (values == null || values.Length == 0)
                {
                    sb.Append(val);
                }
                else
                {
                    sb.Append(val);
                    sb.Append(HttpUtility.UrlEncode(string.Join("&", values)));
                }
            }

            uriBuilder.Query = sb.ToString();
        }

        return uriBuilder.Uri;
    }
}
