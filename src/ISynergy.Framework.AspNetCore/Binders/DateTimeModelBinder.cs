using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ISynergy.Framework.AspNetCore.Binders
{
    /// <summary>
    /// Binds DateTime objects of all kinds as the specified kind.
    /// A date with kind Unspecified is treated as the specified kind and not converted.
    /// </summary>
    public class DateTimeModelBinder : IModelBinder
    {
        /// <summary>
        /// The specified kind
        /// </summary>
        private readonly DateTimeKind specifiedKind;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeModelBinder"/> class.
        /// </summary>
        /// <param name="specifiedKind">The specified kind.</param>
        public DateTimeModelBinder(DateTimeKind specifiedKind)
        {
            EnumUtility.ThrowIfUndefined(typeof(DateTimeKind), specifiedKind);
            this.specifiedKind = specifiedKind;
        }

        /// <summary>
        /// Attempts to bind a model.
        /// </summary>
        /// <param name="bindingContext">The <see cref="T:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingContext" />.</param>
        /// <returns><para>
        /// A <see cref="T:System.Threading.Tasks.Task" /> which will complete when the model binding process completes.
        /// </para>
        /// <para>
        /// If model binding was successful, the <see cref="P:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingContext.Result" /> should have
        /// <see cref="P:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingResult.IsModelSet" /> set to <c>true</c>.
        /// </para>
        /// <para>
        /// A model binder that completes successfully should set <see cref="P:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingContext.Result" /> to
        /// a value returned from <see cref="M:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingResult.Success(System.Object)" />.
        /// </para></returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Argument.IsNotNull(bindingContext);

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
