namespace ISynergy
{
    public class Constants
    {
        public const string SecuritySchemeKey = "OAuth2";

        // Claims
        public const string Claim_ConnectionString = "Claim_ConnectionString";
        public const string Claim_Modules = "Claim_Modules";

        public const string PasswordRegEx = @"(?=^.{6,}$)(?=[^\d]*\d)(?=[^A-Z]*[A-Z])(?=[^a-z]*[a-z])";
        public const string RfidUidRegeEx = @"^[A-F0-9]{8}$";
        public const string DateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:ssZ";
        public const string DefaultCultureInfoName = "en";

        public const string ActivateStateKey = "ActiveState";
        public const string InactiveStateKey = "InactiveState";

        public const string DownloadContentType = "application/x-msdownload";

        public const string View = "View";
        public const string Page = "Page";
        public const string ViewModel = "ViewModel";

        public const string Documents = "documents";
        public const string Images = "images";
        public const string Temaplates = "templates";
        public const string Commodities = "commodities";
        public const string Relations = "relations";
    }

    public class Exception_Constants
    {
        public const string Error_547 = "The DELETE statement conflicted with the REFERENCE constraint";
    }
}
