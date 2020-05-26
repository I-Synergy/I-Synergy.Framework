using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace ISynergy.Framework.AspNetCore.Extensions
{
    public static class ImageHelperExtensions
    {
        public static HtmlString InlineImageAsync(this IHtmlHelper html, string notfoundPath, string imagePath, object? attributes = null)
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
