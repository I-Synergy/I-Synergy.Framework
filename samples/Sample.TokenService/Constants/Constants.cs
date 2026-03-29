namespace Sample.TokenService
{
    /// <summary>
    /// Specific Claim types to be used in auth token service
    /// </summary>
    public static class CustomClaimTypes
    {
        //Used by identity token
        public const string ApplicationIdType = "application_id";
        public const string DocumentIdType = "document_id";
        public const string AccountIdType = "account_id";
        public const string UserIdType = "user_id";
    }

    /// <summary>
    /// Document roles for Office Online viewer
    /// </summary>
    public static class OfficeOnlineDocumentModes
    {
        public const string View = "view";
        public const string Edit = "edit";
        public const string EditNew = "editnew";
    }
}
