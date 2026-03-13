using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

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
    /// <param name="attributes">
    /// Optional HTML attributes as a dictionary. Prefer this overload over the <c>object?</c> overload for AOT-safe publishing.
    /// </param>
    /// <returns>HtmlString.</returns>
    public static HtmlString InlineImageAsync(this IHtmlHelper html, string path, byte[] image, string extension, IDictionary<string, object?>? attributes)
    {
        if (image is not null && !string.IsNullOrWhiteSpace(extension))
        {
            return ConvertArrayToHtmlString(image, GetFileContentType(extension), attributes);
        }

        var contentType = GetFileContentType(path);

        if (html.ViewContext.HttpContext.RequestServices.GetService<IWebHostEnvironment>() is IWebHostEnvironment env)
        {
            using var stream = env.WebRootFileProvider.GetFileInfo(path).CreateReadStream();
            var buffer = new byte[stream.Length];

            stream.ReadExactly(buffer);
            return ConvertArrayToHtmlString(buffer, contentType, attributes);
        }

        return new HtmlString(string.Empty);
    }

    /// <summary>
    /// Inlines the image asynchronous.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="path">The path.</param>
    /// <param name="image">The image.</param>
    /// <param name="extension">The extension.</param>
    /// <param name="attributes">
    /// Optional HTML attributes as an anonymous object. This overload uses runtime reflection and is not AOT-safe.
    /// Prefer the <see cref="InlineImageAsync(IHtmlHelper, string, byte[], string, IDictionary{string, object?}?)"/> overload instead.
    /// </param>
    /// <returns>HtmlString.</returns>
    [RequiresUnreferencedCode("Reflects over arbitrary attribute object type. Use the IDictionary<string, object?> overload for AOT-safe publishing.")]
    [RequiresDynamicCode("Reflects over arbitrary attribute object type. Use the IDictionary<string, object?> overload for AOT-safe publishing.")]
    public static HtmlString InlineImageAsync(this IHtmlHelper html, string path, byte[] image, string extension, object? attributes = null)
    {
        if (attributes is IDictionary<string, object?> dict)
        {
            return InlineImageAsync(html, path, image, extension, dict);
        }

        if (image is not null && !string.IsNullOrWhiteSpace(extension))
        {
            return ConvertArrayToHtmlStringWithReflection(image, GetFileContentType(extension), attributes);
        }

        var contentType = GetFileContentType(path);

        if (html.ViewContext.HttpContext.RequestServices.GetService<IWebHostEnvironment>() is IWebHostEnvironment env)
        {
            using var stream = env.WebRootFileProvider.GetFileInfo(path).CreateReadStream();
            var buffer = new byte[stream.Length];

            stream.ReadExactly(buffer);
            return ConvertArrayToHtmlStringWithReflection(buffer, contentType, attributes);
        }

        return new HtmlString(string.Empty);
    }

    /// <summary>
    /// Converts the array to HTML string using a dictionary of attributes (AOT-safe).
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="attributes">The attributes dictionary.</param>
    /// <returns>HtmlString.</returns>
    private static HtmlString ConvertArrayToHtmlString(byte[] array, string contentType, IDictionary<string, object?>? attributes)
    {
        var base64 = Convert.ToBase64String(array);

        var attrs = (attributes is null)
            ? string.Empty
            : string.Join(" ", attributes.Select(x => string.Format("{0}=\"{1}\"", x.Key, x.Value)));

        var img = $"<img src=\"data:{contentType};base64,{base64}\" {attrs}/>";

        return new HtmlString(img);
    }

    /// <summary>
    /// Converts the array to HTML string using runtime reflection over an arbitrary object (not AOT-safe).
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="attributes">The attributes object.</param>
    /// <returns>HtmlString.</returns>
    [RequiresUnreferencedCode("Reflects over arbitrary attribute object type.")]
    [RequiresDynamicCode("Reflects over arbitrary attribute object type.")]
    private static HtmlString ConvertArrayToHtmlStringWithReflection(byte[] array, string contentType, object? attributes)
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
