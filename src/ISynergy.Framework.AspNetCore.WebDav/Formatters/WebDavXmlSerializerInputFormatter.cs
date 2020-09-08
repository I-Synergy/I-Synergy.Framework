using Microsoft.AspNetCore.Mvc.Formatters;

namespace ISynergy.Framework.AspNetCore.WebDav.Formatters
{
    /// <summary>
    /// The formatter for the WebDAV request body
    /// </summary>
    public class WebDavXmlSerializerInputFormatter : XmlSerializerInputFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavXmlSerializerInputFormatter"/> class.
        /// </summary>
        public WebDavXmlSerializerInputFormatter()
        {
            SupportedMediaTypes.Add("text/plain");
        }

        /// <inheritdoc />
        public override bool CanRead(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            if (request.ContentType == null)
            {
                var contentLength = request.ContentLength;
                if (contentLength.GetValueOrDefault() == 0)
                {
                    // We allow that the following types have an optional body
                    switch (request.Method)
                    {
                        case "LOCK":
                            return true;
                        case "PROPFIND":
                            return true;
                    }
                }
            }

            return base.CanRead(context);
        }
    }
}
