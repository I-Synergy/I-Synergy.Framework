using ISynergy.Framework.AspNetCore.Binders;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace ISynergy.Framework.AspNetCore.Providers
{
    /// <summary>
    /// Class DateTimeModelBinderProvider.
    /// Implements the <see cref="IModelBinderProvider" />
    /// </summary>
    /// <seealso cref="IModelBinderProvider" />
    public class DateTimeModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// The specified kind
        /// </summary>
        private readonly DateTimeKind specifiedKind;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeModelBinderProvider"/> class.
        /// </summary>
        /// <param name="specifiedKind">The specified kind.</param>
        public DateTimeModelBinderProvider(DateTimeKind specifiedKind)
        {
            EnumUtility.ThrowIfUndefined(typeof(DateTimeKind), specifiedKind);
            this.specifiedKind = specifiedKind;
        }

        /// <summary>
        /// Gets the binder.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>System.Nullable&lt;IModelBinder&gt;.</returns>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            Argument.IsNotNull(context);

            return (context.Metadata.ModelType == typeof(DateTime))
                   || (context.Metadata.ModelType == typeof(DateTime?))
                ? new DateTimeModelBinder(specifiedKind)
                : null;
        }
    }
}
