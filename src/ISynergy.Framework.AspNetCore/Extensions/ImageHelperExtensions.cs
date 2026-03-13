using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;

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
            string? srcPath = null;

            if (env.WebRootFileProvider.GetFileInfo(imagePath).Exists)
                srcPath = imagePath;
            else if (env.WebRootFileProvider.GetFileInfo(notfoundPath).Exists)
                srcPath = notfoundPath;

            if (srcPath is not null)
                return BuildImageTag(srcPath, attributes);
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
            return InlineImageAsync(html, notfoundPath, imagePath, dict);

        var props = attributes?.GetType().GetProperties()
            .ToDictionary(x => x.Name, x => x.GetValue(attributes));

        return InlineImageAsync(html, notfoundPath, imagePath, props);
    }

    /// <summary>
    /// Builds an HTML <c>&lt;img&gt;</c> tag using <see cref="TagBuilder"/> so that all attribute values
    /// and the <c>src</c> URL are HTML-encoded via <see cref="HtmlEncoder.Default"/>, preventing XSS.
    /// </summary>
    /// <param name="srcPath">The image source path.</param>
    /// <param name="attributes">Optional extra HTML attributes.</param>
    /// <returns>A safely encoded <see cref="HtmlString"/>.</returns>
    private static HtmlString BuildImageTag(string srcPath, IDictionary<string, object?>? attributes)
    {
        var tagBuilder = new TagBuilder("img");
        tagBuilder.Attributes["src"] = srcPath;

        if (attributes is not null)
        {
            foreach (var attr in attributes)
                tagBuilder.Attributes[attr.Key] = attr.Value?.ToString() ?? string.Empty;
        }

        tagBuilder.TagRenderMode = TagRenderMode.SelfClosing;

        using var writer = new StringWriter();
        tagBuilder.WriteTo(writer, HtmlEncoder.Default);
        return new HtmlString(writer.ToString());
    }
}
