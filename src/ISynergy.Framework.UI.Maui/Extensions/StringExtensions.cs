using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Controls.Shapes;

namespace ISynergy.Framework.UI.Extensions;

public static class StringExtensions
{
    public static bool IsDevelopment(this string environment) => environment.IsEnvironment(Environments.Development);
    public static bool IsStaging(this string environment) => environment.IsEnvironment(Environments.Staging);
    public static bool IsProduction(this string environment) => environment.IsEnvironment(Environments.Production);

    public static bool IsEnvironment(this string? environment, string environmentName)
    {
        if (string.IsNullOrEmpty(environment) || string.IsNullOrEmpty(environmentName))
            return false;

        return string.Equals(environment, environmentName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the Geometry object.
    /// </summary>
    /// <param name="path">Path Data string</param>
    /// <returns>System.String.</returns>
    public static Geometry? ToGeometry(this string? path)
    {
        if (!string.IsNullOrWhiteSpace(path))
            return (Geometry?)new PathGeometryConverter().ConvertFromInvariantString(path);

        return null;
    }
}
