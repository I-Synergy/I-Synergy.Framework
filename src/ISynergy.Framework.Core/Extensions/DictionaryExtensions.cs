namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Dictionary extensions.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    ///   Checks whether two dictionaries have the same contents.
    /// </summary>
    /// 
    public static bool IsEqual<TKey, TValue>(this IDictionary<TKey, TValue>? a, IDictionary<TKey, TValue>? b)
        where TKey : notnull
    {
        if (ReferenceEquals(a, b))
            return true;

        if (a is null || b is null)
            return false;

        if (a.Count != b.Count)
            return false;

        var aKeys = new HashSet<TKey>(a.Keys);
        var bKeys = new HashSet<TKey>(b.Keys);
        if (!aKeys.SetEquals(bKeys))
            return false;

        if (StringExtensions.HasMethod<TValue>("SetEquals"))
        {
            var setEquals = typeof(TValue).GetMethod("SetEquals");
            if (setEquals is not null)
            {
                foreach (var k in aKeys.EnsureNotNull())
                {
                    var aValue = a[k];
                    var bValue = b[k];

                    if (aValue is null)
                    {
                        if (bValue is not null)
                            return false;
                        continue;
                    }

                    if (!(bool)(setEquals.Invoke(aValue, [bValue]) ?? false))
                        return false;
                }
            }
        }
        else
        {
            foreach (var k in aKeys.EnsureNotNull())
            {
                var aValue = a[k];
                var bValue = b[k];

                if (aValue is null)
                    return bValue is null;

                if (!aValue.Equals(bValue))
                    return false;
            }
        }
        return true;
    }
}
