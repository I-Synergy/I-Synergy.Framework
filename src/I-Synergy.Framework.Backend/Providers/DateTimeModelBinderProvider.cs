using ISynergy.Binders;
using ISynergy.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace ISynergy.Providers
{
    public class DateTimeModelBinderProvider : IModelBinderProvider
    {
        private readonly DateTimeKind specifiedKind;

        public DateTimeModelBinderProvider(DateTimeKind specifiedKind)
        {
            EnumUtility.ThrowIfUndefined(typeof(DateTimeKind), specifiedKind);
            this.specifiedKind = specifiedKind;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            Argument.IsNotNull(nameof(context), context);

            return (context.Metadata.ModelType == typeof(DateTime))
                   || (context.Metadata.ModelType == typeof(DateTime?))
                ? new DateTimeModelBinder(specifiedKind)
                : null;
        }
    }
}