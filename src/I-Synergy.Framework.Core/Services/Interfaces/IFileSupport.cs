namespace ISynergy.Services
{
    public interface IFileSupport
    {
        string FileName { get; set; }
        string Filter { get; set; }
        bool AddExtension { get; set; }
        bool CheckFileExists { get; set; }
        bool CheckPathExists { get; set; }
        int FilterIndex { get; set; }
        string InitialDirectory { get; set; }
        string Title { get; set; }
        bool ValidateNames { get; set; }
    }
}