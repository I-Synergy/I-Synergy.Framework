using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace ISynergy.Framework.AspNetCore.Extensions
{
    /// <summary>
    /// Class ImageHelperExtensions.
    /// </summary>
    public static class ImageHelperExtensions
    {
        /// <summary>
        /// Inlines the image asynchronous.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="notfoundPath">The notfound path.</param>
        /// <param name="imagePath">The image path.</param>
        /// <param name="attributes">The attributes.</param>
        /// <returns>HtmlString.</returns>
        public static HtmlString InlineImageAsync(this IHtmlHelper html, string notfoundPath, string imagePath, object attributes = null)
        {
            if(html.ViewContext.HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment)) is IWebHostEnvironment env)
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
            else
            {
                return new HtmlString(string.Empty);
            }
        }
    }
}
