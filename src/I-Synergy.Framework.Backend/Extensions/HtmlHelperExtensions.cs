using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISynergy.Extensions
{
    public static class HtmlHelperExtensions
    {
        private static string GetFileContentType(string path)
        {
            if (path.EndsWith(".JPG", StringComparison.OrdinalIgnoreCase) == true)
            {
                return "image/jpeg";
            }
            else if (path.EndsWith(".GIF", StringComparison.OrdinalIgnoreCase) == true)
            {
                return "image/gif";
            }
            else if (path.EndsWith(".PNG", StringComparison.OrdinalIgnoreCase) == true)
            {
                return "image/png";
            }

            throw new ArgumentException("Unknown file type");
        }

        public static HtmlString InlineImageAsync(this IHtmlHelper html, string path, byte[] image, string extension, object attributes = null)
        {
            if (image != null && !string.IsNullOrWhiteSpace(extension))
            {
                return ConvertArrayToHtmlString(image, GetFileContentType(extension), attributes);
            }
            else
            {
                var contentType = GetFileContentType(path);
                var env = html.ViewContext.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment)) as IHostingEnvironment;

                using (var stream = env.WebRootFileProvider.GetFileInfo(path).CreateReadStream())
                {
                    var array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);

                    return ConvertArrayToHtmlString(array, contentType, attributes);
                }
            }
        }

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
}
