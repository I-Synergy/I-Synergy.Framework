namespace ISynergy.Framework.Core.Linq.Abstractions
{
    public interface IKeywordsHelper
    {
        bool TryGetValue(string name, out object type);
    }
}
