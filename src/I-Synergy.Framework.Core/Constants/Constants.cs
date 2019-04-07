namespace ISynergy
{
    public static class Constants
    {
        public const int DefaultPageSize = 250;
        public const int TopDimension = 5;
        public const int RestRetryCount = 3;
        public const int RestRetryDelayInSeconds = 5;

        public const int AutoSuggestBoxDelay = 500;

        public const string Authorization = "Authorization";
        public const string AuthenticationError = "authentication_error";
        public const string InvalidGrantError = "invalid_grant";
        public const string UnauthorizedClientError = "unauthorized_client";

        public const string SecuritySchemeKey = "OAuth2";

        // Claims
        public const string Claim_ConnectionString = "Claim_ConnectionString";
        public const string Claim_Modules = "Claim_Modules";

        public const string TenantId = "TenantId";

        public const string PasswordRegEx = @"(?=^.{6,}$)(?=[^\d]*\d)(?=[^A-Z]*[A-Z])(?=[^a-z]*[a-z])";
        public const string RfidUidRegeEx = @"^[A-F0-9]{8}$";
        public const string DateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:ssZ";
        public const string DefaultCultureInfoName = "en";

        public const string ActivateStateKey = "ActiveState";
        public const string InactiveStateKey = "InactiveState";

        public const string DownloadContentType = "application/x-msdownload";

        public const string View = "View";
        public const string Page = "Page";
        public const string Window = "Window";
        public const string ViewModel = "ViewModel";
        public const string ShellViewModel = "ShellViewModel";

        public const string Documents = "documents";
        public const string Images = "images";
        public const string Temaplates = "templates";
        public const string Commodities = "commodities";
        public const string Relations = "relations";

        public const string UsernamePrefixTest = "test:";
        public const string UsernamePrefixLocal = "local:";

        public const string TemporaryUrl = @"temp:///";

        public const string ApiName = "api";
        public const string ApiClient = "ISynergy";

        public const string SignalPath = "/api/signal";

        public const string IsOffline = "IsOffline";
        public const string HasInternetConnection = "HasInternetConnection";

        public const int NumberOfPaymentsToList = 50;
    }

    public static class ExceptionConstants
    {
        public const string Error_547 = "The DELETE statement conflicted with the REFERENCE constraint";
    }
}
