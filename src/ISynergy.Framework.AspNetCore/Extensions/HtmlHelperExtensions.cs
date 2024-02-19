using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ISynergy.Framework.AspNetCore.Extensions;

/// <summary>
/// Class HtmlHelperExtensions.
/// </summary>
public static class HtmlHelperExtensions
{
    /// <summary>
    /// Gets the type of the file content.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="ArgumentException">Unknown file type</exception>
    private static string GetFileContentType(string path)
    {
        if (path.EndsWith(".JPG", StringComparison.OrdinalIgnoreCase))
        {
            return "image/jpeg";
        }

        if (path.EndsWith(".GIF", StringComparison.OrdinalIgnoreCase))
        {
            return "image/gif";
        }
        if (path.EndsWith(".PNG", StringComparison.OrdinalIgnoreCase))
        {
            return "image/png";
        }
        if (path.EndsWith(".WEBP", StringComparison.OrdinalIgnoreCase))
        {
            return "image/webp";
        }

        throw new ArgumentException("Unknown file type");
    }

    /// <summary>
    /// Inlines the image asynchronous.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="path">The path.</param>
    /// <param name="image">The image.</param>
    /// <param name="extension">The extension.</param>
    /// <param name="attributes">The attributes.</param>
    /// <returns>HtmlString.</returns>
    public static HtmlString InlineImageAsync(this IHtmlHelper html, string path, byte[] image, string extension, object attributes = null)
    {
        if (image is not null && !string.IsNullOrWhiteSpace(extension))
        {
            return ConvertArrayToHtmlString(image, GetFileContentType(extension), attributes);
        }

        var contentType = GetFileContentType(path);

        if (html.ViewContext.HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment)) is IWebHostEnvironment env)
        {
            using var stream = env.WebRootFileProvider.GetFileInfo(path).CreateReadStream();
            var array = new byte[stream.Length];
            stream.Read(array, 0, array.Length);

            return ConvertArrayToHtmlString(array, contentType, attributes);
        }

        return new HtmlString(string.Empty);
    }

    /// <summary>
    /// Converts the array to HTML string.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="attributes">The attributes.</param>
    /// <returns>HtmlString.</returns>
    private static HtmlString ConvertArrayToHtmlString(byte[] array, string contentType, object attributes = null)
    {
        var base64 = Convert.ToBase64String(array);

        var props = attributes?.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(attributes));

        var attrs = (props is null)
            ? string.Empty
            : string.Join(" ", props.Select(x => string.Format("{0}=\"{1}\"", x.Key, x.Value)));

        var img = $"<img src=\"data:{contentType};base64,{base64}\" {attrs}/>";

        return new HtmlString(img);
    }
}
