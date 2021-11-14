namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Dictionary extensions.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        ///   Checks whether two dictionaries have the same contents.
        /// </summary>
        /// 
        public static bool IsEqual<TKey, TValue>(this IDictionary<TKey, TValue> a, IDictionary<TKey, TValue> b)
        {
            if (a == b)
                return true;

            if (a.Count != b.Count)
                return false;

            var aKeys = new HashSet<TKey>(a.Keys);
            var bKeys = new HashSet<TKey>(b.Keys);
            if (!aKeys.SetEquals(bKeys))
                return false;

            if (StringExtensions.HasMethod<TValue>("SetEquals"))
            {
                var setEquals = typeof(TValue).GetMethod("SetEquals");
                foreach (var k in aKeys)
                {
                    if (!(bool)setEquals.Invoke(a[k], new object[] { b[k] }))
                        return false;
                }
            }
            else
            {
                foreach (var k in aKeys)
                    if (!a[k].Equals(b[k]))
                        return false;
            }
            return true;
        }
    }
}
