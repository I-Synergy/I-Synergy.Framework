using ISynergy.Framework.AspNetCore.Binders;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace ISynergy.Framework.AspNetCore.Providers
{
    public class DateTimeModelBinderProvider : IModelBinderProvider
    {
        private readonly DateTimeKind specifiedKind;

        public DateTimeModelBinderProvider(DateTimeKind specifiedKind)
        {
            EnumUtility.ThrowIfUndefined(typeof(DateTimeKind), specifiedKind);
            this.specifiedKind = specifiedKind;
        }

        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            Argument.IsNotNull(nameof(context), context);

            return (context.Metadata.ModelType == typeof(DateTime))
                   || (context.Metadata.ModelType == typeof(DateTime?))
                ? new DateTimeModelBinder(specifiedKind)
                : null;
        }
    }
}
