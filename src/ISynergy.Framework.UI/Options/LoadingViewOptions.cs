using ISynergy.Framework.UI.Enumerations;

namespace ISynergy.Framework.UI.Options;
public class LoadingViewOptions
{
    public Func<Task<Stream>> AssetStreamProvider { get; set; }
    public string ContentType { get; set; }
    public LoadingViewTypes ViewType { get; set; }
}
