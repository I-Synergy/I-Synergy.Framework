using System.Web;

namespace ISynergy.Framework.UI.Extensions;

public static class DateTimeOffsetExtensions
{
    public static string ToStringISO8601(this DateTimeOffset value) =>
        value.ToString("o");

    public static string ToStringUrlEncoded(this DateTimeOffset value) =>
        HttpUtility.UrlEncode(value.ToStringISO8601());
}
