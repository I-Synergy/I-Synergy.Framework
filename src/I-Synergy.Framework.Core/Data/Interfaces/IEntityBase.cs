namespace ISynergy
{
    public interface IEntityBase : IClassBase
    {
        string Memo { get; set; }
        string InputFirst { get; set; }
        string InputLast { get; set; }
    }
}
