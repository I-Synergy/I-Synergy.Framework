namespace ISynergy.Framework.Core.Data
{
    public interface IClassBase
    {
        int Version { get; set; }
        bool IsDeleted { get; set; }
    }
}
