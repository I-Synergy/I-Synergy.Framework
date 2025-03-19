namespace ISynergy.Framework.Core.Constants;

/// <summary>
/// Class GenericConstants.
/// </summary>
public static class GenericConstants
{
    /// <summary>
    /// The default page size
    /// </summary>
    public const int DefaultPageSize = 250;
    /// <summary>
    /// The top dimension
    /// </summary>
    public const int TopDimension = 5;
    /// <summary>
    /// The rest retry count
    /// </summary>
    public const int RestRetryCount = 1;
    /// <summary>
    /// The rest retry delay in seconds
    /// </summary>
    public const int RestRetryDelayInSeconds = 5;

    /// <summary>
    /// The automatic suggest box delay
    /// </summary>
    public const int AutoSuggestBoxDelay = 500;

    public const string Parameter = "parameter";

    /// <summary>
    /// The authorization
    /// </summary>
    public const string Authorization = "Authorization";
    /// <summary>
    /// The authentication error
    /// </summary>
    public const string AuthenticationError = "authentication_error";
    /// <summary>
    /// The invalid grant error
    /// </summary>
    public const string InvalidGrantError = "invalid_grant";
    /// <summary>
    /// The unauthorized client error
    /// </summary>
    public const string UnauthorizedClientError = "unauthorized_client";

    /// <summary>
    /// The tenant identifier
    /// </summary>
    public const string TenantId = "TenantId";

    /// <summary>
    /// The password reg ex
    /// </summary>
    public const string PasswordRegEx = @"(?=^.{6,}$)(?=[^\d]*\d)(?=[^A-Z]*[A-Z])(?=[^a-z]*[a-z])";
    /// <summary>
    /// The rfid uid rege ex
    /// </summary>
    public const string RfidUidRegeEx = @"^[A-F0-9]{8}$";
    /// <summary>
    /// The date time offset format
    /// </summary>
    public const string DateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:ssZ";
    /// <summary>
    /// The default culture information name
    /// </summary>
    public const string DefaultCultureInfoName = "en";

    /// <summary>
    /// The activate state key
    /// </summary>
    public const string ActivateStateKey = "ActiveState";
    /// <summary>
    /// The inactive state key
    /// </summary>
    public const string InactiveStateKey = "InactiveState";

    /// <summary>
    /// The download content type
    /// </summary>
    public const string DownloadContentType = "application/x-msdownload";
    /// <summary>
    /// The json content type
    /// </summary>
    public const string JsonContentType = "application/json; charset=utf-8";
    /// <summary>
    /// The text content type
    /// </summary>
    public const string TextContentType = "text/plain; charset=utf-8";

    /// <summary>
    /// The json media type
    /// </summary>
    public const string JsonMediaType = "application/json";

    /// <summary>
    /// The view
    /// </summary>
    public const string View = "View";
    /// <summary>
    /// The page
    /// </summary>
    public const string Page = "Page";
    /// <summary>
    /// The window
    /// </summary>
    public const string Window = "Window";
    /// <summary>
    /// The view model
    /// </summary>
    public const string ViewModel = "ViewModel";
    /// <summary>
    /// The viewmodel with generic parameter
    /// </summary>
    public const string ViewModelTRegex = ".*ViewModel`.*";
    /// <summary>
    /// The shell view model
    /// </summary>
    public const string ShellViewModel = "ShellViewModel";
    public const string ShellView = "ShellView";

    /// <summary>
    /// The documents
    /// </summary>
    public const string Documents = "documents";
    /// <summary>
    /// The images
    /// </summary>
    public const string Images = "images";
    /// <summary>
    /// The temaplates
    /// </summary>
    public const string Temaplates = "templates";
    /// <summary>
    /// The commodities
    /// </summary>
    public const string Commodities = "commodities";
    /// <summary>
    /// The relations
    /// </summary>
    public const string Relations = "relations";

    /// <summary>
    /// The username prefix test
    /// </summary>
    public const string UsernamePrefixTest = "test?";

    /// <summary>
    /// The username prefix local
    /// </summary>
    public const string UsernamePrefixLocal = "local?";

    /// <summary>
    /// The temporary URL
    /// </summary>
    public const string TemporaryUrl = @"temp:///";

    /// <summary>
    /// The API name
    /// </summary>
    public const string ApiName = "api";

    /// <summary>
    /// The API version
    /// </summary>
    public const string ApiVersion = "x-ms-version";

    /// <summary>
    /// The signal path
    /// </summary>
    public const string SignalPath = "/api/signal";

    /// <summary>
    /// The is offline
    /// </summary>
    public const string IsOffline = "IsOffline";
    /// <summary>
    /// The has internet connection
    /// </summary>
    public const string HasInternetConnection = "HasInternetConnection";

    /// <summary>
    /// The number of payments to list
    /// </summary>
    public const int NumberOfPaymentsToList = 50;
}
