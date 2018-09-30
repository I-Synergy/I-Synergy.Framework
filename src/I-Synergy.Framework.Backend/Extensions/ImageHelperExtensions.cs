using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace ISynergy.Extensions
{
    public static class ImageHelperExtensions
    {
        public static HtmlString InlineImageAsync(this IHtmlHelper html, string notfoundPath, string imagePath, object attributes = null)
        {
            var env = html.ViewContext.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment)) as IHostingEnvironment;

            string img = "";

            var props = (attributes != null) ? attributes.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(attributes)) : null;

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
    }
}
