using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.AspNetCore.Extensions;

/// <summary>
/// Class ImageHelperExtensions.
/// </summary>
public static class ImageHelperExtensions
{
    /// <summary>
    /// Inlines the image asynchronous using a dictionary of HTML attributes (AOT-safe).
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="notfoundPath">The notfound path.</param>
    /// <param name="imagePath">The image path.</param>
    /// <param name="attributes">
    /// Optional HTML attributes as a dictionary. Prefer this overload over the <c>object?</c> overload for AOT-safe publishing.
    /// </param>
    /// <returns>HtmlString.</returns>
    public static HtmlString InlineImageAsync(this IHtmlHelper html, string notfoundPath, string imagePath, IDictionary<string, object?>? attributes)
    {
        if (html.ViewContext.HttpContext.RequestServices.GetService<IWebHostEnvironment>() is IWebHostEnvironment env)
        {
            var img = "";

            var attrs = (attributes is null)
                ? string.Empty
                : string.Join(" ", attributes.Select(x => string.Format("{0}=\"{1}\"", x.Key, x.Value)));

            if (env.WebRootFileProvider.GetFileInfo(imagePath).Exists)
            {
                img = $"<img src=\"{imagePath}\" {attrs}/>";
            }
            else if (env.WebRootFileProvider.GetFileInfo(notfoundPath).Exists)
            {
                img = $"<img src=\"{notfoundPath}\" {attrs}/>";
            }

            return new HtmlString(img);
        }

        return new HtmlString(string.Empty);
    }

    /// <summary>
    /// Inlines the image asynchronous using an anonymous object for HTML attributes.
    /// This overload uses runtime reflection and is not AOT-safe.
    /// Prefer the <see cref="InlineImageAsync(IHtmlHelper, string, string, IDictionary{string, object?}?)"/> overload instead.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="notfoundPath">The notfound path.</param>
    /// <param name="imagePath">The image path.</param>
    /// <param name="attributes">The attributes.</param>
    /// <returns>HtmlString.</returns>
    [RequiresUnreferencedCode("Reflects over arbitrary attribute object type. Use the IDictionary<string, object?> overload for AOT-safe publishing.")]
    [RequiresDynamicCode("Reflects over arbitrary attribute object type. Use the IDictionary<string, object?> overload for AOT-safe publishing.")]
    public static HtmlString InlineImageAsync(this IHtmlHelper html, string notfoundPath, string imagePath, object? attributes = null)
    {
        if (attributes is IDictionary<string, object?> dict)
        {
            return InlineImageAsync(html, notfoundPath, imagePath, dict);
        }

        if (html.ViewContext.HttpContext.RequestServices.GetService<IWebHostEnvironment>() is IWebHostEnvironment env)
        {
            var img = "";

            var props = attributes?.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(attributes));

            var attrs = (props is null)
                ? string.Empty
                : string.Join(" ", props.Select(x => string.Format("{0}=\"{1}\"", x.Key, x.Value)));

            if (env.WebRootFileProvider.GetFileInfo(imagePath).Exists)
            {
                img = $"<img src=\"{imagePath}\" {attrs}/>";
            }
            else if (env.WebRootFileProvider.GetFileInfo(notfoundPath).Exists)
            {
                img = $"<img src=\"{notfoundPath}\" {attrs}/>";
            }

            return new HtmlString(img);
        }

        return new HtmlString(string.Empty);
    }
}
