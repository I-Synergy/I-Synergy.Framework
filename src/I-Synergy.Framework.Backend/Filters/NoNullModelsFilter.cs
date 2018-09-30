using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Collections;

namespace ISynergy.Filters
{
    /// <summary>
    /// Checks if there are model parameters (ActionArguments) with a null value,
    /// and tries to add a model error to the model state.
    /// Only optional parameters with a default value of null are allowed.
    /// </summary>
    public class NoNullModelsFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var optionalParameters = context.ActionDescriptor.Parameters
                .Where(p =>
                {
                    var parameterInfo = (p as ControllerParameterDescriptor)?.ParameterInfo;
                    if (parameterInfo is null) return false;
                    return parameterInfo.IsOptional && parameterInfo.DefaultValue is null;
                })
                .Select(p => p.Name)
                .ToList();

            if (context.ActionArguments.Any(arg => arg.Value is null && !optionalParameters.Contains(arg.Key)))
            {
                var args = context.ActionArguments.Where(arg => arg.Value is null).ToList();
                foreach (var arg in args.EnsureNotNull())
                {
                    context.ModelState.TryAddModelError(arg.Key, $"The {arg.Key} field is required.");
                }
            }
        }
    }
}