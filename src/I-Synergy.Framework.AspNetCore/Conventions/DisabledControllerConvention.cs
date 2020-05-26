using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ISynergy.Framework.AspNetCore.Conventions
{
    /// <summary>
    /// Disabled controllers with namespaces than end with '.Disabled'.
    /// </summary>
    public class DisabledControllerConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            for (var i = application.Controllers.Count - 1; i >= 0; i--)
            {
                var controller = application.Controllers[i];

                if (controller.ControllerType.Namespace != null
                    && controller.ControllerType.Namespace.EndsWith(".Disabled"))
                    application.Controllers.Remove(controller);
            }
        }
    }
}