using ISynergy.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace ISynergy.Binders
{
    /// <summary>
    /// Binds DateTime objects of all kinds as the specified kind.
    /// A date with kind Unspecified is treated as the specified kind and not converted.
    /// </summary>
    public class DateTimeModelBinder : IModelBinder
    {
        private readonly DateTimeKind specifiedKind;

        public DateTimeModelBinder(DateTimeKind specifiedKind)
        {
            EnumUtility.ThrowIfUndefined(typeof(DateTimeKind), specifiedKind);
            this.specifiedKind = specifiedKind;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Argument.IsNotNull(nameof(bindingContext), bindingContext);

            if ((bindingContext.ModelType != typeof(DateTime))
                && (bindingContext.ModelType != typeof(DateTime?)))
                return Task.CompletedTask;

            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueResult == ValueProviderResult.None)
                return Task.CompletedTask;

            if (valueResult.FirstValue is null)
                return Task.CompletedTask;

            if (!DateTime.TryParse(valueResult.FirstValue, out var value))
                return Task.CompletedTask;

            EnumUtility.ThrowIfUndefined(typeof(DateTimeKind), value.Kind);

            switch (value.Kind)
            {
                case DateTimeKind.Local:
                    switch (specifiedKind)
                    {
                        case DateTimeKind.Local:
                            // Value is already Local.
                            break;

                        case DateTimeKind.Unspecified:
                            value = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
                            break;

                        case DateTimeKind.Utc:
                            value = value.ToUniversalTime();
                            break;
                    }
                    break;

                case DateTimeKind.Unspecified:
                    switch (specifiedKind)
                    {
                        case DateTimeKind.Local:
                            value = DateTime.SpecifyKind(value, DateTimeKind.Local);
                            break;

                        case DateTimeKind.Unspecified:
                            // Value is already Unspecified.
                            break;

                        case DateTimeKind.Utc:
                            value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                            break;
                    }
                    break;

                case DateTimeKind.Utc:
                    switch (specifiedKind)
                    {
                        case DateTimeKind.Local:
                            value = value.ToLocalTime();
                            break;

                        case DateTimeKind.Unspecified:
                            value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                            break;

                        case DateTimeKind.Utc:
                            // Value is already Utc.
                            break;
                    }
                    break;
            }

            bindingContext.Result = ModelBindingResult.Success(value);
            return Task.CompletedTask;
        }
    }
}