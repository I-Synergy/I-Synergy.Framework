using ISynergy.Framework.AspNetCore.Globalization.Options;
using ISynergy.Framework.Core.Validation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.AspNetCore.Globalization.Constraints;

public class CultureRouteConstraint : IRouteConstraint
{
    private readonly GlobalizationOptions _options;

    public CultureRouteConstraint(IOptions<GlobalizationOptions> options)
    {
        Argument.IsNotNull(options);

        _options = options.Value;
    }

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey,
        RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (!values.TryGetValue(routeKey, out var value) || value == null)
            return false;

        var culture = value.ToString();
        return _options.SupportedCultures.Contains(culture, StringComparer.OrdinalIgnoreCase);
    }
}