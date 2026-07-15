using ISynergy.Framework.Core.Abstractions.Base;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ISynergy.Framework.Core.Extensions;

public static class ObservableClassExtensions
{
    [UnconditionalSuppressMessage("Trimming", "IL2075:UnrecognizedReflectionPattern",
        Justification = "Intentionally reflects over the runtime type of the ObservableClass-derived instance; a generic parameter would bind to the compile-time base type and change behavior. Public instance properties of these model types are rooted by the consuming application bindings/serialization; a property removed by the trimmer has no observable dirty/validation state to enumerate.")]
    public static IEnumerable<PropertyInfo> GetProperties(this object observableClass, bool includeMemo = false, bool includeAudit = false, bool includeReadOnly = false)
    {
        return observableClass.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(q =>

            (includeReadOnly || q.CanWrite) &&
            !q.Name.Equals(nameof(IObservableClass.IsDisposed)) &&
            !q.Name.Equals(nameof(IObservableClass.IsDirty)) &&
            !q.Name.Equals(nameof(IObservableClass.Properties)) &&
            !q.Name.Equals(nameof(IObservableValidatedClass.AutomaticValidationTrigger)) &&
            !q.Name.Equals(nameof(IObservableValidatedClass.Error)) &&
            !q.Name.Equals(nameof(IObservableValidatedClass.Errors)) &&
            !q.Name.Equals(nameof(IObservableValidatedClass.HasErrors)) &&
            !q.Name.Equals(nameof(IObservableValidatedClass.IsValid)) &&
            !q.Name.Equals(nameof(IObservableValidatedClass.Validator)) &&
            !q.Name.Equals(nameof(IModel.Version)) &&
            !q.Name.Equals("Item") &&
            !q.Name.Equals("Items") &&
            (includeMemo || !q.Name.Equals(nameof(IModel.Memo))) &&
            (includeAudit || !q.Name.Equals(nameof(IModel.CreatedDate))) &&
            (includeAudit || !q.Name.Equals(nameof(IModel.CreatedBy))) &&
            (includeAudit || !q.Name.Equals(nameof(IModel.ChangedDate))) &&
            (includeAudit || !q.Name.Equals(nameof(IModel.ChangedBy))));
    }
}
