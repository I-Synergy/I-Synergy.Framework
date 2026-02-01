using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ISynergy.Framework.AspNetCore.Conventions;

/// <summary>
/// Disabled controllers with namespaces than end with '.Disabled'.
/// </summary>
public class DisabledControllerConvention : IApplicationModelConvention
{
    /// <summary>
    /// Applies the specified application.
    /// </summary>
    /// <param name="application">The application.</param>
    public void Apply(ApplicationModel application)
    {
        for (var i = application.Controllers.Count - 1; i >= 0; i--)
        {
            var controller = application.Controllers[i];

            if (controller.ControllerType.Namespace is not null
                && controller.ControllerType.Namespace.EndsWith(".Disabled"))
                application.Controllers.Remove(controller);
        }
    }
}
